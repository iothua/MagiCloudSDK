using MagiCloud.Core;

namespace MagiCloud
{
    /// <summary>
    /// 存储操作key
    /// </summary>
    public struct OperateKey
    {
        public int handIndex;
        public OperatePlatform platform;

        public OperateKey(int handIndex, OperatePlatform platform)
        {
            this.handIndex = handIndex;
            this.platform = platform;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            var key = (OperateKey)obj;

            return this.handIndex == key.handIndex
                && this.platform == key.platform;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
