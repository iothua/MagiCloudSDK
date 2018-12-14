using System.Collections.Generic;
using LiquidVolumeFX;
using Chemistry.Chemicals;
using UnityEngine;

namespace Chemistry.Liquid
{
    public class LiquidSystem : MonoBehaviour
    {
        [HideInInspector]
        public LiquidVolume _liquidVolume;
        public DrugSystem drugSystem { get; set; }

        [HideInInspector]
        public Color curWaterColor; //当前水体颜色
        [HideInInspector]
        public Color curSurfaceColor; //当前水面颜色
        [HideInInspector]
        public Color _colorWaterTarget; //目标水体颜色
        [HideInInspector]
        public Color _colorSurfaceTarget; //目标水面颜色
        [HideInInspector]
        public float curSparklingIntensity;
        [HideInInspector]
        public float _fltSparklingIntensity;
        [HideInInspector]
        private float _fltColorChangeSpeed; //颜色变化速率

        private void Awake()
        {
            if (_liquidVolume == null)
                _liquidVolume = gameObject.GetComponentInChildren<LiquidVolume>();
        }

        /// <summary>
        /// 编辑器生成初始化
        /// </summary>
        /// <param name="drugSystem"></param>
        /// <param name="liquid"></param>
        public void OnInitialize_Editor(DrugSystem drugSystem, GameObject liquid, TOPOLOGY oPOLOGY = TOPOLOGY.Irregular)
        {
            //初始化液面
            this.drugSystem = drugSystem;
            if (liquid != null)
                _liquidVolume = liquid.GetComponent<LiquidVolume>() ?? liquid.AddComponent<LiquidVolume>();
            else
                _liquidVolume = gameObject.GetComponentInChildren<LiquidVolume>();

            InitData(oPOLOGY);

            ////更改颜色
            //SelfChangeColor();
            //设置液体颜色
            SetWaterColorUpdate(_colorWaterTarget, _fltColorChangeSpeed);
            //设置液面颜色
            SetSurfaceColorUpdate(_colorSurfaceTarget, _fltColorChangeSpeed);
            //设置杂质
            SetSparklingIntensityUpdate(_fltSparklingIntensity, _fltColorChangeSpeed);
        }

        /// <summary>
        /// 代码调用初始化
        /// </summary>
        /// <param name="drugSystem"></param>
        public void OnInitialize(DrugSystem drugSystem,TOPOLOGY oPOLOGY = TOPOLOGY.Irregular)
        {
            this.drugSystem = drugSystem;
            InitData(oPOLOGY);

            SetWaterColorTarget(drugSystem.GetColor());

            ////更改颜色
            //SelfChangeColor();
            //设置液体颜色
            SetWaterColorUpdate(_colorWaterTarget, _fltColorChangeSpeed);
            //设置液面颜色
            SetSurfaceColorUpdate(_colorSurfaceTarget, _fltColorChangeSpeed);
            //设置杂质
            SetSparklingIntensityUpdate(_fltSparklingIntensity, _fltColorChangeSpeed);
        }

        /// <summary>
        /// 设置液体的量（百分比）
        /// </summary>
        /// <param name="percent">0-1</param>
        public void SetValue(float percent)
        {
            _liquidVolume.level = Mathf.Clamp(percent, 0f, 0.98f);
            if (percent <= 0.01)
            {
                _liquidVolume.foamTurbulence = 0f;
                CloseLiquidVolume();
            }
            else
            {
                _liquidVolume.foamTurbulence = 0.5f;
                OpenLiquidVolume();
            }
        }

        /// <summary>
        /// 设置液体的量
        /// </summary>
        /// <param name="curValue">当前量</param>
        /// <param name="sumValue">总量</param>
        public void SetValue(float curValue, float sumValue)
        {
            _liquidVolume.level = curValue / sumValue;
            if (_liquidVolume.level<0.001f)
            {
                CloseLiquidVolume();
            }
            else
            {
                OpenLiquidVolume();
            }
        }

