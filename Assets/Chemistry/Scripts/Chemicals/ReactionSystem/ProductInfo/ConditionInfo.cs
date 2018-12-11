using System.Collections.Generic;

namespace Chemistry.Chemicals
{
    /// <summary>
    /// 一次反应 反应条件信息
    /// </summary>
    public class ConditionInfo
    {
        public List<ConditionBase> LstConditionBases = new List<ConditionBase>();
    }

    /// <summary>
    /// 一次反应 一个反应条件
    /// </summary>
    [System.Serializable]
    public class ConditionBase
    {
        private string _name;

        public ConditionBase(string name)
        {
            _name = name;
        }

        public string Name
        {
            get { return _name; }
        }

        public override bool Equals(object obj)
        {
            var condition = (ConditionBase)obj;

            return Name.Equals(condition.Name);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    /// <summary>
    /// 温度
    /// </summary>
    public class ConditionTemperature : ConditionBase
    {
        private float _value;

        private float _range;

        /// <summary>
        /// 值
        /// </summary>
        public float Value {
            get { return _value; }
            set { _value = value; }
        }

        /// <summary>
        /// 范围值，用于判断温度是否在范围内
        /// </summary>
        public float Range {
            get {
                return _range;
            }
        }

        public ConditionTemperature(string name, float value, float range) : base(name)
        {
            this._value = value;
            this._range = range;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var condition = (ConditionTemperature)obj;
            if (condition == null) return false;

            return Name.Equals(condition.Name) &&
                condition.Value >= (Value - Range) && condition.Value <= (Value + Range);
        }

    }

    /// <summary>
    /// 催化
    /// </summary>
    public class ConditionCatalyze
    {

    }
}
