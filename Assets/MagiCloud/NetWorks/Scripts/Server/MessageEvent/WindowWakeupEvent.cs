using System;
using System.Diagnostics;
using UnityEngine;

namespace MagiCloud.NetWorks.Server
{
    /// <summary>
    /// 窗口唤醒
    ///     当点击开启实验按钮时，唤醒实验窗口，接收到回执后最小化，到接收到实验退出时，恢复窗口
    /// 
    /// </summary>
    public class WindowWakeupEvent
    {
        // Process p;
        ProcessHelper processHelper;
        IntPtr ptr;

        public WindowReq windowReq;
        public WindowRes windowRes;
        private MessageDistribution messageDistribution;
        public WindowWakeupEvent(MessageDistribution messageDistribution)
        {
            this.messageDistribution=messageDistribution;
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
            messageDistribution.AddListener((int)EnumCmdID.WindowwakeupRes,ResCallback);
        }

        #region 从服务端唤醒外部程序
        /// <summary>
        /// 唤醒程序窗口
        /// </summary>
        /// <param name="path"></param>
        public void SendWakeup(string path = "")
        {
            //   ptr=SystemDllHelper.GetForegroundWindow();
            //最小化自身窗口
            // SystemDllHelper.ShowWindow(ptr,2);
            windowReq.Path=path;
            ServerNetManager.connetion.BeginSendMessages(GetWakeupProtocol());
        }

        public ProtobufTool GetWakeupProtocol()
        {
            ProtobufTool tool = new ProtobufTool();
            byte[] data = tool.CreatData((int)EnumCmdID.WindowwakeupReq,windowReq);
            return tool;
        }

        /// <summary>
        /// 收到窗口唤醒回执后，最小化窗口
        /// </summary>
        /// <param name="data"></param>
        private void ResCallback(ProtobufTool data)
        {
            data.DeSerialize(windowRes,data.bytes);
            SystemDllHelper.ShowWindow(ptr,2);
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