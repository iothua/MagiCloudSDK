using Chemistry.Data;
using UnityEngine;

namespace Chemistry.Effects
{
    /// <summary>
    /// 特效-火焰
    ///     控制火焰的状态
    /// </summary>
    public class Effect_Fire : EffectBase
    {
        private Transform _traMove;
        private GameObject _smallFire;
        private GameObject _bigFire;
        private GameObject _smoke;

        public override void OnInitialize()
        {
            base.OnInitialize();
            _traMove = transform.Find("Move");
            _smallFire = _traMove.Find("SmallFire").gameObject;
            _bigFire = _traMove.Find("BigFire").gameObject;
            _smoke = _traMove.Find("Smoke").gameObject;
        }

        public void SetState(TriggerFireState fireState)
        {
            switch (fireState)
            {
                case TriggerFireState.初始:
                    _smallFire.SetActive(false);
                    _bigFire.SetActive(false);
                    _smoke.SetActive(false);
                    break;
                case TriggerFireState.灭:
                    _smallFire.SetActive(false);
                    _bigFire.SetActive(false);
                    _smoke.SetActive(true);
                    break;
                case TriggerFireState.火星:
                    _smallFire.SetActive(false);
                    _bigFire.SetActive(false);
                    _smoke.SetActive(true);
                    break;
                case TriggerFireState.燃烧:
                    _smallFire.SetActive(true);
                    _bigFire.SetActive(false);
                    _smoke.SetActive(false);
                    break;
                case TriggerFireState.旺:
                    _smallFire.SetActive(false);
                    _bigFire.SetActive(true);
                    _smoke.SetActive(false);
                    break;
            }
        }

    }

}