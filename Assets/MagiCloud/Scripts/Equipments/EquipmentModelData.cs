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
    }

    /// <summary>
    /// 仪器模型数据
    /// </summary>
    [System.Serializable]
    public class EquipmentModelDataItem
    {
        [InlineEditor(InlineEditorModes.LargePreview)]
        public GameObject modelObject;

        [InfoBox("Transform信息")]
        public TransformData transformData;
    }
}
