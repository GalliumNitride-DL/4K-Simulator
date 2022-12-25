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
}