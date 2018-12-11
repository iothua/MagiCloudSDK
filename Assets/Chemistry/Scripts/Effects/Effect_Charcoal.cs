using System;
using System.Collections;
using UnityEngine;

namespace Chemistry.Effects
{
    /// <summary>
    /// 特效控制-木炭
    /// </summary>
    public class Effect_Charcoal : EffectBase
    {
        private Transform _traHeatNode;

        private GameObject _goHeatRed; //变红的那个
        private GameObject _goSmog; //熄灭之后的那个烟
        private GameObject _goLight; //加热的时候得那个光

        private Material _matHeatRed;
        private float _curTemperature; //当前温度
        private bool _isCanHeat = true;

        public override void OnInitialize()
        {
            base.OnInitialize();
            _traHeatNode = transform.Find("Heat");

            _goHeatRed = _traHeatNode.Find("FX_mutan_fire").gameObject;
            _goSmog = _traHeatNode.Find("fire").gameObject;
            _goLight = _traHeatNode.Find("glow_out (1)").gameObject;

            _matHeatRed = _goHeatRed.GetComponent<MeshRenderer>().material;

            ShowHeatEffect(0f);
            _goSmog.SetActive(false);
            _goLight.SetActive(false);
        }

        /// <summary>
        /// 显示加热效果
        /// </summary>
        /// <param name="temperature">当前温度（0-1）</param>
        public void ShowHeatEffect(float temperature)
        {
            temperature = Mathf.Clamp(temperature, 0f, 1f);
            _curTemperature = temperature;
            //0-3
            float val = temperature * 3f;
            _matHeatRed.SetFloat("_fire_intensity", val);
        }

        /// <summary>
        /// 显示熄灭特效
        /// </summary>
        public void ShowExtinguishEffect(float time)
        {
            if (_goSmog.activeSelf == false && _isCanHeat && _curTemperature >= 0.15f)
            {
                _goSmog.SetActive(true);
                ShowHeatEffect(0);
                _isCanHeat = false;
                StartCoroutine(WaitForSec(time, () =>
                {
                    _isCanHeat = true;
                    _goSmog.SetActive(false);
                }));
            }

        }

        IEnumerator WaitForSec(float time, Action function)
        {
            yield return new WaitForSeconds(time);
            function();
        }
    }

}