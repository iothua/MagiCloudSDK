using UnityEngine;
using System;

namespace MagiCloud
{
    public struct Vector2Int :IEquatable<Vector2Int>
    {
        public Vector2Int(int x,int y)
        {
            this.x=x;
            this.y=y;
        }
        public int x { get; set; }
        public int y { get; set; }
        public static Vector2Int operator +(Vector2Int a,Vector2Int b)
        {
            a.x+= b.x;
            a.y+=b.y;
            return a;
        }
        public static Vector2Int operator -(Vector2Int a,Vector2Int b)
        {
            a.x-=b.x;
            a.y-=b.y;
            return a;
        }
        public static Vector2Int operator *(Vector2Int a,int b)
        {
            a.x*=b;
            a.y*=b;
            return a;
        }

        public static implicit operator Vector2(Vector2Int v)
        {
            return new Vector2(v.x,v.y);
        }
        public bool Equals(Vector2Int other)
        {
            return other.x==x&&other.y==y;
        }
    }
}
