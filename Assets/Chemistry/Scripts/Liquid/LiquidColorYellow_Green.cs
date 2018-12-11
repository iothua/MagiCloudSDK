using UnityEngine;


namespace Chemistry.Liquid
{
    /// <summary>
    /// 铁钉生锈变黄又变绿   2018年6月11日 17:14:25 苏金明
    /// </summary>
    public class LiquidColorYellow_Green : LiquidColorBase
    {
        private readonly Color _colorWater = new Color(0.825f, 0.6196079f, 0.01176471f, 0.15f);
        private readonly Color _colorSurface = new Color(0.825f, 0.6196079f, 0.15f, 0.30f);
        private readonly float _fltSparklingIntensity = 0.0f;

        protected override LiquidColorInfo ColorInfo
        {
            get { return new LiquidColorInfo(_colorWater, _colorSurface, _fltSparklingIntensity); }
        }
    }

}