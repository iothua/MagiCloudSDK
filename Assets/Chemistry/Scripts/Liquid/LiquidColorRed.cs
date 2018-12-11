using UnityEngine;

namespace Chemistry.Liquid
{
    /// <summary>
    /// 无色酚酞遇碱变红
    /// </summary>
    public class LiquidColorRed : LiquidColorBase
    {
        private readonly Color _colorWater = new Color(0.95f, 0.11f, 0.74f, 0.15f);
        private readonly Color _colorSurface = new Color(0.95f, 0.11f, 0.74f, 0.3f);
        private readonly float _fltSparklingIntensity = 0.0f;

        protected override LiquidColorInfo ColorInfo
        {
            get { return new LiquidColorInfo(_colorWater, _colorSurface, _fltSparklingIntensity); }
        }
    }

}