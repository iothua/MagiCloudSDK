using UnityEngine;

namespace Chemistry.Liquid
{
    /// <summary>
    /// 浅绿色（亚铁离子）
    /// </summary>
    public class LiquidColorGreen : LiquidColorBase
    {
        private readonly Color _colorWater = new Color(0.90f, 0.93f, 0.19f, 0.15f);
        private readonly Color _colorSurface = new Color(0.90f, 0.93f, 0.19f, 0.3f);
        private readonly float _fltSparklingIntensity = 0.0f;

        protected override LiquidColorInfo ColorInfo
        {
            get { return new LiquidColorInfo(_colorWater, _colorSurface, _fltSparklingIntensity); }
        }
    }

}