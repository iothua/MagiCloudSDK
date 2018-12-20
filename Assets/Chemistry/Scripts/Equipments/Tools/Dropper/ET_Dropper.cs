using UnityEngine;
using MagiCloud.Equipments;
using Chemistry.Chemicals;
using Chemistry.Data;
using MagiCloud.Interactive;
using Chemistry.Liquid;
using Chemistry.Effects;
using Chemistry.Equipments.Actions;
using System.Collections;

namespace Chemistry.Equipments
{
    /// <summary>
    /// 胶头滴管。完整10滴，最多耗时5秒，可自由决定一次滴加多少滴
    /// 吸药接口：I_ET_D_BreatheIn
    /// 滴药接口：I_ET_D_Drip
    /// </summary>
    //[ExecuteInEditMode]
    public class ET_Dropper : EO_Cover
    {
        public int maxVolume = 10;            //最大容量

        [Header("该次滴多少滴，耗时为滴数 * 0.5"), Range(1, 10), SerializeField]
        private int _numberOfDrop;
        /// <summary>
        /// 一次最多可滴多少滴
        /// </summary>
        public int NumberOfDrop
        {
            get
            {
                if (_numberOfDrop > remainderNumber)
                    return remainderNumber;
                else
                    return _numberOfDrop;
            }
            set
            {
                if (value > remainderNumber)
                    _numberOfDrop = remainderNumber;
                else
                    _numberOfDrop = value;
            }
        }

        private int remainderNumber;            //剩余多少滴

        private EquipmentBase interactionEquipmentBase;     //与滴管交互的仪器，排除一个滴管与多个仪器交互
        private EA_DropperTrajectoryContent dropperTrajectoryContent;

        [SerializeField]
        private EA_Dropper eA_Dropper;          //胶帽变化动画

        [HideInInspector]
        public LiquidSystem liquidEffect;       //滴管内液体特效

        [HideInInspector]
        public Effect_Dropper Effect_Dropper;   //吸药或滴药过程动画

        private Transform _liquidNode;
        public Transform LiquidNode
        {
            get
            {
                if (_liquidNode == null)
                    _liquidNode = transform.Find("Liquid");

                return _liquidNode;
            }
        }

        private DrugSystem _drugSystem;         //药品系统
        /// <summary>
        /// 药品系统
        /// </summary>
        public DrugSystem DrugSystemIns
        {
            get { return _drugSystem ?? (_drugSystem = new DrugSystem(maxVolume)); }
        }

        //当前已经操作的物体
        private I_ET_D_BreatheIn currentBreatheIn = null;

        private bool isEmpty = true;            //滴管内是否有药（会修改为根据滴管内药品量判断）
        private Coroutine coroutine;

        protected override void Start()
        {
            base.Start();
            OnInitializeEquipment();
        }

        public override void OnInitializeEquipment()
        {
            base.OnInitializeEquipment();

            if (Effect_Dropper == null)
                Effect_Dropper = EffectNode.GetComponent<Effect_Dropper>();

            if (liquidEffect == null)
                liquidEffect = GetComponent<LiquidSystem>();

            //实例化液体
            liquidEffect.OnInitializeLiquidChange(maxVolume);
        }

