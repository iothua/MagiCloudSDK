using System;

namespace MagiCloud.Common
{
    /// <summary>
    /// 时间数值
    /// </summary>
    public struct MTimerValue
    {
        public int Value;

        public MTimerValue(float value)
        {
            this.Value = (int)value;
        }

        public TimeSpan Time {
            get {
                return new TimeSpan(0, 0, 0, Value);
            }
        }

        /// <summary>
        /// 根据不同的格式，转化为相应的需求
        /// </summary>
        /// <returns>The format.</returns>
        /// <param name="format">Format.</param>
        public string ToFormat(string format)
        {
            string[] datas = format.Split(':');

            switch (datas.Length)
            {
                case 0:
                    return Value.ToString().PadLeft(2, '0');
                case 1:
                    return (Time.Minutes + (Time.Hours + Time.Days * 24) * 60).ToString().PadLeft(2, '0') + ":" + Time.Milliseconds.ToString().PadLeft(2, '0');
                case 2:
                    return (Time.Hours + Time.Days * 24).ToString().PadLeft(2, '0') + ":" + Time.Minutes.ToString().PadLeft(2, '0') + ":" + Time.Milliseconds.ToString().PadLeft(2, '0');
                case 3:
                    return Time.Days.ToString().PadLeft(2, '0') + ":" + Time.Hours.ToString().PadLeft(2, '0') + 
                        ":" + Time.Minutes.ToString().PadLeft(2, '0') + ":" + Time.Milliseconds.ToString().PadLeft(2, '0');
            }

            //先暂时这么处理
            return Value.ToString();
        }
    }

    /// <summary>
    /// 时间数据
    /// 用途：比如你需要设置多少时间段，才开始执行你需要的功能。如铁钉生锈需要第七天才开始生锈，
    /// 那么你可以实例化这个字段，设置初始值，之后在根据MTimer的Time属性与这个结构类的Time属性去做比对。
    /// </summary>
    [Serializable]
    public struct MTimerData
    {
        public int day;//天
        public int house;//时
        public int minute;//分
        public int second;//秒

        public TimeSpan Time {
            get {
                return new TimeSpan(day, house, minute, second);
            }
        }

        public MTimerData(int day, int house, int minute, int second)
        {
            this.day = day;
            this.house = house;
            this.minute = minute;
            this.second = second;
        }

        public MTimerData(MTimerData timerData)
        {
            this.day = timerData.day;
            this.house = timerData.house;
            this.minute = timerData.minute;
            this.second = timerData.second;
        }
    }

    /// <summary>
    /// 计时器
    /// </summary>
    public class MTimer
    {

        public float startTime;
        public Action onStart;
        public MTimerData duration;
        public Action onEnd;

        public float sumTime;

        public MTimerValue TimerValue {
            get {
                return new MTimerValue(sumTime);
            }
        }

        /// <summary>
        /// 时间运行速度，默认为1
        /// </summary>
        /// <value>The time speed.</value>
        public float TimeSpeed { get; set; }

        public bool IsPlaying { get; private set; }
        public bool IsPaurse { get; set; }

        public Action<MTimerValue, float> TimerValues;

        /// <summary>
        /// 具有时间延迟
        /// </summary>
        /// <param name="startTime">开始时间.</param>
        /// <param name="start">开始事件</param>
        /// <param name="duration">时长</param>
        /// <param name="end">结束事件</param>
        public MTimer(float startTime = 0, Action start = null, Action<MTimerValue,float> timerValue = null, Action end = null,float timeSpeed = 1)
        {
            this.startTime = startTime;
            this.onStart = start;

            this.duration = new MTimerData
            {
                second = -1
            };

            this.onEnd = end;

            TimeSpeed = timeSpeed;
            sumTime = 0;

            IsPlaying = false;
            IsPaurse = false;

            TimerValues = timerValue;

            MTimerController.Timers.Add(this);
        }

        /// <summary>
        /// 每帧时长
        /// </summary>
        /// <param name="deletaTime">Deleta time.</param>
        public void OnUpdate(float deletaTime)
        {
            if (IsPaurse) return;

            sumTime += deletaTime * TimeSpeed;//添加时间系数

            if (!IsPlaying)
            {
                if(sumTime>=startTime)
                {
                    IsPlaying = true;

                    onStart?.Invoke();
                }
            }
            else
            {
                TimerValues?.Invoke(TimerValue, deletaTime);

                if (duration.Time.TotalSeconds > 0)
                {
                    if (TimerValue.Time.TotalSeconds >= duration.Time.TotalSeconds)
                    {

                        OnDestoryTime();
                    }
                }
            }
        }

        public void OnDestoryTime()
        {
            MTimerController.Timers.Remove(this);

            IsPlaying = false;

            onEnd?.Invoke();
        }
    }
}
