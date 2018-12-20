using Chemistry.Data;
using MagiCloud.Equipments;
using System;

namespace Chemistry.Equipments
{
    /// <summary>
    /// 滴管吸药
    /// </summary>
    public interface I_ET_D_BreatheIn
    {
        /// <summary>
        /// 要吸的药品名字
        /// </summary>
        string DrugName { get; }

        /// <summary>
        /// 滴管吸药时动画下落的数值（滴管作为子物体后）
        /// </summary>
        float Height { get;}

        /// <summary>
        /// 提供给滴管自己所属的仪器类型
        /// </summary>
        DropperInteractionType InteractionEquipment { get;}

        /// <summary>
        /// 要吸收的体积
        /// </summary>
        /// <param name="volume"></param>
        void OnBreatheIn(float volume);
    }
}
