using System;
using UnityEngine;

namespace MagiCloud
{
    /// <summary>
    /// 碰撞体大小
    /// </summary>
    [Serializable]
    public class ColliderData
    {
        [Header("中心点(Center)")]
        public MVector3 Center;
        [Header("碰撞大小(Size)")]
        public MVector3 Size = new MVector3(Vector3.one);

        public ColliderData() { }

        public ColliderData(BoxCollider boxCollider)
        {
            Center.Vector = boxCollider.center;
            Size.Vector = boxCollider.size;
        }
    }
}
