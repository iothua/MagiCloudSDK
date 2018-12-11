using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MagiCloud.Equipments;
using DG.Tweening;

namespace Chemistry.Equipments
{
    /// <summary>
    /// 盖子
    /// </summary>
    public class EO_Cap : EquipmentBase
    {
        public bool IsRotate;

        private Tween _tweenRotate;
        public Vector3 rotateValue;

        private bool isCap;
        
        /// <summary>
        /// 盖子状态
        /// </summary>
        public bool IsCap {
            get { return isCap; }
            set {

                if (isCap == value) return;
                isCap = value;

                if (isCap)
                {
                    OpenCap();
                }
                else
                {
                    CloseCap();
                }
            }
        }

        protected override void Start()
        {
            base.Start();

            OnInitializeEquipment();
        }

        public override void OnInitializeEquipment()
        {
            base.OnInitializeEquipment();
            if (IsRotate)
                _tweenRotate = transform.DORotate(rotateValue, 0.3f).SetAutoKill(false).Pause();

        }

        /// <summary>
        /// 打开盖子(第一次离开)
        /// </summary>
        public virtual void OpenCap()
        {
            transform.SetParent(null);
            if (_tweenRotate != null)
                _tweenRotate.PlayForward();
        }

        /// <summary>
        /// 关闭盖子（第一次进入）
        /// </summary>
        public virtual void CloseCap()
        {
            if (_tweenRotate != null)
                _tweenRotate.PlayBackwards();
        }
    }
}

