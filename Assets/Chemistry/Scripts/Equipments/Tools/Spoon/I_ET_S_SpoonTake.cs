using Chemistry.Data;
using MagiCloud.Equipments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Chemistry.Equipments
{
    /// <summary>
    /// 药匙取药接口
    /// </summary>
    public interface I_ET_S_SpoonTake
    {
        /// <summary>
        /// 药品名字
        /// </summary>
        string DrugName { get; }
        /// <summary>
        /// 药品在药匙内的本地坐标
        /// </summary>
        Vector3 LocalPosition { get; }

        /// <summary>
        /// 药品在药匙内的本地旋转
        /// </summary>
        Vector3 LocalRotation { get; }
        /// <summary>
        /// 药匙一次获取的量
        /// </summary>
        float TakeAmount { get; }

        /// <summary>
        /// 取药动画药匙下降的数值
        /// </summary>
        float Height { get; }

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
