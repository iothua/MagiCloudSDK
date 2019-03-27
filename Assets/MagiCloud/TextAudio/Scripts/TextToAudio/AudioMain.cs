using MagiCloud.TextAudio;
using UnityEngine;
using UnityEngine.Events;
using System;
namespace MagiCloud.TextToAudio
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioMain :MonoBehaviour
    {
        private readonly string[] speakerNames = { "xiaoyan","yanping","xiaofeng","jinger","babyxu","nannan","xiaomeng","xiaolin","xiaoqian","xiaorong","xiaokun","xiaoqiang","xiaomei","dalong" };
        /// <summary>
        /// 文本输入组件
        /// </summary>
        // public KGUI.KGUI_Text text;

        private Coroutine coroutine;

        /// <summary>
        /// 控制开关
        /// </summary>
        // public Button Btn_Play;

        private UnityAction<string> onComplete;
        /// <summary>
        /// 音频组件
        /// </summary>
        private AudioSource audioSource;

        /// <summary>
        /// 文本转音频控制
        /// </summary>
        public TextToAudioController AudioController { get { return TextToAudioController.Instance; } }

        public bool IsOn { get; set; }

        public SpeekerType speeker = SpeekerType.小燕_青年女声_普通话;

        public SpeedType speedType = SpeedType.medium;

        public void SetSpeed(int speedType)
        {
            switch (speedType)
            {
                case 0:
                    Speed=50;
                    break;
                case 1:
                    Speed=25;
                    break;
                case 2:
                    Speed=36;
                    break;
                case 3:
                    Speed=75;
                    break;
                case 4:
                    Speed=100;
                    break;
                default:
                    break;
            }
        }

        public byte Speed
        {
            get { return AudioController.DefulParams.speed; }
            set
            {
                AudioController.DefulParams.speed=value;
            }
        }


        public AudioSource AudioSource1
        {
            get
            {
                return audioSource;
            }
        }

        /// <summary>
        /// 设置声音
        /// </summary>
        public void SetSpeeker(int id)
        {
            AudioController.DefulParams.voice_name = speakerNames[id];
        }



        /// <summary>
        /// 音频播放控制
        /// </summary>
        private AudioPlayer audioPlayer = new AudioPlayer();


        private void Awake()
        {
            IsOn = true;
            //  AudioController.DefulParams.voice_name = speakerNames[(int)speeker];
            SetSpeeker((int)speeker);
            SetSpeed((int)speedType);
            audioSource = gameObject.GetComponent<AudioSource>();
            // Btn_Play.onClick.AddListener(() => PlayAudio(text.Text));
        }
        private void Start()
        {

        }
        private void Update()
        {
            if (!IsOn) return;
            audioPlayer.Update();
        }
        private void OnDestroy()
        {
            AudioController.ClearCache();
        }

        public void OnlyCreateAudio(string text)
        {
            StartCoroutine(AudioController.GetAudioClip(text,(x) =>
           {
               if (x != null)
               {
                   AudioSource1.clip = x;
                   AudioSource1.Play();
                   audioPlayer.Init(text,x.length,OnComplete);
               }
           }));
        }

        public void PlayAudio(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                throw new System.ArgumentException("message",nameof(text));
            }

            if (!IsOn) return;
            if (AudioSource1.isPlaying)
            {
                AudioSource1.Stop();
                audioPlayer.Stop();
            }

            if (coroutine != null)
            {
                StopCoroutine(coroutine);
                coroutine = null;
            }
            //print(text);
            coroutine = StartCoroutine(AudioController.GetAudioClip(text,(x) =>
            {
                Play(text,x);
            }));
        }

        public void Play(string text,AudioClip x)
        {
            if (x != null)
            {
                AudioSource1.clip = x;
                AudioSource1.Play();
                audioPlayer.Init(text,x.length,OnComplete);
            }
        }

        public void TogglePause(bool on)
        {
            if (!IsOn) return;
            if (audioPlayer == null) return;
            if (!on)
            {
                AudioSource1.Pause();
                audioPlayer.Pause();
            }
            else
            {
                AudioSource1.UnPause();
                audioPlayer.UnPause();
            }
        }

        public void StopCurrent()
        {
            if (!IsOn) return;
            AudioSource1.Stop();
            audioPlayer.Stop();
        }
        public void Stop(string data)
        {
            if (!IsOn || audioPlayer.text != data) return;
            AudioSource1.Stop();
            audioPlayer.Stop();
        }

        public void RegistCallback(UnityAction<string> onComplete)
        {
            if (!IsOn) return;
            this.onComplete = onComplete;
        }
        private void OnComplete(string text)
        {
            if (onComplete != null)
                onComplete(text);
        }
    }

}
