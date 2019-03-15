using System;

namespace MagiCloudPlatform.Data
{
    /// <summary>
    /// 产品表
    /// </summary>
    public class Products
    {
        private int _id;

        /// <summary>
        /// ID
        /// </summary>
        public int ID {
            get {
                return _id;
            }
            set {
                _id = value;
            }
        }

        private string _name;
        /// <summary>
        /// 姓名
        /// </summary>
        public string Name {
            get {
                return _name;
            }
            set {
                _name = value;
            }
        }

        private string _img;
        /// <summary>
        /// 图片地址
        /// </summary>
        public string Img {
            get {
                return _img;
            }
            set {
                _img = value;
            }
        }

        private string _summarize;
        /// <summary>
        /// 产品概述
        /// </summary>
        public string Summarize {
            get {
                return _summarize;
            }
            set {
                _summarize = value;
            }
        }

        private string _productPath;
        /// <summary>
        /// 产品资源路径
        /// </summary>
        public string ProductPath {
            get {
                return _productPath;
            }
            set {
                _productPath = value;
            }
        }

        private string _version;
        /// <summary>
        /// 版本号
        /// </summary>
        public string Version {
            get {
                return _version;
            }
            set {
                _version = value;
            }
        }

        private string _experimentPath;

        /// <summary>
        /// 实验资源路径
        /// </summary>
        public string ExperimentPath {
            get {
                return _experimentPath;
            }
            set {
                _experimentPath = value;
            }
        }
    }
}
