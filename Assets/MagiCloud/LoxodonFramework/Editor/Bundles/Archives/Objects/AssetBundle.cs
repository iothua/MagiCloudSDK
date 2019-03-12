#if UNITY_EDITOR
using Loxodon.Framework.Bundles.Archives;
using System.Collections.Generic;
using System.Text;

namespace Loxodon.Framework.Bundles.Objects
{
    public class AssetBundle
    {
        public string Name { get; set; }

        public string FullName { get; set; }

        public AssetInfo MainAsset { get; set; }

        public bool IsStreamed { get; set; }

        public uint RuntimeCompatibility { get; set; }

        public readonly List<PPtr> Preloads = new List<PPtr>();

        public readonly List<AssetPair> Container = new List<AssetPair>();

        public readonly List<string> Dependencies = new List<string>();

        public ObjectArchive Archive { get; private set; }


        public AssetBundle(ObjectArchive archive)
        {
            this.Archive = archive;
        }

        public virtual bool IsPublic(long pathId)
        {
            foreach (var pair in this.Container)
            {
                if (pair.Second.PPtr.PathID == pathId)
                    return true;
            }
            return false;
        }

        public override string ToString()
        {
            StringBuilder buf = new StringBuilder();
            buf.Append("AssetBundle { ");
            buf.AppendFormat("Name : {0}, ", this.Name);
            buf.AppendFormat("FullName : {0}, ", this.FullName);
            buf.AppendFormat("IsStreamed : {0}, ", this.IsStreamed);
            buf.AppendFormat("Archive : {0} ", this.Archive);
            buf.Append("}");
            return buf.ToString();
        }
    }

    public class AssetPair
    {
        public string First { get; private set; }
        public AssetInfo Second { get; private set; }

        public AssetPair(string first, AssetInfo second)
        {
            this.First = first;
            this.Second = second;
        }
    }

    public class AssetInfo
    {
        public int PreloadIndex { get; private set; }
        public int PreloadSize { get; private set; }
        public PPtr PPtr { get; private set; }

        public AssetInfo(int preloadIndex, int preloadSize, PPtr pptr)
        {
            this.PreloadIndex = preloadIndex;
            this.PreloadSize = preloadSize;
            this.PPtr = pptr;
        }

        public override string ToString()
        {
            StringBuilder buf = new StringBuilder();
            buf.Append("AssetInfo { ");
            buf.AppendFormat("PreloadIndex : {0}, ", this.PreloadIndex);
            buf.AppendFormat("PreloadSize : {0}, ", this.PreloadSize);
            buf.AppendFormat("PPtr : {0} ", this.PPtr);
            buf.Append("}");
            return buf.ToString();
        }
    }
}
#endif