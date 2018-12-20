using Chemistry.Chemicals;
using Chemistry.Data;
using MagiCloud.Equipments;
using System.Collections.Generic;

namespace Chemistry.Equipments
{
    /// <summary>
    /// 药匙放药接口
    /// </summary>
    public interface I_ET_S_SpoonPut
    {
        /// <summary>
        /// 药匙放药动画下降的数值
        /// </summary>
        float SpoonPutHeight { get;}
        /// <summary>
        /// 提供给药匙自己所属的仪器类型
        /// </summary>
        DropperInteractionType InteractionEquipment { get; }

        /// <summary>
        /// 从药匙获取到的药品
        /// </summary>
        /// <param name="drugs"></param>
        void OnDripDrug(DrugData drugData);

        /// <summary>
        /// 记录与改接口正在交互的仪器（用于屏蔽其他同类仪器与该接口交互）
        /// </summary>
        EquipmentBase InInteractionEquipment { get; set; }
    }
}
