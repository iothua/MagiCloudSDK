using UnityEngine;

namespace Chemistry.Liquid
{
    /// <summary>
    /// 硫酸铜溶液
    /// </summary>
    public class LiquidColorBlue : LiquidColorBase
    {
        private readonly Color _colorWater = new Color(0.08f, 0.51f, 0.96f, 0.15f);
        private readonly Color _colorSurface = new Color(0.08f, 0.62f, 0.96f, 0.35f);
        private readonly float _fltSparklingIntensity = 0.0f;

        protected override LiquidColorInfo ColorInfo
        {
            get { return new LiquidColorInfo(_colorWater, _colorSurface, _fltSparklingIntensity); }
        }
    }

}