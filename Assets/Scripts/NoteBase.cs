using UnityEngine;

namespace Simulator
{
    public abstract class Note : MonoBehaviour
    {
        public float judgeTime = -1f;
        public int track = -1;
        public float localSpeed = 1f;
        public new SpriteRenderer renderer;

        internal virtual void Awake()
        {
            //renderer = this.GetComponent<SpriteRenderer>();
            renderer.enabled = false;
        }
        public void ReInitialize(NoteInfo info)
        {
            judgeTime = info.time; track = info.track; localSpeed = info.localSpeed;
            renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, 1f);
        }
        public abstract void UpdateNote(float time, out bool destroyAfterCall);
        public abstract void JudgeNote(float time, out bool destroyAfterCall, out bool makeNoLongerJudgeable);
    }
}