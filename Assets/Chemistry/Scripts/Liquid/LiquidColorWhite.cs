using System;
using UnityEngine;

namespace Chemistry.Liquid
{
    /// <summary>
    /// 浑浊
    /// </summary>
    public class LiquidColorWhite : LiquidColorBase
    {
        private float _alphaPercent = -1;

        private Color _colorWater = new Color(1f, 1f, 1f, 0.5f);
        private Color _colorSurface = new Color(1f, 1f, 1f, 0.5f);
        private float _fltSparklingIntensity = 1.1f;

        protected override LiquidColorInfo ColorInfo
        {
            get
            {
                if (_alphaPercent != -1)
                {
                    _alphaPercent = Mathf.Clamp(_alphaPercent, 0, 1);
                    if (_alphaPercent == 0)
                    {
                        IWaterColor colornode = new LiquidColorNode();
                        _colorWater = colornode.WaterColor;
                        _colorSurface = colornode.SurfaceColor;
                        _fltSparklingIntensity = colornode.SparklingIntensity;
                    }
                    else
                    {
                        try
                        {
                            checked
                            {
                                _colorWater.a = 0.001f + (_alphaPercent / 2.5f);
                                _colorSurface.a = 0.3f + (_alphaPercent / 5);
                                _fltSparklingIntensity = 0.1f + _alphaPercent;
                            }
                        }
                        catch (OverflowException)
                        {
                            _colorWater.a = 0f;
                            _colorSurface.a = 0f;
                            _fltSparklingIntensity = 0f;
                            Debug.LogError("运算溢出...");
                            throw;
                        }
                    }
                }

                return new LiquidColorInfo(_colorWater, _colorSurface, _fltSparklingIntensity);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="val">0-1</param>
        public LiquidColorWhite(float val)
        {
            _alphaPercent = val;
        }

        public LiquidColorWhite()
        {

        }
    }

}