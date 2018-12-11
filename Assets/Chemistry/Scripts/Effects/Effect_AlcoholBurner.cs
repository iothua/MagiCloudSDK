using UnityEngine;

namespace Chemistry.Effects
{
    /// <summary>
    /// 特效-酒精灯火焰
    /// </summary>
    public class Effect_AlcoholBurner : EffectBase
    {
        private GameObject _fire;
        private ParticleSystem _particleSystem;
        private ParticleSystem.ShapeModule _shapeModule;

        public override void OnInitialize()
        {
            base.OnInitialize();
            _fire = transform.Find("Fire").gameObject;
            _particleSystem = _fire.transform.Find("FX_jjd_fire").GetComponent<ParticleSystem>();
            _shapeModule = _particleSystem.shape;
        }

        public void SetFireWeight(float percent)
        {
            if (_particleSystem == null) return;

            _shapeModule.angle = 80f * percent;
        }
    }

}