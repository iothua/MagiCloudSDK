using System;
using System.Runtime.InteropServices;

namespace MCServer
{
    internal static class NativeMethods
    {
        [DllImport("user32.dll")]
        internal static extern bool ShowWindow(IntPtr intPtr,int show);
        [DllImport("user32.dll")]
        internal static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")]
        internal static extern void SwitchToThisWindow(IntPtr intPtr,bool altTab);
    }
}
