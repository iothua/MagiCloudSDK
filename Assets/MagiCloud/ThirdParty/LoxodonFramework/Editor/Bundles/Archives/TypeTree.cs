#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Hash128 = Loxodon.Framework.Bundles.Objects.Hash128;

namespace Loxodon.Framework.Bundles.Archives
{
    static class ByteExtensions
    {
        public static string GetString(this byte[] data, int position)
        {
            int totalLength = data.Length;
            int offset = position;
            while (true)
            {
                byte b = data[offset++];
                if (b == 0 || offset >= totalLength)
                    break;
            }

            byte[] buffer = new byte[offset - position - 1];
            Array.Copy(data, position, buffer, 0, buffer.Length);
            return Encoding.ASCII.GetString(buffer);
        }
    }

    [Serializable]
    public class TypeNode
    {
        protected List<TypeNode> children = new List<TypeNode>();
        public TypeTree Root { get; internal set; }
        public ushort Version { get; internal set; }
        public bool IsArray { get; internal set; }
        public string TypeName { get; internal set; }
        public string FieldName { get; internal set; }
        public int Size { get; internal set; }
        public int Index { get; internal set; }
        public int Flags { get; internal set; }
        public byte Depth { get; internal set; }
        public List<TypeNode> Children { get { return children; } }
        public bool IsAlign { get { return (this.Flags & 0x4000) > 0; } }

        public string TypeFlag
        {
            get
            {
                string typeFlag = this.TypeName;
                if (Regex.IsMatch(typeFlag, "^PPtr<"))
                    typeFlag = "PPtr";

                return typeFlag;
            }
        }

        public TypeNode(TypeTree root)
        {
            this.Root = root;
        }

        public override string ToString()
        {
            StringBuilder buf = new StringBuilder();
            ToString(buf, this.Depth);
            return buf.ToString();
        }

        internal virtual void ToString(StringBuilder buf, int rootDepth)
        {
            string indent = "";
            for (int i = 0; i < this.Depth - rootDepth; i++)
                indent += "\t";

            buf.Append(indent);
            buf.Append("{");
            buf.AppendFormat("Version:{0}, ", this.Version);
            buf.AppendFormat("Depth:{0}, ", this.Depth);
            buf.AppendFormat("IsArray:{0}, ", this.IsArray);
            buf.AppendFormat("TypeName:\"{0}\", ", this.TypeName);
            buf.AppendFormat("FieldName:\"{0}\", ", this.FieldName);
            buf.AppendFormat("Size:{0}, ", this.Size);
            //buf.AppendFormat("Index:{0}, ", this.Index);
            buf.AppendFormat("Flags:{0}, ", this.Flags);
            buf.AppendFormat("IsAlign:{0}, ", this.IsAlign);

            if (this.Children != null && this.Children.Count > 0)
            {
                buf.Append("Children:[");
                buf.Append("\r\n");
                for (int i = 0; i < this.Children.Count; i++)
                {
                    var child = this.Children[i];
                    child.ToString(buf, rootDepth);
                    if (i < this.Children.Count - 1)
                        buf.Append(",\r\n");
                }
                buf.Append("\r\n");
                buf.Append(indent).Append("]");
            }
            buf.Append("}");
        }
    }

    [System.Serializable]
    public class TypeTree : TypeNode
    {
        private static readonly Hash128 zero = new Hash128(new byte[16]);
        private static readonly byte[] baseStringTable = Encoding.ASCII.GetBytes("AABB\0AnimationClip\0AnimationCurve\0AnimationState\0Array\0Base\0BitField\0bitset\0bool\0char\0ColorRGBA\0Component\0data\0deque\0double\0dynamic_array\0FastPropertyName\0first\0float\0Font\0GameObject\0Generic Mono\0GradientNEW\0GUID\0GUIStyle\0int\0list\0long long\0map\0Matrix4x4f\0MdFour\0MonoBehaviour\0MonoScript\0m_ByteSize\0m_Curve\0m_EditorClassIdentifier\0m_EditorHideFlags\0m_Enabled\0m_ExtensionPtr\0m_GameObject\0m_Index\0m_IsArray\0m_IsStatic\0m_MetaFlag\0m_Name\0m_ObjectHideFlags\0m_PrefabInternal\0m_PrefabParentObject\0m_Script\0m_StaticEditorFlags\0m_Type\0m_Version\0Object\0pair\0PPtr<Component>\0PPtr<GameObject>\0PPtr<Material>\0PPtr<MonoBehaviour>\0PPtr<MonoScript>\0PPtr<Object>\0PPtr<Prefab>\0PPtr<Sprite>\0PPtr<TextAsset>\0PPtr<Texture>\0PPtr<Texture2D>\0PPtr<Transform>\0Prefab\0Quaternionf\0Rectf\0RectInt\0RectOffset\0second\0set\0short\0size\0SInt16\0SInt32\0SInt64\0SInt8\0staticvector\0string\0TextAsset\0TextMesh\0Texture\0Texture2D\0Transform\0TypelessData\0UInt16\0UInt32\0UInt64\0UInt8\0unsigned int\0unsigned long long\0unsigned short\0vector\0Vector2f\0Vector3f\0Vector4f\0m_ScriptingClassIdentifier\0Gradient\0Type*\0");

