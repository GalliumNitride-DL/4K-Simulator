using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Simulator
{
    public class HoldNote : Note
    {
        bool isMissed;
        bool isJudged;
        public override void ReInitialize(NoteInfo info)
        {
            base.ReInitialize(info);
            isMissed = isJudged = false;
        }
        public override void UpdateNote(float time, out bool destroyAfterCall)
        {
            destroyAfterCall = false;
            renderer.enabled = true;
            var x = GameController.Instance.coords[track];
            var dt = judgeTime - time;
            //var y1 = (judgeTime - time) * localSpeed * GameController.Instance.speedMultiplier;
            //var y2 = (judgeTime - time + duration) * localSpeed * GameController.Instance.speedMultiplier;
            var y = (dt > 0 ? dt + (duration / 2f) : (dt + duration) / 2f) * localSpeed * GameController.Instance.speedMultiplier;
            transform.localPosition = new Vector3(x, y, 0f);

            var scale = (dt > 0 ? duration * localSpeed * GameController.Instance.speedMultiplier : y * 2) / renderer.transform.localScale.y;
            transform.localScale = new Vector3(transform.localScale.x, scale, transform.localScale.z);
        }
        public override void JudgeNote(float time, out bool destroyAfterCall, out bool makeNoLongerJudgeable)
        {
            destroyAfterCall = false;
            makeNoLongerJudgeable = false;
            var dt = (judgeTime - time) * 1000;

            if (dt + duration * 1000 < 0)
            { 
                if (!isMissed) GameController.Instance.comboCount++;
                destroyAfterCall = true; 
            }

            if (dt > NoteManager.Instance.goodRange) { return; }

            var FXPos = new Vector3(transform.position.x, GameController.Instance.judgementLine.position.y, 0f);

            if (dt < -NoteManager.Instance.goodRange && !isJudged) //Miss, no bad
            {
                isMissed = true; isJudged = true;
                //PlayEffect(3);
                //Destroy(Instantiate(NoteManager.Instance.FXs[3], FXPos, Quaternion.identity), 0.5f);
                renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, 0.6f);
                GameController.Instance.Miss(this);
                return;
            }

            dt = Mathf.Abs(dt);

            if (Input.GetKeyDown(GameController.Instance.keys[track]))
            {
                isJudged = true;
                makeNoLongerJudgeable = true;
                if (dt > NoteManager.Instance.perfectRange) //Good
                {
                    //PlayEffect(1);
                    Destroy(Instantiate(NoteManager.Instance.FXs[1], FXPos, Quaternion.identity), 0.5f);
                    return;
                }
                else //Perfect
                {
                    //PlayEffect(0);
                    Destroy(Instantiate(NoteManager.Instance.FXs[0], FXPos, Quaternion.identity), 0.5f);
                    return;
                }
            }

            if (!Input.GetKey(GameController.Instance.keys[track]) && isJudged && !isMissed && duration - dt < NoteManager.Instance.goodRange)
            {
                isMissed = true;
                //PlayEffect(3);
                renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, 0.6f);
                GameController.Instance.Miss(this);
                return;
            }
        }
    }

}
