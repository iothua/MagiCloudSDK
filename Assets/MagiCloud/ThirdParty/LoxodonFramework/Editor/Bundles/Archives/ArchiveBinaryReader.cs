#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using Hash128 = Loxodon.Framework.Bundles.Objects.Hash128;

namespace Loxodon.Framework.Bundles.Archives
{
    public class ArchiveBinaryReader : BinaryReader
    {
        private Encoding encoding;
        private bool isBigEndian = false;

        public ArchiveBinaryReader(Stream input) : this(input, Encoding.UTF8, false)
        {
        }

        public ArchiveBinaryReader(Stream input, Encoding encoding) : this(input, encoding, false)
        {
        }

        public ArchiveBinaryReader(Stream input, Encoding encoding, bool isBigEndian) : base(input, encoding)
        {
            this.encoding = encoding;
            this.isBigEndian = isBigEndian;
        }

        public virtual long Position
        {
            get { return this.BaseStream.Position; }
            set { this.BaseStream.Position = value; }
        }

        public virtual Encoding Encoding
        {
            get { return this.encoding; }
        }

        public virtual bool IsBigEndian
        {
            get { return this.isBigEndian; }
            set { this.isBigEndian = value; }
        }

        public virtual void Align(int align)
        {
            var mode = (int)(this.BaseStream.Position % align);
            if (mode != 0)
                this.ReadBytes(align - mode);
        }

        public virtual string ReadCString()
        {
            List<byte> bytes = new List<byte>();
            byte b;
            while ((b = this.ReadByte()) != 0)
                bytes.Add(b);

            return this.encoding.GetString(bytes.ToArray());

            //long position = this.Position;
            //int length = 0;
            //while (this.ReadByte() != 0)
            //{
            //    length++;
            //}
            //this.Position = position;
            //if (length <= 0)
            //    return string.Empty;

            //byte[] buffer = this.ReadBytes(length);
            //this.ReadByte();
            //return this.encoding.GetString(buffer);
        }

        public override short ReadInt16()
        {
            return ReadInt16(IsBigEndian);
        }

        public virtual short ReadInt16(bool isBigEndian)
        {
            if (!isBigEndian)
                return base.ReadInt16();

            return Reverse(base.ReadInt16());
        }

        public override int ReadInt32()
        {
            return this.ReadInt32(this.IsBigEndian);
        }

        public virtual int ReadInt32(bool isBigEndian)
        {
            if (!isBigEndian)
                return base.ReadInt32();

            return Reverse(base.ReadInt32());
        }

        public override long ReadInt64()
        {
            return this.ReadInt64(this.IsBigEndian);
        }

        public virtual long ReadInt64(bool isBigEndian)
        {
            if (!isBigEndian)
                return base.ReadInt64();

            return Reverse(base.ReadInt64());
        }

        public override ushort ReadUInt16()
        {
            return this.ReadUInt16(this.IsBigEndian);
        }

        public virtual ushort ReadUInt16(bool isBigEndian)
        {
            if (!isBigEndian)
                return base.ReadUInt16();

            return Reverse(base.ReadUInt16());
        }

        public override uint ReadUInt32()
        {
            return this.ReadUInt32(this.IsBigEndian);
        }

        public virtual uint ReadUInt32(bool isBigEndian)
        {
            if (!isBigEndian)
                return base.ReadUInt32();

            return Reverse(base.ReadUInt32());
        }

        public override ulong ReadUInt64()
        {
            return this.ReadUInt64(this.IsBigEndian);
        }

        public virtual ulong ReadUInt64(bool isBigEndian)
        {
            if (!isBigEndian)
                return base.ReadUInt64();

            return Reverse(base.ReadUInt64());
        }

        public override float ReadSingle()
        {
            return this.ReadSingle(this.IsBigEndian);
        }

        public virtual float ReadSingle(bool isBigEndian)
        {
            if (!isBigEndian)
                return base.ReadSingle();

            return Reverse(base.ReadSingle());
        }

        public override double ReadDouble()
        {
            return this.ReadDouble(this.IsBigEndian);
        }

        public virtual double ReadDouble(bool isBigEndian)
        {
            if (!isBigEndian)
                return base.ReadDouble();

            return Reverse(base.ReadDouble());
        }

        public override string ReadString()
        {
            int length = this.ReadInt32();
            if (length <= 0)
                return string.Empty;

            var stringData = this.ReadBytes(length);
            this.Align(4);
            return this.Encoding.GetString(stringData);
        }

        public virtual Quaternion ReadQuaternion()
        {
            return new Quaternion
            {
                x = this.ReadSingle(),
                y = this.ReadSingle(),
                z = this.ReadSingle(),
                w = this.ReadSingle()
            };
        }

        public virtual Vector4 ReadVector4()
        {
            return new Vector4
            {
                x = this.ReadSingle(),
                y = this.ReadSingle(),
                z = this.ReadSingle(),
                w = this.ReadSingle()
            }; ;
        }

        public virtual Vector3 ReadVector3()
        {
            return new Vector3
            {
                x = this.ReadSingle(),
                y = this.ReadSingle(),
                z = this.ReadSingle()
            };
        }

        public virtual Vector2 ReadVector2()
        {
            return new Vector2
            {
                x = this.ReadSingle(),
                y = this.ReadSingle()
            };
        }

        public virtual Color ReadColor()
        {
            return new Color()
            {
                r = this.ReadSingle(),
                g = this.ReadSingle(),
                b = this.ReadSingle(),
                a = this.ReadSingle()
            };
        }

        public virtual Color32 ReadColor32()
        {
            return new Color32()
            {
                r = this.ReadByte(),
                g = this.ReadByte(),
                b = this.ReadByte(),
                a = this.ReadByte()
            };
        }

        public virtual Matrix4x4 ReadMatrix4x4()
        {
            return new Matrix4x4
            {
                m00 = this.ReadSingle(),
                m01 = this.ReadSingle(),
                m02 = this.ReadSingle(),
                m03 = this.ReadSingle(),
                m10 = this.ReadSingle(),
                m11 = this.ReadSingle(),
                m12 = this.ReadSingle(),
                m13 = this.ReadSingle(),
                m20 = this.ReadSingle(),
                m21 = this.ReadSingle(),
                m22 = this.ReadSingle(),
                m23 = this.ReadSingle(),
                m30 = this.ReadSingle(),
                m31 = this.ReadSingle(),
                m32 = this.ReadSingle(),
                m33 = this.ReadSingle()
            };
        }

        public virtual Hash128 ReadHash128()
        {
            return new Hash128(this.ReadBytes(16));
        }

        private short Reverse(short value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            Array.Reverse(bytes);
            return BitConverter.ToInt16(bytes, 0);
        }

        private int Reverse(int value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            Array.Reverse(bytes);
            return BitConverter.ToInt32(bytes, 0);
        }

        private long Reverse(long value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            Array.Reverse(bytes);
            return BitConverter.ToInt64(bytes, 0);
        }

        private ushort Reverse(ushort value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            Array.Reverse(bytes);
            return BitConverter.ToUInt16(bytes, 0);
        }

        private uint Reverse(uint value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            Array.Reverse(bytes);
            return BitConverter.ToUInt32(bytes, 0);
        }

        private ulong Reverse(ulong value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            Array.Reverse(bytes);
            return BitConverter.ToUInt64(bytes, 0);
        }

        private float Reverse(float value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            Array.Reverse(bytes);
            return BitConverter.ToSingle(bytes, 0);
        }

        private double Reverse(double value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            Array.Reverse(bytes);
            return BitConverter.ToDouble(bytes, 0);
        }
    }
}
#endif