        public int ID { get; private set; }

        public TypeID TypeID { get; private set; }

        public short ScriptIndex { get; private set; }

        public Hash128 Hash { get; private set; }

        public Hash128 PropertiesHash { get; private set; }

        public ObjectArchive Archive { get; private set; }

        public TypeTree(ObjectArchive archive, int id, TypeID typeId, short scriptIndex, Hash128 hash, Hash128 propertiesHash) : base(null)
        {
            this.Root = this;
            this.Archive = archive;
            this.ID = id;
            this.TypeID = typeId;
            this.ScriptIndex = scriptIndex;
            this.Hash = hash;
            this.PropertiesHash = propertiesHash;
        }

        internal virtual void Load(ArchiveBinaryReader reader)
        {
            var nodeCount = reader.ReadInt32();
            var typeTableBufferSize = reader.ReadInt32();

            var nodeBuffer = reader.ReadBytes(24 * nodeCount);

            byte[] stringTable = reader.ReadBytes(typeTableBufferSize);

            using (var nodeReader = new ArchiveBinaryReader(new MemoryStream(nodeBuffer)))
            {
                var stack = new Stack<TypeNode>();
                stack.Push(this);

                this.Version = nodeReader.ReadUInt16();
                this.Depth = nodeReader.ReadByte();
                this.IsArray = nodeReader.ReadBoolean();

                ushort position = nodeReader.ReadUInt16();
                ushort flag = nodeReader.ReadUInt16();
                this.TypeName = flag == 0 ? stringTable.GetString(position) : baseStringTable.GetString(position);

                position = nodeReader.ReadUInt16();
                flag = nodeReader.ReadUInt16();
                this.FieldName = flag == 0 ? stringTable.GetString(position) : baseStringTable.GetString(position);

                this.Size = nodeReader.ReadInt32();
                this.Index = nodeReader.ReadInt32();
                this.Flags = nodeReader.ReadInt32();

                for (var i = 1; i < nodeCount; i++)
                {
                    var current = new TypeNode(this);
                    current.Version = nodeReader.ReadUInt16();
                    current.Depth = nodeReader.ReadByte();
                    current.IsArray = nodeReader.ReadBoolean();

                    position = nodeReader.ReadUInt16();
                    flag = nodeReader.ReadUInt16();
                    current.TypeName = flag == 0 ? stringTable.GetString(position) : baseStringTable.GetString(position);

                    position = nodeReader.ReadUInt16();
                    flag = nodeReader.ReadUInt16();
                    current.FieldName = flag == 0 ? stringTable.GetString(position) : baseStringTable.GetString(position);

                    current.Size = nodeReader.ReadInt32();
                    current.Index = nodeReader.ReadInt32();
                    current.Flags = nodeReader.ReadInt32();

                    while (stack.Count > current.Depth)
                        stack.Pop();

                    stack.Peek().Children.Add(current);
                    stack.Push(current);
                }
            }
        }

        internal override void ToString(StringBuilder buf, int rootDepth)
        {
            string indent = "";
            for (int i = 0; i < this.Depth - rootDepth; i++)
                indent += "\t";

            buf.Append(indent);
            buf.Append("{");
            buf.AppendFormat("ID:{0}, ", this.ID);
            buf.AppendFormat("TypeID:{0}, ", this.TypeID);
            if (this.ScriptIndex >= 0)
                buf.AppendFormat("ScriptIndex:{0}, ", this.ScriptIndex);
            buf.AppendFormat("Version:{0}, ", this.Version);
            buf.AppendFormat("Depth:{0}, ", this.Depth);
            buf.AppendFormat("IsArray:{0}, ", this.IsArray);
            buf.AppendFormat("TypeName:\"{0}\", ", this.TypeName);
            buf.AppendFormat("FieldName:\"{0}\", ", this.FieldName);
            buf.AppendFormat("Size:{0}, ", this.Size);
            //buf.AppendFormat("Index:{0}, ", this.Index);
            buf.AppendFormat("Flags:{0}, ", this.Flags);
            buf.AppendFormat("IsAlign:{0}, ", this.IsAlign);
            buf.AppendFormat("Hash:{0}, ", this.Hash);
            if (this.PropertiesHash != zero)
                buf.AppendFormat("PropertiesHash:{0}, ", this.PropertiesHash);

            if (this.Children != null && this.Children.Count > 0)
            {
                buf.Append("Children:[");
                buf.Append("\r\n");
                for (int i = 0; i < this.Children.Count; i++)
                {
                    var child = this.Children[i];
                    child.ToString(buf, rootDepth);
                    if (i < this.Children.Count - 1)
                        buf.Append(",\r\n");
                }
                buf.Append("\r\n");
                buf.Append(indent).Append("]");
            }
            buf.Append("}");
        }
    }
}
#endif