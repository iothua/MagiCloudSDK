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
        /// 按下UI
        /// </summary>
        Pressed,
        /// <summary>
        /// 抓取
        /// </summary>
        Grab,
        /// <summary>
        /// 抓取中……
        /// </summary>
        Grabing,
        /// <summary>
        /// 旋转
        /// </summary>
        Rotate,
        /// <summary>
        /// 缩放
        /// </summary>
        Zoom,
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
