using System;
using System.Collections.Generic;
using UnityEngine;
using MagiCloud.Equipments;
using Chemistry.Chemicals;
using Chemistry.Data;
using MagiCloud.Interactive;
using DG.Tweening;
using Chemistry.Liquid;
using Chemistry.Effects;
using System.Linq;
using Chemistry.Equipments.Actions;
using System.Collections;
using MagiCloud.Features;

namespace Chemistry.Equipments
{
    /// <summary>
    /// 胶头滴管。完整10滴，最多耗时5秒，可自由决定一次滴加多少滴
    /// 吸药接口：I_ET_D_BreatheIn
    /// 滴药接口：I_ET_D_Drip
    /// </summary>
    //[ExecuteInEditMode]
    public class ET_Dropper : EquipmentBase, I_EO_Cap
    {
        [HideInInspector]
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
                if (_numberOfDrop >= remainderNumber)
                    return remainderNumber;
                else
                    return _numberOfDrop;
            }
            set
            {
                if (value >= remainderNumber)
                    _numberOfDrop = remainderNumber;
                else
                    _numberOfDrop = value;
            }
        }
        private int remainderNumber;            //剩余多少滴

        private EquipmentBase interactionEquipmentBase;     //与滴管交互的仪器，排除一个滴管与多个仪器交互
        private EA_DropperTrajectoryContent dropperTrajectoryContent;
        [SerializeField, Header("滴管液面下降曲线")]
        private AnimationCurve animationCurve;
        private float animationCurveTime;       //使用AnimationCurve的时间累积
        private float startTime;                //开始取值时间
        private float endTime;                  //结束取值时间
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

        [SerializeField]
        private string _onlyDrugName;           //滴管内吸取过的药品名字
        public string OnlyDrugName
        {
            get { return _onlyDrugName ?? (_onlyDrugName = ""); }
        }
        private EO_Cap eC_Cap;
        public EO_Cap EC_Cap
        {
            get
            {
                if (eC_Cap == null)
                    eC_Cap = GetComponent<EO_Cap>();
                return eC_Cap;
            }
            set
            {

            }
        }

        public bool ActivationCap { get { return EC_Cap.enabled; } set { EC_Cap.enabled = value; } }
        [Header("可配置的初始化药品")]
        //public ContainerDrugInfo containerDrugInfo;
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

            if (liquidEffect == null)
                liquidEffect = GetComponent<LiquidSystem>();

            //liquidEffect.OnInitialize(DrugSystemIns, LiquidVolumeFX.TOPOLOGY.Cylinder);
            //if (containerDrugInfo.drugName != "")
                //BreatheInDrug("稀盐酸");
        }

        public override bool IsCanInteraction(InteractionEquipment interaction)
        {
            base.IsCanInteraction(interaction);
            if (interactionEquipmentBase != null && interactionEquipmentBase != interaction.Equipment) return false;
            if (interaction.Equipment is I_ET_D_BreatheIn)
            {
                I_ET_D_BreatheIn breatheIn = interaction.Equipment as I_ET_D_BreatheIn;
                if (OnlyDrugName == "")         //是一个新的滴管
                {
                    return true;
                }
                else
                {
                    //EC_Save save = interaction.Equipment as EC_Save;
                    //if (save.DrugSystemIns.IsHaveDrugForName(OnlyDrugName))   //对于存储混合物的容器可能会出问题
                    interactionEquipmentBase = interaction.Equipment;
                    return true;
                    if (OnlyDrugName.Equals(breatheIn.DrugName))                //药品名字是否相等
                    {
                        interactionEquipmentBase = interaction.Equipment;
                        return true;
                    }
                    else
                        return false;
                }
            }
            if (interaction.Equipment is I_ET_D_Drip)
            {
                if (isEmpty) return false;
                //I_ET_D_Drip drip = interaction.Equipment as I_ET_D_Drip;
                if (OnlyDrugName == "")
                    return false;
                else
                {
                    interactionEquipmentBase = interaction.Equipment;
                    return true;
                }
            }
            return false;
        }

        public override void OnDistanceRelease(InteractionEquipment interaction)
        {
            base.OnDistanceRelease(interaction);
            GetComponent<FeaturesObjectController>().spaceLimit.CloseLimit();
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
                    BreatheInDrug(breatheIn.DrugName);
                });
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
                    DripDrug(interaction.Equipment as I_ET_D_Drip, 1.0f);
                });
            }
        }

        public override void OnDistanceExit(InteractionEquipment interaction)
        {
            base.OnDistanceExit(interaction);
            GetComponent<FeaturesObjectController>().spaceLimit.OpenLimit();
            if (interaction.Equipment is I_ET_D_BreatheIn)
            {
                //I_ET_D_BreatheIn breatheIn = interaction.Equipment as I_ET_D_BreatheIn;
                interactionEquipmentBase = null;
            }
            if (interaction.Equipment is I_ET_D_Drip)
            {
                //I_ET_D_Drip drip = interaction.Equipment as I_ET_D_Drip;
                Effect_Dropper effect_Dropper = GetComponent<Effect_Dropper>();
                effect_Dropper.HideDripEffect();
                effect_Dropper.HidePoppleEffect();
                interactionEquipmentBase = null;
            }
        }
        /// <summary>
        /// 吸药
        /// </summary>
        /// <param name="drugName"></param>
        public void BreatheInDrug(string drugName)
        {
            if (DrugSystemIns.IsHaveDrugForName(drugName))
            {
                DrugSystemIns.ReduceDrug(drugName, maxVolume, false);
                DrugSystemIns.AddDrug(drugName, maxVolume);
            }
            else
            {
                DrugSystemIns.AddDrug(drugName, maxVolume);
            }

            if (_onlyDrugName == "")
                _onlyDrugName = drugName;
            //设置液面
            //liquidEffect.SetValue(DrugSystemIns.CurSumVolume);
            liquidEffect.SetValue(maxVolume);                   //一次加满
            isEmpty = false;
            remainderNumber = maxVolume;
        }

        /// <summary>
        /// 滴药
        /// </summary>
        public void DripDrug(I_ET_D_Drip drip, float percent)
        {
            List<Drug> drugs = new List<Drug>();

            //foreach (var item in DrugSystemIns.DicAllDrugs.Values.ToList())
            //{
            //    if (item.DrugType == EDrugType.Liquid)
            //    {
            //        float val = maxVolume * percent;
            //        DrugSystemIns.ReduceDrug(item.Name, val, false);
            //        drugs.Add(new Drug(item.Name, val));
            //    }
            //    //else if (item.DrugType == EDrugType.Solid_Powder)
            //    //{
            //    //    float val = volume * percent * _solubility;
            //    //    DrugSystemIns.ReduceDrug(item.Name, val, false);
            //    //    drugs.Add(new Drug(item.Name, val));
            //    //}
            //}
            drip.OnDripDrug(drugs);
            if (remainderNumber <= 0)
                isEmpty = true;
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
            Effect_Dropper effect_Dropper = GetComponent<Effect_Dropper>();

            eA_Dropper.OnStart(0, 150);
            effect_Dropper.ShowDripEffect(drip, NumberOfDrop);
            effect_Dropper.ShowPoppleEffect(drip, NumberOfDrop);

            startTime = 5 - remainderNumber * 0.5f;                     //根据remainderNumber设置AnimationCurve开始取值时间
            endTime = startTime + NumberOfDrop * 0.5f;                  //根据NumberOfDrop设置AnimationCurve结束取值时间
            remainderNumber -= NumberOfDrop;                            //设置剩余滴数
            animationCurveTime = startTime;
            coroutine = StartCoroutine(LiquidHeightTween());
        }

        /// <summary>
        /// 协程设置液面高度
        /// </summary>
        /// <returns></returns>
        IEnumerator LiquidHeightTween()
        {
            while (true)
            {
                animationCurveTime += Time.deltaTime;
                if (animationCurveTime <= endTime)
                {
                    liquidEffect.SetValue(animationCurve.Evaluate(animationCurveTime));
                }
                else
                {
                    StopCoroutine(coroutine);
                    yield return null;
                }
                yield return null;
            }
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
            CreateLiquidEffect("胶头滴管模型", Vector3.up * -0.737f);
            Effect_Dropper = EffectNode.gameObject.AddComponent<Effect_Dropper>();
            maxVolume = 9;

            eA_Dropper = gameObject.AddComponent<EA_Dropper>();

            eA_Dropper.meshRenderer = ModelNode.GetComponentInChildren<SkinnedMeshRenderer>();

            //设置这个物体的碰撞体
            var boxCollider = (BoxCollider)Collider;
            boxCollider.center = new Vector3(0.008f, -0.4414f, -0.0136f);
            boxCollider.size = new Vector3(0.2274f, 1.24f, 0.221f);
        }

        private void CreateLiquidEffect(string name, Vector3 pos)
        {
            GameObject go = Instantiate(Resources.Load<GameObject>("Models\\LiquidVolume\\" + name));
            go.name = "LiquidVolume";
            //go.hideFlags = HideFlags.HideInHierarchy;
            go.transform.SetParent(LiquidNode);
            go.transform.localPosition = pos;

            liquidEffect = gameObject.AddComponent<LiquidSystem>();
            liquidEffect.OnInitialize_Editor(DrugSystemIns, go, LiquidVolumeFX.TOPOLOGY.Cylinder);
            liquidEffect.SetWaterColorToTarget(new LiquidColorNode());
        }

        #endregion
    }
}
