#if UNITY_EDITOR
using Loxodon.Framework.Bundles.Archives;
using System.Collections.Generic;

namespace Loxodon.Framework.Bundles.Objects
{
    public class Map
    {
        private List<KeyValuePair<object, object>> list = new List<KeyValuePair<object, object>>();
        private readonly TypeNode typeNode;

        public Map(TypeNode typeNode)
        {
            this.typeNode = typeNode;
        }

        public virtual TypeNode TypeNode { get { return this.typeNode; } }

        public virtual object this[object key]
        {
            get
            {
                object value;
                this.TryGetValue(key, out value);
                return value;
            }
            set { this.Add(key, value); }
        }

        public virtual int Count { get { return list.Count; } }

        public virtual void Add(object key, object value)
        {
            var kv = new KeyValuePair<object, object>(key, value);
            this.list.Add(kv);
        }

        public virtual void Add(KeyValuePair<object, object> kv)
        {
            this.list.Add(kv);
        }

        public virtual void Clear()
        {
            this.list.Clear();
        }

        public virtual bool ContainsKey(object key)
        {
            return this.list.Exists(kv => kv.Key.Equals(key));
        }

        public virtual IEnumerator<KeyValuePair<object, object>> GetEnumerator()
        {
            return this.list.GetEnumerator();
        }

        public virtual bool Remove(object key)
        {
            return this.list.RemoveAll(kv => kv.Key.Equals(key)) > 0;
        }

        public virtual bool TryGetValue(object key, out object value)
        {
            var item = this.list.Find(kv => kv.Key.Equals(key));
            if (item.Key != null && item.Value != null)
            {
                value = item.Value;
                return true;
            }
            value = null;
            return false;
        }
    }
}
#endif