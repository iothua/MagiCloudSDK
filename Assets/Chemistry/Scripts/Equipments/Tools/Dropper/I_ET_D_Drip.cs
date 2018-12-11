using Chemistry.Chemicals;
using System.Collections.Generic;
using Chemistry.Data;
using MagiCloud.Equipments;

namespace Chemistry.Equipments
{
    /// <summary>
    /// 滴管滴液体接口
    /// </summary>
    public interface I_ET_D_Drip
    {
        /// <summary>
        /// 对应滴管shui_kuosan特效的localPosition.y
        /// </summary>
        float LocalPositionYForEffect { get; set; }

        /// <summary>
        /// 对应滴管shuidi特效中粒子系统组件面板上的VelocityOverLifetime中Linear.y
        /// </summary>
        float LowestY { get; set; }

        /// <summary>
        /// 滴管放药时动画下落的高度（滴管作为子物体后）
        /// </summary>
        float Height { get; set; }

        /// <summary>
        /// 提供给滴管自己所属的仪器类型
        /// </summary>
        DropperInteractionType InteractionEquipment { get; }

        /// <summary>
        /// 从滴管获取到的药品
        /// </summary>
        /// <param name="drugs"></param>
        void OnDripDrug(List<Drug> drugs);

        /// <summary>
        /// 记录与改接口正在交互的仪器（用于屏蔽其他同类仪器与该接口交互）
        /// </summary>
        EquipmentBase InInteractionEquipment { get; set; }
    }
}
