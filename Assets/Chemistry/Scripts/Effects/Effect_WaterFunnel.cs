using Chemistry.Chemicals;
using UnityEngine;

namespace Chemistry.Effects
{
    /// <summary>
    /// 特效控制-漏斗水面
    /// </summary>
    public class Effect_WaterFunnel : EffectBase
    {

        [SerializeField] private GameObject _goUpWater; //上边
        [SerializeField] private GameObject _goDownWater; //下边

        private Material _matUp;
        private Material _matDown;
        private float _percent; //当前的液体的量
        private float _lastPercent;

        [SerializeField] private float _speed;
        private float _fltDownUpValue;
        private float _fltDownDownValue;


        public override void OnInitialize()
        {
            base.OnInitialize();
            _matUp = _goUpWater.GetComponent<MeshRenderer>().material;
            _matDown = _goDownWater.GetComponent<MeshRenderer>().material;

            if (_matUp == null || _matDown == null)
                Debug.LogError("漏斗液面特效材质为空，请检查...");
        }

        /// <summary>
        /// 设置液体的量
        /// </summary>
        /// <param name="percenet"></param>
        public void SetWaterValue(DrugSystem drugSystem, float percenet, bool ispaper)
        {
            //up 0-0.358 
            _matUp.SetFloat("_height", ConventFormPercent(0, 0.358f, percenet));
            _percent = percenet;
            if (_percent == _lastPercent && _percent != 0f && _percent < 0.5f)
            {
                //停止变化 
                //Debug.Log(_percent);
                _fltDownUpValue = Mathf.Max(_fltDownUpValue - Time.deltaTime * _speed, 0);
                if (_fltDownUpValue == 0)
                    _fltDownDownValue = 0;
            }
            else
            {
                if (_percent > 0)
                {
                    _fltDownDownValue = Mathf.Min(1f, _fltDownDownValue + Time.deltaTime * _speed);
                    _fltDownUpValue = 1f;
                }
            }
            _matDown.SetFloat("_Offset", ConventFormPercent(-0.43f, 0.43f, _fltDownUpValue));
            _matDown.SetFloat("_height", ConventFormPercent(0, 0.85f, _fltDownDownValue));
            _lastPercent = _percent;
            SetColor(drugSystem, ispaper);
        }

        private void SetDown()
        {
            //_Offset 0.43 最上边的位置
            //_height 0.85 最长 

            //加水 offset不变 height 0 - 0.85

            //结束 offset 0.43 - -0.43
        }

        private float ConventFormPercent(float min, float max, float per)
        {
            return min + (max - min) * per;
        }

        private void SetColor(DrugSystem drugSystem, bool ispaper)
        {
            if (drugSystem == null) return;
            Color coloryellow = new Color(178f / 255f, 106f / 255f, 8f / 255f);
            Color colornone = new Color(204f / 255f, 199f / 255f, 190f / 255f);

            if (drugSystem.IsHaveDrugForName("沙"))
            {
                _matUp.SetColor("_WaveColor", coloryellow);
                if (ispaper)
                    _matDown.SetColor("_DownColor", colornone);
                else
                    _matDown.SetColor("_DownColor", coloryellow);
            }
            else
            {
                _matUp.SetColor("_UpColor", colornone);
                _matDown.SetColor("_DownColor", colornone);
            }
            //178 106 8
        }
    }

}
