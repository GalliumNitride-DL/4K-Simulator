using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Simulator
{
    public class DragNote : Note
    {
        bool isJudged = false;
        public override void ReInitialize(NoteInfo info)
        {
            base.ReInitialize(info);
            isJudged = false;
        }
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
                if (a <= 0f) destroyAfterCall = true;
                renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, a);
            }
            else renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, 1f);
        }
        public override void JudgeNote(float time, out bool destroyAfterCall, out bool makeNoLongerJudgeable)
        {
            makeNoLongerJudgeable = false;
            destroyAfterCall = false;
            var dt = (judgeTime - time) * 1000;

            if (isJudged)
            {
                if (transform.localPosition.y <= 0) //Perfect
                {
                    PlayEffect(0); destroyAfterCall = true;
                }
                else return;
            }

            if (dt > NoteManager.Instance.goodRange) { return; }
            if (dt < -NoteManager.Instance.goodRange) //Miss
            {
                PlayEffect(3);
                GameController.Instance.Miss(this); destroyAfterCall = true; return; 
            }

            if (Input.GetKey(GameController.Instance.keys[track])) //Pending Perfect
            {
                isJudged = true;
            }
        }
    }
}

