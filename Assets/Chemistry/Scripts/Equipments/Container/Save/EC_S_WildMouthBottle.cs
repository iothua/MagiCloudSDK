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
        

        public Vector3 LocalPosition {
            get {
                if (EquipmentDrug == null) return Vector3.zero;
                if (!EquipmentDrug.addModel) return Vector3.zero;

                return EquipmentDrug.drugInteractionModel.transformData.localPosition.Vector;
            }
        }

        public Vector3 LocalRotation
        {
            get {
                if (EquipmentDrug == null) return Vector3.zero;
                if (!EquipmentDrug.addModel) return Vector3.zero;

                return EquipmentDrug.drugInteractionModel.transformData.localRotation.Vector;
            }
        }

        /// <summary>
        /// 药品模型
        /// </summary>
        public GameObject DrugObject {
            get {
                if (EquipmentDrug == null) return null;
                if (!EquipmentDrug.addModel) return null;

                return EquipmentDrug.drugInteractionModel.modelObject;
            }
        }

        [SerializeField, Header("药匙一次取药的量")]
        private float takeAmount = 10;
        public float TakeAmount { get { return takeAmount; } }

        private EquipmentBase inInteractionEquipment;
        public EquipmentBase InInteractionEquipment
        {
            get { return inInteractionEquipment; }
            set { inInteractionEquipment = value; }
        }

        public DropperInteractionType InteractionEquipment { get { return DropperInteractionType.广口瓶; } }
        private DrugSystem _drugSystem;         //药品系统
        
        [SerializeField, Header("药匙取药时动画下降的数值")]
        private float height;

        /// <summary>
        /// 药匙下降高度
        /// </summary>
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
        }
        
        public override bool IsCanInteraction(InteractionEquipment interaction)
        {
            if ( InInteractionEquipment!= null && InInteractionEquipment != interaction.Equipment) return false;


            if (!base.IsCanInteraction(interaction)) return false;

            if (!isOpen) return false;

            if (interaction.Equipment is ET_Spoon)
            {
                return true;
            }
            return true;
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
