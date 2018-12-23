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

        public EA_Spoon ea_Spoon;

        private GameObject drugObject;//药品模型,药匙内是否有药（会修改为根据药匙内药品量判断）

        protected override void Start()
        {
            base.Start();
            OnInitializeEquipment();
        }

        public override void OnInitializeEquipment()
        {
            base.OnInitializeEquipment();
            if (ea_Spoon == null)
            {
                ea_Spoon = gameObject.GetComponentInChildren<EA_Spoon>();
            }
        }
        public override bool IsCanInteraction(InteractionEquipment interaction)
        {
            base.IsCanInteraction(interaction);
            if (interactionEquipmentBase != null && interactionEquipmentBase != interaction.Equipment) return false;

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
                if (drugObject == null) return false;

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
                    spoonTrajectoryContent = new EA_SpoonTrajectoryContent(this, spoonTake, interaction.gameObject.transform, SpoonTakeAnimComplete);
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
                    spoonTrajectoryContent = new EA_SpoonTrajectoryContent(this, spoonPut, interaction.gameObject.transform, SpoonPutAnimComplete);
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

            //执行放药动画
            var drugData = DrugSystemIns.OnTakeDrug(Chemistry.Data.EDrugType.Solid_Powder);
            spoonPut.OnDripDrug(new DrugData(drugData.DrugName, drugData.Volume));

            DrugSystemIns.ReduceDrug(drugData.Volume, true);

            if (drugObject != null)
                Destroy(drugObject);
        }

        /// <summary>
        /// 取药动画结束后的回调
        /// </summary>
        /// <param name="spoonTake"></param>
        private void SpoonTakeAnimComplete(I_ET_S_SpoonTake spoonTake)
        {
            //显示药品
            if (spoonTake.DrugObject == null) return;

            drugObject = Instantiate(spoonTake.DrugObject, transform);
            drugObject.name = spoonTake.DrugObject.name;
            drugObject.transform.localPosition = spoonTake.LocalPosition;
            drugObject.transform.localRotation = Quaternion.Euler(spoonTake.LocalRotation);

            DrugSystemIns.AddDrug(spoonTake.DrugName, spoonTake.TakeAmount, Chemistry.Data.EMeasureUnit.g);
        }

        /// <summary>
        /// 放药动画结束后的回调
        /// </summary>
        /// <param name="spoonPut"></param>
        private void SpoonPutAnimComplete(I_ET_S_SpoonPut spoonPut)
        {
            
        }

        public override void OnInitializeEquipment_Editor(string equipmentName)
        {
            base.OnInitializeEquipment_Editor(equipmentName);

            ea_Spoon = GetComponent<EA_Spoon>() ?? gameObject.AddComponent<EA_Spoon>();
        }
    }
}
