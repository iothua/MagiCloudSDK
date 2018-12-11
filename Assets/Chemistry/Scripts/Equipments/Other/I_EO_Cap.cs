using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chemistry.Equipments
{
    /// <summary>
    /// 针对类似于滴管的临时盖子
    /// </summary>
    public interface I_EO_Cap
    {
        /// <summary>
        /// 滴管身上的盖子组件
        /// </summary>
        EO_Cap EC_Cap { get; set; }
        /// <summary>
        /// 滴管身上的盖子组件激活状态
        /// </summary>
        bool ActivationCap { get; set; }
    } 
}
