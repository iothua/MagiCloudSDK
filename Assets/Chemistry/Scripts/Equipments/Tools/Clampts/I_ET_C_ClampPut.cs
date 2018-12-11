using Chemistry.Chemicals;
using Chemistry.Data;
using MagiCloud.Equipments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chemistry.Equipments
{
    /// <summary>
    /// 可被夹取类工具放物品的接口
    /// </summary>
    public interface I_ET_C_ClampPut
    {
        /// <summary>
        /// 夹取动画下移的高度
        /// </summary>
        float Height { get; set; }
        /// <summary>
        /// 提供给夹取工具自己所属的仪器类型
        /// </summary>
        DropperInteractionType InteractionEquipment { get; }

        /// <summary>
        /// 是否接收夹送过来的药品
        /// </summary>
        bool CanReceive { get; set; }

        /// <summary>
        /// 从夹取工具获取到的药品
        /// </summary>
        /// <param name="drugs"></param>
        void OnDripDrug(List<Drug> drugs);

        /// <summary>
        /// 记录与改接口正在交互的仪器（用于屏蔽其他同类仪器与该接口交互）
        /// </summary>
        EquipmentBase InInteractionEquipment { get; set; }
    }
}
