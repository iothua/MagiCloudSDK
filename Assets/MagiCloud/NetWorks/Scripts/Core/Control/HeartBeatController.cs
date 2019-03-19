using System;
using System.Timers;

namespace MagiCloud.NetWorks
{
    /// <summary>
    /// 心跳包控制端
    /// </summary>
    public sealed class HeartBeatController
    {
        Timer timer = new Timer(1000);
        public long heartBeatTime = 20;
        ControlTerminal controlTerminal;

        public HeartBeatController(ControlTerminal server, long heartBeatTime = 10)
        {
            timer = new Timer(1000);
            this.controlTerminal = server;
            this.heartBeatTime = heartBeatTime;
            timer.Elapsed += new ElapsedEventHandler(HandleMainTimer);
            timer.AutoReset = false;
            timer.Enabled = true;
            //   Console.WriteLine("开启心跳包检查");
        }

        private void HandleMainTimer(object sender, ElapsedEventArgs e)
        {
            HeartBeat();
            timer.Start();
        }

        private void HeartBeat()
        {
            long timeNow = TimeHelper.GetTimeStamp();
            for (int i = 0; i < controlTerminal.connects.Length; i++)
            {
                ConnectControl connect = controlTerminal.connects[i];
                if (connect == null) continue;
                if (!connect.isUse) continue;
                if (connect.lastTickTime < timeNow - heartBeatTime)
                {
                    lock (connect)
                    {
                        connect.Close();
                    }
                }
            }
        }


    }
}
