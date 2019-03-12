#if UNITY_EDITOR
using Loxodon.Framework.Bundles.Objects;
using System.Collections.Generic;
using System.Text;

namespace Loxodon.Framework.Bundles.Archives
{
    public interface IObjectInfo
    {
        long ID { get; }

        string Name { get; }

        TypeID TypeID { get; }

        bool IsStreamed { get; }

        bool IsBuiltin { get; }

        bool IsPotentialRedundancy { get; }

        long Size { get; }

        AssetArchive Archive { get; }

        IFingerprint Fingerprint { get; }
    }

    public abstract class AbstractObjectInfo : IObjectInfo
    {
        protected object _lock = new object();

        protected IFingerprint fingerprint;

        public virtual long ID { get; protected set; }

        public virtual string Name { get { return string.Empty; } }

        public virtual TypeID TypeID { get; protected set; }

        public virtual long Size { get { return 0; } }

        public virtual bool IsStreamed { get; protected set; }

        public virtual bool IsBuiltin { get; protected set; }

        public virtual bool IsPotentialRedundancy { get; protected set; }

        public virtual AssetArchive Archive { get; protected set; }

        public virtual IFingerprint Fingerprint { get { return this.fingerprint; } }

        public AbstractObjectInfo(AssetArchive archive)
        {
            this.Archive = archive;
            this.IsBuiltin = this.Archive == null ? false : Archive.Type == ArchiveType.BUILTIN;
            this.IsStreamed = this.Archive == null ? false : Archive.Type == ArchiveType.CAB;
        }
    }

    public class BuiltinObjectInfo : AbstractObjectInfo
    {
        public override string Name { get { return this.obj.name; } }

        private UnityEngine.Object obj;

        public BuiltinObjectInfo(BuiltinArchive archive, long id, TypeID typeId, UnityEngine.Object obj) : base(archive)
        {
            this.ID = id;
            this.TypeID = typeId;
            this.IsPotentialRedundancy = false;
            this.obj = obj;
            this.fingerprint = new IdentifierFingerprint(this.ID, this.Archive.Name);
        }

        public virtual T GetObject<T>() where T : class
        {
            return this.obj as T;
        }

        public override string ToString()
        {
            StringBuilder buf = new StringBuilder();
            buf.AppendFormat("{0}", this.GetType().Name).Append(" { ");
            buf.AppendFormat("ID:{0}, ", this.ID);
            buf.AppendFormat("TypeID:{0}, ", this.TypeID);
            buf.AppendFormat("IsStreamed:{0}, ", this.IsStreamed);
            buf.AppendFormat("IsBuiltin:{0}, ", this.IsBuiltin);
            buf.AppendFormat("IsPotentialRedundancy:{0}, ", this.IsPotentialRedundancy);
            buf.AppendFormat("Name:{0}, ", this.Name);
            buf.AppendFormat("Archive:{0} ", this.Archive == null ? "" : this.Archive.Name);
            buf.Append("}");
            return buf.ToString();
        }
    }

    public class ObjectInfo : AbstractObjectInfo
    {
        private string name = "";

        private int dataSize;

        private long dataOffset;

        public new string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }

        public override long Size
        {
            get
            {
                if (this.Resources == null)
                    return this.dataSize;

                return this.dataSize + (long)this.Resources.Size;
            }
        }

        public bool IsPublic { get; protected set; }

        public TypeTree TypeTree { get; private set; }

        public new IFingerprint Fingerprint
        {
            get { return this.fingerprint; }
            set { this.fingerprint = value; }
        }

        public StreamedResource Resources { get; set; }

        public new ObjectArchive Archive { get { return (ObjectArchive)base.Archive; } }

        public List<IObjectInfo> GetReferences()
        {
            PropertiesFingerprint fingerprint = this.fingerprint as PropertiesFingerprint;
            if (fingerprint != null)
                return fingerprint.GetReferences();

            return new List<IObjectInfo>();
        }

        public byte[] Data { get { return this.Archive.GetObjectData(this.dataOffset, this.dataSize); } }

        public ObjectInfo(ObjectArchive archive, long id, TypeTree tree, long offset, int size, bool isPublic) : base(archive)
        {
            this.ID = id;
            this.IsPublic = isPublic;
            this.TypeTree = tree;

            this.dataSize = size;
            this.dataOffset = offset;

            this.TypeID = this.TypeTree.TypeID;
            this.IsPotentialRedundancy = PotentialRedundancy();
        }

