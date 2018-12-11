using UnityEngine;

namespace Chemistry.Effects
{
    /// <summary>
    /// 特效-铁钉-铁生锈
    /// </summary>
    public class Effect_IronNailRust : EffectBase
    {

        /// <summary>
        /// 上边的高度 0 无  1 最大
        /// </summary>
        private float _upHigh;

        /// <summary>
        /// 下边的高度 0 最大  1 无
        /// </summary>
        private float _downHigh;

        /// <summary>
        /// 生锈强度 -颜色深度+体积+数量
        /// </summary>
        private float _degree;

        [SerializeField] private GameObject _effCopper;
        private Material _matCopper;

        [SerializeField] private GameObject _effCopper1;
        private Material _matCopper1;

        public override void OnInitialize()
        {
            base.OnInitialize();
            if (_effCopper == null)
            {
                Transform tra = transform.Find("FX_tieding_shengxiu");
                if (tra != null)
                    _effCopper = tra.gameObject;
            }

            if (_effCopper != null)
            {
                _matCopper = _effCopper.GetComponent<MeshRenderer>().material;
            }
            else
            {
                Debug.LogError("特效物体为空，请注意....");
            }

            if (_effCopper1 == null)
            {
                Transform tra = transform.Find("FX_tieding_shengxiu1");
                if (tra != null)
                    _effCopper1 = tra.gameObject;
            }

            if (_effCopper1 != null)
            {
                _matCopper1 = _effCopper1.GetComponent<MeshRenderer>().material;
            }
            else
            {
                Debug.LogError("特效物体为空，请注意....");
            }

            CloseEffect();
        }

        private void CloseEffect()
        {
            SetUpHigh(0);
            SetDownHigh1(1);
        }

        /// <summary>
        /// 设置上边高度
        /// </summary>
        /// <param name="val"></param>
        public void SetUpHigh(float val)
        {
            _upHigh = val;
            //0.5-1
            float value = 0.5f + (0f - 0.5f) * val;

            _matCopper.SetFloat("_Clip_top", value);
        }

        public void SetDownHigh(float val)
        {
            _downHigh = val;
            //0-0.5
            float value = 0.5f + (1f - 0.5f) * val;

            _matCopper.SetFloat("_Clip_bottom", value);
        }

        /// <summary>
        /// 设置强度
        /// </summary>
        /// <param name="val"></param>
        public void SetDegree(float val)
        {
            //_opacity 0-1 透明度
            //_dong 0-0.7 残破度

            float value = val / 0.7f;

            _matCopper.SetFloat("_opacity", val);
            _matCopper.SetFloat("_dong", 1 - (0.5f + value * 0.5f));
        }

        /// <summary>
        /// 设置上边高度
        /// </summary>
        /// <param name="val"></param>
        public void SetUpHigh1(float val)
        {
            _upHigh = val;
            //0.5-1
            float value = 0.5f + (0f - 0.5f) * val;

            _matCopper1.SetFloat("_Clip_top", value);
        }

        public void SetDownHigh1(float val)
        {
            _downHigh = val;
            //0-0.5
            float value = 0.5f - val * 0.5f;

            _matCopper1.SetFloat("_Clip_bottom", value);
        }

        /// <summary>
        /// 设置强度
        /// </summary>
        /// <param name="val"></param>
        public void SetDegree1(float val)
        {
            //_opacity 0-1 透明度
            //_dong 0-0.7 残破度

            float value = val / 0.7f;

            _matCopper1.SetFloat("_opacity", val);
            _matCopper1.SetFloat("_dong", 1 - (0.5f + value * 0.5f));
        }
    }

}
