using MagiCloud.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MagiCloud.NetWorks
{
    /// <summary>
    /// 实验窗口程序管理
    /// </summary>
    public abstract class WindowsManager
    {
        public string processPath;

        public virtual bool IsOpening { get; protected set; }

        public virtual bool IsTopping { get; protected set; }
        /// <summary>
        ///开始实验程序
        /// </summary>
        /// <param name="info"></param>
        public abstract void OpenExe();
        /// <summary>
        /// 退出实验程序
        /// </summary>
        /// <param name="exitAction"></param>
        public virtual void ExitExe(Action exitAction = null)
        {
        }

        public virtual void SetTop()
        {

        }
    }
}
