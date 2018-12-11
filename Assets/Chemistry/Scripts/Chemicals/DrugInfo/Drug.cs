using System;
using Chemistry.Data;
using UnityEngine;

//云幻-化学药品
namespace Chemistry.Chemicals
{
    /// <summary>
    /// 纯净物
    /// 1.铁钉 2.水 3.氧气 4.酚酞等
    /// </summary>
    [Serializable]
    public class Drug
    {
        [SerializeField]
        private string _name;
        /// <summary>
        /// 体积（毫升）
        /// </summary>
        [SerializeField]
        private float _volume = -1f;
        /// <summary>
        /// 质量（克）
        /// </summary>
        private float _mass = -1f;
        /// <summary>
        /// 密度
        /// </summary>
        private float _density;
        /// <summary>
        /// 类型
        /// </summary>
        private EDrugType _eDrugType;
        /// <summary>
        /// 溶解度(只有可溶解物体才会有这个参数)
        /// </summary>
        private float _solubility;
        /// <summary>
        /// 当期溶解度
        /// </summary>
        private float _curSolubility;

        public string Name
        {
            get { return _name; }
        }
        /// <summary>
        /// 体积（ml）
        /// </summary>
        public float Volume
        {
            get { return _volume == -1 ? Mass / _density : _volume; }
            set { _volume = value; }
        }
        /// <summary>
        /// 质量（g）
        /// </summary>
        public float Mass
        {
            get { return _mass == -1 ? Volume * _density : _mass; }
            set { _mass = value; }
        }
        /// <summary>
        /// 纯净物类型
        /// </summary>
        public EDrugType DrugType
        {
            get { return _eDrugType; }
        }
        /// <summary>
        /// 溶解度
        /// </summary>
        public float Solubility
        {
            get { return DrugType == EDrugType.Solid_Powder ? _solubility : 0; }
        }
        /// <summary>
        /// 当前溶解度(设置时传入参数为溶剂质量)
        /// </summary>
        public float CurrentSolubility
        {
            get { return DrugType == EDrugType.Solid_Powder ? _curSolubility : 0; }
            set { _curSolubility = Mass / value; }
        }
        /// <summary>
        /// 密度
        /// </summary>
        public float Density { get { return _density; } }

        /*构造函数*/

        /// <summary>
        /// 新建药品（纯净物）
        /// </summary>
        /// <param name="name">名字</param>
        /// <param name="val">剂量</param>
        /// <param name="unit">单位</param>
        public Drug(string name, float val)
        {
            DI_DrugPureInfo pureInfo;
            if (DataLoading.DicDrugPureLoadingInfo.TryGetValue(name, out pureInfo))
            {
                //名字
                _name = name;
                //密度
                _density = pureInfo.density;
                //类型
                _eDrugType = (EDrugType)pureInfo.drugType;
                //量
                if ((int)pureInfo.unit == 0)
                {
                    //毫升
                    _mass = -1f;
                    _volume = val;
                }
                else if ((int)pureInfo.unit == 1)
                {
                    //克
                    _mass = val;
                    _volume = -1f;
                }
                //溶解度
                if (DrugType == EDrugType.Solid_Powder)
                    _solubility = pureInfo.solubility;


            }
            else
            {
                Debug.LogError("数据中没有“" + name + "”这个药品...");
            }
        }

        /// <summary>
        /// 新建药品（纯净物）
        /// </summary>
        /// <param name="name">名字</param>
        /// <param name="val">剂量</param>
        /// <param name="unit">单位</param>
        public Drug(string name, float val, EMeasureUnit unit)
        {
            DI_DrugPureInfo pureInfo;
            if (DataLoading.DicDrugPureLoadingInfo.TryGetValue(name, out pureInfo))
            {
                //名字
                _name = name;
                //密度
                _density = pureInfo.density;
                //类型
                _eDrugType = (EDrugType)pureInfo.drugType;
                //量
                if ((int)pureInfo.unit == 0)
                {
                    //毫升
                    _mass = -1f;
                    _volume = val;
                    if (unit == EMeasureUnit.g)
                        _volume = val / _density;
                }
                else if ((int)pureInfo.unit == 1)
                {
                    //克
                    _mass = val;
                    _volume = -1f;
                    if (unit == EMeasureUnit.ml)
                        _mass = val * _density;
                }

                //溶解度
                if (DrugType == EDrugType.Solid_Powder)
                    _solubility = pureInfo.solubility;


            }
            else
            {
                Debug.LogError("数据中没有“" + name + "”这个药品...");
            }
        }

