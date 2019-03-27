using System;
using System.Collections.Generic;

namespace MagiCloudPlatform.Data
{
    [Serializable]
    public class ProductData
    {
        public string status;

        public List<Product> datas;
    }

    /// <summary>
    /// 产品表
    /// </summary>
    [Serializable]
    public class Product
    {
        /// <summary>
        /// ID
        /// </summary>
        public int id;
        /// <summary>
        /// 项目名称
        /// </summary>
        public string name;
        /// <summary>
        /// 产品概述
        /// </summary>
        public string summarize;
        /// <summary>
        /// 图片路径
        /// </summary>
        public string img;
        /// <summary>
        /// 产品路径(如果是Web实例的，则是下载路径。如果是本地实例的，则是相对路径)
        /// </summary>
        public string productPath;
        /// <summary>
        /// 实验资源路径
        /// </summary>
        public string experimentPath;
        /// <summary>
        /// 版本号
        /// </summary>
        public string version;
        
    }
}
