using System;
using UnityEngine;
using MagiCloud.Equipments;

namespace Chemistry.Equipments.Actions
{
    /// <summary>
    /// 动作处理
    /// </summary>
    public class EA_Handle
    {
        /// <summary>
        /// 持续时间
        /// </summary>
        public float duration { get; set; }

        /// <summary>
        /// 是否开始
        /// </summary>
        public bool IsStart { get; set; }

        /// <summary>
        /// 是否完成
        /// </summary>
        public bool IsComplete { get; set; }

        /// <summary>
        /// 开始
        /// </summary>
        public Action OnStart { get; set; }

        /// <summary>
        /// 成功
        /// </summary>
        public Action OnSuccess { get; set; }

        private float startTime;

        public EquipmentBase Equipment { get; private set; }

        public bool IsBreak { get; set; }

        public void OnComplete(Action action)
        {
            OnSuccess = action;

        }

        public EA_Handle(EquipmentBase equipment, Action onStart, float duration = 1.0f)
        {
            this.OnStart = onStart;
            this.OnSuccess = null;

            this.duration = duration;

            IsStart = false;
            IsComplete = false;

            startTime = 0;

            Equipment = equipment;

            EA_HandleController.OnAddHandle(this);
            IsBreak = false;
            Equipment.EventDestory += OnBreak;
        }

        void OnBreak()
        {
            Equipment.EventDestory -= OnBreak;
            IsBreak = true;
            EA_HandleController.OnRemoveHandle(this);
        }

        public void OnUpdate()
        {
            if (IsBreak) return;

            if (!IsStart)
            {
                IsStart = true;

                if (OnStart != null)
                {
                    OnStart();
                }

                if (Equipment != null)
                {
                    Equipment.IsEnable = false;
                }
            }
            else
            {
                if (startTime >= duration)
                {
                    IsComplete = true;

                    if (OnSuccess != null)
                    {
                        OnSuccess();
                    }

                    EA_HandleController.OnRemoveHandle(this);
                    Equipment.IsEnable = true;
                }

                startTime += Time.deltaTime;
            }
        }

        
    }
}
