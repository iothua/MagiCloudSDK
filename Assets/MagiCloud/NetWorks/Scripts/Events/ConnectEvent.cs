namespace MagiCloud.NetWorks
{
    /// <summary>
    /// 连接事件，收到客户端于服务器连接的信息
    /// </summary>
    public class ConnectEvent :EventBase
    {
        public ConnectEvent()
        {
            Command=CommandID.Connect;
            ConnectInfo connect = new ConnectInfo() { };
            Proto=connect;
        }
    }

    /// <summary>
    /// 断开连接事件
    /// </summary>
    public class BreakConnectEvent :EventBase
    {
        public BreakConnectEvent()
        {
            Command=CommandID.BreakConnection;
            ConnectInfo connect = new ConnectInfo() { };
            Proto=connect;
        }
    }


    /// <summary>
    /// 断开连接事件
    /// </summary>
    public class WindowWakeUpRequestEvent :EventBase
    {
        public WindowWakeUpRequestEvent()
        {
            Command=CommandID.WindowWakeUpRequest;
            Proto=new ConnectInfo();
        }
    }

    /// <summary>
    /// 实验打开请求事件
    /// </summary>
    public class ExperimentRequestEvent :EventBase
    {
        public ExperimentRequestEvent()
        {
            Command=CommandID.ExperimentInfoRequest;
            Proto=new ConnectInfo();
        }
    }
    /// <summary>
    /// 实验打开回执事件
    /// </summary>
    public class ExperimentReceiptEvent :EventBase
    {
        public ExperimentReceiptEvent()
        {
            Command=CommandID.ExperimentInfoReceipt;
            Proto=new ConnectInfo();
        }
    }

    public class TTT
    {
        public void T()
        {

        }
    }
}
