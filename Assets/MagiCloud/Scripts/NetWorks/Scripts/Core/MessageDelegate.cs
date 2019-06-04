namespace MagiCloud.NetWorks
{
    /// <summary>
    /// 收到消息
    /// </summary>
    /// <param name="connectID">连接器，来源</param>
    /// <param name="data">收到的协议数据</param>
    public delegate void MessageDelegate(int connectID,ProtobufTool data);
}