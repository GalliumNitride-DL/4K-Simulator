using UnityEngine;

namespace Simulator
{
    public abstract class Note : MonoBehaviour
    {
        public float judgeTime = -1f;
        public int track = -1;
        public float localSpeed = 1f;
        public float duration = 0f;
        public new SpriteRenderer renderer;

        protected virtual void Awake()
        {
            renderer.enabled = false;
            renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, 1f);
            var scale = GameController.Instance.halfScreenWidth / 2 - 100f;
            transform.localScale = new Vector3(scale, scale, scale);
        }
        public virtual void ReInitialize(NoteInfo info)
        {
            judgeTime = info.time; track = info.track; localSpeed = info.localSpeed; duration = info.duration;
            renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, 1f);
            if (judgeTime >= 0) renderer.enabled = true;
        }
        public abstract void UpdateNote(float time, out bool destroyAfterCall);
        public abstract void JudgeNote(float time, out bool destroyAfterCall, out bool makeNoLongerJudgeable);
        protected void PlayEffect(int index)
        {
            Destroy(Instantiate(NoteManager.Instance.FXs[index], transform.position, Quaternion.identity), 0.5f);
        }
    }
}