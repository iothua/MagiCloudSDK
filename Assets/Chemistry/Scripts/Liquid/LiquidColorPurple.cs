using UnityEngine;

namespace Chemistry.Liquid
{
    /// <summary>
    /// 紫色石蕊
    /// </summary>
    public class LiquidColorPurple : LiquidColorBase
    {
        private readonly Color _colorWater = new Color(0.10f, 0.02f, 0.18f, 0.15f);
        private readonly Color _colorSurface = new Color(0.10f, 0.02f, 0.18f, 0.3f);
        private readonly float _fltSparklingIntensity = 0.0f;

        protected override LiquidColorInfo ColorInfo
        {
            get { return new LiquidColorInfo(_colorWater, _colorSurface, _fltSparklingIntensity); }
        }
    }

}