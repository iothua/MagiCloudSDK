using UnityEngine;

namespace Chemistry.Effects
{
    /// <summary>
    /// 特效-铁钉-铜析出
    /// </summary>
    public class Effect_IronNailCopper : EffectBase
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

        public override void OnInitialize()
        {
            base.OnInitialize();
            if (_effCopper == null)
            {
                Transform tra = transform.Find("FX_tieding_tong");
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
            CloseEffect();
        }

        private void CloseEffect()
        {
            SetUpHigh(0);
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

            _matCopper.SetFloat("_Clip", value);
        }

        public void SetDownHigh(float val)
        {
            _downHigh = val;
            //0-0.5
            float value = 0.5f + (1f - 0.5f) * val;

            _matCopper.SetFloat("_Clip_copy", value);
        }

        /// <summary>
        /// 设置强度
        /// </summary>
        /// <param name="val"></param>
        public void SetDegree(float val)
        {
            //opacity 0-1 透明度
            //_dong 0-0.7 残破度
            //_JT_Length 0-1 毛糙度

            _matCopper.SetFloat("_opacity", val);
            //_matCopper.SetFloat("_dong", 1 - (0.5f + value * 0.5f));
            _matCopper.SetFloat("_JT_Length", val * 0.5f);
        }
    }

}
