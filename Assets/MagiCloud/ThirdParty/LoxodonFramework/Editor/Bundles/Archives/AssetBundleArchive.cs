#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using AssetBundle = Loxodon.Framework.Bundles.Objects.AssetBundle;
using Hash128 = Loxodon.Framework.Bundles.Objects.Hash128;

namespace Loxodon.Framework.Bundles.Archives
{
#pragma warning disable 0414, 0219
    public class AssetBundleArchive : IDisposable
    {
        private const string SIGNATURE_WEB = "UnityWeb";
        private const string SIGNATURE_RAW = "UnityRaw";
        private const string SIGNATURE_FS = "UnityFS";

        private object _lock = new object();
        private string dataFilename;
        //private Stream dataStream;

        private Dictionary<string, AssetArchive> archives = new Dictionary<string, AssetArchive>();
        /// <summary>
        /// 6 in Unity 5.3+ (UnityFS files)
        /// 3 in Unity 3.5 and 4
        /// 2 in Unity 2.6 to 3.4
        /// 1 in Unity 1 to 2.5
        /// </summary>
        public int FormatVersion { get; private set; }
        /// <summary>
        /// main version string
        /// 2.x.x for Unity 2
        /// 3.x.x for Unity 3/4
        /// 5.x.x for Unity 5
        /// </summary>
        public string MainVersion { get; private set; }
        /// <summary>
        /// build version string
        /// 5.6.0f2
        /// </summary>
        public string BuildVersion { get; private set; }
        public Hash128 GUID { get; private set; }
        public string Name { get { return this.AssetBundle != null ? this.AssetBundle.FullName : ""; } }
        public long FileSize { get; private set; }
        public bool IsStreamed { get { return this.AssetBundle != null ? this.AssetBundle.IsStreamed : false; } }
        public List<AssetArchive> AssetArchives { get { return new List<AssetArchive>(this.archives.Values); } }
        public AssetBundle AssetBundle { get; set; }
        public string Path { get; set; }
        public ArchiveContainer ArchiveContainer { get; set; }


        public AssetBundleArchive(int formatVersion, string mainVersion, string buildVersion)
        {
            this.FormatVersion = formatVersion;
            this.MainVersion = mainVersion;
            this.BuildVersion = buildVersion;
        }

        public void AddAssetArchive(AssetArchive archive)
        {
            if (this.archives.ContainsKey(archive.Name))
                return;
            this.archives.Add(archive.Name, archive);
        }

        public void RemoveAssetArchive(AssetArchive archive)
        {
            this.archives.Remove(archive.Name);
        }

        public AssetArchive GetAssetArchive(string name)
        {
            var key = System.IO.Path.GetFileName(name);
            AssetArchive asset = null;
            this.archives.TryGetValue(key, out asset);
            return asset;
        }

        public Stream GetDataStream()
        {
            //lock (_lock)
            //{
            //    if (this.dataStream == null)
            //        this.dataStream = new FileStream(this.dataFilename, FileMode.Open, FileAccess.Read, FileShare.Read, 8192);

            //    return this.dataStream;
            //}

            return new FileStream(this.dataFilename, FileMode.Open, FileAccess.Read, FileShare.Read, 8192);
        }

        public override string ToString()
        {
            StringBuilder buf = new StringBuilder();
            buf.AppendFormat("{0}", this.GetType().Name).Append(" { ");
            buf.AppendFormat("Name:{0}, ", this.Name);
            buf.AppendFormat("Path:{0}, ", this.Path);
            buf.AppendFormat("GUID:{0}, ", this.GUID);
            buf.AppendFormat("FormatVersion:{0}, ", this.FormatVersion);
            buf.AppendFormat("MainVersion:{0}, ", this.MainVersion);
            buf.AppendFormat("BuildVersion:{0}, ", this.BuildVersion);
            buf.AppendFormat("FileSize:{0} ", this.FileSize);
            buf.Append("}");

            return buf.ToString();
        }

        #region IDisposable Support
        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    foreach (var asset in this.archives.Values)
                    {
                        asset.Dispose();
                    }
                    this.archives.Clear();
                    this.archives = null;
                }

                //if (this.dataStream != null)
                //{
                //    this.dataStream.Dispose();
                //    this.dataStream = null;
                //}

                if (File.Exists(this.dataFilename))
                    File.Delete(this.dataFilename);

