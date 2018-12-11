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
        private EA_TweezerTrajectoryContent tweezerTrajectoryContent;
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
            if (interactionEquipmentBase != null) return false;
            if (interaction.Equipment is I_ET_C_CanClamp)
            {
                I_ET_C_CanClamp canClamp = interaction.Equipment as I_ET_C_CanClamp;
                if (!canClamp.CanClamp) return false;
                if (canClamp.InInteractionEquipment != null)
                    return false;
                else
                    return true;
            }
            if (interaction.Equipment is I_ET_C_ClampPut)
            {
                I_ET_C_ClampPut clampPut = interaction.Equipment as I_ET_C_ClampPut;
                if (!clampPut.CanReceive) return false;
                if (clampPut.InInteractionEquipment != null)
                    return false;
                else
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
                    canClamp.ProgressEffectStatus = true;
                    canClamp.ProgressEffect.SetActive(true);
                    interactionEquipmentBase = interaction.Equipment;

                    timeProgress += Time.deltaTime * 10;
                    if (timeProgress >= 2)
                    {
                        isSuccess = true;
                        interaction.Equipment.gameObject.transform.SetParent(transform);
                        interaction.Equipment.gameObject.transform.localPosition = canClamp.LocalPosition;
                        interaction.Equipment.gameObject.transform.localEulerAngles = canClamp.LocalRotation;

                        interaction.Equipment.IsEnable = false;
                        canClamp.ProgressEffectStatus = false;
                        canClamp.ProgressEffect.SetActive(false);
                        canClamp.CanClamp = false;
                    }
                }
            }
            if (interaction.Equipment is I_ET_C_ClampPut)
            {
                interactionEquipmentBase = interaction.Equipment;
                if (!isSuccess)
                {
                    timeProgress += Time.deltaTime;
                    if (timeProgress >= 2)
                    {
                        isSuccess = true;
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
                canClamp.ProgressEffectStatus = false;
                canClamp.ProgressEffect.SetActive(false);
                canClamp.CanClamp = true;
            }
            if (interaction.Equipment is I_ET_C_ClampPut)
            {
                interactionEquipmentBase = null;
                timeProgress = 0;
                isSuccess = false;
            }
        }
    }
}
