using System;

namespace MagiCloud.Common
{
    public abstract class Variable
    {
        protected Variable()
        {
        }
        public abstract Type Type { get; }

        public abstract object GetValue();

        public abstract void SetValue(object value);

        public abstract void Reset();
    }

    public abstract class Variable<T> :Variable
    {
        private T _value;
        protected Variable()
        {
            _value=default(T);
        }
        protected Variable(T value)
        {
            _value=value;
        }
        public override Type Type => typeof(T);

        public T Value => _value;

        public override object GetValue()
        {
            return _value;
        }
        public override void SetValue(object value)
        {
            _value=(T)value;
        }

        public override void Reset()
        {
            _value=default(T);
        }
        public override string ToString()
        {
            return _value!=null ? _value.ToString() : "<Null>";
        }
    }
}
