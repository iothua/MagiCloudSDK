namespace MagiCloud.NetWorks.Client
{
    public class MessageEvent
    {
        public WindowWakeupEvent wakeupEvent;
        public ExperimentEvent experimentEvent;
        public SettingEvent settingEvent;
        public MessageEvent()
        {
            settingEvent=new SettingEvent();
            wakeupEvent = new WindowWakeupEvent();
            experimentEvent=new ExperimentEvent();
        }
    }
}

