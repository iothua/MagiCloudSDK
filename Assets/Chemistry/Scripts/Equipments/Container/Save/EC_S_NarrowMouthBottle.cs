using System;
using UnityEngine;
using MagiCloud.Interactive;
using Chemistry.Data;
using MagiCloud.Equipments;

namespace Chemistry.Equipments
{
    /// <summary>
    /// 容器-存储容器-细口瓶
    /// 功能：  I. 1.容积 2.液体
    ///         II.盖子，药品系统
    ///         III.倒水
    /// </summary>
    public class EC_S_NarrowMouthBottle : EC_Save, I_ET_D_BreatheIn
    {
        /// <summary>
        /// 胶头滴管下落的高度
        /// </summary>
        public float Height { get { return 0.75f; } set { } }
        public DropperInteractionType InteractionEquipment
        {
            get
            {
                return DropperInteractionType.细口瓶;
            }
        }
        private EquipmentBase inInteractionEquipment;
        public EquipmentBase InInteractionEquipment
        {
            get
            {
                return inInteractionEquipment;
            }

            set
            {
                inInteractionEquipment = value;
            }
        }

        protected override void Start()
        {
            base.Start();
            OnInitializeEquipment();
        }

        public override void OnInitializeEquipment()
        {
            containerType = EContainerType.细口瓶;
            if (_Cap != null)
            {
                //OpenCap();      //初始打开，滴管初始交互后关闭

            }

            base.OnInitializeEquipment();
        }

        public override bool IsCanInteraction(InteractionEquipment interaction)
        {
            //普通盖子
            if (interaction.Equipment is EO_Cap)
            {
                if (_Cap.IsCap)     //盖子打开状态
                    return true;
                else
                    return false;
            }
            //滴管
            if (interaction.Equipment is ET_Dropper)
            {
                return true;
                //if (InInteractionEquipment != null && InInteractionEquipment != interaction.Equipment) return false;
                //if (_Cap == null) return true;
                //if (_Cap.IsCap)     //盖子打开状态
                //{
                //    ET_Dropper dropper = interaction.Equipment as ET_Dropper;
                //    if (DrugSystemIns.CurSumVolume >= dropper.maxVolume)
                //    {
                //        InInteractionEquipment = interaction.Equipment;
                //        return true;
                //    }
                //    else
                //        return false;
                //}
                //else
                //    return false;
            }
            return _Cap.IsCap;
        }

        public override void OnDistanceRelease(InteractionEquipment interaction)
        {

            base.OnDistanceRelease(interaction);

            if (interaction.Equipment is EO_Cap)
                DropperJoin();
            if (interaction.Equipment is ET_Dropper)
                DropperJoin();
        }

        public override void OnDistanceExit(InteractionEquipment interaction)
        {
            base.OnDistanceExit(interaction);

            if (interaction.Equipment is EO_Cap)
                DropperLeave();
            if (interaction.Equipment is ET_Dropper)
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

        public override void OnInitializeEquipment_Editor(string equipmentName)
        {
            base.OnInitializeEquipment_Editor(equipmentName);

            var boxCollider = Collider as BoxCollider;
            boxCollider.center = new Vector3(0.00414f, 0.347f, -0.012f);
            boxCollider.size = new Vector3(0.537f, 0.708f, 0.457f);
        }
    }
}
