using UnityEngine;
using System.Collections;
using Chemistry.Data;
using MagiCloud.Interactive;
using Chemistry.Equipments.Actions;

namespace Chemistry.Equipments
{
    /// <summary>
    /// 烧杯
    /// 开发者：阮榆皓
    /// </summary>
    public class EC_Beaker : EC_Container
    {
        //液面变化功能--在编辑器初始化时附加对应脚本
        [SerializeField]
        private EA_EquipmentLiquidChange LiquidChange;

        //[Header("初始液体体积(为负则取默认配置值)")]
        //public float CurV = -1.0f;

        [Header("放入的仪器下降至的高度值(如：温度计,玻璃棒)")]
        public float FallHight = 1.03f;

        //用于胶头滴管获取当前仪器类型--可能会用到
        //public DropperInteractionType InteractionEquipment
        //{
        //    get
        //    {
        //        return DropperInteractionType.烧杯;
        //    }
        //}

        protected override void Start()
        {
            base.Start();

            //Debug.Log("EC_Beaker 的Start被调用");
            OnInitializeEquipment();
        }

        public override void OnInitializeEquipment()
        {
            containerType = EContainerType.烧杯;
            base.OnInitializeEquipment();

            if (LiquidChange==null)
            {
                LiquidChange = GetComponent<EA_EquipmentLiquidChange>();
            }

            LiquidChange.OnInit(LiquidEffect, Volume);
        }

        //这里在编辑器窗口进行了设置
        /// <summary>
        /// 碰撞体大小和中心点设置
        /// </summary>
        /// <param name="equipmentName"></param>
        public override void OnInitializeEquipment_Editor(string equipmentName)
        {
            base.OnInitializeEquipment_Editor(equipmentName);

            containerType = EContainerType.烧杯;

            //var boxCollider = Collider as BoxCollider;
            //boxCollider.center = new Vector3(0.0f, 0.5f, 0.0f);
            //boxCollider.size = new Vector3(0.74f, 1.0f, 0.74f);
            //添加容器液面升降行为
            LiquidChange = gameObject.AddComponent<EA_EquipmentLiquidChange>();

        }

        /// <summary>
        /// 测试
        /// </summary>
        //protected override void Update()
        //{
        //    if (Input.GetKeyDown(KeyCode.Q))
        //    {
        //        ChangeLiquid(50.0f);
        //    }
        //    if (Input.GetKeyDown(KeyCode.W))
        //    {
        //        ChangeLiquid(-50.0f);
        //    }
        //    base.Update();
        //}


        /// <summary>
        /// 液体量变化
        /// </summary>
        /// <param name="changeVolume">变化量(正为增，负为减)</param>
        /// <param name="time">时间（为0时突变）</param>
        public void ChangeLiquid(float changeVolume, float time = 0.5f)
        {
            //DrugSystemIns.AllDrugs[DrugName].Volume = LiquidChange.ChangeLiquid(DrugSystemIns.AllDrugs[DrugName].Volume, changeVolume, time);
            var drug = DrugSystemIns.GetDrug(DrugName);
            drug.Volume = LiquidChange.ChangeLiquid(drug.Volume, changeVolume, time);
        }

        /// <summary>
        /// 可交互条件
        /// </summary>
        /// <param name="interaction"></param>
        /// <returns></returns>
        public override bool IsCanInteraction(InteractionEquipment interaction)
        {
            //if (interaction.Equipment is ET_GlassBar)
            //{
            //    return true;
            //}
            return base.IsCanInteraction(interaction);
        }


    }
}
