using System;
using System.Diagnostics;

namespace MagiCloud.NetWorks.Server
{
    /// <summary>
    /// 消息事件
    /// </summary>
    public class MessageEvent
    {
        public static HeartBeatInfo hearInfo;
        public SettingEvent settingEvent;
        public WindowWakeupEvent wakeupEvent;
        public ExperimentEvent experimentEvent;
        public ClientConnectEvent clientConnectEvent;
        public MessageEvent(MessageDistribution messageDistribution)
        {
            messageDistribution=new MessageDistribution();
            settingEvent =new SettingEvent(messageDistribution);

            wakeupEvent =new WindowWakeupEvent(messageDistribution);
            experimentEvent=new ExperimentEvent(this,messageDistribution);
            clientConnectEvent=new ClientConnectEvent(messageDistribution);

        }
    }
}