#if UNITY_EDITOR
using System;
using System.Collections;

namespace Loxodon.Framework.Bundles.Objects
{
    public struct Hash128 : IEquatable<Hash128>
    {
        private const int length = 16;
        private readonly byte[] bytes;

        public Hash128(byte[] bytes)
        {
            if (bytes == null)
                throw new ArgumentNullException("input");

            if (bytes.Length != length)
                throw new ArgumentException("Must have length 16");

            this.bytes = bytes;
        }

        public bool Equals(Hash128 other)
        {
            if (this.bytes == null)
                return other.bytes == null;

            for (int i = 0; i < this.bytes.Length; i++)
            {
                if (this.bytes[i] != other.bytes[i])
                    return false;
            }
            return true;
        }

        public override bool Equals(object other)
        {
            return (other is Hash128) && this.Equals((Hash128)other);
        }

        public override int GetHashCode()
        {
            if (this.bytes == null)
                return 0;

            int hash = 17;
            unchecked
            {
                for (int i = 0; i < this.bytes.Length; i++)
                {
                    hash = hash * 23 + this.bytes[i];
                }
            }
            return hash;
        }

        public static bool operator ==(Hash128 a, Hash128 b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(Hash128 a, Hash128 b)
        {
            return !(a == b);
        }

        public byte[] GetBytes()
        {
            byte[] bytes = new byte[length];
            if (this.bytes != null)
                Array.Copy(this.bytes, bytes, this.bytes.Length);
            return bytes;
        }

        public IEnumerator GetEnumerator()
        {
            return this.bytes.GetEnumerator();
        }

        public override string ToString()
        {
            return this.bytes == null ? string.Empty : BitConverter.ToString(this.bytes).Replace("-", "").ToLower();
        }
    }
}
#endif