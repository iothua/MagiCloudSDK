using MagiCloud.KGUI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System;

namespace MagiCloud.Common
{
    /// <summary>
    /// 时间控制
    /// </summary>
    public class TimeController :MonoBehaviour
    {
        [Header("虚拟目标时间，秒为单位")]
        public float virtualTime = 10;      //虚拟时间，单位秒
        [Header("真实花费时间，秒为单位")]
        public float realTime = 20;         //真实时间，单位秒

        public int DecimalNum = 1;
        public KGUI_Toggle timeToggle;      //控制开关
        public Text showTimeText;           //显示文本
        [Header("是否显示时/分/秒，false=（分/秒）")]
        public bool showAll = false;
        private float time = 0;              //计时
        private float lastTime = 0;
        private float sTime = 0;
        private float minTime = 0;
        private float hTime = 0;
        private int dayTime = 0;
        private int status = -1;             //-1表示未开始，1表示开始，0表示暂停   
        private Timer timer;

        public UnityEvent playEvent;
        public UnityEvent pauseEvent;
        public UnityEvent stopEvent;

        public TimeEvent playingEvent;
        [Serializable]
        public class TimeEvent :UnityEvent<float> { }

        public float TempS
        {
            get
            {
                return sTime;
            }
            set
            {
                sTime=value;
                if (sTime>=60)
                {
                    sTime-=60;
                    Min+=1;
                }
            }
        }
        /// <summary>
        /// 天
        /// </summary>
        public int Day
        {
            get { return dayTime; }
            set { dayTime=value; }
        }
        /// <summary>
        /// 小时
        /// </summary>
        public int H
        {
            get { return (int)hTime; }
            set
            {
                hTime=value;
                if (H>=24)
                {
                    hTime-=24;
                    Day++;
                }
            }
        }
        /// <summary>
        /// 分钟
        /// </summary>
        public int Min
        {
            get
            {
                return (int)minTime;
            }
            set
            {
                minTime=value;
                if (minTime>=60)
                {
                    minTime-=60;
                    H+=1;
                }

            }
        }
        /// <summary>
        /// 秒
        /// </summary>
        public int S
        {
            get
            {
                return (int)TempS;
            }
        }
        /// <summary>
        /// 小时
        /// </summary>
        public string HString => H.ToString();
        /// <summary>
        /// 分钟
        /// </summary>
        public string MinString => Min.ToString();
        /// <summary>
        /// 秒
        /// </summary>
        public string SString => S.ToString();
        /// <summary>
        /// 小时
        /// </summary>
        public string HStringF => (H>=10) ? H.ToString() : "0"+H.ToString();
        /// <summary>
        /// 分钟（保持两位）
        /// </summary>
        public string MinStringF => (Min>=10) ? Min.ToString() : "0"+Min.ToString();
        /// <summary>
        /// 秒（保持两位）
        /// </summary>
        public string SStringF => (S>=10) ? S.ToString() : "0"+S.ToString();

        /// <summary>
        /// 得到时间，以分钟为单位
        /// </summary>
        /// <param name="decimalNum">保留小数点后位数</param>
        /// <returns></returns>
        public string GetTimeByMin(int decimalNum = 1)
        {
            float t = S==0 ? Min : Min+S/60f;
            return t.ToString("f"+decimalNum);
        }

        // public string TimeString => (time%1>=0.1f) ? (time).ToString("f"+DecimalNum.ToString()) : time.ToString("f0");
        //public string TextTimeString
        //{
        //    get
        //    {
        //        var str = (time).ToString("f2");
        //        return (time<10) ? "0"+str : str;
        //    }
        //}
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
            TempS+=time-lastTime;
            lastTime=time;
            if (showTimeText!=null)
                showTimeText.text=showAll ? HStringF+":"+MinStringF+":"+SStringF : MinStringF+":"+SStringF;
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
            lastTime=0;
            TempS=0;
            Min=0;
            H=0;
            Day=0;
            if (timer==null)
            {
                timer = new GameObject("timer").AddComponent<Timer>();
                timer.transform.SetParent(transform);
                timer.StartTiming(realTime,OnCompleted,OnTime);
            }
            else
            {
                timer.ConnitueTimer();
                timer.ReStartTimer(realTime);
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
