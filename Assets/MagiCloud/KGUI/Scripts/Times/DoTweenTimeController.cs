using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MagiCloud.KGUI;
using DG.Tweening;
using UnityEngine.UI;

namespace MagiCloud.Common
{
    [DefaultExecutionOrder(-500)]
    public class DoTweenTimeController : MonoBehaviour
    {
        public KGUI_Toggle toggle;

        public Tween tween;

        public int Value = 1;
        public int SumTime;
        private bool IsComplete = false;

        public float percet;//进度条

        public Text txtTime;

        public Core.Events.Handlers.EventFloat OnPercet = new Core.Events.Handlers.EventFloat();
        public Core.Events.Handlers.EventFloat OnStart = new Core.Events.Handlers.EventFloat();
        public Core.Events.Handlers.EventNone OnParus = new Core.Events.Handlers.EventNone();
        public Core.Events.Handlers.EventNone OnStop = new Core.Events.Handlers.EventNone();

        public static DoTweenTimeController Instance {
            get; private set;
        }

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            toggle.OnValueChanged.AddListener(OnValueChanged);

            StartDoTween();

            tween.OnComplete(() =>
            {
                IsComplete = true;
                toggle.IsValue = false; //完成后，设置为默认。
            });

        }

        void StartDoTween()
        {
            tween = DOTween.To(() => Value, x => Value = x, SumTime, SumTime);
            tween.SetEase(Ease.Linear);
            tween.Pause();
            tween.OnUpdate(OnUpdate);
        }

        void OnUpdate()
        {
            percet = Value / (float)SumTime;

            OnPercet.SendListener(percet);

            txtTime.text = Value.ToString();
        }

        void OnValueChanged(bool result)
        {
            if (IsComplete)
            {
                IsComplete = false;
                Value = 0;
                //OnUpdate();
                //StartDoTween();

                OnStop.SendListener();
            }
            else
            {
                if (result)
                {
                    OnStart.SendListener(SumTime);
                    tween.Play();
                }
                else
                {
                    OnParus.SendListener();
                    tween.Pause();
                }
            }
        }

    }
}

