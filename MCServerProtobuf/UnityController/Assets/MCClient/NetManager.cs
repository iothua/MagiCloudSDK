using MCServer;
using UnityEngine;

namespace MagiCloudServer
{
    public class NetManager
    {
        public MCConnetion connetion = new MCConnetion();
        public static HeartBeatInfo hearInfo;
        public SettingReq setting;
        #region Init

        public NetManager()
        {
            InitSetting();
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

        #endregion
        public void Update()
        {
            connetion.Update();
        }


        public static ProtobufTool GetHeatBeatProtocol()
        {
            ProtobufTool protocol = new ProtobufTool();
            hearInfo=new HeartBeatInfo()
            {
                Curtime=0,
                Hostip="127.0.0.1",
            };
            protocol.CreatData((int)EnumCmdID.Heartbeat,hearInfo);
            return protocol;
        }
        #region Setting
        public void SendQulitySet(SystemSettingInfo.Types.Performance type)
        {
            setting.Info.Type= type;
            connetion.BeginSendMessages(GetSystemSetting());
        }
        public ProtobufTool GetSystemSetting()
        {
            ProtobufTool protocol = new ProtobufTool();
            protocol.CreatData((int)EnumCmdID.SettingReq,setting);
            return protocol;
        }

        public void SendVolumeSet(float v)
        {
            setting.Info.Volume= (int)(v*100);
            connetion.BeginSendMessages(GetSystemSetting());
        }

        #endregion
    }
}