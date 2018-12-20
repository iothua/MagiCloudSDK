using UnityEngine;

namespace Chemistry.Liquid
{
    public interface IWaterColor
    {
        /// <summary>
        /// 水体颜色
        /// </summary>
        Color WaterColor { get; }
        /// <summary>
        /// 水面颜色
        /// </summary>
        Color SurfaceColor { get; }
        /// <summary>
        /// 颜色变化速率
        /// </summary>
        float SparklingIntensity { get; }

    }
}


