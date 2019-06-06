#if UNITY_EDITOR
using System.Text;

namespace Loxodon.Framework.Bundles.Objects
{
    public class StreamedResource
    {
        public string Source { get; private set; }

        public ulong Offset { get; private set; }

        public ulong Size { get; private set; }

        public StreamedResource(string source, ulong offset, ulong size)
        {
            this.Source = source;
            this.Offset = offset;
            this.Size = size;
        }

        public override string ToString()
        {
            StringBuilder buf = new StringBuilder();
            buf.Append("StreamedResource { ");
            buf.AppendFormat("Source:{0}, ", Source);
            buf.AppendFormat("Offset:{0}, ", Offset);
            buf.AppendFormat("Size:{0} ", Size);
            buf.Append("}");
            return buf.ToString();
        }
    }
}
#endif