using System;
using System.Diagnostics;

namespace MagiCloud.NetWorks
{

    /// <summary>
    /// 实验窗口程序管理,当启动不同项目时,会关闭当前窗口,开启新的项目窗口
    /// </summary>
    public class ExperimentWindowsManager :WindowsManager
    {
        private ProcessHelper processHelper;      //进程辅助工具
        private IntPtr curWindowIntPtr;                     //自身窗口
        private IntPtr curID;
        private IntPtr foreID;
        public ExperimentWindowsManager() : base()
        {
            processHelper=new ProcessHelper();
            curWindowIntPtr = SystemDllHelper.GetActiveWindow();
            curID = SystemDllHelper.GetCurrentThreadId();
            foreID = SystemDllHelper.GetWindowThreadProcessId(curWindowIntPtr,default);
        }

        public override void SetTop()
        {
            SystemDllHelper.AttachThreadInput(curID,foreID,1);
            SystemDllHelper.ShowWindow(curWindowIntPtr,3);
            SystemDllHelper.SetWindowPos(curWindowIntPtr,-1,0,0,0,0,1 | 2);
            SystemDllHelper.SetWindowPos(curWindowIntPtr,-2,0,0,0,0,1 | 2);
            bool max = SystemDllHelper.SetForegroundWindow(curWindowIntPtr);
            SystemDllHelper.AttachThreadInput(curID,foreID,0);
        }

        protected override void OpenOther(ExperimentInfo info)
        {
            processHelper.OpenExe(projectPaths[info.OwnProject]);
            processHelper.p.StartInfo.UseShellExecute=true;
            processHelper.p.StartInfo.WindowStyle=ProcessWindowStyle.Minimized;
        }
        public override bool IsTopping()
        {
            return (curWindowIntPtr == SystemDllHelper.GetForegroundWindow());
        }
        public override void ExitOther(Action exitAction = null)
        {
            processHelper.Exit();
        }
    }
}
