#if UNITY_EDITOR
using System.Text;

namespace Loxodon.Framework.Bundles.Archives
{
    public class IdentifierFingerprint : IFingerprint
    {
        private string assetArchiveName;

        private long pathId;

        public IdentifierFingerprint(long pathId, string assetArchiveName)
        {
            this.assetArchiveName = string.IsNullOrEmpty(assetArchiveName) ? string.Empty : assetArchiveName;
            this.pathId = pathId;
        }

        public override bool Equals(object obj)
        {
            if (this == obj)
                return true;

            if (obj == null || !(obj is IdentifierFingerprint))
                return false;

            IdentifierFingerprint other = (IdentifierFingerprint)obj;
            return this.assetArchiveName.Equals(other.assetArchiveName) && this.pathId == other.pathId;
        }

        public override int GetHashCode()
        {
            return this.pathId.GetHashCode();
        }

        public override string ToString()
        {
            StringBuilder buf = new StringBuilder();
            buf.Append("IdentifierFingerprint { ");
            buf.AppendFormat("assetArchiveName : {0}, ", this.assetArchiveName);
            buf.AppendFormat("pathId : {0} ", this.pathId);
            buf.Append("}");
            return buf.ToString();
        }
    }
}
#endif