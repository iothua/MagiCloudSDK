using System;
using UnityEngine;
using Sirenix.OdinInspector;

namespace MagiCloud
{
    /// <summary>
    /// 碰撞体大小
    /// </summary>
    [Serializable]
    [LabelText("碰撞体信息")]
    public class ColliderData
    {
        [LabelText("中心点(Center)")]
        public MVector3 Center;
        [LabelText("碰撞大小(Size)")]
        public MVector3 Size = new MVector3(Vector3.one);

        public ColliderData() { }

        public ColliderData(BoxCollider boxCollider)
        {
            Center.Vector = boxCollider.center;
            Size.Vector = boxCollider.size;
        }
    }
}
