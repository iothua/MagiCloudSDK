using Chemistry.Chemicals;
using System.Collections.Generic;
using UnityEngine;
using Chemistry.Liquid;

namespace Chemistry.Effects
{
    public class Effect_Beaker : EffectBase
    {
        private GameObject _effRotateNone; //旋转无

        private GameObject _effRotateSand; //旋转沙
        private GameObject _effRotateSalt; //旋转盐
        private ParticleSystem[] _effParticleSystems; //粒子集合
        private List<ParticleSystem.MainModule> _effMainModile; //粒子控制


        private GameObject _effRotateEndSand;
        private GameObject _effRotateEndSalt;

        private GameObject _effJoinSand;
        private GameObject _effJoinSalt;

        private bool _isSand;
        private bool _isSalt;

        public override void OnInitialize()
        {
            base.OnInitialize();
            _effJoinSalt = transform.Find("盐").gameObject;
            _effJoinSand = transform.Find("沙").gameObject;

            _effRotateNone = transform.Find("搅拌无").gameObject;
            _effRotateSand = transform.Find("搅拌沙").gameObject;
            _effRotateSalt = transform.Find("搅拌盐").gameObject;
            _effParticleSystems = _effRotateSalt.GetComponentsInChildren<ParticleSystem>();
            _effMainModile = new List<ParticleSystem.MainModule>();
            for (int i = 0; i < _effParticleSystems.Length; i++)
            {
                _effMainModile.Add(_effParticleSystems[i].main);
            }

            _effRotateEndSand = transform.Find("搅拌结束沙").gameObject;
            _effRotateEndSalt = transform.Find("搅拌结束盐").gameObject;

            CloseEffects();
        }

        /// <summary>
        /// 加入盐
        /// </summary>
        public void JoinSalt()
        {
            _effJoinSalt.SetActive(true);
            _isSalt = true;
        }

        /// <summary>
        /// 加入沙
        /// </summary>
        public void JoinSand()
        {
            _effJoinSand.SetActive(true);
            _isSand = true;
        }

        /// <summary>
        /// 开始旋转
        /// </summary>
        /// <param name="percent"></param>
        public void StartRotate(DrugSystem drugSystem, LiquidSystem liquid)
        {
            float percent = drugSystem.Percent;
            //0-2.8
            SetScaleHigh(percent);

            bool isrotate = false;
            if (_isSand)
            {
                liquid.SetWaterColorTarget(new LiquidColorYellow_Soil());
                _effRotateSand.SetActive(true);
                _effJoinSand.SetActive(false);
                isrotate = true;
            }

            if (_isSalt)
            {
                _effRotateSalt.SetActive(true);
                _effJoinSalt.SetActive(false);
                isrotate = true;
            }

            if (isrotate == false)
                _effRotateNone.SetActive(true);

            _effRotateEndSalt.SetActive(false);
            _effRotateEndSand.SetActive(false);
        }

        /// <summary>
        /// 结束旋转
        /// </summary>
        public void EndRotate()
        {

            if (_isSand)
            {
                _effRotateSand.SetActive(false);
                _effRotateEndSand.SetActive(true);
            }

            if (_isSalt)
            {
                _effRotateSalt.SetActive(false);
                _effRotateEndSalt.SetActive(true);
            }

            if (_effRotateNone.activeSelf)
                _effRotateNone.SetActive(false);
        }

        public void SetSaltValue(float percent)
        {
            ParticleSystem.MainModule main;
            for (int i = 0; i < _effMainModile.Count; i++)
            {
                main = _effMainModile[i];
                main.startLifetime = 3f * percent;
            }
        }

        private void CloseEffects()
        {
            _effRotateNone.SetActive(false);

            _effRotateSand.SetActive(false);
            _effRotateSalt.SetActive(false);
            _effRotateEndSand.SetActive(false);
            _effRotateEndSalt.SetActive(false);
            _effJoinSand.SetActive(false);
            _effJoinSalt.SetActive(false);
        }

        private void SetScaleHigh(float percent)
        {
            float val = Mathf.Clamp(2.8f * percent, 0.1f, 2.8f);
            Vector3 v3 = new Vector3(1f, val, 1f);
            _effRotateEndSalt.transform.GetChild(0).localScale = v3;
            _effRotateEndSand.transform.GetChild(0).localScale = v3;
            _effRotateSalt.transform.GetChild(0).localScale = v3;
            _effRotateSand.transform.GetChild(0).localScale = v3;
            _effRotateNone.transform.GetChild(0).localScale = v3;
        }

    }

}
