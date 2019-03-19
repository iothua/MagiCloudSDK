using System;
using System.Runtime.InteropServices;

namespace MagiCloud.NetWorks
{
    public static class SystemDllHelper
    {
        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr intPtr, int show);
        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")]
        public static extern void SwitchToThisWindow(IntPtr intPtr, bool altTab);
    }
}