        /// <summary>
        /// 新建药品（纯净物,不在数据库中）
        /// </summary>
        /// <param name="name">名字</param>
        /// <param name="val">剂量</param>
        /// <param name="den">密度</param>
        /// <param name="sol">溶解度</param>
        /// <param name="eDrugType">药品类型</param>
        /// <param name="unit">单位</param>
        public Drug(string name, float vol, float den, float sol, EDrugType eDrugType, EMeasureUnit unit = EMeasureUnit.ml)
        {
            //名字
            _name = name;
            //密度
            _density = den;
            //类型
            _eDrugType = eDrugType;
            //量
            if (unit == EMeasureUnit.ml)
            {
                _mass = -1f;
                _volume = vol;
            }
            else
            {
                _mass = vol;
                _volume = -1f;
            }
            //溶解度
            if (DrugType == EDrugType.Solid_Powder)
                _solubility = sol;
        }


        /*功能*/

        /// <summary>
        /// 添加药品
        /// </summary>
        /// <param passiveName="val"></param>
        public void AddDrug(float val, EMeasureUnit unit = EMeasureUnit.ml)
        {
            if (unit == EMeasureUnit.ml)
            {
                if (_volume != -1f)
                {
                    _volume += val;
                    _volume = Mathf.Max(0, _volume);
                }
                else
                {
                    _mass += (val * _density);
                    _mass = Mathf.Max(0, _mass);
                }
            }
            else
            {
                if (_mass != -1f)
                {
                    _mass += val;
                    _mass = Mathf.Max(0, _mass);
                }
                else
                {
                    _volume += (val / _density);
                    _volume = Mathf.Max(0, _volume);
                }
            }
        }

        /// <summary>
        /// 减少药品
        /// </summary>
        /// <param passiveName="val"></param>
        public void ReduceDrug(float val, EMeasureUnit unit = EMeasureUnit.ml)
        {
            if (unit == EMeasureUnit.ml)
            {
                if (_volume != -1f)
                {
                    _volume -= val;
                    _volume = Mathf.Max(0, _volume);
                }
                else
                {
                    _mass -= (val * _density);
                    _mass = Mathf.Max(0, _mass);
                }
            }
            else
            {
                if (_mass != -1f)
                {
                    _mass -= val;
                    _mass = Mathf.Max(0, _mass);
                }
                else
                {
                    _volume -= (val / _density);
                    _volume = Mathf.Max(0, _volume);
                }
            }
        }

        /// <summary>
        /// 设置药品量
        /// </summary>
        /// <param name="val"></param>
        public void SetDrugVolume(float val, EMeasureUnit unit = EMeasureUnit.ml)
        {
            if (unit == EMeasureUnit.ml)
            {
                if (_volume != -1f)
                {
                    _volume = val;
                    _volume = Mathf.Max(0, _volume);
                }
                else
                {
                    _mass = (val * _density);
                    _mass = Mathf.Max(0, _mass);
                }
            }
            else
            {
                if (_mass != -1f)
                {
                    _mass = val;
                    _mass = Mathf.Max(0, _mass);
                }
                else
                {
                    _volume = (val / _density);
                    _volume = Mathf.Max(0, _volume);
                }
            }
        }

        /// <summary>
        /// 是否溶解(20℃)
        /// </summary>
        /// <param name="mass">溶剂的质量</param>
        public bool IsDissolve(float mass)
        {
            //可溶解质量>自己质量
            return Solubility * mass > Mass;
        }

        /// <summary>
        /// 获取可溶解的质量(20℃)
        /// </summary>
        /// <param name="mass">溶剂的质量</param>
        /// <returns></returns>
        public float GetDissolveMass(float mass)
        {
            return Solubility * mass;
        }

    }

}

