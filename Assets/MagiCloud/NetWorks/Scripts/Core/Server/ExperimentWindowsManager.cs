using System;
using System.Collections.Generic;
using System.Diagnostics;
using MagiCloud.Json;
using UnityEngine;

namespace MagiCloud.NetWorks
{
    /// <summary>
    /// 实验窗口程序管理,当启动不同项目时,会关闭当前窗口,开启新的项目窗口
    /// </summary>
    public class ExperimentWindowsManager
    {
        private ExperimentInfoManager expInfoManager = new ExperimentInfoManager();
        private ProcessHelper processHelper = new ProcessHelper();      //进程辅助工具
        private Dictionary<string,string> projectPaths;         //项目路径
        public ExperimentInfo CurExpInfo { get; private set; }                          //当前项目名称

        public ExperimentWindowsManager()
        {
            projectPaths=new Dictionary<string,string>();
            string json = JsonHelper.ReadJsonString(Application.streamingAssetsPath+ "/ProjectPaths.json");
            projectPaths= JsonHelper.JsonToObject<Dictionary<string,string>>(json);
        }

        /// <summary>
        /// 选择实验
        /// </summary>
        /// <param name="i"></param>
        public void Select(int i)
        {
            ExperimentInfo info = expInfoManager.GetInfo(i);
            if (info==null)
                throw new Exception("数据不存在");
            if ((CurExpInfo==null||info.OwnProject!=CurExpInfo.OwnProject)&&projectPaths.ContainsKey(info.OwnProject))
            {
                processHelper.Exit();
                processHelper.OpenExe(projectPaths[info.OwnProject]);
                processHelper.p.StartInfo.UseShellExecute=true;
                processHelper.p.StartInfo.WindowStyle=ProcessWindowStyle.Minimized;
            }
            CurExpInfo=info;
        }


        public void Exit()
        {
            processHelper.Exit();
        }
    }
}
