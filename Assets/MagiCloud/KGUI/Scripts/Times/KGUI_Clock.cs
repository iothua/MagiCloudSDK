using UnityEngine;
using MagiCloud.Common;
using System;
using UnityEngine.UI;

namespace MagiCloud.KGUI
{
    /// <summary>
    /// 钟表
    /// </summary>
    public class KGUI_Clock : MonoBehaviour 
    {
        public Transform houseTransform; //小时
        public Transform minuteTransform;//分钟

        private MTimer timer;
        public Text txtDay;

        private int day = 0;
        private int house = 0;

        public bool IsPlaying {
            get {
                if (timer == null) return false;
                return timer.IsPlaying;
            }
        }

        public bool IsPaurse {
            get {
                if (timer == null) return false;
                return timer.IsPaurse;
            }
            set {
                if (timer == null) return;
                timer.IsPaurse = value;
            }
        }

        private void OnDestroy()
        {
            if (timer != null)
                timer.OnDestoryTime();
        }

        public void StartClock(MTimerData duration, Action<MTimerValue, float> timerAction, Action start = null, Action end = null, float timeSpeed = 1)
        {
            timer = new MTimer(start: start, timerValue: (timerValue, deleta) =>
              {

                  if (house != timerValue.Time.Hours)
                  {
                      house = timerValue.Time.Hours;
                      timer.sumTime = day * 24 * 60 * 60 + house * 60 * 60;
                  }

                  if (day != timerValue.Time.Days)
                  {
                      day = timerValue.Time.Days;
                      
                      if(txtDay!=null)
                      {
                        //校验时间
                        txtDay.text = timerValue.Time.Days.ToString();
                      }
                      
                      timer.sumTime = day * 24 * 60 * 60;
                  }

                  timerAction?.Invoke(timer.TimerValue, deleta);
                  //根据钟表
                  SetHouse(timer.TimerValue);

              }, timeSpeed: timeSpeed, end: end);

            timer.duration = new MTimerData(duration);
        }

        void SetHouse(MTimerValue timerValue)
        {
            float minuteRotation = timerValue.Time.Minutes * 6;

            if (houseTransform != null && minuteTransform != null)
            {
                houseTransform.localRotation = Quaternion.Euler(0, 0, 90 - (timerValue.Time.Hours * 30 + minuteRotation / 12));
                minuteTransform.localRotation = Quaternion.Euler(0, 0, 90 - minuteRotation);
            }
        }
    }
}
