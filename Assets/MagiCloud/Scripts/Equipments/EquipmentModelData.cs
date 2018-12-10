using UnityEngine;
using Sirenix.OdinInspector;

namespace MagiCloud.Equipments
{
    [System.Serializable]
    public class EquipmentModelData
    {
        [Title("仪器关联信息")]
        public EquipmentModelDataItem geneterItem;//已经生成后的
        [Title("资源信息")]
        public EquipmentModelDataItem resourcesItem; //资源内的

        /// <summary>
        /// 创建模型
        /// </summary>
        /// <param name="parent">Parent.</param>
        public void CreateModel(Transform parent)
        {
            geneterItem.modelObject = GameObject.Instantiate(resourcesItem.modelObject, parent);

            geneterItem.transformData = resourcesItem.transformData;
            geneterItem.modelObject.transform.SetTransform(resourcesItem.transformData);
        }
    }

    /// <summary>
    /// 仪器模型数据
    /// </summary>
    [System.Serializable]
    public class EquipmentModelDataItem
    {
        [InlineEditor(InlineEditorModes.LargePreview)]
        public GameObject modelObject;

        public TransformData transformData;

        public void Assignment()
        {
            transformData = new TransformData(modelObject.transform);
        }
    }
}
