using System;
using UnityEngine;
using System.Collections.Generic;

namespace MagiCloudPlatform.Data
{

    /// <summary>
    /// 解析Json/Web数据库数据
    /// </summary>
    [Serializable]
    public class ExperimentData
    {
        /// <summary>
        /// 状态，用于从数据库获取时
        /// </summary>
        public string status;

        public List<Experiment> data;
    }

    /// <summary>
    /// 实验
    /// </summary>
    [Serializable]
    public class Experiment
    {
        /// <summary>
        /// ID
        /// </summary>
        public int id;
        /// <summary>
        /// 产品ID
        /// </summary>
        public int productID;
        /// <summary>
        /// 实验名称
        /// </summary>
        public string experimentName;
        /// <summary>
        /// 实验简介
        /// </summary>
        public string introduction;
        /// <summary>
        /// 是否激活
        /// </summary>
        public bool active;
        /// <summary>
        /// 实验图片
        /// </summary>
        public string image1;
        /// <summary>
        /// 实验简介图片
        /// </summary>
        public string image2;
        /// <summary>
        /// 实验预制物体路径
        /// </summary>
        public string sourthPath;
        /// <summary>
        /// 版本号
        /// </summary>
        public string version;

    }
}
