#if UNITY_EDITOR
using Loxodon.Framework.Bundles.Objects;
using System;
using System.Collections.Generic;
using System.IO;
using Hash128 = Loxodon.Framework.Bundles.Objects.Hash128;

namespace Loxodon.Framework.Bundles.Archives
{
    public class FeatureInfo
    {
        private List<PPtr> references = new List<PPtr>();

        public List<PPtr> References { get { return this.references; } }

        public StreamedResource Resources { get; set; }

        public Hash128 PropertyHash { get; set; }

        public string Name { get; set; }

        public FeatureInfo()
        {
        }

        public void Add(PPtr pptr)
        {
            this.references.Add(pptr);
        }
    }

    public class FeatureExtractor
    {
        public FeatureInfo Extract(ObjectInfo info, ArchiveBinaryReader reader)
        {
            FeatureInfo data = new FeatureInfo();
            using (MemoryStream output = new MemoryStream())
            {
                this.ReadNode(info.TypeTree, reader, info.Archive, output, data);
                output.Seek(0, SeekOrigin.Begin);
                var hash = ArchiveUtil.Hash(output);
                data.PropertyHash = hash;
            }
            return data;
        }

        protected virtual void ReadNode(TypeNode typeNode, ArchiveBinaryReader reader, ObjectArchive archive, Stream output, FeatureInfo data)
        {
            Stream src = reader.BaseStream;
            switch (typeNode.TypeFlag)
            {
                case "bool":
                case "SInt8":
                case "char":
                case "UInt8":
                case "short":
                case "SInt16":
                case "unsigned short":
                case "UInt16":
                case "int":
                case "SInt32":
                case "unsigned int":
                case "UInt32":
                case "Type*":
                case "long long":
                case "SInt64":
                case "unsigned long long":
                case "UInt64":
                case "float":
                case "double":
                case "Quaternionf":
                case "float4":
                case "Vector4f":
                case "float3":
                case "Vector3f":
                case "float2":
                case "Vector2f":
                case "ColorRGBA":
                case "Matrix4x4f":
                case "Hash128":
                    {
                        this.CopyTo(src, output, typeNode.Size);
                        break;
                    }
                case "string":
                    {
                        if ((typeNode.Depth == 1 || (typeNode.Depth == 2 && typeNode.Root.TypeID == TypeID.Shader)) && typeNode.FieldName.Equals("m_Name"))
                        {
                            var position = reader.BaseStream.Position;
                            data.Name = reader.ReadString();
                            reader.BaseStream.Position = position;
                        }

                        int size = reader.ReadInt32();
                        if (size > 0)
                            this.CopyTo(src, output, size);
                        reader.Align(4);
                        break;
                    }
                case "Array":
                    {
                        var valueTypeNode = typeNode.Children[1];
                        var size = reader.ReadInt32();
                        if (size <= 0)
                            break;

                        this.ReadArrayNode(valueTypeNode, size, reader, archive, output, data);
                        break;
                    }
                case "TypelessData":
                    {
                        var size = reader.ReadInt32();
                        if (size > 0)
                            this.CopyTo(src, output, size);
                        break;
                    }
                case "PPtr":
                    {
                        var fileID = reader.ReadInt32();
                        var pathID = reader.ReadInt64();
                        var pptr = new PPtr(fileID, pathID, typeNode.TypeName);
                        data.Add(pptr);
                        break;
                    }
                case "StreamedResource":
                    {
                        var source = reader.ReadString();
                        var offset = reader.ReadUInt64();
                        var size = reader.ReadUInt64();
                        var streamedResource = new StreamedResource(source, offset, size);
                        data.Resources = streamedResource;

                        if (size <= 0)
                            break;

                        using (Stream dataStream = archive.GetResourceStream(streamedResource))
                        {
                            if (dataStream == null)
                                break;

                            byte[] buffer = ArchiveUtil.HashBytes(dataStream);
                            output.Write(buffer, 0, buffer.Length);
                        }
                        break;
                    }
                default:
                    {
                        foreach (TypeNode childNode in typeNode.Children)
                        {
                            ReadNode(childNode, reader, archive, output, data);
                        }
                        break;
                    }
            }

            if (typeNode.IsAlign)
                reader.Align(4);
        }

        protected virtual void ReadArrayNode(TypeNode typeNode, int size, ArchiveBinaryReader reader, ObjectArchive archive, Stream output, FeatureInfo data)
        {
            if (size <= 0)
                return;

            Stream src = reader.BaseStream;
            switch (typeNode.TypeFlag)
            {
                case "bool":
                case "SInt8":
                case "char":
                case "UInt8":
                case "short":
                case "SInt16":
                case "unsigned short":
                case "UInt16":
                case "int":
                case "SInt32":
                case "unsigned int":
                case "UInt32":
                case "Type*":
                case "long long":
                case "SInt64":
                case "unsigned long long":
                case "UInt64":
                case "float":
                case "double":
                case "Quaternionf":
                case "float4":
                case "Vector4f":
                case "float3":
                case "Vector3f":
                case "float2":
                case "Vector2f":
                case "ColorRGBA":
                case "Matrix4x4f":
                case "Hash128":
                    {
                        this.CopyTo(src, output, typeNode.Size * size);
                        break;
                    }
                default:
                    {
                        for (int i = 0; i < size; i++)
                        {
                            this.ReadNode(typeNode, reader, archive, output, data);
                        }
                        break;
                    }
            }
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
    }
}
#endif