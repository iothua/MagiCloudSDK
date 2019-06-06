#if UNITY_EDITOR
using System;
using System.Text;

namespace Loxodon.Framework.Bundles.Objects
{
    public class TypelessData
    {
        private readonly byte[] data;

        public TypelessData() : this(new byte[0])
        {
        }

        public TypelessData(byte[] data)
        {
            this.data = data;
        }

        public int Size { get { return this.data.Length; } }

        public byte[] Data { get { return this.data; } }

        public override string ToString()
        {
            StringBuilder buf = new StringBuilder();
            buf.Append("TypelessData {");
            buf.AppendFormat(" Size : {0}, ", data.Length);
            buf.AppendFormat("Data : {0} ", data.Length < 50 ? BitConverter.ToString(data).Replace("-", " ") : BitConverter.ToString(data).Replace("-", " ").Substring(0, 100));
            buf.Append("}");
            return buf.ToString();
        }
    }
}
#endif