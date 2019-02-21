/***
 * 
 *    Title: 手发射线激活控制
 *           
 *    Description: 
 *           1.每个检测到的用户都会从双手发射线，允许去激活手的控制。
 *           2.如果一个用户激活了手，则设为主ID，其他用户都停止发射线。
 *           3.操作中的用户停止了控制，则回到1。
 *                  
 */

using UnityEngine;

namespace MagiCloud.Kinect
{
    public class KinectRayHandControl : MonoBehaviour
    {

        public long userID;

        public bool IsActive { get; set; }

        private KinectUserManager kinectUser;

        /// <summary>
        /// 每检测到一个人都生成这个ID的两个关节
        /// </summary>
        public void OnInitialize(long userID, KinectUserManager kinectUser)
        {
            this.userID = userID;
            this.kinectUser = kinectUser;
        }

        /// <summary>
        /// 是否存在此ID
        /// </summary>
        public bool IsUser(long id)
        {
            return id == userID;
        }


        void Update()
        {
            if (KinectConfig.GetHandStartStatus() == KinectActiveHandStadus.None) return;

            //处于检测状态时，才会执行
            if (IsActive)
            {
                kinectUser.identifyManager.StartJointRay(userID,  kinectUser);
            }
        }
    }
}
