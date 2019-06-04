#if UNITY_IOS
using System;
using System.Runtime.InteropServices;
using UnityEngine;
namespace MagiCloud.NetWorks
{

    /// <summary>
    /// 在Ios上的实验窗口管理
    /// </summary>
    public class IosWindowsManager :WindowsManager
    {
        /// <summary>
        /// 与Ios通信的API
        /// </summary>
        public class IosDllHelper
        {
            [DllImport("__Internal")]
            protected internal static extern void ActiveOtherApp(string name);
            /// <summary>
            /// 退出自己
            /// </summary>
            [DllImport("__Internal")]
            protected internal static extern void Exit();
        }

        protected override void Open(ExperimentInfo info)
        {
            IosDllHelper.ActiveOtherApp(info.OwnProject);
        }
    }
}
#endif
