using UnityEngine;

namespace Chemistry.Liquid
{
    /// <summary>
    /// 液体颜色基类
    /// </summary>
    public abstract class LiquidColorBase : IWaterColor
    {
        public virtual Color WaterColor
        {
            get { return ColorInfo.WaterColor; }
        }

        public virtual Color SurfaceColor
        {
            get { return ColorInfo.SurfaceColor; }
        }

        public virtual float SparklingIntensity
        {
            get { return ColorInfo.SparklingIntensity; }
        }

        /// <summary>
        /// 颜色信息
        /// </summary>
        protected abstract LiquidColorInfo ColorInfo { get; }
    }

    /// <summary>
    /// 液体颜色信息
    /// </summary>
    public class LiquidColorInfo
    {
        private Color _colorWater;
        private Color _colorSurface;
        private float _fltSparklingIntensity;

        public Color WaterColor
        {
            get { return _colorWater; }
        }

        public Color SurfaceColor
        {
            get { return _colorSurface; }
        }

        public float SparklingIntensity
        {
            get { return _fltSparklingIntensity; }
        }

        public LiquidColorInfo(Color water, Color surface, float sparklingintensity)
        {
            _colorWater = water;
            _colorSurface = surface;
            _fltSparklingIntensity = sparklingintensity;
        }
    }

}