using System;
using System.Diagnostics;
using UnityEngine;

namespace MagiCloud.NetWorks.Server
{

    public class WindowWakeupEvent
    {
        // Process p;
        ProcessHelper processHelper;
        IntPtr ptr;

        public WindowReq windowReq;
        public WindowRes windowRes;

        public WindowWakeupEvent(MessageDistribution messageDistribution)
        {
            InitWindowReq();
            InitWindowRes();
            processHelper = new ProcessHelper();
            ptr=SystemDllHelper.GetForegroundWindow();
        }
        #region WindowWakeup

        /// <summary>
        /// 初始化
        /// </summary>
        private void InitWindowReq()
        {
            windowReq=new WindowReq()
            {
                Path=AppDomain.CurrentDomain.BaseDirectory,
            };
        }

        private void InitWindowRes()
        {
            windowRes=new WindowRes()
            {
                Status=WindowStatus.None
            };
        }

        #region 从服务端唤醒外部程序
        /// <summary>
        /// 唤醒程序窗口
        /// </summary>
        /// <param name="path"></param>
        public void SendWakeup(string path = "")
        {
            ptr=SystemDllHelper.GetForegroundWindow();
            //最小化自身窗口
           // SystemDllHelper.ShowWindow(ptr,2);
            windowReq.Path=path;
            NetManager.connetion.BeginSendMessages(GetWakeupProtocol());
        }
       
        public ProtobufTool GetWakeupProtocol()
        {
            ProtobufTool tool = new ProtobufTool();
            byte[] data = tool.CreatData((int)EnumCmdID.WindowwakeupReq,windowReq);
            return tool;
        }


        private void ResCallback(ProtobufTool data)
        {
            data.DeSerialize(windowRes,data.bytes);
            SystemDllHelper.ShowWindow(ptr,3);
        }
        #endregion

        #region 从本地唤醒外部程序

        /// <summary>
        /// 最小化
        /// </summary>
        public void SetMin()
        {
            if (ptr!=null)
                SystemDllHelper.ShowWindow(ptr,2);
            if (processHelper.p!=null)
                processHelper.p.StartInfo.WindowStyle=ProcessWindowStyle.Maximized;
        }

        /// <summary>
        /// 最大化
        /// </summary>
        public void SetMax()
        {
            if (ptr!=null)
                SystemDllHelper.ShowWindow(ptr,3);
            if (processHelper.p!=null)
                processHelper.p.StartInfo.WindowStyle=ProcessWindowStyle.Minimized;
        }

        /// <summary>
        /// 启动客户端进程
        /// </summary>
        /// <param name="path"></param>
        /// <param name="action"></param>
        public void OpenExe(string path,Action action = null)
        {
            processHelper.OpenExe(path,false,ExitEvent);
            if (action!=null)
                action.Invoke();
        }

        /// <summary>
        /// 客户端进程退出回调
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExitEvent(object sender,EventArgs e)
        {
            SystemDllHelper.ShowWindow(ptr,3);
        }

        public void Exit()
        {
            processHelper.Exit();
        }

        #endregion
        #endregion
    }
}