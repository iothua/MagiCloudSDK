using UnityEngine;
using System.Collections;

namespace MagiCloud.Kinect
{
    /// <summary>
    /// 双手识别
    /// </summary>
    public class TwoHandControl : MonoBehaviour
    {
        public KinectHandIdentifyManager handIdentifyManager { get; set; }

        void Update()
        {
            if (KinectConfig.GetHandStartStatus() != KinectActiveHandStadus.Two) return;

            if(KinectConfig.UserID==0)
                handIdentifyManager.userManager.StartUsers();
        }
       
        public void StartJointRay2(long userID, KinectUserManager kinectUser)
        {
            //userID是当前用户，KinectConfig.UserID是操作中的用户

            //如果操作中的用户在检测区域则不检测其他人,反之则允许其他用户抢夺操控权
            if (KinectTransfer.IsUserNear(KinectConfig.UserID)) return;

            //如果当前用户在检测区域中并且不是操作中的用户，则设置当前用户为操作用户
            if (KinectTransfer.IsUserNear(userID)&&userID!=KinectConfig.UserID)
            {
                KinectConfig.SetUserID(userID);
                KinectTransfer.StartHand(2);
                KinectConfig.SetKinectHandActiveStatus(KinectHandStatus.Enable);

            }

            //如果当前用户是操作用户并且不在检测区域中，则取消当前用户的操作权
            if (!KinectTransfer.IsUserNear(userID)&&userID==KinectConfig.UserID)
            {
                KinectConfig.SetUserIDNull();
                KinectTransfer.StopHand(2);
                KinectConfig.SetKinectHandActiveStatus(KinectHandStatus.Identify);
                kinectUser.StartNearUsers();
            }
        }

    }
}
