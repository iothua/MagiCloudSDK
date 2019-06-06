#if UNITY_EDITOR
using System;
using System.IO;
using System.Text;

namespace Loxodon.Framework.Bundles.Archives
{
    public abstract class AssetArchive : IDisposable
    {
        public AssetBundleArchive Bundle { get; protected set; }

        public string Name { get; protected set; }

        public long FileSize { get { return this.fileStreamSize; } }

        public ArchiveType Type { get; protected set; }

        protected long fileStreamOffset;

        protected long fileStreamSize;

        public AssetArchive(AssetBundleArchive bundle, string name, long fileStreamOffset, long fileStreamSize, ArchiveType type)
        {
            this.Bundle = bundle;
            this.Name = name;
            this.Type = type;

            this.fileStreamOffset = fileStreamOffset;
            this.fileStreamSize = fileStreamSize;
        }

        public virtual Stream GetDataStream()
        {
            if (this.Bundle == null)
                throw new NotSupportedException();
            return new ReadOnlyBlockStream(this.Bundle.GetDataStream(), this.fileStreamOffset, this.fileStreamSize, false);
        }

        public override string ToString()
        {
            StringBuilder buf = new StringBuilder();
            buf.AppendFormat("{0}", this.GetType().Name).Append(" { ");
            buf.AppendFormat("Name : {0}, ", this.Name);
            buf.AppendFormat("FileSize : {0}, ", this.FileSize);
            buf.AppendFormat("Type : {0} ", this.Type);
            buf.Append("}");
            return buf.ToString();
        }

        protected virtual void CopyTo(Stream src, Stream dest, long size)
        {
            byte[] buffer = new byte[8192];
            long offset = 0;
            while (offset < size)
            {
                var count = src.Read(buffer, 0, (int)Math.Min(size - offset, buffer.Length));
                if (count <= 0)
                    break;

                dest.Write(buffer, 0, count);
                offset += count;
            }
        }

        #region IDisposable Support
        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                disposed = true;
            }
        }

        ~AssetArchive()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }

    public enum ArchiveType
    {
        CAB = 0,
        BUILD_PLAYER = 1,
        SHARED_DATA = 2,
        RESOURCE_DATA = 3,
        GI_DATA = 4,
        BUILTIN = 5
    }
}
#endif
