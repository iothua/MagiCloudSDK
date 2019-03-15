namespace MagiCloud.NetWorks.Client
{
    public class MessageEvent
    {
        public WindowWakeupEvent wakeupEvent;
        public ExperimentEvent experimentEvent;
        public SettingEvent settingEvent;
        public MessageEvent(MessageDistribution messageDistribution)
        {
            messageDistribution=new MessageDistribution();
            settingEvent =new SettingEvent(messageDistribution);
            wakeupEvent = new WindowWakeupEvent(messageDistribution);
            experimentEvent=new ExperimentEvent(messageDistribution);
        }
    }
}

