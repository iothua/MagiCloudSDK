using System;
using System.Threading;

namespace MCServer
{
    public sealed class NetWork 
    {
        private AutoResetEvent exitEvent;
        private readonly int waitTime = 1;
        Thread thread;
        public NetWork(int time = 1)
        {
            exitEvent=new AutoResetEvent(false);
            waitTime=time;
        }

        public void Init(string ip,int port)
        {
            Server.Instance.Start(ip,port);
            thread  = new Thread(() => Update());
            thread.Start();
        }

        public void Update()
        {
            while (true)
            {
                MessageDistribution.Update();
                if (exitEvent.WaitOne(waitTime))
                {
                    break;
                }
            }
        }

        public void Stop()
        {
            exitEvent.Set();
            thread.Join();
            Server.Instance.Close();
        }

    }

}
