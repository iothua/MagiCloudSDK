using System;
using System.Linq;
using Chemistry.Chemicals;
using Chemistry.Data;
using MagiCloud.Equipments;
using MagiCloud.Interactive;
using UnityEngine;

namespace Chemistry.Equipments
{
    /// <summary>
    /// 广口瓶
    /// </summary>
    public class EC_S_WildMouthBottle : EC_Save, I_ET_S_SpoonTake
    {
        #region I_ET_S_SpoonTake接口属性
        [SerializeField, Header("小份药品在药匙内的本地坐标")]
#pragma warning disable CS0436
        private Vector3 localPosition;
#pragma warning restore CS0436
        public Vector3 LocalPosition { get { return localPosition; } }

        [SerializeField, Header("小份药品在药匙内的本地旋转")]
        private Vector3 localRotation;
        public Vector3 LocalRotation
        {
            get { return Vector3.zero; }
        }

        [SerializeField, Header("药匙一次取药的量")]
        private float takeAmount = 10;
        float I_ET_S_SpoonTake.TakeAmount { get { return takeAmount; } }

        private EquipmentBase inInteractionEquipment;
        public EquipmentBase InInteractionEquipment
        {
            get { return inInteractionEquipment; }
            set { inInteractionEquipment = value; }
        }

        public DropperInteractionType InteractionEquipment { get { return DropperInteractionType.广口瓶; } }
        private DrugSystem _drugSystem;         //药品系统
        

        [SerializeField]
        private string _onlyDrugName;           //滴管内吸取过的药品名字
        public string OnlyDrugName
        {
            get { return _onlyDrugName ?? (_onlyDrugName = ""); }
        }

        [SerializeField, Header("药匙取药时动画下降的数值")]
        private float height;

        public float Height { get { return height; } } 
        #endregion

        protected override void Start()
        {
            base.Start();
            OnInitializeEquipment();
        }

        public override void OnInitializeEquipment()
        {
            base.OnInitializeEquipment();
            CloseCap();
        }
        
        public override bool IsCanInteraction(InteractionEquipment interaction)
        {
            //普通盖子
            if (interaction.Equipment is EO_Cap)
            {
                if (interaction.Equipment == _Cap) return true;
                if (_Cap.IsCap)     //盖子打开状态
                    return true;
                else
                    return false;
            }
            if (interaction.Equipment as ET_Spoon)
            {
                if (_Cap == null) return true;
                if (_Cap.IsCap)     //盖子打开状态
                {
                    return true;    //药品系统暂时有问题
                    //if (DrugSystemIns.CurSumVolume >= takeAmount)
                    //    return true;
                    //else
                    //    return false;
                }
                else
                    return false;
            }
            return _Cap.IsCap;
        }
        public override void OnDistanceRelease(InteractionEquipment interaction)
        {
            base.OnDistanceRelease(interaction);
            if (interaction.Equipment is EO_Cap)
                DropperJoin();
            if (interaction.Equipment is ET_Spoon)
            {
                InInteractionEquipment = interaction.Equipment;
                DropperJoin();
            }
        }
        public override void OnDistanceExit(InteractionEquipment interaction)
        {
            base.OnDistanceExit(interaction);
            if (interaction.Equipment is EO_Cap)
                DropperLeave();
            if (interaction.Equipment is ET_Spoon)
            {
                InInteractionEquipment = null;
                DropperLeave();
            }
        }
        /// <summary>
        /// 盖子加入
        /// </summary>
        public void DropperJoin()
        {
            //滴管加入后，应该执行一段动画，缓慢的放回去
            CloseCap();
        }

        /// <summary>
        /// 盖子离开
        /// </summary>
        public void DropperLeave()
        {
            //滴灌离开时，应该限制一段距离，然后在离开
            OpenCap();
        }
    }
}
