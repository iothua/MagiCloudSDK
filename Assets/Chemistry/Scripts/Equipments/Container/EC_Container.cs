using UnityEngine;
using MagiCloud.Equipments;
using Chemistry.Chemicals;
using Chemistry.Liquid;
using Chemistry.Data;
using Sirenix.OdinInspector;
using Chemistry.Equipments.Actions;

namespace Chemistry.Equipments
{
    public class EC_Container : EquipmentBase
    {
        #region 数据创建

        [Title("药品数据配置")]
        [SerializeField]
        protected Data.EquipmentDrugInfo EquipmentDrug;

        #endregion

        private DrugSystem _drugSystem; //药品系统

        [HideInInspector]
        public LiquidSystem LiquidEffect; //液体特效

        [PropertySpace(10)]
        [Title("容器数据信息")]
        [ReadOnly]
        public string equipmentName = string.Empty;

        private Transform _liquidNode;
        public EContainerType containerType;

        public Transform LiquidNode {
            get {
                _liquidNode = transform.Find("Liquid");
                if (_liquidNode == null)
                {
                    _liquidNode = new GameObject("Liquid").transform;
                    _liquidNode.SetParent(transform);
                    _liquidNode.localPosition = Vector3.zero;
                    _liquidNode.localRotation = Quaternion.identity;
                    _liquidNode.localScale = Vector3.one;
                }

                return _liquidNode;
            }
        }

        /// <summary>
        /// 药品系统
        /// </summary>
        public DrugSystem DrugSystemIns {
            get { return _drugSystem ?? (_drugSystem = new DrugSystem(EquipmentDrug == null ? 0 : EquipmentDrug.sumVolume)); }
        }

        /// <summary>
        /// 药品名称
        /// </summary>
        public string DrugName {
            get {
                if (EquipmentDrug == null)
                    return string.Empty;

                return  EquipmentDrug.drugName;
            }
        }

        /// <summary>
        /// 容积
        /// </summary>
        public float Volume {
            get {
                return EquipmentDrug.sumVolume; 
            }
        }

        /// <summary>
        /// 初始化液面
        /// </summary>
        protected virtual void OnInitializeLiquid()
        {
            if (LiquidEffect == null)
                LiquidEffect = GetComponent<LiquidSystem>();


            if (LiquidEffect != null)
            {
                LiquidEffect.OnInitialize(DrugSystemIns);
                LiquidEffect.OnInitializeLiquidChange(Volume);
            }
        }

        
        /// <summary>
        /// 初始化药品
        /// </summary>
        public void OnInitializeDrug()
        {

            if(!string.IsNullOrEmpty(EquipmentDrug.drugName))
                DrugSystemIns.AddDrug(EquipmentDrug.drugName, EquipmentDrug.drugVolume);

            if (LiquidEffect != null)
            {
                LiquidEffect.SetValue(DrugSystemIns.Percent);
            }
        }

        /// <summary>
        /// 液体量变化
        /// </summary>
        /// <param name="changeVolume">变化量(正为增，负为减)</param>
        /// <param name="time">时间（为0时突变）</param>
        public virtual void ChangeLiquid(float changeVolume, float time = 0.5f)
        {
            LiquidEffect.ChangeLiquid(DrugSystemIns, changeVolume);
        }
        /// <summary>
        /// 这个方法一般是从外部去调用
        /// </summary>
        public override void OnInitializeEquipment()
        {
            base.OnInitializeEquipment();

            OnInitializeDrug();
            OnInitializeLiquid();
        }

        #region 编辑器调用
#if UNITY_EDITOR
        public override void OnInitializeEquipment_Editor(string equipmentName)
        {
            this.equipmentName = equipmentName;
        }
#endif
        #endregion

        #region 公有方法

        /// <summary>
        /// 设置颜色（渐变）
        /// </summary>
        /// <param name="color"></param>
        public void SetLiquidColor(IWaterColor color, float speed = 1)
        {
            LiquidEffect.SetWaterColorTarget(color, speed);
        }

        /// <summary>
        /// 设置颜色（突变）
        /// </summary>
        public void SetLiquidColor1(IWaterColor color)
        {
            LiquidEffect.SetWaterColorToTarget(color);
        }

        /// <summary>
        /// 设置液体的量
        /// </summary>
        /// <param name="val"></param>
        public void SetLiquidLevel(float val)
        {
            val = Mathf.Clamp(val, 0f, 1f);
            LiquidEffect.SetValue(val);
        }

        #endregion
    }
}
