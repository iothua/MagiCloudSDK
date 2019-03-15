using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MagiCloudPlatform.Data
{
    public class UserID
    {
        private int _userID;
        public int userID {
            get { return _userID; }
            set {
                _userID = value;
            }
        }

        private string _name;
        public string Name {
            get {
                return _name;
            }
            set {
                _name = value;
            }
        }

        private string _organization;
        /// <summary>
        /// 用户组织
        /// </summary>
        public string Organization {
            get {
                return _organization;
            }
            set {
                _organization = value;
            }
        }

        private string _career;
        /// <summary>
        /// 从事职业
        /// </summary>
        public string Career {
            get {
                return _career;
            }
            set {
                _career = value;
            }
        }

        private string _telephone;
        /// <summary>
        /// 电话号码
        /// </summary>
        public string Telephone {
            get {
                return _telephone;
            }
            set {
                _telephone = value;
            }
        }

        private string _email;
        /// <summary>
        /// 邮件
        /// </summary>
        public string Email {
            get {
                return _email;
            }
            set {
                _email = value;
            }
        }

        private string _permission;
        public string Permission {
            get {
                return _permission;
            }
            set {
                _permission = value;
            }
        }

        private string _password;
        public string Password {
            get {
                return _password;
            }
            set {
                _password = value;
            }
        }
    }
}

