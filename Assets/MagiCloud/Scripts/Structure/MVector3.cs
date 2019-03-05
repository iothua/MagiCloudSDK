using Newtonsoft.Json;
using System;
using UnityEngine;

namespace MagiCloud
{
    [JsonObject(MemberSerialization.OptOut)]
    [Serializable]
    public struct MVector3
    {
        public float X;
        public float Y;
        public float Z;

        [JsonIgnore]
        public Vector3 Vector {
            get {
                return new Vector3(X, Y, Z);
            }
            set {

                X = value.x;
                Y = value.y;
                Z = value.z;
            }
        }

        public MVector3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public MVector3(Vector3 vector)
        {
            X = vector.x;
            Y = vector.y;
            Z = vector.z;
        }

        public override string ToString()
        {
            return Vector.ToString();
        }
    }

}
