namespace MagiCloud.Core
{
    /// <summary>
    /// Input手状态
    /// </summary>
    public enum MInputHandStatus
    {
        
        /// <summary>
        /// 释放
        /// </summary>
        Idle,
        /// <summary>
        /// 握拳
        /// </summary>
        Grip,
        /// <summary>
        /// 抓取
        /// </summary>
        Grab,
        /// <summary>
        /// 抓取中……
        /// </summary>
        Grabing,
        /// <summary>
        /// 抓取物体释放
        /// </summary>
        Release,
        /// <summary>
        /// 超出范围
        /// </summary>
        Invalid,
        /// <summary>
        /// 错误
        /// </summary>
        Error
    }
}
