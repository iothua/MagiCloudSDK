using Chemistry.Chemicals;
using UnityEngine;

/// <summary>
/// 暂时没用
/// </summary>
namespace Chemistry.Interactions
{
    public interface IPourWater
    {
        /// <summary>
        /// 一般IPourWater应该EquipmentBase继承，然后该属性获取到IsCanInteraction属性值
        /// </summary>
        bool IsCanPourWater { get; }

        /// <summary>
        /// 倒水开始
        /// </summary>
        bool IsPourStarting { get; set; }

        /// <summary>
        /// 倒水暂停
        /// </summary>
        bool IsPourPausing { get; set; }

        /// <summary>
        /// 需要进行倾倒的物体
        /// </summary>
        GameObject PourWaterObject { get; }

        /// <summary>
        /// 更新液体信息
        /// </summary>
        /// <param name="drug"></param>
        void UpdateDrugWater(Drug drug);
    }
}
