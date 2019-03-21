using System;
using System.Runtime.InteropServices;

namespace MagiCloud.NetWorks
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
        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr intPtr,int show);
        /// <summary>
        /// 获得当前置顶窗口
        /// </summary>
        /// <returns></returns>
        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();
        /// <summary>
        /// 切换窗口
        /// </summary>
        /// <param name="intPtr"></param>
        /// <param name="altTab"></param>
        [DllImport("user32.dll")]
        public static extern void SwitchToThisWindow(IntPtr intPtr,bool altTab);
        /// <summary>
        /// 窗口置顶
        /// </summary>
        /// <param name="hWnd"></param>
        /// <returns></returns>
        [DllImport("USER32.DLL")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);
    }
}
