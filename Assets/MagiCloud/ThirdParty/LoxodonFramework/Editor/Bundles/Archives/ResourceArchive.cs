#if UNITY_EDITOR
using System.IO;

namespace Loxodon.Framework.Bundles.Archives
{
    public class ResourceArchive : AssetArchive
    {
        public ResourceArchive(AssetBundleArchive bundle, string name, long fileStreamOffset, long fileStreamSize, ArchiveType type) : base(bundle, name, fileStreamOffset, fileStreamSize, type)
        {
        }

        public virtual Stream GetResourceStream(long offset, long size)
        {
            return new ReadOnlyBlockStream(this.GetDataStream(), offset, size, false);
        }
    }
}
#endif
