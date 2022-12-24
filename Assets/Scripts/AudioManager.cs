using System.Collections;
using UnityEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;

namespace Simulator
{
    public enum BGMState
    {
        Stopped, Scheduling, FadingOut, Playing
    }

    public class AudioManager
    {
        private static AudioManager instance;
        private AudioSource bgmSource;
        private BGMState _state;
        private double bgmStartTime;
        private double bgmPlayTime;

        public static AudioManager Instance
        {
            get { return instance; }
        }
        public bool isPlaying => _state == BGMState.Playing;
        public BGMState State => _state;
        public float bgmTime
        {
            get
            {
                if (_state != BGMState.Playing) return -1;
                var time = Mathf.Max((float)(Time.time - bgmPlayTime + bgmStartTime), 0f);
                return time;
            }
        }

        [RuntimeInitializeOnLoadMethod]
        public static void Init()
        {
            if (instance != null) { return; }
            instance = new AudioManager();
            var gameObject = new GameObject("[Audio Source]");
            Object.DontDestroyOnLoad(gameObject);
            var bgmSource = gameObject.AddComponent<AudioSource>();
            bgmSource.playOnAwake = false; //bgmSource.minDistance = bgmSource.maxDistance = float.PositiveInfinity;
            instance.bgmSource = bgmSource;
            instance._state = BGMState.Stopped;
        }

        public static void UnLoad()
        {
            instance = null;
        }

        public void SetClip(AudioClip clip, bool loop = false)
        {
            if (isPlaying || bgmSource.clip == clip || _state != BGMState.Stopped) return;
            bgmSource.clip = clip;
            bgmSource.loop = loop;
        }

        public void Play()
        {
            if (_state != BGMState.Stopped) return;
            _state = BGMState.Playing;
            bgmSource.Play();
        }

        public async void PlayAfter(double delay)
        {  
            if (_state != BGMState.Stopped) return;
            if (delay <= double.Epsilon) { Play(); return; }
            var dspTime = AudioSettings.dspTime;
            var playTime = dspTime + delay;
            bgmStartTime = bgmSource.time;
            _state = BGMState.Scheduling;
            bgmSource.PlayScheduled(playTime);
            await UniTask.WaitUntil(() => AudioSettings.dspTime >= playTime, PlayerLoopTiming.Update);
            _state = BGMState.Playing;
            bgmPlayTime = Time.time;
        }

        public void Stop()
        {
            if (_state != BGMState.Playing) return;
            _state = BGMState.FadingOut;
            bgmSource.DOFade(0f, 1f).OnComplete(() => 
            {
                bgmSource.Stop();
				bgmSource.volume = 1f;
                _state = BGMState.Stopped;
            });

        }

        public void StopImmediately()
        {
            if (_state == BGMState.FadingOut)
                bgmSource.DOComplete(true);
            else if (_state == BGMState.Playing)
            {
                bgmSource.Stop();
				bgmSource.volume = 1f;
                _state = BGMState.Stopped;
            }
        }

        public void SetTime(float time)
        {
            if (_state != BGMState.Stopped) return;
            bgmSource.time = Mathf.Clamp(time, 0f, bgmSource.clip.length);
        }

        public void PlayEffect(AudioClip clip) => AudioSource.PlayClipAtPoint(clip, Camera.main.transform.position);

        
    }
}

