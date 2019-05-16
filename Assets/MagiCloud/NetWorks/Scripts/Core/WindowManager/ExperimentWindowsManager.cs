using System;
using System.Diagnostics;

namespace MagiCloud.NetWorks
{

    /// <summary>
    /// 实验窗口程序管理,当启动不同项目时,会关闭当前窗口,开启新的项目窗口
    /// </summary>
    public class ExperimentWindowsManager : WindowsManager
    {
        private ProcessHelper processHelper;      //进程辅助工具
        private IntPtr curWindowIntPtr;                     //自身窗口
        private IntPtr curID;
        private IntPtr foreID;
        public ExperimentWindowsManager() : base()
        {
            processHelper = new ProcessHelper();

#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
            curWindowIntPtr = SystemDllHelper.GetActiveWindow();
            curID = SystemDllHelper.GetCurrentThreadId();
            foreID = SystemDllHelper.GetWindowThreadProcessId(curWindowIntPtr, default(IntPtr));
#endif

        }

        public override void SetTop()
        {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
            SystemDllHelper.AttachThreadInput(curID, foreID, 1);
            SystemDllHelper.ShowWindow(curWindowIntPtr, 3);
            SystemDllHelper.SetWindowPos(curWindowIntPtr, -1, 0, 0, 0, 0, 1 | 2);
            SystemDllHelper.SetWindowPos(curWindowIntPtr, -2, 0, 0, 0, 0, 1 | 2);
            bool max = SystemDllHelper.SetForegroundWindow(curWindowIntPtr);
            SystemDllHelper.AttachThreadInput(curID, foreID, 0);
#endif
        }

        public override void OpenExe()
        {
            processHelper.OpenExe(processPath);
            processHelper.p.StartInfo.UseShellExecute = true;
            processHelper.p.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
        }
        public override bool IsTopping {
            get {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
                return curWindowIntPtr == SystemDllHelper.GetForegroundWindow();
#endif
                return false;

            }
        }

        public override bool IsOpening {
            get {
                return processHelper.p != null && !processHelper.p.HasExited;
            }
        }
        public override void ExitExe(Action exitAction = null)
        {
            processHelper.Exit();
        }
    }
}
