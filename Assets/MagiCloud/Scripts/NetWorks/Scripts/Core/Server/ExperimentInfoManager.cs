using MagiCloud.Json;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MagiCloud.NetWorks
{
    /// <summary>
    /// 实验信息数据管理
    /// </summary>
    public class ExperimentInfoManager
    {
        /// <summary>
        /// 缓存实验信息数据
        /// </summary>
        private readonly Dictionary<int,ExperimentInfo> expInfos = new Dictionary<int,ExperimentInfo>();

        public ExperimentInfoManager()
        {
            string json = JsonHelper.ReadJsonString(Application.streamingAssetsPath+ "/ExpInfo.json");
            expInfos= JsonHelper.JsonToObject<Dictionary<int,ExperimentInfo>>(json);
        }

        public ExperimentInfo[] GetAllInfos()
        {
            return expInfos.Values.ToArray();
        }

        /// <summary>
        /// 添加实验信息
        /// </summary>
        /// <param name="info"></param>
        public void AddInfo(ExperimentInfo info)
        {
            int key = info.Id;
            if (!expInfos.ContainsKey(key))
            {
                expInfos.Add(key,info);
            }
        }
        /// <summary>
        /// 移除实验信息
        /// </summary>
        /// <param name="info"></param>
        public void RemoveInfo(ExperimentInfo info)
        {
            int key = info.Id;
            if (expInfos.ContainsKey(key)&&expInfos[key]==info)
            {
                expInfos.Remove(key);
            }
        }

        /// <summary>
        /// 是否存在实验数据信息
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public bool HasInfo(ExperimentInfo info)
        {
            if (expInfos.ContainsKey(info.Id)&&expInfos[info.Id]==info)
                return true;
            return false;
        }

        /// <summary>
        /// 获得实验信息
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public ExperimentInfo GetInfo(int i)
        {
            if (expInfos.ContainsKey(i))
                return expInfos[i];
            return null;
        }

        public void Clear()
        {
            expInfos.Clear();
        }
    }
}
