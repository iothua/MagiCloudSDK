#if UNITY_EDITOR
using Loxodon.Framework.Bundles.Objects;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using AssetBundle = Loxodon.Framework.Bundles.Objects.AssetBundle;
using Hash128 = Loxodon.Framework.Bundles.Objects.Hash128;

namespace Loxodon.Framework.Bundles.Archives
{
    public class TypeNodeParser
    {
        public virtual object Create(ObjectInfo info)
        {
            if (info == null)
                return null;

            lock (info)
            {
                using (var reader = new ArchiveBinaryReader(new MemoryStream(info.Data)))
                {
                    var typeTree = info.TypeTree;
                    var value = Parse(typeTree, reader);
                    if (value is UnityDynamicObject)
                    {
                        UnityDynamicObject uo = value as UnityDynamicObject;
                        uo.ID = info.ID;
                        uo.Size = info.Size;
                        uo.ObjectInfo = info;
                    }
                    return value;
                }
            }
        }

        public object Parse(TypeNode typeNode, ArchiveBinaryReader reader)
        {
            object result = null;
            switch (typeNode.TypeFlag)
            {
                case "bool":
                    result = reader.ReadBoolean();
                    break;
                case "SInt8":
                    result = reader.ReadSByte();
                    break;
                case "char":
                case "UInt8":
                    result = reader.ReadByte();
                    break;
                case "short":
                case "SInt16":
                    result = reader.ReadInt16();
                    break;
                case "unsigned short":
                case "UInt16":
                    result = reader.ReadUInt16();
                    break;
                case "int":
                case "SInt32":
                    result = reader.ReadInt32();
                    break;
                case "unsigned int":
                case "UInt32":
                case "Type*":
                    result = reader.ReadUInt32();
                    break;
                case "long long":
                case "SInt64":
                    result = reader.ReadInt64();
                    break;
                case "unsigned long long":
                case "UInt64":
                    result = reader.ReadUInt64();
                    break;
                case "float":
                    result = reader.ReadSingle();
                    break;
                case "double":
                    result = reader.ReadDouble();
                    break;
                case "Quaternionf":
                    {
                        result = reader.ReadQuaternion();
                        break;
                    }
                case "float4":
                case "Vector4f":
                    {
                        result = reader.ReadVector4();
                        break;
                    }
                case "float3":
                case "Vector3f":
                    {
                        result = reader.ReadVector3();
                        break;
                    }
                case "float2":
                case "Vector2f":
                    {
                        result = reader.ReadVector2();
                        break;
                    }
                case "ColorRGBA":
                    {
                        if (typeNode.Version == 2)
                            result = reader.ReadColor32();
                        else
                            result = reader.ReadColor();
                        break;
                    }
                case "Matrix4x4f":
                    {
                        result = reader.ReadMatrix4x4();
                        break;
                    }
                case "Hash128":
                    {
                        result = reader.ReadHash128();
                        break;
                    }
                case "string":
                    {
                        result = reader.ReadString();
                        break;
                    }
                case "vector":
                case "staticvector":
                case "set":
                    {
                        var valueTypeNode = typeNode.Children[0];
                        result = this.Parse(valueTypeNode, reader);
                        break;
                    }
                case "map":
                    {
                        var pairTypeNode = typeNode.Children[0].Children[1];
                        var keyTypeNode = pairTypeNode.Children[0];
                        var valueTypeNode = pairTypeNode.Children[1];

                        var size = reader.ReadInt32();
                        Map map = new Map(typeNode);
                        for (int i = 0; i < size; i++)
                        {
                            var key = this.Parse(keyTypeNode, reader);
                            var value = this.Parse(valueTypeNode, reader);
                            map.Add(key, value);
                        }
                        result = map;
                        break;
                    }
                case "Array":
                    {
                        var valueTypeNode = typeNode.Children[1];
                        var size = reader.ReadInt32();
                        result = this.ParseArray(valueTypeNode, size, reader);
                        break;
                    }
                case "PPtr":
                    {
                        var fileID = reader.ReadInt32();
                        var pathID = reader.ReadInt64();
                        result = new PPtr(fileID, pathID, typeNode.TypeName);
                        break;
                    }
                case "TypelessData":
                    {
                        var size = reader.ReadInt32();
                        result = new TypelessData(reader.ReadBytes(size));
                        break;
                    }
                case "StreamedResource":
                    {
                        var source = reader.ReadString();
                        var offset = reader.ReadUInt64();
                        var size = reader.ReadUInt64();
                        var streamedResource = new StreamedResource(source, offset, size);
                        result = streamedResource;
                        break;
                    }
                case "AssetBundle":
                    {
                        AssetBundle bundle = new AssetBundle(((TypeTree)typeNode).Archive);
                        bundle.FullName = reader.ReadString();

                        var size = reader.ReadInt32();
                        List<PPtr> preloadTable = new List<PPtr>(size);
                        for (int i = 0; i < size; i++)
                        {
                            PPtr pptr = new PPtr(reader.ReadInt32(), reader.ReadInt64(), "PPtr<Object>");
                            preloadTable.Add(pptr);
                        }

                        bundle.Preloads.AddRange(preloadTable);

                        size = reader.ReadInt32();
                        List<AssetPair> container = new List<AssetPair>(size);
                        for (int i = 0; i < size; i++)
                        {
                            var first = reader.ReadString();
                            var preloadIndex = reader.ReadInt32();
                            var preloadSize = reader.ReadInt32();
                            var pptr = new PPtr(reader.ReadInt32(), reader.ReadInt64(), "PPtr<Object>");
                            var pair = new AssetPair(first, new Objects.AssetInfo(preloadIndex, preloadSize, pptr));
                            container.Add(pair);
                        }
                        bundle.Container.AddRange(container);

                        bundle.MainAsset = new Objects.AssetInfo(reader.ReadInt32(), reader.ReadInt32(), new PPtr(reader.ReadInt32(), reader.ReadInt64(), "PPtr<Object>"));

                        bundle.RuntimeCompatibility = reader.ReadUInt32();
                        bundle.Name = reader.ReadString();

                        size = reader.ReadInt32();
                        List<string> dependencies = new List<string>(size);
                        for (int i = 0; i < size; i++)
                        {
                            dependencies.Add(reader.ReadString());
                        }
                        bundle.Dependencies.AddRange(dependencies);

                        bundle.IsStreamed = reader.ReadBoolean();
                        result = bundle;
                        break;
                    }
                case "PreloadData":
                    {
                        PreloadData preloadData = new PreloadData(((TypeTree)typeNode).Archive);

                        preloadData.Name = reader.ReadString();

                        var size = reader.ReadInt32();
                        List<PPtr> preloadTable = new List<PPtr>(size);
                        for (int i = 0; i < size; i++)
                        {
                            PPtr pptr = new PPtr(reader.ReadInt32(), reader.ReadInt64(), "PPtr<Object>");
                            preloadTable.Add(pptr);
                        }

                        preloadData.Preloads.AddRange(preloadTable);

                        size = reader.ReadInt32();
                        List<string> dependencies = new List<string>(size);
                        for (int i = 0; i < size; i++)
                        {
                            dependencies.Add(reader.ReadString());
                        }
                        preloadData.Dependencies.AddRange(dependencies);
                        result = preloadData;
                        break;
                    }
                case "AssetBundleManifest":
                    {
                        Objects.AssetBundleManifest obj = new Objects.AssetBundleManifest((TypeTree)typeNode);
                        foreach (TypeNode childNode in typeNode.Children)
                        {
                            var key = childNode.FieldName;
                            var childValue = this.Parse(childNode, reader);
                            obj[key] = childValue;
                        }
                        result = obj;
                        break;
                    }
                default:
                    {
                        DynamicObject obj = typeNode is TypeTree ? new UnityDynamicObject((TypeTree)typeNode) : new DynamicObject(typeNode);
                        foreach (TypeNode childNode in typeNode.Children)
                        {
                            var key = childNode.FieldName;
                            var childValue = this.Parse(childNode, reader);
                            obj[key] = childValue;
                        }
                        result = obj;
                        break;
                    }
            }

