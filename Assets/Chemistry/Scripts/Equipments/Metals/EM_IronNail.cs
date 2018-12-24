using UnityEngine;
using MagiCloud.Equipments;
using Chemistry.Effects;
using Chemistry.Chemicals;
using MagiCloud.Common;

namespace Chemistry.Equipments
{
    /// <summary>
    /// 铁钉
    /// </summary>
    public class EM_IronNail : EquipmentBase
    {
        public Effect_IronNailRust ironNailRust;

        private DrugSystem _durgSystem;

        public float rustySpeed = 0.1f; //生锈速度

        [Header("生锈程度")]
        [Range(0,0.5f)]
        public float degree = 0f;

        public DrugSystem DrugSystemIns {
            get {
                if (_durgSystem == null)
                    _durgSystem = new DrugSystem();

                return _durgSystem;
            }
        }

        protected override void Start()
        {
            OnInitializeEquipment();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            OnRemoveTime();
        }

        public override void OnInitializeEquipment()
        {
            base.OnInitializeEquipment();

            DrugSystemIns.AddDrug("铁", 5, Chemistry.Data.EMeasureUnit.g);

            //实例化特效
            ironNailRust.OnInitialize();
            ironNailRust.SetTop(degree);
            ironNailRust.SetOpacity(0);

            OnRegisterTime();
        }

        public override void OnInitializeEquipment_Editor(string equipmentName)
        {
            base.OnInitializeEquipment_Editor(equipmentName);

            ironNailRust = EffectNode.GetComponent<Effect_IronNailRust>() 
                ?? EffectNode.gameObject.AddComponent<Effect_IronNailRust>();
        }

        private void OnRegisterTime()
        {
            if (DoTweenTimeController.Instance != null)
            {
                DoTweenTimeController.Instance.OnPercet.AddListener(MagiCloud.Core.ExecutionPriority.Mid, OnTimePerce);

                DoTweenTimeController.Instance.OnStart.AddListener(this, MagiCloud.Core.ExecutionPriority.Mid, OnTimeStart);
                DoTweenTimeController.Instance.OnStop.AddListener(this, MagiCloud.Core.ExecutionPriority.Mid, OnTimeStop);
                DoTweenTimeController.Instance.OnParus.AddListener(this, MagiCloud.Core.ExecutionPriority.Mid, OnTimeParus);
            }
            
        }

        void OnRemoveTime()
        {

            if (DoTweenTimeController.Instance != null)
            {
                DoTweenTimeController.Instance.OnPercet.RemoveListener(OnTimePerce);

                DoTweenTimeController.Instance.OnStart.RemoveListener(this, OnTimeStart);
                DoTweenTimeController.Instance.OnStop.RemoveListener(this, OnTimeStop);
                DoTweenTimeController.Instance.OnParus.RemoveListener(this, OnTimeParus);
            }
            
        }

        void OnTimePerce(float perce)
        {
            //百分比
            ironNailRust.SetOpacity(perce * rustySpeed);
        }

        void OnTimeStart()
        {
            //开始

        }

        void OnTimeParus()
        {
            //暂停

        }

        void OnTimeStop()
        {
            //停止
        }
    }
}
