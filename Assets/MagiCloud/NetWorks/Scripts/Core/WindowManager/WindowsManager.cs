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
        public WindowsManager()
        {
            expInfoManager=new ExperimentInfoManager();
            projectPaths =new Dictionary<string,string>();
            string json = ReadJson();
            projectPaths= JsonHelper.JsonToObject<Dictionary<string,string>>(json);
        }

        protected ExperimentInfoManager expInfoManager;
        protected Dictionary<string,string> projectPaths;         //项目路径
        protected virtual string Path => "/ProjectPaths.json";
        public string Url => Application.streamingAssetsPath+ Path;
        public ExperimentInfo CurExpInfo { get; protected set; }

        /// <summary>
        /// 选择实验
        /// </summary>
        /// <param name="i"></param>
        /// <param name="exitAction"></param>
        public virtual void Select(int i,Action exitAction = null)
        {
            ExperimentInfo info = expInfoManager.GetInfo(i);
            if (info==null)
                throw new Exception("数据不存在");
            if ((CurExpInfo==null||info.OwnProject!=CurExpInfo.OwnProject)&&projectPaths.ContainsKey(info.OwnProject))
            {
                ExitOther(exitAction);
                OpenOther(info);
            }
            CurExpInfo =info;
        }

        public virtual bool IsTopping()
        {
            return false;
        }
        /// <summary>
        ///开始实验程序
        /// </summary>
        /// <param name="info"></param>
        protected abstract void OpenOther(ExperimentInfo info);
        /// <summary>
        /// 退出实验程序
        /// </summary>
        /// <param name="exitAction"></param>
        public virtual void ExitOther(Action exitAction = null)
        {
            if (CurExpInfo!=null&&!string.IsNullOrEmpty(CurExpInfo.OwnProject)&&exitAction!=null)
            {
                exitAction();
            }
        }

        public virtual void SetTop()
        {

        }
        protected virtual string ReadJson() { return JsonHelper.ReadJsonString(Url); }
    }
}