                disposed = true;
            }
        }

        ~AssetBundleArchive()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

        public static AssetBundleArchive Load(string path)
        {
            using (var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                AssetBundleArchive bundle = Load(fileStream);
                bundle.Path = path;
                return bundle;
            }
        }

        public static AssetBundleArchive Load(Stream stream)
        {
            using (var reader = new ArchiveBinaryReader(stream))
            {
                var signature = reader.ReadCString();
                var formatVersion = reader.ReadInt32(true);
                var mainVersion = reader.ReadCString();
                var buildVersion = reader.ReadCString();

                if (signature != SIGNATURE_FS && signature != SIGNATURE_WEB)
                    throw new NotSupportedException("Not supported signature : " + signature);

                if (formatVersion != 6)
                    throw new NotSupportedException("Not supported format version : " + formatVersion);

                AssetBundleArchive bundle = new AssetBundleArchive(formatVersion, mainVersion, buildVersion);
                bundle.FileSize = reader.ReadInt64(true);

                var compressedBlockSize = reader.ReadInt32(true);
                var uncompressedBlockSize = reader.ReadInt32(true);
                var flags = reader.ReadInt32(true);

                var compressionType = (CompressionType)(flags & 0x3f);

                byte[] buffer = null;
                if ((flags & 0x80) > 0)
                {
                    var currentPos = reader.BaseStream.Position;
                    reader.BaseStream.Seek(-compressedBlockSize, SeekOrigin.End);
                    buffer = reader.ReadBytes(compressedBlockSize);
                    reader.BaseStream.Seek(currentPos, SeekOrigin.Begin);
                }
                else
                {
                    buffer = reader.ReadBytes(compressedBlockSize);
                }

                switch (compressionType)
                {
                    case CompressionType.NONE:
                        break;
                    case CompressionType.LZ4:
                    case CompressionType.LZ4HC:
                        buffer = LZ4.LZ4Codec.Decode(buffer, 0, compressedBlockSize, uncompressedBlockSize);
                        break;
                    default:
                        throw new NotSupportedException("Not supported compression type : " + compressionType);
                }

                List<BlockInfo> blocks = new List<BlockInfo>();
                List<AssetInfo> assets = new List<AssetInfo>();
                using (var metaReader = new ArchiveBinaryReader(new MemoryStream(buffer)))
                {
                    bundle.GUID = metaReader.ReadHash128();

                    int blockCount = metaReader.ReadInt32(true);
                    for (int i = 0; i < blockCount; i++)
                    {
                        BlockInfo block;
                        block.uncompressedSize = metaReader.ReadUInt32(true);
                        block.compressedSize = metaReader.ReadUInt32(true);
                        block.flag = metaReader.ReadInt16(true);
                        blocks.Add(block);
                    }

                    var assetCount = metaReader.ReadInt32(true);
                    for (int i = 0; i < assetCount; i++)
                    {
                        AssetInfo asset;
                        asset.offset = metaReader.ReadInt64(true);
                        asset.size = metaReader.ReadInt64(true);
                        asset.flag = metaReader.ReadInt32(true);
                        var name = metaReader.ReadCString();
                        name = string.IsNullOrEmpty(name) ? "" : name.ToLower();
                        asset.name = name;
                        assets.Add(asset);
                    }
                }

                var totalDataSize = blocks.Sum(b => b.uncompressedSize);

                var dataFilename = string.Format("{0}/{1}", ArchiveUtil.GetTemporaryCachePath(), Guid.NewGuid().ToString());
                FileInfo file = new FileInfo(dataFilename);
                if (!file.Directory.Exists)
                    file.Directory.Create();
                bundle.dataFilename = file.FullName;

                using (Stream dataStream = new FileStream(file.FullName, FileMode.Create, FileAccess.ReadWrite, FileShare.Read, 8192))
                {
                    foreach (var block in blocks)
                    {
                        switch (block.CompressionType)
                        {
                            case CompressionType.NONE:
                                {
                                    buffer = reader.ReadBytes((int)block.compressedSize);
                                    dataStream.Write(buffer, 0, buffer.Length);
                                    break;
                                }
                            case CompressionType.LZMA:
                                {
                                    var properties = reader.ReadBytes(5);
                                    var decoder = new SevenZip.Compression.LZMA.Decoder();
                                    decoder.SetDecoderProperties(properties);
                                    decoder.Code(reader.BaseStream, dataStream, block.compressedSize - 5, block.uncompressedSize, null);
                                    break;
                                }
                            case CompressionType.LZ4:
                            case CompressionType.LZ4HC:
                                {
                                    buffer = reader.ReadBytes((int)block.compressedSize);
                                    var data = LZ4.LZ4Codec.Decode(buffer, 0, (int)block.compressedSize, (int)block.uncompressedSize);
                                    dataStream.Write(data, 0, data.Length);
                                    break;
                                }
                            default:
                                throw new NotSupportedException("Not supported compression type : " + block.CompressionType);
                        }
                    }
                }
                foreach (var assetInfo in assets)
                {
                    switch (assetInfo.flag)
                    {
                        case 4:
                            {
                                ObjectArchive asset = new ObjectArchive(bundle, assetInfo.name, assetInfo.offset, assetInfo.size, assetInfo.Type);
                                bundle.AddAssetArchive(asset);
                                break;
                            }
                        default:
                            {
                                ResourceArchive asset = new ResourceArchive(bundle, assetInfo.name, assetInfo.offset, assetInfo.size, assetInfo.Type);
                                bundle.AddAssetArchive(asset);
                                break;
                            }
                    }

                    //switch (assetInfo.Type)
                    //{
                    //    case ArchiveType.CAB:
                    //    case ArchiveType.BUILD_PLAYER:
                    //    case ArchiveType.SHARED_DATA:
                    //        {
                    //            ObjectArchive asset = new ObjectArchive(bundle, assetInfo.name, assetInfo.offset, assetInfo.size, assetInfo.Type);
                    //            bundle.AddAssetArchive(asset);
                    //            break;
                    //        }
                    //    default:
                    //        {
                    //            ResourceArchive asset = new ResourceArchive(bundle, assetInfo.name, assetInfo.offset, assetInfo.size, assetInfo.Type);
                    //            bundle.AddAssetArchive(asset);
                    //            break;
                    //        }
                    //}
                }
                return bundle;
            }
        }
    }

    enum CompressionType
    {
        NONE = 0,
        LZMA,
        LZ4,
        LZ4HC,
        LZHAM
    }

    struct AssetInfo
    {
        public long offset;
        public long size;
        public int flag;
        public string name;

        public ArchiveType Type
        {
            get
            {
                string name = Path.GetFileName(this.name).ToUpper();
                string extension = Path.GetExtension(this.name).ToUpper();
                if (name.EndsWith(".RESOURCE") || name.EndsWith(".RESS"))
                    return ArchiveType.RESOURCE_DATA;
                if (name.EndsWith(".SHAREDASSETS"))
                    return ArchiveType.SHARED_DATA;
                if (name.StartsWith("BUILDPLAYER-"))
                    return ArchiveType.BUILD_PLAYER;
                if (name.StartsWith("GI/"))
                    return ArchiveType.GI_DATA;
                if (name.StartsWith("CAB-") && string.IsNullOrEmpty(extension))
                    return ArchiveType.CAB;

                return ArchiveType.RESOURCE_DATA;
            }
        }

        public override string ToString()
        {
            StringBuilder buf = new StringBuilder();
            buf.Append("{");
            buf.AppendFormat("offset:{0}, ", this.offset);
            buf.AppendFormat("size:{0}, ", this.size);
            buf.AppendFormat("flag:{0}, ", this.flag);
            buf.AppendFormat("name:{0}, ", this.name);
            buf.AppendFormat("type:{0} ", this.Type);
            buf.Append("}");
            return buf.ToString();
        }
    }

    struct BlockInfo
    {
        public uint uncompressedSize;
        public uint compressedSize;
        public short flag;
        public CompressionType CompressionType { get { return (CompressionType)(this.flag & 0x3f); } }

        public override string ToString()
        {
            StringBuilder buf = new StringBuilder();
            buf.Append("{");
            buf.AppendFormat("uncompressedSize:{0}, ", this.uncompressedSize);
            buf.AppendFormat("compressedSize:{0}, ", this.compressedSize);
            buf.AppendFormat("flag:{0}, ", this.flag);
            buf.AppendFormat("CompressionType:{0} ", this.CompressionType);
            buf.Append("}");
            return buf.ToString();
        }
    }
}
#endif