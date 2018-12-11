using Chemistry.Data;
using MagiCloud.Equipments;
using UnityEngine;

namespace Chemistry.Equipments
{
    /// <summary>
    /// 物品可被夹取的接口（该接口交互时无需松手）
    /// </summary>
    public interface I_ET_C_CanClamp
    {
        /// <summary>
        /// 该仪器或药品名字
        /// </summary>
        string DrugName { get; }

        /// <summary>
        /// 在夹取工具下的本地位置
        /// </summary>
        Vector3 LocalPosition { get; }

        /// <summary>
        /// 在夹取工具下的本地旋转
        /// </summary>
        Vector3 LocalRotation { get; }

        /// <summary>
        /// 读条特效是否播放中
        /// </summary>
        bool ProgressEffectStatus { get; set; }

        /// <summary>
        /// 读条特效
        /// </summary>
        GameObject ProgressEffect { get; }

        /// <summary>
        /// 设置读条特效的方法
        /// </summary>
        void SetProgressEffectStatus();

        /// <summary>
        /// 当前是否可以被夹取（比如盖上盖子的广口瓶中的物品不能被夹取）
        /// </summary>
        bool CanClamp { get; set; }

        /// <summary>
        /// 记录与改接口正在交互的仪器（用于屏蔽其他同类仪器与该接口交互）
        /// </summary>
        EquipmentBase InInteractionEquipment { get; set; }

        /// <summary>
        /// 提供给夹取工具自己所属的仪器类型
        /// </summary>
        DropperInteractionType InteractionEquipment { get; }
    }
}