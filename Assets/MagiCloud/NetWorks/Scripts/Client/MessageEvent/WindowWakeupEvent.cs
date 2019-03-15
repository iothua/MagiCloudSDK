using System;
using UnityEngine;
namespace MagiCloud.NetWorks.Client
{
    public class WindowWakeupEvent
    {
        MessageDistribution messageDistribution;
        IntPtr ptr;
        public WindowWakeupEvent(MessageDistribution messageDistribution)
        {
            ptr = SystemDllHelper.GetForegroundWindow();
            this.messageDistribution=messageDistribution;
            this.messageDistribution.AddListener((int)EnumCmdID.WindowwakeupReq,WindowWakeupCallback);
        }
        private void WindowWakeupCallback(ProtobufTool data)
        {
            //Debug.Log("收到唤醒信息，正在启动");
            SystemDllHelper.ShowWindow(ptr,3);
            SendWakeupRes();
        }

        /// <summary>
        /// 发送唤醒回执
        /// </summary>
        public void SendWakeupRes()
        {
            messageDistribution.AddListener((int)EnumCmdID.WindowwakeupRes,WindowWakeupCallback);
        }




        public void SetMinWindow()
        {
            SystemDllHelper.ShowWindow(ptr,2);
        }
    }
}

