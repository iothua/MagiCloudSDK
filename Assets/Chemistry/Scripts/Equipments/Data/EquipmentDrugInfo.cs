using UnityEngine;
using MagiCloud;
using Sirenix.OdinInspector;
using Chemistry.Liquid;
using MagiCloud.Equipments;

namespace Chemistry.Equipments.Data
{
    [System.Serializable]
    public class EquipmentDrugInfo
    {
        [LabelText("容器对象")]
        public EC_Container container;


        [LabelText("仪器体积(sumValome)")]
        public float sumVolume = 100;

        [LabelText("药品名称(drugName)")]
        public string drugName;

        [InfoBox("药品体积，如果该药品是固体，会从药品文件中生成相应的质量")]
        [LabelText("药品体积(DrugVolume)")]
        public float drugVolume;

        [Space(10)]
        [HideIf("addModel")]
        [LabelText("添加药品液体(addLiquid)")]
        public bool addLiquid = false;

        [ShowIf("addLiquid")]
        [LabelText("液体模型数据")]
        public EquipmentModelData liquidModelData;

        [HideIf("addLiquid")]
        [LabelText("添加药品模型(addModel)")]
        public bool addModel = false;

        [LabelText("药品模型数据")]
        [ShowIf("addModel")]
        public EquipmentModelData drugModelData;


        [ButtonGroup]
        [Button("创建药品")]
        public void OnCreateDrug()
        {
            OnInitializeDrug();
        }

        [ButtonGroup]
        [Button("设置药品模型数据")]
        public void SetDrugData()
        {
            if (addLiquid)
            {

                liquidModelData.geneterItem.SetTransform();
            }

            if (addModel)
            {
                drugModelData.geneterItem.SetTransform();
            }
        }

        [ButtonGroup]
        [Button("读取药品模型数据")]
        public void GetDrugData()
        {
            if (addLiquid)
            {
                liquidModelData.geneterItem.Assignment();

                if (liquidModelData.geneterItem.modelObject != null)
                    liquidModelData.resourcesItem.Assignment(liquidModelData.geneterItem.modelObject.transform);
            }

            if (addModel)
            {
                drugModelData.geneterItem.Assignment();

                if (liquidModelData.geneterItem.modelObject != null)
                    drugModelData.resourcesItem.Assignment(drugModelData.geneterItem.modelObject.transform);
            }


        }

        private void OnInitializeDrug()
        {
            if (addLiquid)
            {
                CreatLiquidEffect(liquidModelData);
            }

            if (addModel)
            {
                CreateModel(drugModelData);
            }
        }

        private void CreatLiquidEffect(EquipmentModelData modelData)
        {

            container.LiquidNode.DestroyImmediateChildObject();

            var liquidModel = modelData.CreateModel(container.LiquidNode);
            liquidModel.name = "LiquidVolume";

            container.LiquidEffect = container.GetComponent<LiquidSystem>() ?? container.gameObject.AddComponent<LiquidSystem>();

            container.LiquidEffect.OnInitialize_Editor(container.DrugSystemIns, liquidModel);

            container.LiquidEffect.SetWaterColorToTarget(container.DrugSystemIns.GetColor());
        }

        private void CreateModel(EquipmentModelData modelData)
        {
            modelData.CreateModel(container.ModelNode);
        }

    }
}
