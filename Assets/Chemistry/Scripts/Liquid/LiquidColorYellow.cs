using UnityEngine;

namespace Chemistry.Liquid
{
    /// <summary>
    /// 黄色（铁粒子）
    /// </summary>
    public class LiquidColorYellow : LiquidColorBase
    {
        private readonly Color _colorWater = new Color(0.95f, 0.62f, 0.01f, 0.15f);
        private readonly Color _colorSurface = new Color(0.95f, 0.62f, 0.01f, 0.30f);
        private readonly float _fltSparklingIntensity = 0.0f;

        protected override LiquidColorInfo ColorInfo
        {
            get { return new LiquidColorInfo(_colorWater, _colorSurface, _fltSparklingIntensity); }
        }
    }

}