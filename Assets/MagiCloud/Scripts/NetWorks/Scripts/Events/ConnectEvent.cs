using System;

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
            Proto = new ConnectInfo();
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
            Proto  = new ConnectInfo();
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
            Proto=new WindowWakeup();
        }
    }

    public class WindowWakeUpReceiptEvent :EventBase
    {
        public WindowWakeUpReceiptEvent()
        {
            Command=CommandID.WindowWakeUpReceipt;
            Proto =new WindowWakeup();
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
            Proto=new ExperimentInfo();
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
            Proto=new ExperimentInfo();
        }
    }

    public class SystemSettingRequestEvent : EventBase
    {
        public SystemSettingRequestEvent()
        {
            Command = CommandID.SystemSettingRequest;
            Proto = new SystemSettingInfo();
        }
    }

    public class SystemSettingReceiptEvent : EventBase
    {
        public SystemSettingReceiptEvent()
        {
            Command = CommandID.SystemSettingReceipt;
            Proto = new SystemSettingReceipt();
        }
    }
}
