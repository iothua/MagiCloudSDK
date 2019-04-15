#if UNITY_EDITOR
using Loxodon.Framework.Bundles.Archives;
using System.Collections.Generic;
using System.Text;

namespace Loxodon.Framework.Bundles.Objects
{
    public class PreloadData
    {
        public string Name { get; set; }
        public readonly List<PPtr> Preloads = new List<PPtr>();
        public readonly List<string> Dependencies = new List<string>();

        public ObjectArchive Archive { get; private set; }

        public PreloadData(ObjectArchive archive)
        {
            this.Archive = archive;
        }

        public override string ToString()
        {
            StringBuilder buf = new StringBuilder();
            buf.Append("PreloadData { ");
            buf.AppendFormat("Name : {0}, ", this.Name);
            buf.AppendFormat("Archive : {0} ", this.Archive);
            buf.Append("}");
            return buf.ToString();
        }
    }
}
#endif