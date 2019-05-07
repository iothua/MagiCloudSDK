using System;
using System.Runtime.InteropServices;
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN

namespace MagiCloud
{

    /// <summary>
    /// 系统窗口相关API
    /// </summary>
    public static class SystemDllHelper
    {
        

        /// <summary>
        /// 显示窗口
        /// </summary>
        /// <param name="intPtr"></param>
        /// <param name="show"></param>
        /// <returns></returns>
        [DllImport("user32.dll",CharSet = CharSet.Auto)]
        public static extern bool ShowWindow(IntPtr intPtr,int show);
        /// <summary>
        /// 获得当前置顶窗口
        /// </summary>
        /// <returns></returns>
        [DllImport("user32.dll",CharSet = CharSet.Auto)]
        public static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll",CharSet = CharSet.Auto)]
        public static extern IntPtr GetActiveWindow();
        [DllImport("user32.dll")]
        public static extern IntPtr AttachThreadInput(IntPtr idAttach,IntPtr idAttachTo,int fAttach);
        /// <summary>
        /// 获得窗口所属进程ID和线程ID
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="ProcessId"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowThreadProcessId(IntPtr hWnd,IntPtr ProcessId);
        /// <summary>
        /// 获取当前线程一个唯一的线程标识
        /// </summary>
        /// <returns></returns>
        [DllImport("kernel32")]
        public static extern IntPtr GetCurrentThreadId();
        /// <summary>
        /// 切换窗口
        /// </summary>
        /// <param name="intPtr"></param>
        /// <param name="altTab"></param>
        [DllImport("user32.dll",CharSet = CharSet.Auto)]
        public static extern void SwitchToThisWindow(IntPtr intPtr,bool altTab);
        /// <summary>
        /// 窗口置顶
        /// </summary>
        /// <param name="hWnd"></param>
        /// <returns></returns>
        [DllImport("user32.dll",CharSet = CharSet.Auto)]
        public static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll",CharSet = CharSet.Auto)]
        public static extern int SetWindowPos(IntPtr hWnd,int hWndInsertAfter,int x,int y,int Width,int Height,int flags);


        [DllImport("user32.dll")] //引入dll
        public static extern int SetCursorPos(int x, int y);

    }
}
#endif

