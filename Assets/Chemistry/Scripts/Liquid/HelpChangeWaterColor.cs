using LiquidVolumeFX;
using UnityEngine;

namespace Chemistry.Liquid
{
    public class HelpChangeWaterColor : MonoBehaviour
    {
        private LiquidVolume _liquidVolume;
        //private InitialDataSystem _initialDataSystem;

        private Color curWaterColor; //当前水体颜色
        private Color curSurfaceColor; //当前水面颜色

        private Color _colorWaterTarget; //目标水体颜色
        private Color _colorSurfaceTarget; //目标水面颜色

        private float curSparklingIntensity;
        private float _fltSparklingIntensity;

        private float _fltColorChangeSpeed; //颜色变化速率


        void Update()
        {
            //设置液体颜色
            SetWaterColorUpdate(_colorWaterTarget, _fltColorChangeSpeed);
            //设置液面颜色
            SetSurfaceColorUpdate(_colorSurfaceTarget, _fltColorChangeSpeed);
            //设置杂质
            SetSparklingIntensityUpdate(_fltSparklingIntensity, _fltColorChangeSpeed);

            //if (!_initialDataSystem.isHideLiquid)
            //{
            //    if (_initialDataSystem.currentVolume == 0)
            //        _liquidVolume.gameObject.SetActive(false);
            //    else
            //        _liquidVolume.gameObject.SetActive(true);
            //}
        }

        public void OnInitialized(LiquidVolume liquidVolume /*, InitialDataSystem data*/)
        {
            _liquidVolume = liquidVolume;
            //_initialDataSystem = data;

            //if (color is LiquidColorNode)
            {
                _liquidVolume.foamScale = 0.01f;
                _liquidVolume.foamThickness = 0.042f;
                _liquidVolume.foamDensity = 0.19f;
                _liquidVolume.foamWeight = 8f;
                _liquidVolume.foamTurbulence = 1f;
            }

            _liquidVolume.detail = DETAIL.Reflections;
            _liquidVolume.topology = TOPOLOGY.Irregular;
            _liquidVolume.renderQueue = 3000;
        }

        /// <summary>
        /// 趋近于某一个颜色（水体）
        /// </summary>
        /// <param name="watercolor"></param>
        /// <param name="speed"></param>
        private void SetSparklingIntensityUpdate(float watercolor, float speed)
        {
            curSparklingIntensity = ConventFloat(curSparklingIntensity, _fltSparklingIntensity, speed);

            _liquidVolume.sparklingIntensity = curSparklingIntensity;
        }

        /// <summary>
        /// 趋近于某一个颜色（水体）
        /// </summary>
        /// <param name="watercolor"></param>
        /// <param name="speed"></param>
        private void SetWaterColorUpdate(Color watercolor, float speed)
        {
            curWaterColor = ConventColor(curWaterColor, watercolor, speed);

            //_liquidVolume.liquidColor1 = curWaterColor;
            _liquidVolume.liquidColor2 = curWaterColor; //水体
        }

        /// <summary>
        /// 趋近于某一个颜色(水面)
        /// </summary>
        /// <param name="watercolor"></param>
        /// <param name="speed"></param>
        private void SetSurfaceColorUpdate(Color color, float speed)
        {
            curSurfaceColor = ConventColor(curSurfaceColor, color, speed);

            _liquidVolume.foamColor = curSurfaceColor; //水面
        }

        /// <summary>
        /// 趋近的关系转换
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="speed"></param>
        /// <returns></returns>
        private Color ConventColor(Color start, Color end, float speed)
        {
            start.r = ConventFloat(start.r, end.r, speed);
            start.g = ConventFloat(start.g, end.g, speed);
            start.b = ConventFloat(start.b, end.b, speed);
            start.a = ConventFloat(start.a, end.a, speed);
            return start;
        }

        /// <summary>
        /// 趋近的关系转换
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="speed"></param>
        /// <returns></returns>
        private float ConventFloat(float start, float end, float speed)
        {
            speed = Mathf.Clamp(speed, 0.01f, 0.99f);
            return start + (end - start) * speed * Time.deltaTime;
        }

        /// <summary>
        /// 设置液体颜色（渐变）
        /// </summary>
        /// <param name="color"></param>
        /// <param name="speed">0-1之间</param>
        public void SetWaterColorTarget(IWaterColor color, float speed = 1f)
        {
            //Debug.Log(color.WaterColor + "........................");
            _fltColorChangeSpeed = speed;
            _fltSparklingIntensity = color.SparklingIntensity;

            _colorWaterTarget = color.WaterColor;
            _colorSurfaceTarget = color.SurfaceColor;

            _liquidVolume.flaskGlossinessExternal = 0.0f;
            _liquidVolume.textureAlpha = 0.212f;
            _liquidVolume.blurIntensity = 0.312f;
            _liquidVolume.smokeEnabled = false;
        }

        /// <summary>
        /// 设置液体颜色（突变）
        /// </summary>
        /// <param name="color"></param>
        public void SetWaterColorToTarget(IWaterColor color)
        {
            SetWaterColorTarget(color);
            curSparklingIntensity = color.SparklingIntensity;
            curWaterColor = color.WaterColor;
            curSurfaceColor = color.SurfaceColor;
        }
    }

}

