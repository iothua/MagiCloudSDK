using System;
using System.Diagnostics;
using System.IO;

namespace MCServer
{
    /// <summary>
    /// 窗口唤醒
    /// </summary>
    public class WindowWakeupEvent
    {
        Process p;
        public WindowReq windowReq;
        WindowRes windowRes;



        public WindowWakeupEvent()
        {

            InitWakeupReq();
            InitWakeupRes();
        }
        /// <summary>
        /// 初始化请求
        /// </summary>
        private void InitWakeupReq()
        {
            string path = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.Parent.FullName+@"\Build\Client\UnityClient.exe";
            Console.WriteLine(path);
            windowReq=new WindowReq() 
            {
                Path=path
            };

            MessageDistribution.AddListener((int)EnumCmdID.WindowwakeupReq,WindowWakeupCallback);
        }
        /// <summary>
        /// 初始化回执
        /// </summary>
        private void InitWakeupRes()
        {
            windowRes=new WindowRes()
            {
                Status=WindowStatus.None
            };
            MessageDistribution.AddListener((int)EnumCmdID.WindowwakeupRes,WakeupResCallback);
        }

        private void WakeupResCallback(int connectID,ProtobufTool data)
        {
            data.DeSerialize(windowRes,data.bytes);
            ProtobufTool protobuf = new ProtobufTool();
            protobuf.CreatData((int)EnumCmdID.WindowwakeupRes,windowRes);
            Server.Instance.Broadcast(protobuf);
            //switch (windowRes.Status)
            //{
            //    case WindowStatus.None:
            //        break;
            //    case WindowStatus.Restore:
            //        break;
            //    case WindowStatus.Min:
            //        break;
            //    case WindowStatus.Max:
            //        break;
            //    case WindowStatus.Exit:
            //        if (p!=null&&!p.HasExited)
            //        { p.Kill(); }

            //        break;
            //    default:
            //        break;
            //}
        }

        private void WindowWakeupCallback(int connectID,ProtobufTool data)
        {
            //接收到窗口唤醒请求
            data.DeSerialize(windowReq,data.bytes);
            ProtobufTool protobuf = new ProtobufTool();
            protobuf.CreatData((int)EnumCmdID.WindowwakeupRes,windowReq);
            Server.Instance.Broadcast(protobuf);
            ////启动外部程序
            //if (p==null)
            //{
            //    p =new Process
            //    {
            //        StartInfo=new ProcessStartInfo
            //        {
            //            WindowStyle=ProcessWindowStyle.Maximized,
            //            Arguments="2",
            //        }
            //    };
            //    if (!string.IsNullOrEmpty(windowReq.Path))
            //    {
            //        p.StartInfo.FileName=windowReq.Path;
            //    }
            //    p.Start();
            //}
            //else
            //{
            //    if (p.HasExited)
            //    {
            //        p.Start();
            //    }
            //}
            //p.StartInfo.WindowStyle=ProcessWindowStyle.Maximized;
            //p.EnableRaisingEvents=true;
            //p.Exited+=ExitedEvent;
        }

        private void ExitedEvent(object sender,EventArgs e)
        {
            windowRes.Status=WindowStatus.Max;
            ProtobufTool protobuf = new ProtobufTool();
            protobuf.CreatData((int)EnumCmdID.WindowwakeupRes,windowRes);
            Server.Instance.Broadcast(protobuf);
        }
    }
}
