namespace MagiCloud.KGUI
{
    /// <summary>
    /// 记录者接口
    /// </summary>
    public interface IRecord
    {
        bool CanReocrd { get; }
        bool IsCover { get; }
        string[] GetData();
    }
}