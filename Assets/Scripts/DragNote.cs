using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Simulator
{
    public class DragNote : Note
    {


        public override void UpdateNote(float time, out bool destroyAfterCall)
        {
            throw new System.NotImplementedException();
        }
        public override void JudgeNote(float time, out bool destroyAfterCall, out bool makeNoLongerJudgeable)
        {
            throw new System.NotImplementedException();
        }
    }
}

