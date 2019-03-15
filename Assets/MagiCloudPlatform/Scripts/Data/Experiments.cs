using System;
using System.Collections.Generic;
using System.Linq;

namespace MagiCloudPlatform.Data
{
    /// <summary>
    /// 实验
    /// </summary>
    public class Experiments
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

        private int _experimentID;
        /// <summary>
        /// 实验ID
        /// </summary>
        public int ExperimentID {
            get {
                return _experimentID;
            }
            set {
                _experimentID = value;
            }
        }

        private int _projectID;
        /// <summary>
        /// 项目ID
        /// </summary>
        public int ProjectID {
            get {
                return _projectID;
            }
            set {
                _projectID = value;
            }
        }

        private string _experimentName;
        /// <summary>
        /// 实验名称
        /// </summary>
        public string ExperimentName {
            get {
                return _experimentName;
            }
            set {
                _experimentName = value;
            }
        }

        private string _introduction;
        /// <summary>
        /// 实验简介
        /// </summary>
        public string Introduction {
            get {
                return _introduction;
            }
            set {
                _introduction = value;
            }
        }

        private bool _active;
        /// <summary>
        /// 实验是否激活
        /// </summary>
        public bool Active {
            get {
                return _active;
            }
            set {
                _active = value;
            }
        }

        private string _image1;
        /// <summary>
        /// 实验简介图片
        /// </summary>
        public string Image1 {
            get {
                return _image1;
            }
            set {
                _image1 = value;
            }
        }

        private string _image2;
        /// <summary>
        /// 实验详细介绍图片
        /// </summary>
        public string Image2 {
            get {
                return _image2;
            }
            set {
                _image2 = value;
            }
        }

        private string _sourthPath;
        /// <summary>
        /// 实验资源路径
        /// </summary>
        public string SourthPath {
            get {
                return _sourthPath;
            }
            set {
                _sourthPath = value;
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


    }
}
