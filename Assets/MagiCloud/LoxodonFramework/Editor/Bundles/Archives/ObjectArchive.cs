#if UNITY_EDITOR
using Loxodon.Framework.Bundles.Objects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using AssetBundle = Loxodon.Framework.Bundles.Objects.AssetBundle;
using Hash128 = Loxodon.Framework.Bundles.Objects.Hash128;

namespace Loxodon.Framework.Bundles.Archives
{
#pragma warning disable 0414, 0219 
    public class ObjectArchive : AssetArchive
    {
        private List<TypeTree> trees = new List<TypeTree>();

        private Dictionary<long, ObjectInfo> objects = new Dictionary<long, ObjectInfo>();

        private List<ArchiveRef> archiveRefs = new List<ArchiveRef>();

        private List<PPtr> preloadScripts = new List<PPtr>();

        private FeatureExtractor extractor;

        private long assetDataOffset;

        public int Format { get; private set; }

        public string Version { get; private set; }

        public uint TargetPlatform { get; private set; }

        public AssetBundle AssetBundle { get; private set; }

        public PreloadData PreloadData { get; private set; }

        public ObjectArchive(AssetBundleArchive bundle, string name, long fileStreamOffset, long fileStreamSize, ArchiveType type) : base(bundle, name, fileStreamOffset, fileStreamSize, type)
        {
            this.archiveRefs.Add(new ArchiveRef(0, this.Name, 0, new Hash128(new byte[16]), this.Name));
            this.extractor = new FeatureExtractor();

            using (Stream stream = this.GetDataStream())
            {
                using (ArchiveBinaryReader reader = new ArchiveBinaryReader(stream))
                {
                    this.Load(reader);
                }
            }
        }

        public virtual ICollection<ObjectInfo> GetAllObjectInfo()
        {
            return objects.Values;
        }

        public virtual ICollection<TypeTree> GetAllTrees()
        {
            return this.trees.AsReadOnly();
        }

        public virtual ICollection<PPtr> GetAllScripts()
        {
            return this.preloadScripts.AsReadOnly();
        }

        public virtual ICollection<ArchiveRef> GetAllArchiveRefs()
        {
            return this.archiveRefs.AsReadOnly();
        }

        public virtual IObjectInfo GetObjectInfo(PPtr pptr)
        {
            if (pptr.FileID == 0 && pptr.PathID == 0)
                return new NullObjectInfo(pptr.TypeID);

            if (pptr.FileID == 0)
            {
                ObjectInfo info = null;
                if (objects.TryGetValue(pptr.PathID, out info))
                    return info;

                return new MissingObjectInfo(this, pptr.PathID, pptr.TypeID);
            }

            ArchiveRef assetRef = this.archiveRefs[pptr.FileID];
            ObjectArchive asset = this.Bundle.GetAssetArchive(assetRef.FileName) as ObjectArchive;
            if (asset != null)
                return asset.GetObjectInfo(pptr.PathID);

            var container = this.Bundle.ArchiveContainer;
            if (container == null)
                return new MissingObjectInfo(null, pptr.PathID, pptr.TypeID);

            return container.GetObjectInfo(assetRef.FileName, pptr.PathID);
        }

        public virtual IObjectInfo GetObjectInfo(long pathId)
        {
            if (pathId == 0)
                return new NullObjectInfo(TypeID.UnknownType);

            ObjectInfo info = null;
            if (objects.TryGetValue(pathId, out info))
                return info;

            return new MissingObjectInfo(this, pathId, TypeID.UnknownType);
        }

        public virtual Stream GetResourceStream(StreamedResource resource)
        {
            ResourceArchive resourceArchive = this.Bundle.GetAssetArchive(resource.Source) as ResourceArchive;
            if (resourceArchive == null)
                return null;

            return resourceArchive.GetResourceStream((long)resource.Offset, (long)resource.Size);
        }

        public virtual byte[] GetObjectData(long offset, int size)
        {
            byte[] buffer = new byte[size];
            using (Stream stream = this.GetDataStream())
            {
                stream.Seek(this.assetDataOffset + offset, SeekOrigin.Begin);
                stream.Read(buffer, 0, size);
            }
            return buffer;
        }

