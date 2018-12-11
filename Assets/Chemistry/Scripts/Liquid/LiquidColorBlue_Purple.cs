using UnityEngine;

namespace Chemistry.Liquid
{
    /// <summary>
    /// 紫色石蕊遇碱变蓝
    /// </summary>
    public class LiquidColorBlue_Purple : LiquidColorBase
    {
        private readonly Color _colorWater = new Color(0.19f, 0.30f, 0.8f, 0.15f);
        private readonly Color _colorSurface = new Color(0.19f, 0.30f, 0.8f, 0.3f);
        private readonly float _fltSparklingIntensity = 0.0f;

        protected override LiquidColorInfo ColorInfo
        {
            get { return new LiquidColorInfo(_colorWater, _colorSurface, _fltSparklingIntensity); }
        }
    }

}