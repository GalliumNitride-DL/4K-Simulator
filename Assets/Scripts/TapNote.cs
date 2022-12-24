using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Simulator
{
    public class TapNote : Note
    {


        public override void UpdateNote(float time, out bool destroyAfterCall)
        {
            var x = GameController.Instance.coords[track];
            var y = (judgeTime - time) * localSpeed * GameController.Instance.speedMultiplier - GameController.Instance.judgementLineY;
            if (y < 0)
            {
                var a = (GameController.Instance.judgementLineY - y) / (GameController.Instance.halfScreenHeight + GameController.Instance.judgementLineY);
                renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, a);
            }
            destroyAfterCall = false;
        }

        public override void JudgeNote(float time, out bool destroyAfterCall, out bool makeNoLongerJudgeable)
        {
            makeNoLongerJudgeable = false;
            destroyAfterCall = false;
            var dt = Mathf.Abs(judgeTime - time) * 1000;
            
            //Miss or beyond judge range
            if (dt > NoteManager.Instance.badRange) { return; }
            if (dt < -NoteManager.Instance.badRange) { GameController.Instance.Miss(this); destroyAfterCall = true; return; }

            //Key not pressed
            if (!Input.GetKeyDown(GameController.Instance.keys[track])) { return; }

            dt = Mathf.Abs(dt);
            destroyAfterCall = true; makeNoLongerJudgeable = true;
            //Bad
            if (dt > NoteManager.Instance.goodRange) //bad
            {
                return;
            }
            if (dt > NoteManager.Instance.perfectRange) //good
            {
                GameController.Instance.comboCount++;
                return;
            }
            else //Perfect
            {
                GameController.Instance.comboCount++;
                return;
            }
        }
    }
}