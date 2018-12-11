using UnityEngine;

namespace Chemistry.Liquid
{
    public interface IWaterColor
    {
        Color WaterColor { get; }
        Color SurfaceColor { get; }
        float SparklingIntensity { get; }

    }
}


