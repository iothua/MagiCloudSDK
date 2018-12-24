using MagiCloud;
using UnityEngine;
namespace Chemistry.Equipments
{
    /// <summary>
    /// 滑动的瓶口
    /// </summary>
    public interface ISlideBottle
    {
        /// <summary>
        /// 瓶口边界
        /// </summary>
        Vector2 Bound { get; }
        /// <summary>
        /// 在边界内限制移动的范围，比如限制Y轴移动
        /// </summary>
        Vector2 LimitRange { get; }
        // void Active(ISlideCover slideCover,bool active);
        float OpenRate { get; }
        ISlideCover SlideCover { get; set; }
        AxisLimits AxisLimits { get; }
    }
}
