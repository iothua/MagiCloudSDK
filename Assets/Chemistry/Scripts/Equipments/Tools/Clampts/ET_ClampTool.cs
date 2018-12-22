using System;
using UnityEngine;
using MagiCloud.Features;
using MagiCloud.Equipments;
using Chemistry.Chemicals;
using MagiCloud.Interactive;
using Chemistry.Equipments.Actions;

namespace Chemistry.Equipments
{
    /// <summary>
    /// 夹取工具
    /// </summary>
    public class ET_ClampTool : EquipmentBase
    {
        private DrugSystem _drugSystem;         //药品系统
        /// <summary>
        /// 药品系统
        /// </summary>
        public DrugSystem DrugSystemIns
        {
            get { return _drugSystem ?? (_drugSystem = new DrugSystem()); }
        }
        private EquipmentBase interactionEquipmentBase;     //与镊子交互的仪器，排除一个滴管与多个仪器交互
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
        private bool isSuccess;
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
            base.IsCanInteraction(interaction);
            if (interactionEquipmentBase != null && interactionEquipmentBase != interaction.Equipment) return false;
            if (interaction.Equipment is I_ET_C_CanClamp)
            {
                I_ET_C_CanClamp canClamp = interaction.Equipment as I_ET_C_CanClamp;
                if (!canClamp.CanClamp) return false;
                if (canClamp.InInteractionEquipment != null)
                    return false;
                else
                {
                    interactionEquipmentBase = interaction.Equipment;
                    return true;
                }
                    
            }
            if (interaction.Equipment is I_ET_C_ClampPut)
            {
                I_ET_C_ClampPut clampPut = interaction.Equipment as I_ET_C_ClampPut;
                if (!clampPut.CanReceive) return false;
                if (clampPut.InInteractionEquipment != null)
                    return false;
                else
                {
                    interactionEquipmentBase = interaction.Equipment;
                    return true;
                }
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
                        Clamp(canClamp, interaction);
                    }
                }
            }
            if (interaction.Equipment is I_ET_C_ClampPut)
            {
                if (!isSuccess)
                {
                    I_ET_C_ClampPut clampPut = interaction.Equipment as I_ET_C_ClampPut;
                    timeProgress += Time.deltaTime;
                    if (timeProgress >= 2)
                    {
                        //放药代码。。。
                        isSuccess = true;
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
                isSuccess = false;
            }
        }

        /// <summary>
        /// 设置读条特效的方法
        /// </summary>
        void SetProgressEffectStatus(bool status)
        {
            ProgressEffectStatus = status;
            progressEffect.SetActive(status);
        }

        /// <summary>
        /// 夹取成功
        /// </summary>
        /// <param name="canClamp"></param>
        /// <param name="interaction"></param>
        public void Clamp(I_ET_C_CanClamp canClamp, InteractionEquipment interaction)
        {
            interaction.Equipment.gameObject.transform.SetParent(transform);
            interaction.Equipment.gameObject.transform.localPosition = canClamp.LocalPosition;
            interaction.Equipment.gameObject.transform.localEulerAngles = canClamp.LocalRotation;

            interaction.Equipment.IsEnable = false;
            SetProgressEffectStatus(false);
            canClamp.CanClamp = false;
        }

        /// <summary>
        /// 释放成功
        /// </summary>
        /// <param name="clampPut"></param>
        /// <param name="interaction"></param>
        public void Put(I_ET_C_ClampPut clampPut, InteractionEquipment interaction)
        {

        }
    }
}