        public override bool IsCanInteraction(InteractionEquipment interaction)
        {
            base.IsCanInteraction(interaction);
            if (interactionEquipmentBase != null && interactionEquipmentBase != interaction.Equipment) return false;

            if (interaction.Equipment is I_ET_D_BreatheIn)
            {
                if (currentBreatheIn == null)
                {
                    interactionEquipmentBase = interaction.Equipment;
                    return true;
                }

                else
                { 
                    if(currentBreatheIn.Equals(interaction.Equipment as I_ET_D_BreatheIn))
                    {
                        interactionEquipmentBase = interaction.Equipment;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            //滴药
            if (interaction.Equipment is I_ET_D_Drip)
            {
                if (isEmpty) return false;

                interactionEquipmentBase = interaction.Equipment;
                return true;

            }
            return false;
        }

        public override void OnDistanceRelease(InteractionEquipment interaction)
        {
            base.OnDistanceRelease(interaction);

            if (FeaturesObject.ActiveSpaceLimit)
                FeaturesObject.spaceLimit.CloseLimit();

            //吸药
            if (interaction.Equipment is I_ET_D_BreatheIn)
            {
                I_ET_D_BreatheIn breatheIn = interaction.Equipment as I_ET_D_BreatheIn;
                var handle = this.DoEquipmentHandle(() =>
                {
                    eA_Dropper.OnStart(0, 100);                                                 //胶帽变化与动画同时进行
                    dropperTrajectoryContent = new EA_DropperTrajectoryContent(this, breatheIn, BreatheInAnimComplete);
                }, 2.5f);               //eA_Dropper时间 + EA_DropperTrajectoryContent时间 + BreatheInAnimComplete时间
                handle.OnComplete(() =>
                {
                    BreatheInDrug(breatheIn);
                });

                currentBreatheIn = breatheIn;

                IsCover = true;

                return;
            }
            //滴药
            if (interaction.Equipment is I_ET_D_Drip)
            {
                float time;
                if (NumberOfDrop * 0.5f <= 2)
                    time = 2;
                else
                    time = NumberOfDrop * 0.5f;
                I_ET_D_Drip drip = interaction.Equipment as I_ET_D_Drip;
                var handle = this.DoEquipmentHandle(() =>
                {
                    dropperTrajectoryContent = new EA_DropperTrajectoryContent(this, drip, DripAnimComplete);              //先动画变化再胶帽变化
                }, time);                               //一滴0.5秒，最少2秒
                handle.OnComplete(() =>
                {
                    //DripDrug(interaction.Equipment as I_ET_D_Drip, 1.0f);
                });

                IsCover = true;
            }
        }

        public override void OnDistanceExit(InteractionEquipment interaction)
        {
            base.OnDistanceExit(interaction);

            if (FeaturesObject.ActiveSpaceLimit)
                FeaturesObject.spaceLimit.OpenLimit();

            if (interaction.Equipment is I_ET_D_BreatheIn)
            {
                //I_ET_D_BreatheIn breatheIn = interaction.Equipment as I_ET_D_BreatheIn;
                interactionEquipmentBase = null;
            }
            if (interaction.Equipment is I_ET_D_Drip)
            {
                //I_ET_D_Drip drip = interaction.Equipment as I_ET_D_Drip;
                //Effect_Dropper effect_Dropper = GetComponent<Effect_Dropper>();
                if (Effect_Dropper != null)
                {
                    Effect_Dropper.HideDripEffect();
                    Effect_Dropper.HidePoppleEffect();
                }

                interactionEquipmentBase = null;
            }

            IsCover = false;
        }
        /// <summary>
        /// 吸药
        /// </summary>
        /// <param name="drugName"></param>
        public void BreatheInDrug(I_ET_D_BreatheIn breatheIn)
        {
            float breatheValue = 0;

            if (DrugSystemIns.IsHaveDrugForName(breatheIn.DrugName))
            {
                breatheValue = maxVolume - DrugSystemIns.GetDrug(breatheIn.DrugName).Volume;
            }
            else
            {
                breatheValue = maxVolume;
            }

            liquidEffect.SetWaterColorToTarget(DrugSystem.GetColor(breatheIn.DrugName));

            liquidEffect.ChangeLiquid(DrugSystemIns, breatheValue, breatheIn.DrugName);

            breatheIn.OnBreatheIn(breatheValue);

            isEmpty = false;
            remainderNumber = maxVolume;
        }

        /// <summary>
        /// 吸药动画完成回调
        /// </summary>
        public void BreatheInAnimComplete(I_ET_D_BreatheIn breatheIn)
        {

        }

        /// <summary>
        /// 放药动画完成回调
        /// </summary>
        public void DripAnimComplete(I_ET_D_Drip drip)
        {
            eA_Dropper.OnStart(0, 150);

            Effect_Dropper.ShowDripEffect(drip, NumberOfDrop);
            Effect_Dropper.ShowPoppleEffect(drip, NumberOfDrop);

            liquidEffect.ChangeLiquid(DrugSystemIns, -1 * NumberOfDrop, time: 0.5f * NumberOfDrop, actionTrans: (name, percent) =>
               {
                   drip.OnDripDrug(new DrugData(name, Mathf.Abs(percent)));
               });

            remainderNumber -= NumberOfDrop;                            //设置剩余滴数

            if (remainderNumber == 0)
                isEmpty = true;
        }

        protected override void OnDestroy()
        {
            if (coroutine != null)
                StopCoroutine(coroutine);
            dropperTrajectoryContent = null;
        }
        #region 编辑器调用

        public override void OnInitializeEquipment_Editor(string name)
        {
            CreateLiquidEffect();
            Effect_Dropper = EffectNode.gameObject.GetComponent<Effect_Dropper>()??EffectNode.gameObject.AddComponent<Effect_Dropper>();

            eA_Dropper = gameObject.GetComponent<EA_Dropper>() ?? gameObject.AddComponent<EA_Dropper>();

            eA_Dropper.meshRenderer = ModelNode.GetComponentInChildren<SkinnedMeshRenderer>();
        }

        private void CreateLiquidEffect()
        {
            liquidEffect = gameObject.GetComponent<LiquidSystem>() ?? gameObject.AddComponent<LiquidSystem>();

            liquidEffect.OnInitialize_Editor(DrugSystemIns, null, LiquidVolumeFX.TOPOLOGY.Cylinder);

            liquidEffect.SetWaterColorToTarget(new LiquidColorNode());
        }

        #endregion
    }
}
