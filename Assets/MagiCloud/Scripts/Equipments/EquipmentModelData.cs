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
        public GameObject CreateModel(Transform parent)
        {
            if (resourcesItem == null) return null;

            if (geneterItem.modelObject != null)
            {
                GameObject.DestroyImmediate(geneterItem.modelObject);
            }

            geneterItem.modelObject = GameObject.Instantiate(resourcesItem.modelObject, parent);
            geneterItem.modelObject.name = resourcesItem.modelObject.name;

            geneterItem.transformData = resourcesItem.transformData;
            geneterItem.modelObject.transform.SetTransform(resourcesItem.transformData);

            return geneterItem.modelObject;
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
            if (modelObject == null) return;
            transformData = new TransformData(modelObject.transform);
        }

        public void Assignment(Transform transform)
        {
            transformData = new TransformData(transform);
        }

        /// <summary>
        /// 给当前物体设置值
        /// </summary>
        public void SetTransform()
        {
            if (modelObject == null) return;
            modelObject.transform.SetTransform(transformData);
        }
    }
}