        /// <summary>
        /// 设置液体颜色（渐变）
        /// </summary>
        /// <param name="color"></param>
        /// <param name="speed">0-1之间</param>
        public void SetWaterColorTarget(IWaterColor color, float speed = 1f)
        {
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
        /// 设置液体颜色（渐变）
        /// </summary>
        /// <param name="color"></param>
        /// <param name="speed">0-1之间</param>
        public void SetWaterColorTarget(Color color, float speed = 1f)
        {
            _fltColorChangeSpeed = speed;

            _colorWaterTarget = color;
            _colorSurfaceTarget = color;

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

        //private List<Drug> _lstAllColorDrugs = new List<Drug>();
        //private float _sumVolume;

        /// <summary>
        /// 自己变换颜色
        /// </summary>
        private void SelfChangeColor()
        {
            #region 注释


            ////颜色权重 + 颜色数量 + 颜色 + 稀释程度
            //_lstAllColorDrugs.Clear();
            //_sumVolume = 0;
            //object drug;
            //if (drugSystem.FindDrugForName("酚酞", out drug, DrugStyle.纯净物))
            //{
            //    _lstAllColorDrugs.Add((Drug)drug);
            //}
            //if (drugSystem.FindDrugForName("石蕊", out drug, DrugStyle.纯净物))
            //{
            //    _lstAllColorDrugs.Add(drug);
            //}
            //if (drugSystem.FindDrugForName("硫酸铜", out drug, DrugStyle.纯净物))
            //{
            //    _lstAllColorDrugs.Add(drug);
            //}
            //if (drugSystem.FindDrugForName("硫酸亚铁", out drug, DrugStyle.纯净物))
            //{
            //    _lstAllColorDrugs.Add(drug);
            //}
            //if (drugSystem.FindDrugForName("铁离子", out drug, DrugStyle.纯净物))
            //{
            //    _lstAllColorDrugs.Add(drug);
            //}
            //if (drugSystem.FindDrugForName("碳酸钙", out drug, DrugStyle.纯净物))
            //{
            //    _lstAllColorDrugs.Add(drug);
            //}
            //if (drugSystem.FindDrugForName("沙", out drug, DrugStyle.纯净物))
            //{
            //    _lstAllColorDrugs.Add(drug);
            //}
            //if (drugSystem.FindDrugForName("盐酸", out drug, DrugStyle.纯净物))
            //{
            //    _lstAllColorDrugs.Add(drug);
            //}

            //if (_drugSystem.FindDrugForName("水", out drug))
            //{
            //    _lstAllColorDrugs.Add(drug);
            //}
            //foreach (Drug item in _lstAllColorDrugs)
            //{
            //    _sumVolume += item.Volume;
            //}

            #endregion

            Color color = new Color(1f, 1f, 1f, 0);

            #region 注释


            //if (drugSystem.IsHaveDrugForName("水"))
            //{
            //    color.a = 0.2f;

            //    if (drugSystem.FindDrugForName("硫酸铜", out drug))
            //    {
            //        color += (new LiquidColorBlue()).WaterColor * (drug.Volume / _sumVolume);
            //    }
            //    if (drugSystem.FindDrugForName("硫酸亚铁", out drug))
            //    {
            //        color += (new LiquidColorGreen()).WaterColor * (drug.Volume / _sumVolume);
            //    }
            //    if (drugSystem.FindDrugForName("铁离子", out drug))
            //    {
            //        color += (new LiquidColorYellow()).WaterColor * (drug.Volume / _sumVolume);
            //    }
            //    if (drugSystem.FindDrugForName("碳酸钙", out drug))
            //    {
            //        color += (new LiquidColorWhite()).WaterColor * (drug.Volume / _sumVolume);
            //    }
            //    if (drugSystem.FindDrugForName("沙", out drug))
            //    {
            //        color += (new LiquidColorYellow_Soil()).WaterColor * (drug.Volume / _sumVolume);
            //    }
            //    //if (drugSystem.FindDrugForName("盐酸", out drug))
            //    //{
            //    //    //color += (new LiquidColorNode()).WaterColor * (drug.Volume / _sumVolume);
            //    //}
            //}
            #endregion
            DrugData drugMixture;

            if (drugSystem.FindDrugForName("硫酸铜溶液", out drugMixture))
            {
                color.a = 0.2f;
                color += (new LiquidColorBlue()).WaterColor * (drugMixture.Volume / drugSystem.CurSumVolume);
            }

            #region 注释

            //if (drugSystem.FindDrugForName("硫酸亚铁", out drug))
            //{
            //    color += (new LiquidColorGreen()).WaterColor * (drug.Volume / _sumVolume);
            //}
            //if (drugSystem.FindDrugForName("铁离子", out drug))
            //{
            //    color += (new LiquidColorYellow()).WaterColor * (drug.Volume / _sumVolume);
            //}
            //if (drugSystem.FindDrugForName("碳酸钙", out drug))
            //{
            //    color += (new LiquidColorWhite()).WaterColor * (drug.Volume / _sumVolume);
            //}
            //if (drugSystem.FindDrugForName("沙", out drug))
            //{
            //    color += (new LiquidColorYellow_Soil()).WaterColor * (drug.Volume / _sumVolume);
            //}

            #endregion

            SetWaterColorTarget(color, 0.5f);
            //液体颜色

            //液面颜色

            //浑浊度

        }

        #region 私有方法

        /// <summary>
        /// 初始化数据
        /// </summary>
        private void InitData(TOPOLOGY oPOLOGY = TOPOLOGY.Irregular)
        {

            /*初始化一些数据*/
            _liquidVolume.detail = DETAIL.Reflections;
            _liquidVolume.topology = oPOLOGY;
            _liquidVolume.liquidColor1 = new Color(0, 0, 0, 0);
            _liquidVolume.sparklingAmount = 0;
            _liquidVolume.deepObscurance = 0.1f;


            _liquidVolume.foamScale = 0.2f;
            _liquidVolume.foamThickness = 0.037f;
            _liquidVolume.foamDensity = 0f;
            _liquidVolume.foamWeight = 8f;
            _liquidVolume.foamTurbulence = 0.5f;

            _liquidVolume.smokeColor = new Color(0.8f, 0.8f, 0.8f, 0);

            _liquidVolume.textureAlpha = 0;

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
            //curWaterColor = ConventColor(curWaterColor, watercolor, speed);
            curWaterColor = watercolor;
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

        private void CloseLiquidVolume()
        {
            if (_liquidVolume.gameObject.activeSelf)
                _liquidVolume.gameObject.SetActive(false);
        }

        private void OpenLiquidVolume()
        {
            if (_liquidVolume.gameObject.activeSelf == false)
                _liquidVolume.gameObject.SetActive(true);
        }
        #endregion



    }

}