using System;

namespace MagiCloud.Interactive.Distance
{
    /// <summary>
    /// 距离类型
    /// </summary>
    public enum DistanceType
    {
        D3D = 0,
        D2D = 1,
        DScreen = 2
    }

    /// <summary>
    /// 距离形状
    /// </summary>
    public enum DistanceShape
    {
        Sphere,
        Cube
    }

    /// <summary>
    /// 交互检测类型
    /// </summary>
    public enum InteractionDetectType
    {
        /// <summary>
        /// 以接收点为主
        /// </summary>
        Receive,
        /// <summary>
        /// 以发送端为主
        /// </summary>
        Send,
        /// <summary>
        /// 并且
        /// </summary>
        And
    }

    /// <summary>
    /// 交互类型
    /// </summary>
    public enum InteractionType
    {
        /// <summary>
        /// 无
        /// </summary>
        None,
        /// <summary>
        /// 发送
        /// </summary>
        Send,
        /// <summary>
        /// 接收
        /// </summary>
        Receive,
        /// <summary>
        /// 都支持
        /// </summary>
        All,
        /// <summary>
        /// 倒水
        /// </summary>
        Pour
    }

    /// <summary>
    /// 距离状态
    /// </summary>
    public enum DistanceStatus
    {
        /// <summary>
        /// 离开(默认状态)
        /// </summary>
        Exit,
        /// <summary>
        /// 移入(移入状态)
        /// </summary>
        Enter,
        /// <summary>
        /// 完成(完成状态)
        /// </summary>
        Complete
    }
}
