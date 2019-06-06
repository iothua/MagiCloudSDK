#if UNITY_EDITOR
using Loxodon.Framework.Bundles.Archives;
using System.Collections.Generic;
using System.Text;

namespace Loxodon.Framework.Bundles.Redundancy
{
    public class RedundancyInfo
    {
        private List<ObjectInfo> infos = new List<ObjectInfo>();

        private List<AssetBundleArchive> bundles = new List<AssetBundleArchive>();

        public string Name { get; private set; }

        public TypeID TypeID { get; private set; }

        public long FileSize { get; private set; }

        public IList<ObjectInfo> ObjectInfos { get { return infos.AsReadOnly(); } }

        public IList<AssetBundleArchive> Bundles { get { return bundles.AsReadOnly(); } }

        public int Count { get { return this.bundles.Count; } }

        public RedundancyInfo(string name, TypeID typeId, long fileSize)
        {
            this.Name = name;
            this.TypeID = typeId;
            this.FileSize = fileSize;
        }

        public void AddObjectInfo(ObjectInfo info)
        {
            this.infos.Add(info);

            var bundle = info.Archive.Bundle;
            if (bundle != null && !this.bundles.Contains(bundle))
            {
                this.bundles.Add(bundle);
                this.bundles.Sort((x,y)=>x.Name.CompareTo(y.Name));
            }
        }

        public override string ToString()
        {
            StringBuilder buf = new StringBuilder();
            buf.AppendFormat("{0}", this.GetType().Name).Append(" { ");
            buf.AppendFormat("Name:{0}, ", this.Name);
            buf.AppendFormat("TypeID:{0}, ", this.TypeID);
            buf.AppendFormat("FileSize:{0}, ", this.FileSize);
            buf.AppendFormat("Count:{0}, ", this.Count);

            buf.Append("Bundles:[ ");
            int count = this.bundles.Count;
            for (int i = 0; i < count; i++)
            {
                var bundle = bundles[i];
                if (i < count - 1)
                    buf.AppendFormat("{0}, ", bundle.Name);
                else
                    buf.AppendFormat("{0} ", bundle.Name);
            }
            buf.Append("]");

            buf.Append("}");
            return buf.ToString();
        }
    }
}
#endif