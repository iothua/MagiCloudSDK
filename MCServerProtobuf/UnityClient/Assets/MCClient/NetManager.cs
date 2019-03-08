using MCServer;

namespace MagiCloudServer
{
    public class NetManager
    {
        public static MCConnetion connetion = new MCConnetion();
        public static HeartBeatInfo hearInfo;
        public static void Update()
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

        public static ProtobufTool GetSystemSettingProtocol()
        {
            ProtobufTool protocol = new ProtobufTool();
         //   protocol.CreatData((int)EnumCmdID.SettingRes,null);
            return protocol;
        }
    }
}