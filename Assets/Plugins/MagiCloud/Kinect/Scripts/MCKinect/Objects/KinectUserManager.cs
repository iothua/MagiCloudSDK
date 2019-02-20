/***
 * 
 *    Title: 用户管理
 *           
 *    Description: 保存所有用户
 *                 方便管理
 *                  
 */

using UnityEngine;
using System.Collections.Generic;

namespace MagiCloud.Kinect
{
    public class KinectUserManager : MonoBehaviour
    {
        private List<long> usersID = new List<long>();
        private List<KinectRayHandControl> usersControl = new List<KinectRayHandControl>();

        private GameObject DetectedUsers;

        public KinectHandIdentifyManager identifyManager { get; private set; }

        void Start()
        {
            DetectedUsers = gameObject;
            identifyManager = GetComponent<KinectHandIdentifyManager>();
            identifyManager.userManager = this;
        }

        void Update()
        {
            DeleteMissedUser();
        }

        /// <summary>
        /// 如果系统主ID已经丢失了，却没重置，则手动重置
        /// </summary>
        void DeleteMissedUser()
        {
            if ((!usersID.Contains(KinectConfig.UserID)) && KinectConfig.UserID != 0)
            {
                print("missing" + usersID);

                KinectTransfer.StopHand(2);
                KinectConfig.SetUserIDNull();
                KinectConfig.SetKinectHandActiveStatus(KinectHandStatus.Identify); //手势进入检测模式
            }
        }

        /// <summary>
        /// 增加用户
        /// </summary>
        public void AddUser(long ID)
        {
            //保存ID
            usersID.Add(ID);
            //创建好游戏对象并赋好父节点
            GameObject userObject = new GameObject("User" + ID.ToString());

            userObject.transform.SetParent(DetectedUsers.transform);

            //每个用户检测到以后都会绑定一个KinectRayHandControl脚本，作用是控制当前用户是否可以激活控制。
            KinectRayHandControl handControl = userObject.AddComponent<KinectRayHandControl>();

            handControl.OnInitialize(ID, this);

            //保存用户检测
            usersControl.Add(handControl);

            //Logs.WriteLog(name + "AddUser()执行添加用户操作：" + ID);
            //如果已经有用户识别了，则不需要再次开启
            if (KinectConfig.GetKinectHandActiveStatus() == KinectHandStatus.Identify ||
                KinectConfig.GetKinectHandActiveStatus() == KinectHandStatus.Disable)
            {
                StartUsers();
            }
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        public void LostUser(long ID)
        {
            //从列表中删除
            usersID.Remove(ID);
            //Logs.WriteLog(name + "LostUser()执行移除用户操作：" + ID);
            foreach (var item in usersControl)
            {
                if (item.IsUser(ID))
                {
                    //如果离开的用户是操作中的用户
                    if (KinectConfig.UserID == ID)
                    {
                        KinectTransfer.StopHand(2);
                        KinectConfig.SetUserIDNull();
                        KinectConfig.SetKinectHandActiveStatus(KinectHandStatus.Identify); //手势进入检测模式
                    }

                    usersControl.Remove(item);
                    Destroy(item.gameObject);
                    return;
                }
            }
        }

        /// <summary>
        /// 当一个用户开始激活手后，除去这个用户的其他用户会被屏蔽
        /// </summary>
        public void StopOtherUsers(long ID)
        {
            //Logs.WriteLog(name + "StopOtherUsers()停止其他用户监听：" + ID);
            for (int i = 0; i < usersControl.Count; i++)
            {
                if (!usersControl[i].IsUser(ID))
                {
                    usersControl[i].IsActive = false;
                }
            }
        }

        /// <summary>
        /// 当激活中的用户离开或者停止激活或者取消控制后，所有的用户会开启检测
        /// </summary>
        public void StartUsers()
        {
            //Logs.WriteLog(typeof(KinectUserManager).Name + "StartUsers()启动所有用户监听：");
            for (int i = 0; i < usersControl.Count; i++)
            {
                usersControl[i].IsActive = true;
            }
        }

        /// <summary>
        /// 只激活区域内的用户
        /// </summary>
        public void StartNearUsers()
        {
            for (int i = 0; i < usersControl.Count; i++)
            {
                if (KinectTransfer.IsUserNear(usersControl[i].userID))
                {
                    usersControl[i].IsActive = true;
                }
            }
        }

        /// <summary>
        /// 获取场景中用户的个数
        /// </summary>
        /// <returns></returns>
        public int GetUsersCount()
        {
            return usersID.Count;
        }

    }
}
