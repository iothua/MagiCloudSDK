using UnityEngine;
using System.Collections;
using MagiCloud.Equipments;

namespace Chemistry.Equipments
{
    /// <summary>
    /// 封盖，用于封住仪器
    /// </summary>
    public class EO_Cover : EquipmentBase
    {
        public CapOperateType capOperate = CapOperateType.唯一型;

        private bool isCover;

        public bool IsCover
        {
            get {
                return isCover;
            }
            set
            {

                if (isCover == value) return;
                isCover = value;

                if (isCover)
                {
                    OpenCover();
                }
                else
                {
                    CloseCover();
                }
            }
        }

        protected override void Start()
        {
            base.Start();

            OnInitializeEquipment();
        }

        public virtual void OpenCover()
        {

        }

        /// <summary>
        /// 关闭盖子（第一次进入）
        /// </summary>
        public virtual void CloseCover()
        {

        }
    }
}

