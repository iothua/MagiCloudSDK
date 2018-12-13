using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MagiCloud.Equipments;
using Chemistry.Equipments;
using Chemistry.Chemicals;
using System;
using System.Reflection;
using MagiCloud.Features;
using Chemistry.Data;
using MagiCloud.Interactive;

namespace Chemistry.Equipments
{
    //[ExecuteInEditMode]
    public class TestEquipment : EquipmentBase, I_ET_D_Drip
    {
        private EquipmentBase inInteractionEquipment;
        public EquipmentBase InInteractionEquipment
        {
            get { return inInteractionEquipment; }
            set { inInteractionEquipment = value; }
        }
        public float LocalPositionYForEffect
        {
            get { return -0.785f; }
            set { }
        }

        public float LowestY { get { return 0.4f; } set { } }
        public float Height { get { return 0.6f; } set { } }

        public DropperInteractionType InteractionEquipment
        {
            get { return DropperInteractionType.烧杯; }
        }

        private Action action;

        public void OnDripDrug(DrugData drugData)
        {

        }

        protected override void Start()
        {
            base.Start();

            FeaturesObject.OnOperaStatus(ObjectOperaType.能抓取);
            FeaturesObject.CanGrab.GrabObject = gameObject;


        }

        protected override void Update()
        {
            base.Update();

            if (Input.GetKeyDown(KeyCode.Q))
            {
                RotateManager.StartCameraAroundCenter(transform);
            }

            if (Input.GetKeyDown(KeyCode.W))
            {
                RotateManager.StopCameraAroundCenter();
            }
        }
        public override bool IsCanInteraction(InteractionEquipment interaction)
        {
            base.IsCanInteraction(interaction);
            if (InInteractionEquipment != null && InInteractionEquipment != interaction.Equipment) return false;
            if (interaction.Equipment is ET_Dropper)
            {
                InInteractionEquipment = interaction.Equipment;
                return true;
            }
            return false;
        }
        public override void OnDistanceExit(InteractionEquipment interaction)
        {
            base.OnDistanceExit(interaction);
            if (interaction.Equipment is ET_Dropper)
                InInteractionEquipment = null;
        }
    }
}
