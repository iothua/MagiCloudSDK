using MagiCloud;
using UnityEngine;

namespace Chemistry.Equipments
{
    /// <summary>
    /// 滑动盖子
    /// </summary>
    public interface ISlideCover :ISlideEquipment
    {
        /// <summary>
        /// 盖子边界
        /// </summary>
        Vector2 Bound { get; }
        /// <summary>
        /// 激活滑动
        /// </summary>
        /// <param name="bound">边界</param>
        /// <param name="value">限制值</param>
        /// <param name="axis">限制轴</param>
        /// <param name="isLocal">本地坐标</param>
        void Active(ISlideBottle bound,Vector2 value,AxisLimits axis = AxisLimits.Y,bool isLocal = true);
        void Close(AxisLimits axis = AxisLimits.Y);
        float GetRange(ISlideBottle bound);
        float Range { get; }
    }
}
