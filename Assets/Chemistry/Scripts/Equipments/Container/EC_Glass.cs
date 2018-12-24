using Chemistry.Chemicals;
using Chemistry.Data;
using MagiCloud.Equipments;
using UnityEngine;
using Sirenix.OdinInspector;
using MagiCloud.Interactive;
using MagiCloud;
using Chemistry.Interactions;
using System.Collections.Generic;

namespace Chemistry.Equipments
{
    /// <summary>
    /// 玻璃杯
    /// </summary>
    public class EC_Glass : EC_Save, I_ET_S_SpoonPut, ISlideBottle,IStir,I_ET_C_ClampPut
    {
        [SerializeField]
        [LabelText("药匙的下降高度")]
        private float spoonHeight = 0.5f;

        [SerializeField]
        [LabelText("镊子的下降高度")]
        private float clampPutHeight = 1.5f;

        [Title("玻璃片参数配置")]

        [SerializeField]
        private AxisLimits axisLimits;

        [SerializeField]
        [InfoBox("在边界内限制移动的范围，比如限制Y轴移动")]
        [LabelText("边界移动范围")]
        private Vector2 limitRange;

        [SerializeField]
        [LabelText("瓶口边界")]
        private Vector2 bound;



        protected override void Start()
        {
            base.Start();

            OnInitializeEquipment();
        }

        public float SpoonPutHeight {
            get {
                return spoonHeight;
            }
        }

        public DropperInteractionType InteractionEquipment {
            get { return DropperInteractionType.玻璃杯; }
        }

        public EquipmentBase InInteractionEquipment {
            get; set;
        }

        public Vector2 Bound {
            get {
                return bound + new Vector2(transform.position.x, transform.position.x);
            }
        }

        public Vector2 LimitRange {
            get {
                return limitRange;
            }
        }

        public float OpenRate {
            get {
                return SlideCover == null ? 1f : SlideCover.GetRange(this);
            }
        }

        public ISlideCover SlideCover { get; set; }
        public AxisLimits AxisLimits {
            get {
                return axisLimits;
            }
        }


        public bool AllowStir {
            get {
                return DrugSystemIns.FindDrug(EDrugType.Liquid) && DrugSystemIns.FindDrug(EDrugType.Solid_Powder) 
                    || DrugSystemIns.FindDrug(EDrugType.Solution);
            }
        }

        public float ClampPutHeight {
            get {
                return clampPutHeight;
            }

            set {
                clampPutHeight = value;
            }
        }

        public bool CanReceive { get; set; }

        public void OnDripDrug(DrugData drugData)
        {
            DrugSystemIns.AddDrug(drugData.DrugName, drugData.Volume);
        }

        public override bool IsCanInteraction(InteractionEquipment interaction)
        {
            if (InInteractionEquipment != null && InInteractionEquipment != interaction.Equipment) return false;

            if (!base.IsCanInteraction(interaction)) return false;

            if (!isOpen) return false;

            if (interaction.Equipment is EO_Cover) return true;

            //如果是药匙，则进行处理
            if (interaction.Equipment is ET_Spoon)
            {
                InInteractionEquipment = interaction.Equipment;

                return true;
            }

            if (interaction is InteractionPourWater)
            {
                return true;
            }

            return false;
        }

        public override void OnDistanceStay(InteractionEquipment interaction)
        {
            base.OnDistanceStay(interaction);

            if (SlideCover != null && isOpen)
            {
                CloseCap();
            }
        }

        public override void OnDistanceRelease(InteractionEquipment interaction)
        {
            base.OnDistanceRelease(interaction);

            if (interaction.Equipment is EO_Cover)
            {
                CloseCap();
            }
        }

        public override void OnDistanceExit(InteractionEquipment interaction)
        {
            base.OnDistanceExit(interaction);

            if (interaction.Equipment is EO_Cover)
            {
                OpenCap();
            }

            if (interaction.Equipment is ET_Spoon)
                InInteractionEquipment = null;
        }

        public void StartStir(ET_GlassBar glassBar)
        {
        }

        public void StopStir()
        {
        }

        public void OnClampPut()
        {
            
        }
    }
}
