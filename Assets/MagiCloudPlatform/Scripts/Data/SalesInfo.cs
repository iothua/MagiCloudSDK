using System;

namespace MagiCloudPlatform.Data
{
    /// <summary>
    /// 使用记录信息
    /// </summary>
    public class SalesInfo
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

        private int _userID;
        /// <summary>
        /// 用户ID
        /// </summary>
        public int userID {
            get {
                return _userID;
            }
            set {
                _userID = value;
            }
        }

        private int _projectID;
        /// <summary>
        /// 产品ID
        /// </summary>
        public int ProjectID {
            get {
                return _projectID;
            }
            set {
                _projectID = value;
            }
        }

        private string _key;
        /// <summary>
        /// 密钥
        /// </summary>
        public string Key {
            get {
                return _key;
            }
            set {
                _key = value;
            }
        }

        private DateTime _time;
        /// <summary>
        /// 使用时长
        /// </summary>
        public DateTime Time {
            get {
                return _time;
            }
            set {
                _time = value;
            }
        }
    }
}
