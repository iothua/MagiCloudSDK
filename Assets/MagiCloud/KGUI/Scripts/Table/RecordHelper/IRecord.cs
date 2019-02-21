namespace MagiCloud.KGUI
{
    /// <summary>
    /// 记录者接口
    /// </summary>
    public interface IRecord
    {
        /// <summary>
        /// 是否可以记录
        /// </summary>
        bool CanReocrd { get; }
        /// <summary>
        /// 是否覆盖
        /// </summary>
        bool IsCover { get; }
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <returns></returns>
        string[] GetData();
        /// <summary>
        /// 是否列向添加
        /// </summary>
        bool IsColumn { get; }
    }
}