using System.Net.Sockets;

namespace MagiCloud.NetWorks
{
    /// <summary>
    /// 消息分发接口
    /// </summary>
    public interface IMessageDistribution
    {
        void AddListener(int type,MessageDelegate message);
        void RemoveListener(int type,MessageDelegate message);
        void Update();
        void OnMessageEvent(int senderID,ProtobufTool protobuf);
    }
}
