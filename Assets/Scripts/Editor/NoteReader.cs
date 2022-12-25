using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Simulator.Editor
{
    public class NoteReader : EditorWindow
    {
        private TextAsset affChart;
        private GameController controller;

        [MenuItem("Simulator/AffToChart")]
        private static void Init()
        {
            var window = GetWindow<NoteReader>();
            window.Show();
        }

        private void OnGUI()
        {
            affChart = EditorGUILayout.ObjectField("Chart", affChart, typeof(TextAsset), false) as TextAsset;
            controller = EditorGUILayout.ObjectField("Controller", controller, typeof(GameController), true) as GameController;

            if (GUILayout.Button("Read"))
            {
                ReadChart();
            }
        }

        private void ReadChart()
        {
            var text = affChart.text;
            var splitText = new List<string>(text.Split('\n'));
            var offset = 0f;
            if (text.StartsWith("AudioOffset", StringComparison.CurrentCultureIgnoreCase))
            {
                offset = (float)int.Parse(splitText[0].Split(':')[1]) / 1000f;
                splitText.RemoveAt(0);
            }
            splitText.RemoveAt(0);

            foreach (var data in splitText)
            {
                if (data.StartsWith('('))
                {
                    var kvp = data.Replace("(", string.Empty).Replace(");", string.Empty).Split(',');
                    controller.remainingNotes.Add(new NoteInfo
                    {
                        time = (float)int.Parse(kvp[0]) / 1000f + offset,
                        track = int.Parse(kvp[1]) - 1,
                        type = 0,
                        localSpeed = 1,
                        duration = 0
                    });
                    continue;
                }
                if (data.StartsWith("hold"))
                {
                    var kvp = data.Replace("hold(", string.Empty).Replace(");", string.Empty).Split(',');
                    controller.remainingNotes.Add(new NoteInfo
                    {
                        time = (float)int.Parse(kvp[0]) / 1000f + offset,
                        track = int.Parse(kvp[2]) - 1,
                        type = 1,
                        localSpeed = 1,
                        duration = Mathf.Abs((float)int.Parse(kvp[0]) - (float)int.Parse(kvp[1])) / 1000f + offset
                    });
                }

            }
        }
    }

    public class Malody4KReader : EditorWindow
    {
        private TextAsset mcChart;
        private GameController controller;
        private float bpm;
        private float offset;

        [MenuItem("Simulator/Malody4KToChart")]
        private static void Init()
        {
            var window = GetWindow<Malody4KReader>();
            window.Show();
        }

        private void OnGUI()
        {
            mcChart = EditorGUILayout.ObjectField("Chart", mcChart, typeof(TextAsset), false) as TextAsset;
            controller = EditorGUILayout.ObjectField("Controller", controller, typeof(GameController), true) as GameController;
            bpm = EditorGUILayout.FloatField("BPM", bpm);
            offset = EditorGUILayout.FloatField("Offset", offset);

            if (GUILayout.Button("Read"))
            {
                ReadChart();
            }
        }

        private void ReadChart()
        {
            var text = mcChart.text.Replace("\n", string.Empty);
            text = text.Substring(text.IndexOf("note") + 8);
            var notesData = text.Split("},{");
            var bps = (double)bpm / 60d;
            foreach (var data in notesData)
            {
                if (data.Contains("sound")) continue;
                if (data.Contains("endbeat"))
                {
                    var rd = data.Replace("\"beat\":[", string.Empty).Replace("],\"endbeat\":[", ",").Replace("],\"column\":", ",");
                    var nums = rd.Split(",");
                    var startBeat = int.Parse(nums[0]) + (float)int.Parse(nums[1]) / (float)int.Parse(nums[2]);
                    var endBeat = int.Parse(nums[3]) + (float)int.Parse(nums[4]) / (float)int.Parse(nums[5]);
                    var track = int.Parse(nums[6]);
                    var duration = (float)((endBeat - startBeat) / bps);
                    var startTime = (float)(startBeat / bps) + offset;
                    controller.remainingNotes.Add(new NoteInfo
                    {
                        time = startTime, track = track, type = 1, localSpeed = 1, duration = duration
                    });
                }
                else
                {
                    var rd = data.Replace("\"beat\":[", string.Empty).Replace("],\"column\":", ",");
                    var nums = rd.Split(",");
                    var startBeat = int.Parse(nums[0]) + (float)int.Parse(nums[1]) / (float)int.Parse(nums[2]);
                    var track = int.Parse(nums[3]);
                    var startTime = (float)(startBeat / bps) + offset;
                    controller.remainingNotes.Add(new NoteInfo
                    {
                        time = startTime, track = track, type = 0, localSpeed = 1, duration = 0
                    });
                }
            }

        }
    }
}