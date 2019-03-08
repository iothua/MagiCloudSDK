using System;
using System.Threading;

namespace MCServer
{
    class Program
    {
        static void Main(string[] args)
        {
            NetWork netWork = new NetWork();
            netWork.Init("127.0.0.1",8888);// 192.168.1.24
            Thread thread = new Thread(() => netWork.Update());
            thread.Start();
            //WaitForInput(netWork);
            Console.ReadLine();

        }

        private static void WaitForInput(NetWork netWork)
        {
            SettingReq setting = netWork.setting;
            if (setting==null)
                netWork.InitSetting();
            while (true)
            {
                string input = Console.ReadLine();
                bool low = input.Equals("LOW",StringComparison.OrdinalIgnoreCase);
                bool middle = input.Equals("MIDDLE",StringComparison.OrdinalIgnoreCase);
                bool hight = input.Equals("HIGHT",StringComparison.OrdinalIgnoreCase);
                if (low||middle||hight)
                {
                    Connect connect = Server.Instance.connects[0];
                    if (connect!=null&&connect.isUse)
                    {
                        if (low)
                            setting.Info.Type= SystemSettingInfo.Types.Performance.Low;
                        else if (middle)
                            setting.Info.Type= SystemSettingInfo.Types.Performance.Middle;
                        else if (hight)
                            setting.Info.Type= SystemSettingInfo.Types.Performance.Hight;

                        ProtobufTool protobuf = new ProtobufTool();
                        protobuf.CreatData((int)EnumCmdID.SettingReq,setting);
                        Server.Instance.BeginSendMessage(connect,protobuf);
                    }
                }
            }

        }

    }

}
