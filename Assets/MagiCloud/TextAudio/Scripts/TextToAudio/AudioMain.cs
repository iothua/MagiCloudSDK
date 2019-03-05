using MagiCloud.TextAudio;
using UnityEngine;
using UnityEngine.Events;
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

        private SpeekerType speeker = SpeekerType.小燕_青年女声_普通话;
        public SpeekerType Speeker
        {
            get
            {
                return speeker;
            }

            set
            {
                speeker=value;
                SetSpeeker((int)speeker);
            }
        }

        private SpeedType speed;
        public SpeedType Speed
        {
            get { return speed; }
            set
            {
                speed=value;
                //    AudioController.DefulParams.speed_increase=(byte)speed;
            }
        }
        /// <summary>
        /// 设置声音
        /// </summary>
        private void SetSpeeker(int id)
        {
            AudioController.DefulParams.voice_name=speakerNames[id];
        }



        /// <summary>
        /// 音频播放控制
        /// </summary>
        private AudioPlayer audioPlayer = new AudioPlayer();


        private void Awake()
        {
            IsOn=true;
            AudioController.DefulParams.voice_name= speakerNames[(int)speeker];
            audioSource=gameObject.GetComponent<AudioSource>();
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

        public void PlayAudio(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                throw new System.ArgumentException("message",nameof(text));
            }

            if (!IsOn) return;
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
                audioPlayer.Stop();
            }
            StartCoroutine(AudioController.GetAudioClip(text,(x) =>
            {
                if (x!=null)
                {
                    audioSource.clip=x;
                    audioSource.Play();
                    audioPlayer.Init(text,x.length,OnComplete);
                }
            }));
        }
        public void TogglePause(bool on)
        {
            if (!IsOn) return;
            if (audioPlayer==null) return;
            if (!on)
            {
                audioSource.Pause();
                audioPlayer.Pause();
            }
            else
            {
                audioSource.UnPause();
                audioPlayer.UnPause();
            }
        }

        public void StopCurrent()
        {
            if (!IsOn) return;
            audioSource.Stop();
            audioPlayer.Stop();
        }
        public void Stop(string data)
        {
            if (!IsOn||audioPlayer.text!=data) return;
            audioSource.Stop();
            audioPlayer.Stop();
        }

        public void RegistCallback(UnityAction<string> onComplete)
        {
            if (!IsOn) return;
            this.onComplete=onComplete;
        }
        private void OnComplete(string text)
        {
            if (onComplete!=null)
                onComplete(text);
        }
    }

}
