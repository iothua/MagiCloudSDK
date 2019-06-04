namespace MagiCloud.NetWorks
{
    /// <summary>
    /// 连接器接口
    /// </summary>
    public interface IConnect
    {
        void Send(ProtobufTool protobuf);
    }
}
