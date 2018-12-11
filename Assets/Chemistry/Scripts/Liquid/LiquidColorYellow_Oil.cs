using UnityEngine;

namespace Chemistry.Liquid
{
    /// <summary>
    /// 黄色(油)
    /// </summary>
    public class LiquidColorYellow_Oil : LiquidColorBase
    {
        private readonly Color _colorWater = new Color(0.90f, 0.63f, 0.00f, 0.40f);
        private readonly Color _colorSurface = new Color(0.90f, 0.63f, 0.00f, 0.40f);
        private readonly float _fltSparklingIntensity = 0.0f;

        protected override LiquidColorInfo ColorInfo
        {
            get { return new LiquidColorInfo(_colorWater, _colorSurface, _fltSparklingIntensity); }
        }
    }

}