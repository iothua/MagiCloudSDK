using System.Threading;

namespace MCServer
{
    public class NetWork
    {
        public SettingReq setting;

        public NetWork()
        {
            InitSetting();
            MessageDistribution.AddListener((int)EnumCmdID.SettingReq,SystemSettingCallback);
        }

        public void Init(string ip,int port)
        {
            Server.Instance.Start(ip,port);
        }

        public void Update()
        {
            MessageDistribution.Update();
            Thread.Sleep(1);
            Update();
        }

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


        private void SystemSettingCallback(ProtobufTool data)
        {
            data.DeSerialize(setting,data.bytes);
            ProtobufTool tool = new ProtobufTool();
            tool.CreatData((int)EnumCmdID.SettingReq,setting);
            Server.Instance.Broadcast(tool);
        }
    }

}
