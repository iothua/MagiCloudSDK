using UnityEngine;
namespace MagiCloud.Common
{
    /// <summary>
    /// 计时器
    /// </summary>
    public class Timer :MonoBehaviour
    {

        #region 

        public delegate void CompleteEvent();
        public delegate void UpdateEvent(float t);

        // private Transform root;

        bool isLog = true;

        UpdateEvent updateEvent;

        CompleteEvent onCompleted;

        float timeTarget;   // 计时时间/  

        float timeStart;    // 开始计时时间/  

        float timeNow;     // 现在时间/  

        float offsetTime;   // 计时偏差/  

        bool isTimer;       // 是否开始计时/  

        bool isDestory = true;     // 计时结束后是否销毁/  

        bool isEnd;         // 计时是否结束/  

        bool isIgnoreTimeScale = true;  // 是否忽略时间速率  

        bool isRepeate;

        float Time
        {
            get { return isIgnoreTimeScale ? UnityEngine.Time.realtimeSinceStartup : UnityEngine.Time.time; }
        }
        float now;
        private void Start()
        {
            // root = transform.Find("root");
        }
        // Update is called once per frame  
        void Update()
        {
            if (isTimer)
            {
                timeNow = Time - offsetTime;
                now = timeNow - timeStart;
                if (updateEvent != null)
                {
                    float t = Mathf.Clamp01(now / timeTarget);
                    updateEvent(t);
                    // if (root != null)
                    //   root.localEulerAngles = new Vector3(0, 0, -t * 360);
                }
                if (now > timeTarget)
                {
                    if (onCompleted != null)
                        onCompleted();
                    if (!isRepeate)
                        Stop();
                    else
                        ReStartTimer();
                }
            }
        }
        public float GetLeftTime()
        {
            return Mathf.Clamp(timeTarget - now,0,timeTarget);
        }
        //void OnApplicationPause(bool isPause_)
        //{
        //    if (isPause_)
        //    {
        //        PauseTimer();
        //    }
        //    else
        //    {
        //        ConnitueTimer();
        //    }
        //}

        /// <summary>  
        /// 计时结束  
        /// </summary>  
        public void Stop()
        {
            isTimer = false;
            isEnd = true;
            if (isDestory)
                Destroy();
        }

        public void Destroy()
        {
            Destroy(gameObject);
        }

        float pauseTime;
        /// <summary>  
        /// 暂停计时  
        /// </summary>  
        public void PauseTimer()
        {
            if (isEnd)
            {
                if (isLog) Debug.LogWarning("计时已经结束！");
            }
            else
            {
                if (isTimer)
                {
                    isTimer = false;
                    pauseTime = Time;
                }
            }
        }
        /// <summary>  
        /// 继续计时  
        /// </summary>  
        public void ConnitueTimer()
        {
            if (isEnd)
            {
                if (isLog) Debug.LogWarning("计时已经结束！请从新计时！");
            }
            else
            {
                if (!isTimer)
                {
                    offsetTime += (Time - pauseTime);
                    isTimer = true;
                }
            }
        }
        public void ReStartTimer()
        {
            timeStart = Time;
            offsetTime = 0;
        }

        public void ChangeTargetTime(float time)
        {
            timeTarget += time;
        }
        /// <summary>  
        /// 开始计时 :   
        /// </summary>  
        public void StartTiming(float time,CompleteEvent onCompleted,UpdateEvent update = null,bool isIgnoreTimeScale = true,bool isRepeate = false,bool isDestory = true)
        {
            timeTarget = time;
            if (onCompleted != null)
                this.onCompleted = onCompleted;
            if (update != null)
                updateEvent = update;
            this.isDestory = isDestory;
            this.isIgnoreTimeScale = isIgnoreTimeScale;
            this.isRepeate = isRepeate;

            timeStart = Time;
            offsetTime = 0;
            isEnd = false;
            isTimer = true;
        }

        #endregion

    }
}