        private bool PotentialRedundancy()
        {
            if (IsPublic)
                return false;

            switch (this.TypeID)
            {
                case TypeID.TextAsset:
                case TypeID.Texture2D:
                case TypeID.Sprite:
                case TypeID.Shader:
                case TypeID.Mesh:
                case TypeID.Avatar:
                case TypeID.Material:
                case TypeID.Font:
                case TypeID.AudioClip:
                case TypeID.VideoClip:
                case TypeID.AnimationClip:
                case TypeID.AnimatorController:
                case TypeID.AnimatorOverrideController:
                case TypeID.TerrainData:
                    return true;
                default:
                    return false;
            }
        }

        public override string ToString()
        {
            StringBuilder buf = new StringBuilder();
            buf.AppendFormat("{0}", this.GetType().Name).Append(" { ");
            buf.AppendFormat("ID:{0}, ", this.ID);
            buf.AppendFormat("TypeID:{0}, ", this.TypeID);
            buf.AppendFormat("IsStreamed:{0}, ", this.IsStreamed);
            buf.AppendFormat("IsBuiltin:{0}, ", this.IsBuiltin);
            buf.AppendFormat("IsPotentialRedundancy:{0}, ", this.IsPotentialRedundancy);
            buf.AppendFormat("IsPublic:{0}, ", this.IsPublic);
            buf.AppendFormat("Size:{0}, ", this.Size);

            buf.AppendFormat("Archive:{0}, ", this.Archive == null ? "" : this.Archive.Name);

            buf.Append("TypeTree:{ ");
            buf.AppendFormat("ID:{0}, ", this.TypeTree.ID);
            buf.AppendFormat("TypeName:{0}, ", this.TypeTree.TypeName);
            buf.AppendFormat("FieldName:{0} ", this.TypeTree.FieldName);
            buf.Append("}");

            buf.Append("}");
            return buf.ToString();
        }
    }

    public class NullObjectInfo : AbstractObjectInfo
    {
        public NullObjectInfo(TypeID typeId) : base(null)
        {
            this.TypeID = typeId;
            this.ID = 0;
            this.IsStreamed = false;
            this.IsBuiltin = false;
            this.IsPotentialRedundancy = false;
            this.fingerprint = new IdentifierFingerprint(0, "");
        }

        public virtual T GetObject<T>() where T : class
        {
            return null;
        }

        public override string ToString()
        {
            StringBuilder buf = new StringBuilder();
            buf.AppendFormat("{0}", this.GetType().Name).Append(" { ");
            buf.AppendFormat("ID:{0}, ", this.ID);
            buf.AppendFormat("TypeID:{0}, ", this.TypeID);
            buf.AppendFormat("IsStreamed:{0}, ", this.IsStreamed);
            buf.AppendFormat("IsBuiltin:{0}, ", this.IsBuiltin);
            buf.AppendFormat("IsPotentialRedundancy:{0}, ", this.IsPotentialRedundancy);
            buf.AppendFormat("Archive:{0} ", this.Archive == null ? "" : this.Archive.Name);
            buf.Append("}");
            return buf.ToString();
        }
    }

    public class MissingObjectInfo : AbstractObjectInfo
    {
        public MissingObjectInfo(AssetArchive archive, long id, TypeID typeId) : base(archive)
        {
            this.ID = id;
            this.TypeID = typeId;
            this.IsPotentialRedundancy = false;
            this.fingerprint = new IdentifierFingerprint(this.ID, this.Archive == null ? "" : this.Archive.Name);
        }

        public virtual T GetObject<T>() where T : class
        {
            return null;
        }

        public override string ToString()
        {
            StringBuilder buf = new StringBuilder();
            buf.AppendFormat("{0}", this.GetType().Name).Append(" { ");
            buf.AppendFormat("ID:{0}, ", this.ID);
            buf.AppendFormat("TypeID:{0}, ", this.TypeID);
            buf.AppendFormat("IsStreamed:{0}, ", this.IsStreamed);
            buf.AppendFormat("IsBuiltin:{0}, ", this.IsBuiltin);
            buf.AppendFormat("IsPotentialRedundancy:{0}, ", this.IsPotentialRedundancy);
            buf.AppendFormat("Archive:{0} ", this.Archive == null ? "" : this.Archive.Name);
            buf.Append("}");
            return buf.ToString();
        }
    }
}
#endif