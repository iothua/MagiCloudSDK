using Chemistry.Chemicals;
using UnityEngine;

namespace Chemistry.Effects
{
    /// <summary>
    /// 特效-蒸发皿
    /// </summary>
    public class Effect_EvaporatingDish : EffectBase
    {
        [SerializeField] private GameObject _smoke; //烟（加热）
        [SerializeField] private GameObject _crystal; //晶体（析出）
        [SerializeField] private GameObject _splash; //飞溅（没有水）
        [SerializeField] private GameObject _waterSplash; //水飞溅（不搅拌）
        [SerializeField] private GameObject _ripple; //波纹（玻璃棒波纹）

        private ParticleSystem _crystalParticleSystem; //晶体控制
        private ParticleSystem.MainModule _crystalMainModule; //晶体控制

        private DrugSystem _drugSystem;
        private Transform _upPoint;
        private Transform _downPoint;


        public override void OnInitialize()
        {
            base.OnInitialize();
            _smoke = transform.Find("烟").gameObject;
            _crystal = transform.Find("晶体").gameObject;
            _splash = transform.Find("飞溅").gameObject;
            _waterSplash = transform.Find("水飞溅").gameObject;
            _ripple = transform.Find("波纹").gameObject;

            _crystalParticleSystem = _crystal.GetComponentInChildren<ParticleSystem>();
            _crystalMainModule = _crystalParticleSystem.main;

            _upPoint = transform.Find("UpPoint");
            _downPoint = transform.Find("DownPoint");

            ClaseEffect();
        }

        public void HeatBehavior(DrugSystem drugSystem, float temper, bool isGalssBar)
        {
            //TODO...加热特效
            Drug drugwater;
            DrugData objectWater;

            Drug drugsalt;
            DrugData objectSalt;

            drugSystem.FindDrugForName("水", out objectWater);
            drugSystem.FindDrugForName("氯化钠", out objectSalt);

            drugwater = objectWater.DrugObject == null ? null : (Drug)objectWater.DrugObject;
            drugsalt = objectSalt.DrugObject == null ? null : (Drug)objectSalt.DrugObject;

            if (drugwater != null)
            {
                if (drugwater.Volume > 0)
                {
                    //有水

                    //烟控制 与温度有关
                    if (temper >= 80f)
                        ShowSmoke();
                    else
                        HideSmoke();

                    //波纹控制 加热就有波纹
                    //ShowRipple(drugSystem.GetDrugSumPercent());

                    //水飞溅
                    if (temper >= 80f && isGalssBar == false)
                        ShowWaterSplash(drugSystem.Percent);
                    else
                        HideWaterSplash();

                    //析出盐的操作
                    if (drugsalt != null)
                    {
                        if (drugsalt.Mass > drugwater.Mass * drugsalt.Solubility)
                        {
                            //析出的质量
                            float mass = drugsalt.Mass - drugwater.Mass * drugsalt.Solubility;
                            ShowCrystal(mass);
                        }
                    }
                }
                else
                {
                    //水蒸发完了
                    HideSmoke();
                    HideRipple();
                    HideWaterSplash();

                    //盐飞溅
                    if (temper >= 80f && drugsalt != null)
                        ShowSplash();
                    else
                        HideSplash();
                }
            }



        }

        public void GlassBarBehavior(DrugSystem drugSystem, bool isGlassBar)
        {
            //温度为常温
            if (isGlassBar)
                ShowRipple(drugSystem.Percent);
            else
                HideRipple();
        }

        private void ClaseEffect()
        {
            HideCrystal();
            HideRipple();
            HideSmoke();
            HideSplash();
            HideWaterSplash();
        }

        private void ChangeCrystalNumber(float mass)
        {
            _crystalMainModule.startLifetime = mass * 30f;
        }

        /// <summary>
        /// 显示冒烟
        /// </summary>
        public void ShowSmoke()
        {
            if (_smoke.activeSelf == false)
                _smoke.SetActive(true);
        }

        /// <summary>
        /// 隐藏冒烟
        /// </summary>
        public void HideSmoke()
        {
            if (_smoke.activeSelf)
                _smoke.SetActive(false);
        }

        /// <summary>
        /// 显示晶体
        /// </summary>
        public void ShowCrystal(float mass)
        {
            if (_crystal.activeSelf == false)
                _crystal.SetActive(true);
            ChangeCrystalNumber(mass);
        }

        /// <summary>
        /// 隐藏晶体
        /// </summary>
        public void HideCrystal()
        {
            if (_crystal.activeSelf)
                _crystal.SetActive(false);
        }

        /// <summary>
        /// 显示飞溅
        /// </summary>
        public void ShowSplash()
        {
            if (_splash.activeSelf == false)
                _splash.SetActive(true);
        }

        /// <summary>
        /// 隐藏飞溅
        /// </summary>
        public void HideSplash()
        {
            if (_splash.activeSelf)
                _splash.SetActive(false);
        }

        /// <summary>
        /// 显示水飞溅
        /// </summary>
        public void ShowWaterSplash(float high)
        {
            if (_waterSplash.activeSelf == false)
                _waterSplash.SetActive(true);

            float y = _downPoint.position.y + (_upPoint.position.y - _downPoint.position.y) * high;
            _waterSplash.transform.position =
                new Vector3(_waterSplash.transform.position.x, y, _waterSplash.transform.position.z);
        }

        /// <summary>
        /// 隐藏水飞溅
        /// </summary>
        public void HideWaterSplash()
        {
            if (_waterSplash.activeSelf)
                _waterSplash.SetActive(false);
        }

        /// <summary>
        /// 显示波纹
        /// </summary>
        public void ShowRipple(float high)
        {
            if (_ripple.activeSelf == false)
                _ripple.SetActive(true);

            float y = _downPoint.position.y + (_upPoint.position.y - _downPoint.position.y) * high;
            _ripple.transform.position = new Vector3(_ripple.transform.position.x, y, _ripple.transform.position.z);
        }

        /// <summary>
        /// 隐藏波纹
        /// </summary>
        public void HideRipple()
        {
            if (_ripple.activeSelf)
                _ripple.SetActive(false);
        }
    }

}