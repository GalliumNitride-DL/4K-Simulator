using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Simulator.Event;

namespace Simulator
{
    [Serializable]
    public class NoteInfo
    {
        public float time; public int track; public int type; public float localSpeed;
    }
    public enum GameState
    {
        WaitingStart,
        WaitingAudio,
        Playing,
        Completed
    }

    public class GameController : MonoBehaviour
    {
        public static GameController Instance;
        public float LevelTime { get; private set; }
        public List<NoteInfo> remainingNotes = new List<NoteInfo>();
        public List<Note>[] displayingNotes = new List<Note>[4];
        public int halfScreenHeight = 1080;
        public GameState State { get; private set; } = GameState.WaitingStart;

        public AudioClip clip;
        public float speedMultiplier = 5f;
        public int[] coords = new int[4];
        public KeyCode[] keys = new KeyCode[4];
        public int judgementLineY;

        public int comboCount;
        private float time;
        

        private void Awake()
        {
            Instance = this;
            displayingNotes[0] = new List<Note>();
            displayingNotes[1] = new List<Note>();
            displayingNotes[2] = new List<Note>();
            displayingNotes[3] = new List<Note>();
            halfScreenHeight = Screen.height / 2;
            judgementLineY = judgementLineY - halfScreenHeight;
        }
        // Start is called before the first frame update
        void Start()
        {
            //AudioManager.Instance.SetClip(clip);
            time = Time.time;
        }

        // Update is called once per frame
        void Update()
        {
            if (State == GameState.Completed || State == GameState.WaitingAudio) return;
            //Update LevelTime
            LevelTime = Time.time - time;

            //Show Notes
            while (remainingNotes.Count > 0 && IsPendingJudge(remainingNotes[0]))
            {
                var note = NoteManager.Instance.RegisterNote(remainingNotes[0]);
                int i; 
                var display = displayingNotes[remainingNotes[0].track];
                if (display.Count == 0) display.Add(note);
                else
                {
                    for (i = display.Count - 1; i >= 0; i--) //插入法排序
                    {
                        if (display[i].judgeTime < note.judgeTime) break;
                    }
                    display.Insert(++i, note);
                }
            }
            //Update and Judge Notes
            foreach (var trackNotes in displayingNotes)
            {
                var noLongerJudgeable = false;
                for (int i = 0; i < trackNotes.Count; i++)
                {
                    trackNotes[i].UpdateNote(LevelTime, out var destory);
                    if (destory)
                    {
                        NoteManager.Instance.RecycleNote(trackNotes[i]);
                        trackNotes.RemoveAt(i); i--; continue;
                    }

                    if (!noLongerJudgeable)
                    {
                        trackNotes[i].JudgeNote(LevelTime, out destory, out noLongerJudgeable);
                        if (destory)
                        {
                            NoteManager.Instance.RecycleNote(trackNotes[i]);
                            trackNotes.RemoveAt(i); i--; continue;
                        }
                    }
                }
            }
        }

        public void Miss(Note note)
        {
            comboCount = 0;
        }

        private bool IsPendingJudge(NoteInfo info)
        {
            var distanceToTop = halfScreenHeight - judgementLineY;
            var remainTime = info.time - LevelTime;
            if (remainTime < 0) return false;
            return remainTime * info.localSpeed * speedMultiplier <= distanceToTop;
        }
    }

}