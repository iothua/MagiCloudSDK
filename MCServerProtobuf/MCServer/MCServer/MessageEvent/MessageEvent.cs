using System;

namespace MCServer
{
    /// <summary>
    /// 消息事件管理
    /// </summary>
    public class MessageEvent
    {
        public WindowWakeupEvent wakeupEvent;
        public SettingEvent settingEvent;
        public ExperimentEvent experimentEvent;
        public MessageEvent()
        {
            settingEvent=new SettingEvent();
            wakeupEvent =new WindowWakeupEvent();
            experimentEvent=new ExperimentEvent();
        }
    }
}
