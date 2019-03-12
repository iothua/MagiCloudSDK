#if UNITY_EDITOR
using Loxodon.Framework.Bundles.Objects;
using System.Collections.Generic;
using System.Text;
using Hash128 = Loxodon.Framework.Bundles.Objects.Hash128;

namespace Loxodon.Framework.Bundles.Archives
{
    public class PropertiesFingerprint : IFingerprint
    {
        private object _lock = new object();

        private List<PPtr> pptrs = new List<PPtr>();

        private List<IObjectInfo> references;

        private ObjectInfo info;

        public virtual TypeID TypeID { get { return this.info.TypeID; } }

        public virtual Hash128 PropertyHash { get; set; }

        public PropertiesFingerprint(ObjectInfo info, List<PPtr> pptrs, Hash128 propertyHash)
        {
            this.info = info;
            this.pptrs.AddRange(pptrs);
            this.PropertyHash = propertyHash;
        }

        public List<IObjectInfo> GetReferences()
        {
            if (references != null)
                return references;

            lock (_lock)
            {
                if (references != null)
                    return this.references;

                this.references = new List<IObjectInfo>();

                ObjectArchive archive = (ObjectArchive)this.info.Archive;

                foreach (PPtr pptr in pptrs)
                {
                    this.references.Add(archive.GetObjectInfo(pptr));
                }
                return references;
            }
        }

        public override bool Equals(object obj)
        {
            return Equals(this, obj);
        }

        protected virtual bool Equals(PropertiesFingerprint root, object obj)
        {
            if (this == obj)
                return true;

            if (obj == null || !(obj is PropertiesFingerprint))
                return false;

            PropertiesFingerprint other = obj as PropertiesFingerprint;
            if (this.TypeID != other.TypeID || this.PropertyHash != other.PropertyHash || this.pptrs.Count != other.pptrs.Count)
                return false;

            var summaries = this.GetReferences();
            var otherSummaries = other.GetReferences();
            for (int i = 0; i < summaries.Count; i++)
            {
                var f1 = summaries[i].Fingerprint;
                var f2 = otherSummaries[i].Fingerprint;

                if (f1 is PropertiesFingerprint)
                {
                    if (f1 == root)
                        continue;

                    if (!(f1 as PropertiesFingerprint).Equals(root, f2))
                        return false;
                }
                else
                {
                    if (!f1.Equals(f2))
                        return false;
                }
            }
            return true;
        }

        public override int GetHashCode()
        {
            return this.PropertyHash.GetHashCode();
        }

        public override string ToString()
        {
            StringBuilder buf = new StringBuilder();
            buf.Append("PropertiesFingerprint { ");
            buf.AppendFormat("TypeID : {0}, ", this.TypeID);
            buf.AppendFormat("PropertyHash : {0} ", this.PropertyHash);
            buf.Append("}");
            return buf.ToString();
        }
    }
}
#endif