        protected virtual void Load(ArchiveBinaryReader reader)
        {
            try
            {
                long startPos = reader.BaseStream.Position;
                int headerSize = reader.ReadInt32(true);
                var fileSize = reader.ReadInt32(true);
                this.Format = reader.ReadInt32(true);
                this.assetDataOffset = reader.ReadUInt32(true);

                if (this.Format < 17)
                    throw new NotSupportedException(string.Format("The AssetBundle's format not supported,format:{0}", this.Format));

                bool bigEndian = reader.ReadBoolean();
                reader.IsBigEndian = bigEndian;
                reader.ReadBytes(3);
                this.Version = reader.ReadCString();
                this.TargetPlatform = reader.ReadUInt32();

                //读取类型树
                var hasTypeTree = reader.ReadBoolean();
                if (!hasTypeTree)
                    throw new NotSupportedException("Missing type tree, not supported");

                Hash128 zero = new Hash128(new byte[16]);
                var typeTreeCount = reader.ReadInt32();
                for (var i = 0; i < typeTreeCount; i++)
                {
                    var typeId = reader.ReadInt32();
                    reader.ReadByte();
                    var scriptIndex = reader.ReadInt16();
                    var hash = reader.ReadHash128();
                    var propertiesHash = typeId == 114 ? reader.ReadHash128() : zero;
                    var tree = new TypeTree(this, i, Enum.IsDefined(typeof(TypeID), typeId) ? (TypeID)typeId : TypeID.UnknownType, scriptIndex, hash, propertiesHash);
                    tree.Load(reader);
                    this.trees.Add(tree);
                }

                //读取对象信息
                var objectCount = reader.ReadInt32();
                List<ObjectItem> objectItems = new List<ObjectItem>();
                for (var i = 0; i < objectCount; i++)
                {
                    reader.Align(4);
                    var id = reader.ReadInt64();
                    var offset = reader.ReadInt32();
                    var size = reader.ReadInt32();
                    var index = reader.ReadInt32();
                    var typeTree = trees[index];
                    var item = new ObjectItem(id, offset, size, typeTree);
                    objectItems.Add(item);
                }

                //自定义脚本预载表
                var scriptCount = reader.ReadInt32();
                for (int i = 0; i < scriptCount; i++)
                {
                    var fileID = reader.ReadInt32();
                    var pathID = reader.ReadInt64();
                    var pptr = new PPtr(fileID, pathID, "PPtr<MonoScript>");
                    this.preloadScripts.Add(pptr);
                }

                //读取共享对象
                int refCount = reader.ReadInt32();
                for (int i = 1; i <= refCount; i++)
                {
                    var name = reader.ReadCString();
                    var guid = reader.ReadHash128();
                    var type = reader.ReadInt32();
                    var fileName = reader.ReadCString();
                    fileName = string.IsNullOrEmpty(fileName) ? "" : fileName.ToLower();
                    var assetRef = new ArchiveRef(i, name, type, guid, fileName);
                    this.archiveRefs.Add(assetRef);
                }

                foreach (var item in objectItems)
                {
                    TypeID typeId = item.TypeTree.TypeID;
                    if (typeId != TypeID.AssetBundle && typeId != TypeID.PreloadData)
                        continue;

                    ObjectInfo info = new ObjectInfo(this, item.ID, item.TypeTree, item.Offset, item.Size, false);
                    if (info.TypeID == TypeID.AssetBundle)
                    {
                        AssetBundle assetBundle = info.GetObject<AssetBundle>();
                        this.AssetBundle = assetBundle;
                        Bundle.AssetBundle = assetBundle;
                    }
                    else if (info.TypeID == TypeID.PreloadData)
                    {
                        PreloadData preloadData = info.GetObject<PreloadData>();
                        this.PreloadData = preloadData;
                    }
                }

                foreach (var item in objectItems)
                {
                    TypeID typeId = item.TypeTree.TypeID;
                    if (typeId == TypeID.AssetBundle || typeId == TypeID.PreloadData)
                        continue;

                    bool isPublic = !this.Bundle.IsStreamed && this.AssetBundle != null && this.AssetBundle.IsPublic(item.ID);
                    ObjectInfo info = new ObjectInfo(this, item.ID, item.TypeTree, item.Offset, item.Size, isPublic);
                    this.objects.Add(info.ID, info);

                    if (info.IsPotentialRedundancy)
                    {
                        reader.BaseStream.Seek(this.assetDataOffset + item.Offset, SeekOrigin.Begin);
                        FeatureInfo featureInfo = this.extractor.Extract(info, reader);
                        info.Fingerprint = new PropertiesFingerprint(info, featureInfo.References, featureInfo.PropertyHash);
                        info.Name = featureInfo.Name;
                        info.Resources = featureInfo.Resources;
                    }
                    else
                    {
                        info.Fingerprint = new IdentifierFingerprint(info.ID, this.Name);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogErrorFormat("{0}", e);
            }
        }

        #region IDisposable Support
        private bool disposed = false;

        protected override void Dispose(bool disposing)
        {
            if (!disposed)
            {
                base.Dispose(disposing);
                disposed = true;
            }
        }
        #endregion
    }

    public class ArchiveRef
    {
        public int ID { get; private set; }

        public string Name { get; private set; }

        public int Type { get; private set; }

        public Hash128 GUID { get; private set; }

        public string FileName { get; private set; }

        public ArchiveRef(int id, string name, int type, Hash128 guid, string fileName)
        {
            this.ID = id;
            this.Name = name;
            this.Type = type;
            this.GUID = guid;
            this.FileName = fileName;
        }

        public override string ToString()
        {
            StringBuilder buf = new StringBuilder();
            buf.AppendFormat("{0}", this.GetType().Name).Append(" { ");
            buf.AppendFormat("ID:{0}, ", this.ID);
            //buf.AppendFormat("Name:{0}, ", this.Name);
            buf.AppendFormat("Type:{0}, ", this.Type);
            buf.AppendFormat("GUID:{0}, ", this.GUID);
            buf.AppendFormat("FileName:{0} ", this.FileName);
            buf.Append("}");
            return buf.ToString();
        }
    }

    public class ObjectItem
    {
        public long ID { get; private set; }
        public int Offset { get; private set; }
        public int Size { get; private set; }
        public TypeTree TypeTree { get; private set; }

        public ObjectItem(long id, int offset, int size, TypeTree typeTree)
        {
            this.ID = id;
            this.Offset = offset;
            this.Size = size;
            this.TypeTree = typeTree;
        }

        public override string ToString()
        {
            StringBuilder buf = new StringBuilder();
            buf.AppendFormat("{0}", this.GetType().Name).Append(" { ");
            buf.AppendFormat("ID:{0}, ", this.ID);
            buf.AppendFormat("Offset:{0}, ", this.Offset);
            buf.AppendFormat("Size:{0}, ", this.Size);

            buf.Append("TypeTree:{ ");
            buf.AppendFormat("ID:{0}, ", this.TypeTree.ID);
            buf.AppendFormat("TypeName:{0}, ", this.TypeTree.TypeName);
            buf.AppendFormat("FieldName:{0} ", this.TypeTree.FieldName);
            buf.Append("}");

            buf.Append("}");
            return buf.ToString();
        }
    }
}
#endif