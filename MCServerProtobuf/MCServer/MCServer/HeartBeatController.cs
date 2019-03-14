using System;
using System.Timers;

namespace MCServer
{

    public sealed class HeartBeatController 
    {
        Timer timer = new Timer(1000);
        public long heartBeatTime = 20;
        Server server;
        public HeartBeatController(Server server,long heartBeatTime = 10)
        {
            timer = new Timer(1000);
            this.server=server;
            this.heartBeatTime=heartBeatTime;
            timer.Elapsed+=new ElapsedEventHandler(HandleMainTimer);
            timer.AutoReset=false;
            timer.Enabled=true;
            //   Console.WriteLine("开启心跳包检查");
        }

        private void HandleMainTimer(object sender,ElapsedEventArgs e)
        {
            HeartBeat();
            timer.Start();
        }

        private void HeartBeat()
        {
            long timeNow = TimeHelper.GetTimeStamp();
            for (int i = 0; i < server.connects.Length; i++)
            {
                Connect connect = server.connects[i];
                if (connect==null) continue;
                if (!connect.isUse) continue;
                if (connect.lastTickTime<timeNow-heartBeatTime)
                {
                    lock (connect)
                    {
                        Console.WriteLine("未接收到心跳包,关闭连接");
                        connect.Close();
                    }
                }
            }
        }

       
    }
}
