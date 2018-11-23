namespace MagiCloud.Features
{
    public enum ProcessType
    {
        LateUpdate,     //每帧执行
        Grab,           //抓取时执行
        Release         //释放时执行
    }
}
