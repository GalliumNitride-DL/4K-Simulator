using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Simulator
{
    public class NoteManager : MonoBehaviour
    {
        public static NoteManager Instance;
        public GameObject[] prefabs;
        public GameObject[] FXs;
        public Transform parent;
        public Stack<TapNote> taps;
        public Stack<HoldNote> holds;
        public Stack<DragNote> drags;
        public int poolCapacity = 10;
        public int badRange, goodRange, perfectRange;

        private static NoteInfo nullNote = new NoteInfo
        {
            time = -1f, track = -1, type = -1, localSpeed = 0f
        };

        private void Awake()
        {
            Instance = this;
            taps = new Stack<TapNote>(); holds = new Stack<HoldNote>(); drags = new Stack<DragNote>();
            // for (int i = 0; i < poolCapacity; i++ )
            // {
            //     taps.Push(Instantiate(prefabs[0], parent).GetComponent<TapNote>());
            //     holds.Push(Instantiate(prefabs[1], parent).GetComponent<HoldNote>());
            //     drags.Push(Instantiate(prefabs[2], parent).GetComponent<DragNote>());
            // }
        }

        public Note RegisterNote(NoteInfo info)
        {
            Note note = null;
            switch (info.type)
            {
                case 0: //Tap
                    if (taps.TryPop(out var tap)) note = tap as TapNote;
                    else note = Instantiate(prefabs[0], parent).GetComponent<TapNote>();
                    break;
                case 1:
                    if (holds.TryPop(out var hold)) note = hold as HoldNote;
                    else note = Instantiate(prefabs[1], parent).GetComponent<HoldNote>();
                    break;
                case 2:
                    if (drags.TryPop(out var drag)) note = drag as DragNote;
                    else note = Instantiate(prefabs[2], parent).GetComponent<DragNote>();
                    break;
                default:
                    throw new NotImplementedException();
            }
            note.ReInitialize(info);
            return note;
        }

        public void RecycleNote(Note note)
        {
            note.ReInitialize(nullNote); note.renderer.enabled = false;
            if (note is TapNote) taps.Push(note as TapNote);
            if (note is HoldNote) holds.Push(note as HoldNote);
            if (note is DragNote) drags.Push(note as DragNote);
        }
    }
}