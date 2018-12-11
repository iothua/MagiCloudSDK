using System;
using Chemistry.Data;
using UnityEngine;

namespace Chemistry.Chemicals
{
    /// <summary>
    /// 药品数据
    /// </summary>
    public struct DrugData
    {
        /// <summary>
        /// 药品名称
        /// </summary>
        public string DrugName;
        /// <summary>
        /// 药品体积
        /// </summary>
        public float Volume {
            get {
                switch (drugStyle)
                {
                    case DrugStyle.纯净物:
                        return ((Drug)DrugObject).Volume;
                    case DrugStyle.混合物:
                        return ((DrugMixture)DrugObject).Volume;
                    default:
                        return 0;
                }
            }
            set {
                switch (drugStyle)
                {
                    case DrugStyle.纯净物:
                        ((Drug)DrugObject).Volume = value;
                        break;
                    case DrugStyle.混合物:
                       ((DrugMixture)DrugObject).ReduceDrugMixture(value);
                        break;
                    default:
                        break;
                }
            }
        }

        private object _drug;
        private DrugStyle _drugStyle;

        /// <summary>
        /// 药对象，根据药品的类型，可转换相应的类型
        /// </summary>
        public object DrugObject {
            get {
                return _drug;
            }
        }

        /// <summary>
        /// 药品类型
        /// </summary>
        public DrugStyle drugStyle {
            get {
                return _drugStyle;
            }
        }

        /// <summary>
        /// 药品质量
        /// </summary>
        public float Mass {
            get {
                switch (drugStyle)
                {
                    case DrugStyle.纯净物:
                        return ((Drug)DrugObject).Mass;
                    case DrugStyle.混合物:
                        return ((DrugMixture)DrugObject).Mass;
                    default:
                        return 0;
                }
            }
            set {
                switch (drugStyle)
                {
                    case DrugStyle.纯净物:
                        ((Drug)DrugObject).Mass = value;
                        break;
                    case DrugStyle.混合物:
                        ((DrugMixture)DrugObject).ReduceDrugMixtureMass(value);
                        break;
                    default:
                        break;
                }
            }
        }

        public DrugData(string drugName, float volume)
        {
            this = DrugDatabase.AddDrug(drugName, volume);
        }

        public DrugData(Drug drug)
        {
            this.DrugName = drug.Name;

            _drug = drug;
            _drugStyle = DrugStyle.纯净物;
        }

        public DrugData(DrugMixture drug)
        {
            this.DrugName = drug.Name;

            _drug = drug;
            _drugStyle = DrugStyle.混合物;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            DrugData drugData = (DrugData)obj;

            return drugData.DrugName.Equals(DrugName) && drugData.DrugObject.Equals(DrugObject);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
