using System;
using UnityEngine;
using MagiCloud.Features;
using MagiCloud.Equipments;
using Chemistry.Chemicals;
using MagiCloud.Interactive;
using Chemistry.Equipments.Actions;
using MagiCloud.Core.Events;

namespace Chemistry.Equipments
{
    /// <summary>
    /// 夹取工具，用于转移EquipmentBase或GameObject
    /// </summary>
    public class ET_ClampTool : EquipmentBase
    {
        /// <summary>
        /// 是否处于抓取状态
        /// </summary>
        private bool _IsGrab;
        public bool IsGrab
        {
            get { return _IsGrab; }
            set { _IsGrab = value; }
        }

        private DrugSystem _drugSystem;         //药品系统
        /// <summary>
        /// 药品系统
        /// </summary>
        public DrugSystem DrugSystemIns
        {
            get { return _drugSystem ?? (_drugSystem = new DrugSystem()); }
        }
        private EquipmentBase interactionEquipmentBase;     //与镊子交互的仪器，排除一个滴管与多个仪器交互
        /// <summary>
        /// 当前夹取到的Object
        /// </summary>
        private GameObject _ClampObject;
        public GameObject ClampObject
        {
            get { return _ClampObject; }
            set { _ClampObject = value; }
        }
        private EA_ClampTrajectoryContent clampTrajectoryContent;

        /// <summary>
        /// 读条特效是否播放中
        /// </summary>
        private bool _ProgressEffectStatus;
        public bool ProgressEffectStatus
        {
            get { return _ProgressEffectStatus; }
            set { _ProgressEffectStatus = value; }
        }
        [Header("读条特效")]
        public GameObject progressEffect;
        private float timeProgress;     //夹取或放物体时间进度
        private bool isSuccess;         //夹取成功
        protected override void Start()
        {
            base.Start();
            OnInitializeEquipment();
        }

        public override void OnInitializeEquipment()
        {
            base.OnInitializeEquipment();
            gameObject.AddGrabObject(OnGrab, MagiCloud.Core.ExecutionPriority.Low);
            gameObject.AddReleaseObject(OnRelease, MagiCloud.Core.ExecutionPriority.Low);
        }
        protected override void OnDestroy()
        {
            gameObject.RemoveGrabObject(OnGrab);
            gameObject.RemoveReleaseObject(OnRelease);

        }
        private void OnGrab(int obj)
        {
            IsGrab = true;
        }

        private void OnRelease(int obj)
        {
            IsGrab = false;
            if (ClampObject != null)
                SetOnRelease();
        }
        public override bool IsCanInteraction(InteractionEquipment interaction)
        {
            if(interactionEquipmentBase != null)
            Debug.Log(interactionEquipmentBase.gameObject.name);
            base.IsCanInteraction(interaction);
            if (interactionEquipmentBase != null && interactionEquipmentBase != interaction.Equipment) return false;
            if (interaction.Equipment is I_ET_C_CanClamp)
            {
                I_ET_C_CanClamp canClamp = interaction.Equipment as I_ET_C_CanClamp;
                if (isSuccess)
                    return false;
                else
                {
                    if (!canClamp.CanClamp) return false;
                    if (ClampObject != null) return false;
                }
                interactionEquipmentBase = interaction.Equipment;
                return true;

            }
            if (interaction.Equipment is I_ET_C_ClampPut)
            {
                I_ET_C_ClampPut clampPut = interaction.Equipment as I_ET_C_ClampPut;
                if (!clampPut.CanReceive) return false;
                if (ClampObject == null) return false;
                interactionEquipmentBase = interaction.Equipment;
                return true;
            }
            return false;
        }
        public override void OnDistanceStay(InteractionEquipment interaction)
        {
            base.OnDistanceStay(interaction);
            if (interaction.Equipment is I_ET_C_CanClamp)
            {
                if (!isSuccess)
                {
                    I_ET_C_CanClamp canClamp = interaction.Equipment as I_ET_C_CanClamp;
                    SetProgressEffectStatus(true);
                    timeProgress += Time.deltaTime * 10;
                    if (timeProgress >= 2)
                    {
                        //取药代码。。。
                        isSuccess = true;
                        timeProgress = 0;
                        Clamp(canClamp, interaction);
                    }
                }
                return;         //防止同一帧里还执行I_ET_C_ClampPut接口方法
            }
            if (interaction.Equipment is I_ET_C_ClampPut)
            {
                if (isSuccess)
                {
                    I_ET_C_ClampPut clampPut = interaction.Equipment as I_ET_C_ClampPut;
                    SetProgressEffectStatus(true);
                    timeProgress += Time.deltaTime;
                    if (timeProgress >= 2)
                    {
                        //放药代码。。。
                        isSuccess = false;
                        timeProgress = 0;
                        Put(clampPut, interaction);
                    }
                }
            }
        }
        public override void OnDistanceExit(InteractionEquipment interaction)
        {
            base.OnDistanceExit(interaction);
            if (interaction.Equipment is I_ET_C_CanClamp)
            {
                I_ET_C_CanClamp canClamp = interaction.Equipment as I_ET_C_CanClamp;
                interactionEquipmentBase = null;
                timeProgress = 0;
                isSuccess = false;
                SetProgressEffectStatus(false);

                canClamp.CanClamp = true;
            }
            if (interaction.Equipment is I_ET_C_ClampPut)
            {
                interactionEquipmentBase = null;
                timeProgress = 0;
                SetProgressEffectStatus(false);
            }
        }

        /// <summary>
        /// 夹取成功
        /// </summary>
        /// <param name="canClamp"></param>
        /// <param name="interaction"></param>
        public void Clamp(I_ET_C_CanClamp canClamp, InteractionEquipment interaction)
        {
            //镊子及坩埚钳的张开角度还未考虑。。。


            ClampObject = canClamp.OnClamp();
            interaction.Equipment.gameObject.transform.SetParent(transform);
            interaction.Equipment.gameObject.transform.localPosition = canClamp.LocalPosition;
            interaction.Equipment.gameObject.transform.localEulerAngles = canClamp.LocalRotation;

            //interaction.Equipment.IsEnable = false;
            SetProgressEffectStatus(false);
            canClamp.CanClamp = false;
        }

        /// <summary>
        /// 药品释放成功
        /// </summary>
        /// <param name="clampPut"></param>
        /// <param name="interaction"></param>
        public void Put(I_ET_C_ClampPut clampPut, InteractionEquipment interaction)
        {
            clampPut.OnClampPut();
            interaction.Equipment.gameObject.transform.SetParent(null);
            SetProgressEffectStatus(false);
            ClampObject = null;
        }

        /// <summary>
        /// 夹取工具松手自动停止夹取
        /// </summary>
        public void SetOnRelease()
        {
            ClampObject.transform.SetParent(null);
            ClampObject.transform.position = new Vector3(ClampObject.transform.position.x - 1, ClampObject.transform.position.y, ClampObject.transform.position.z);
            SetProgressEffectStatus(false);
            interactionEquipmentBase = null;
            ClampObject = null;
            timeProgress = 0;
            isSuccess = false;
        }

        /// <summary>
        /// 设置读条特效的方法
        /// </summary>
        void SetProgressEffectStatus(bool status)
        {
            ProgressEffectStatus = status;
            progressEffect.SetActive(status);
        }
    }
}
