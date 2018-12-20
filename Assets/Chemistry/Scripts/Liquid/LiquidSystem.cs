using System.Collections.Generic;
using LiquidVolumeFX;
using Chemistry.Chemicals;
using UnityEngine;
using System;
using System.Collections;
using Sirenix.OdinInspector;

namespace Chemistry.Liquid
{
    //[RequireComponent(typeof(EquipmentLiquidChange))]
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

        [HideInInspector]
        public EquipmentLiquidChange liquidChange;

        //下降动画及其曲线
        [SerializeField, LabelText("液面下降动画曲线")]
        public AnimationCurve animationCurve;

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

        public void OnInitializeLiquidChange(float containerVolume, Action spillAction = null, Action overAction = null)
        {
            //if (liquidChange == null)
            //    liquidChange = GetComponent<EquipmentLiquidChange>();
            if (liquidChange == null)
                liquidChange = new EquipmentLiquidChange(this, containerVolume, spillAction, overAction);

            //liquidChange.OnInit(this, containerVolume, spillAction, overAction);
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

            _liquidVolume.liquidColor2 = curWaterColor; //水体
            _liquidVolume.foamColor = curSurfaceColor; //水面
        }

        //private List<Drug> _lstAllColorDrugs = new List<Drug>();
        //private float _sumVolume;
        
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


        /// <summary>
        /// 液体量变化
        /// </summary>
        /// <param name="startV">起始变化量</param>
        /// <param name="changeVolume">变化量(正为增，负为减)</param>
        /// <param name="time">变化时间（为0时突变）</param>
        /// <returns>变化后液体量</returns>
        public float ChangeLiquid(float startV, float changeVolume, float time = 0.5f)
        {
            return liquidChange.ChangeLiquid(startV, changeVolume, time);
        }



        /// <summary>
        /// 液体量变化--连接药品系统--实时更新药品量--若对变化过程不严格建议使用上一个
        /// </summary>
        /// <param name="drugSystem">当前容器的药品系统</param>
        /// <param name="changeVolume">变化量(正为增，负为减)</param>
        /// <param name="drugName">需要增减量的药品名</param>
        /// <param name="time">变化时间（为0时突变）</param>
        /// <returns>变化后液体量</returns>
        public void ChangeLiquid(DrugSystem drugSystem, float changeVolume, string drugName = "", float time = 0.5f, Action<string, float> actionTrans = null)
        {
            liquidChange.ChangeLiquid(drugSystem, changeVolume, drugName, time, actionTrans);
        }

    }

}