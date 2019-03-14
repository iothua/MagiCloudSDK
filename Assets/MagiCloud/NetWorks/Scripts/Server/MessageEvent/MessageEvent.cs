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
        public MessageEvent()
        {
            settingEvent=new SettingEvent();

            wakeupEvent =new WindowWakeupEvent();
            experimentEvent=new ExperimentEvent(this);
            clientConnectEvent=new ClientConnectEvent();

        }
    }
}