            if (typeNode.IsAlign)
                reader.Align(4);

            return result;
        }

        protected virtual IList ParseArray(TypeNode typeNode, int size, ArchiveBinaryReader reader)
        {
            switch (typeNode.TypeFlag)
            {
                case "bool":
                    {
                        bool[] result = new bool[size];
                        for (int i = 0; i < size; i++)
                        {
                            result[i] = reader.ReadBoolean();
                            if (typeNode.IsAlign)
                                reader.Align(4);
                        }
                        return result;
                    }
                case "SInt8":
                    {
                        sbyte[] result = new sbyte[size];
                        for (int i = 0; i < size; i++)
                        {
                            result[i] = reader.ReadSByte();
                        }
                        return result;
                    }
                case "char":
                case "UInt8":
                    {
                        if (size <= 0)
                            return new byte[0];

                        return reader.ReadBytes(size);
                    }
                case "short":
                case "SInt16":
                    {
                        short[] result = new short[size];
                        for (int i = 0; i < size; i++)
                        {
                            result[i] = reader.ReadInt16();
                        }
                        return result;
                    }
                case "unsigned short":
                case "UInt16":
                    {
                        ushort[] result = new ushort[size];
                        for (int i = 0; i < size; i++)
                        {
                            result[i] = reader.ReadUInt16();
                        }
                        return result;
                    }
                case "int":
                case "SInt32":
                    {
                        int[] result = new int[size];
                        for (int i = 0; i < size; i++)
                        {
                            result[i] = reader.ReadInt32();
                        }
                        return result;
                    }
                case "unsigned int":
                case "UInt32":
                case "Type*":
                    {
                        uint[] result = new uint[size];
                        for (int i = 0; i < size; i++)
                        {
                            result[i] = reader.ReadUInt32();
                        }
                        return result;
                    }
                case "long long":
                case "SInt64":
                    {
                        long[] result = new long[size];
                        for (int i = 0; i < size; i++)
                        {
                            result[i] = reader.ReadInt64();
                        }
                        return result;
                    }
                case "unsigned long long":
                case "UInt64":
                    {
                        ulong[] result = new ulong[size];
                        for (int i = 0; i < size; i++)
                        {
                            result[i] = reader.ReadUInt64();
                        }
                        return result;
                    }
                case "float":
                    {
                        float[] result = new float[size];
                        for (int i = 0; i < size; i++)
                        {
                            result[i] = reader.ReadSingle();
                        }
                        return result;
                    }
                case "double":
                    {
                        double[] result = new double[size];
                        for (int i = 0; i < size; i++)
                        {
                            result[i] = reader.ReadDouble();
                        }
                        return result;
                    }
                case "Quaternionf":
                    {
                        Quaternion[] result = new Quaternion[size];
                        for (int i = 0; i < size; i++)
                        {
                            result[i] = reader.ReadQuaternion();
                        }
                        return result;
                    }
                case "float4":
                case "Vector4f":
                    {
                        Vector4[] result = new Vector4[size];
                        for (int i = 0; i < size; i++)
                        {
                            result[i] = reader.ReadVector4();
                        }
                        return result;
                    }
                case "float3":
                case "Vector3f":
                    {
                        Vector3[] result = new Vector3[size];
                        for (int i = 0; i < size; i++)
                        {
                            result[i] = reader.ReadVector3();
                        }
                        return result;
                    }
                case "float2":
                case "Vector2f":
                    {
                        Vector2[] result = new Vector2[size];
                        for (int i = 0; i < size; i++)
                        {
                            result[i] = reader.ReadVector2();
                        }
                        return result;
                    }
                case "ColorRGBA":
                    {
                        Color[] result = new Color[size];
                        for (int i = 0; i < size; i++)
                        {
                            result[i] = reader.ReadColor();
                        }
                        return result;
                    }
                case "Matrix4x4f":
                    {
                        Matrix4x4[] result = new Matrix4x4[size];
                        for (int i = 0; i < size; i++)
                        {
                            result[i] = reader.ReadMatrix4x4();
                        }
                        return result;
                    }
                case "Hash128":
                    {
                        Hash128[] result = new Hash128[size];
                        for (int i = 0; i < size; i++)
                        {
                            result[i] = reader.ReadHash128();
                        }
                        return result;
                    }
                case "string":
                    {
                        string[] result = new string[size];
                        for (int i = 0; i < size; i++)
                        {
                            result[i] = reader.ReadString();
                        }
                        return result;
                    }
                default:
                    {
                        object[] result = new object[size];
                        for (int i = 0; i < size; i++)
                        {
                            result[i] = this.Parse(typeNode, reader);
                        }
                        return result;
                    }
            }
        }
    }
}
#endif