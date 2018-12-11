using UnityEngine;


namespace Chemistry.Liquid
{
    /// <summary>
    /// 无色溶液
    /// </summary>
    public class LiquidColorNode : LiquidColorBase
    {
        private readonly Color _colorWater = new Color(1f, 1f, 1f, 0.2f);
        private readonly Color _colorSurface = new Color(1f, 1f, 1f, 0.3f);
        private readonly float _fltSparklingIntensity = 0.0f;

        protected override LiquidColorInfo ColorInfo
        {
            get { return new LiquidColorInfo(_colorWater, _colorSurface, _fltSparklingIntensity); }
        }
    }

}