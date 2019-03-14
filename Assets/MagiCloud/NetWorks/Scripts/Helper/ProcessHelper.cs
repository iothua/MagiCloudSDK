using System;
using System.Diagnostics;

namespace MagiCloud.NetWorks
{
    /// <summary>
    /// 进程辅助脚本
    /// </summary>
    public class ProcessHelper
    {
        public Process p;
        public ProcessHelper() { }
        /// <summary>
        /// 启动客户端进程
        /// </summary>
        /// <param name="path"></param>
        /// <param name="action"></param>
        public void OpenExe(string path,bool hidden = false,EventHandler onExited = null)
        {
            if (p==null)
            {
                p=new Process();
                p.StartInfo.FileName=path;
                if (hidden)
                {
                    p.StartInfo.UseShellExecute=true;
                    // p.StartInfo.CreateNoWindow=true;
                    p.StartInfo.WindowStyle=ProcessWindowStyle.Hidden;
                }
                try
                {
                    p.Start();
                }
                catch (Exception e)
                {
                    
                }
            }
            else
            {
                if (p.HasExited)
                {
                    p.Start();
                }
            }
            p.EnableRaisingEvents=true;
            if (onExited!=null)
                p.Exited+=onExited;
        }

        public void Exit()
        {
            //关闭客户端进程
            if (p!=null&&!p.HasExited)
                p.Kill();
        }
    }
}