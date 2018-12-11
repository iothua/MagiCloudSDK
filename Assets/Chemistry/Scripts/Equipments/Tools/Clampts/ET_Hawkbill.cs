using System;
using UnityEngine;
using MagiCloud.Equipments;
using Chemistry.Chemicals;
using MagiCloud.Interactive;

namespace Chemistry.Equipments
{
    /// <summary>
    /// 坩埚钳
    /// </summary>
    public class ET_Hawkbill : EquipmentBase
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
        private EA_HawkbillTrajectoryContent hawkbillTrajectoryContent;
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
                interactionEquipmentBase = interaction.Equipment;
            }
            if (interaction.Equipment is I_ET_C_ClampPut)
            {
                interactionEquipmentBase = interaction.Equipment;
            }
        }
        public override void OnDistanceExit(InteractionEquipment interaction)
        {
            base.OnDistanceExit(interaction);
            if (interaction.Equipment is I_ET_C_CanClamp)
            {
                interactionEquipmentBase = null;
            }
            if (interaction.Equipment is I_ET_C_ClampPut)
            {
                interactionEquipmentBase = null;
            }
        }
    }
}
