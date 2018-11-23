namespace MagiCloud.KGUI
{
    /// <summary>
    /// 轴
    /// </summary>
    public enum Axis
    {
        X,
        Y,
        Z
    }

    public enum Horizontal
    {
        /// <summary>
        /// 左到右
        /// </summary>
        LeftToRight,
        /// <summary>
        /// 右到左
        /// </summary>
        RightToLeft,
    }

    public enum Vertical
    {
        /// <summary>
        /// 顶到底
        /// </summary>
        TopToBottom = 2,
        /// <summary>
        /// 底到顶
        /// </summary>
        BottomToTop = 3
    }

    /// <summary>
    /// 物体类型（是UI还是Object）
    /// </summary>
    public enum ObjectType
    {
        Object,
        UI
    }

    /// <summary>
    /// 按钮触发方式
    /// </summary>
    public enum TriggerType
    {
        Enter,
        Down,
        Click,
        Drag
    }

}