using MagiCloud.KGUI;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace MagiCloud.Common
{
    /// <summary>
    /// 时间控制
    /// </summary>
    public class TimeController :SerializedMonoBehaviour
    {
        public float virtualTime = 10;      //虚拟时间，单位秒
        public float realTime = 20;         //真实时间，单位秒
        public KGUI_Toggle timeToggle;      //控制开关
        public Text showTimeText;           //显示文本

        private float time = 0;              //计时
        private int status = -1;             //-1表示未开始，1表示开始，0表示暂停   
        private Timer timer;

        public UnityEvent playEvent;
        public UnityEvent pauseEvent;
        public UnityEvent stopEvent;
        public UnityEvent<float> playingEvent;
        public int DecimalNum = 1;
        //public int h => virtualTime/3600;
        //public int min => virtualTime/60;
        //public float s => virtualTime;
        public string TimeString => (time%1>=0.1f) ? (time).ToString("f"+DecimalNum.ToString()) : time.ToString("f0");

        public float Progress { get; private set; }

        public bool Playing => status==1;
        public bool Pausing => status==0;
        public bool Stopping => status==-1;

        private void Start()
        {
            timeToggle?.OnValueChanged.AddListener(OnChange);
        }

        #region 计时

        public void OnChange(bool play)
        {
            if (status==-1)
                Play();
            else if (status==0)
                Connitue();
            else
                Pause();
        }


        private void OnTime(float t)
        {
            Progress=t;
            time =t*virtualTime;
            if (showTimeText!=null)
                showTimeText.text=TimeString;

            playingEvent?.Invoke(t);
        }

        private void OnCompleted()
        {
            if (timeToggle!=null)
                timeToggle.IsValue=false;
            stopEvent?.Invoke();
            status=-1;
        }

        public void Pause()
        {
            pauseEvent?.Invoke();
            timer.PauseTimer();
            status=0;
        }

        public void Connitue()
        {
            timer.ConnitueTimer();
            status=1;
        }

        public void Play()
        {
            if (timer==null)
            {
                timer = new GameObject("timer").AddComponent<Timer>();
                timer.transform.SetParent(transform);
                timer.StartTiming(realTime,OnCompleted,OnTime);
            }
            else
            {
                timer.ConnitueTimer();
                timer.ReStartTimer();
            }
            playEvent?.Invoke();
            status =1;
        }

        #endregion
        private void OnDestroy()
        {

            timeToggle?.OnValueChanged.RemoveListener(OnChange);

            if (timer!=null)
                Destroy(timer);
        }
    }
}
