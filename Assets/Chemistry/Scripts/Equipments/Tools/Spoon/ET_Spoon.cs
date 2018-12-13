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
    /// 药匙
    /// </summary>
    public class ET_Spoon : EquipmentBase
    {

        public float amount;

        private DrugSystem _drugSystem;         //药品系统
        /// <summary>
        /// 药品系统
        /// </summary>
        public DrugSystem DrugSystemIns
        {
            get { return _drugSystem ?? (_drugSystem = new DrugSystem()); }
        }
        private EquipmentBase interactionEquipmentBase;     //与药匙交互的仪器，排除一个滴管与多个仪器交互
        private EA_SpoonTrajectoryContent spoonTrajectoryContent;

        private I_ET_S_SpoonTake currentSpoonTake;

        private bool isEmpty = true;            //药匙内是否有药（会修改为根据药匙内药品量判断）
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
            if (interaction.Equipment is I_ET_S_SpoonTake)
            {
                if (currentSpoonTake == null)
                {
                    interactionEquipmentBase = interaction.Equipment;
                    return true;
                }
                else
                {
                    I_ET_S_SpoonTake spoonTake = interaction.Equipment as I_ET_S_SpoonTake;

                    if (!currentSpoonTake.Equals(spoonTake)) return false;

                    if (spoonTake.InInteractionEquipment != null)
                        return false;

                    return true;
                }

            }
            if (interaction.Equipment is I_ET_S_SpoonPut)
            {
                if (isEmpty) return false;

                return true;
            }
            return false;
        }
        public override void OnDistanceRelease(InteractionEquipment interaction)
        {
            base.OnDistanceRelease(interaction);
            //取药
            if (interaction.Equipment is I_ET_S_SpoonTake)
            {
                interactionEquipmentBase = interaction.Equipment;
                I_ET_S_SpoonTake spoonTake = interaction.Equipment as I_ET_S_SpoonTake;
                var handle = this.DoEquipmentHandle(() =>
                {
                    spoonTrajectoryContent = new EA_SpoonTrajectoryContent(this, spoonTake, SpoonTakeAnimComplete);
                }, 2.5f);
                handle.OnComplete(() =>
                {
                    SpoonTakeDrug(spoonTake.DrugName);
                });
                return;
            }
            //放药
            if (interaction.Equipment is I_ET_S_SpoonPut)
            {
                interactionEquipmentBase = interaction.Equipment;
                I_ET_S_SpoonPut spoonPut = interaction.Equipment as I_ET_S_SpoonPut;
                var handle = this.DoEquipmentHandle(() =>
                {
                    spoonTrajectoryContent = new EA_SpoonTrajectoryContent(this, spoonPut, SpoonPutAnimComplete);
                }, 2.5f);
                handle.OnComplete(() =>
                {
                    SpoonPutDrug(spoonPut);
                });
            }
        }
        public override void OnDistanceExit(InteractionEquipment interaction)
        {
            base.OnDistanceExit(interaction);
            if (interaction.Equipment is I_ET_S_SpoonTake)
            {
                interactionEquipmentBase = null;
            }
            if (interaction.Equipment is I_ET_S_SpoonPut)
            {
                interactionEquipmentBase = null;
            }
        }
        protected override void OnDestroy()
        {
            spoonTrajectoryContent = null;
        }

        /// <summary>
        /// 取药
        /// </summary>
        /// <param name="drugName"></param>
        private void SpoonTakeDrug(string drugName)
        {
           
        }

        /// <summary>
        /// 放药
        /// </summary>
        /// <param name="spoonPut"></param>
        private void SpoonPutDrug(I_ET_S_SpoonPut spoonPut)
        {
            
        }

        /// <summary>
        /// 取药动画结束后的回调
        /// </summary>
        /// <param name="spoonTake"></param>
        private void SpoonTakeAnimComplete(I_ET_S_SpoonTake spoonTake)
        {
            
        }

        /// <summary>
        /// 放药动画结束后的回调
        /// </summary>
        /// <param name="spoonPut"></param>
        private void SpoonPutAnimComplete(I_ET_S_SpoonPut spoonPut)
        {

        }
    }
}
