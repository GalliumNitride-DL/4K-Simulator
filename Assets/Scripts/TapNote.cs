using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Simulator
{
    public class TapNote : Note
    {
        public override void UpdateNote(float time, out bool destroyAfterCall)
        {
            renderer.enabled = true;
            var x = GameController.Instance.coords[track];
            var y = (judgeTime - time) * localSpeed * GameController.Instance.speedMultiplier;
            transform.localPosition = new Vector3(x, y, 0f);
            destroyAfterCall = false;
            if (y < 0)
            {
                var a = 1f + y / (float)(GameController.Instance.halfScreenHeight + GameController.Instance.judgementLineY);
                //if (a <= 0f) destroyAfterCall = true;
                renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, a);
            }
            else renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, 1f);
        }

        public override void JudgeNote(float time, out bool destroyAfterCall, out bool makeNoLongerJudgeable)
        {
            makeNoLongerJudgeable = false;
            destroyAfterCall = false;
            var dt = (judgeTime - time) * 1000;
            //Debug.Log(dt);
            
            //Miss or beyond judge range
            if (dt > NoteManager.Instance.badRange) { return; }
            if (dt < -NoteManager.Instance.badRange) //Miss
            {
                //PlayEffect(3);
                //Debug.Log("miss");
                GameController.Instance.Miss(this); destroyAfterCall = true; return; 
            }

            //Key not pressed
            if (!Input.GetKeyDown(GameController.Instance.keys[track])) { return; }

            renderer.enabled = false;
            dt = Mathf.Abs(dt);
            destroyAfterCall = true; makeNoLongerJudgeable = true;
            //Bad
            if (dt > NoteManager.Instance.goodRange) //bad
            {
                PlayEffect(2);
                return;
            }
            if (dt > NoteManager.Instance.perfectRange) //good
            {
                PlayEffect(1);
                GameController.Instance.comboCount++;
                return;
            }
            else //Perfect
            {
                PlayEffect(0);
                GameController.Instance.comboCount++;
                return;
            }
        }
    }
}