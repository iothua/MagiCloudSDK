namespace MagiCloud.UIFrame
{
    /// <summary>
    /// UI类型
    /// </summary>
    public enum UIType
    {
        /// <summary>
        /// 2D
        /// </summary>
        SpriteRender,
        /// <summary>
        /// 画布
        /// </summary>
        Canvas
    }

    /// <summary>
    /// 等级
    /// </summary>
    public enum Level
    {
        /// <summary>
        /// 无
        /// </summary>
        None,
        /// <summary>
        /// 低
        /// </summary>
        Low,
        /// <summary>
        /// 中
        /// </summary>
        Mid,
        /// <summary>
        /// 高
        /// </summary>
        High
    }

    /// <summary>
    /// 动作类型
    /// </summary>
    public enum ActionType
    {
        /// <summary>
        /// 默认，不处理
        /// </summary>
        None,
        /// <summary>
        /// 直接隐藏
        /// </summary>
        Hide,
        /// <summary>
        /// 渐变
        /// </summary>
        Gradient,
        /// <summary>
        /// 离开
        /// </summary>
        Leave,
        /// <summary>
        /// 最前面
        /// </summary>
        Facede
    }

    /// <summary>
    /// UI操作类型
    /// </summary>
    public enum UIOperate
    {
        Set,
        Loading,
        Register,
        Experiment,
        MainInterface,
        Embed
    }
}
