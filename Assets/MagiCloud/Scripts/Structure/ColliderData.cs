using System;

namespace MagiCloud
{
    /// <summary>
    /// 碰撞体大小
    /// </summary>
    [Serializable]
    public struct ColliderData
    {
        public MVector3 Center;
        public MVector3 Size;
    }
}
