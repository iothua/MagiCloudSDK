namespace MCServer
{
    public class SettingEvent
    {
        public SettingReq setting;
        public SettingEvent()
        {
            InitSetting();
            MessageDistribution.AddListener((int)EnumCmdID.SettingReq,SystemSettingCallback);

        }
        #region Setting

        public void InitSetting()
        {
            setting=new SettingReq()
            {
                Info=new SystemSettingInfo()
                {
                    Type= SystemSettingInfo.Types.Performance.Middle,
                    Volume=10,
                },
            };
        }


        private void SystemSettingCallback(int connectID,ProtobufTool data)
        {
            data.DeSerialize(setting,data.bytes);
            ProtobufTool tool = new ProtobufTool();
            tool.CreatData((int)EnumCmdID.SettingReq,setting);
            Server.Instance.Broadcast(tool);
        }
        #endregion
    }
}
