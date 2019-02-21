using UnityEngine;

namespace MagiCloud.Kinect
{
    /// <summary>
    /// Kinect操作类
    /// </summary>
    public class KinectTransfer
    {
        private static KinectOperate kinectOperate;

        internal KinectTransfer(KinectOperate operate)
        {
            kinectOperate = operate;
        }

        #region 关于手交互的方法

        /// <summary>
        /// 开启手
        /// </summary>
        /// <param name="handIndex"></param>
        internal static void StartHand(int handIndex)
        {
            switch (handIndex)
            {
                case 0:
                case 1:
                    kinectOperate.StartHand(handIndex);
                    break;
                case 2:
                    kinectOperate.StartHand(0);
                    kinectOperate.StartHand(1);
                    break;
            }
        }

        /// <summary>
        /// 停止手
        /// </summary>
        /// <param name="handIndex"></param>
        internal static void StopHand(int handIndex)
        {
            switch (handIndex)
            {
                case 0:
                case 1:
                    kinectOperate.StopHand(handIndex);
                    break;
                case 2:
                    kinectOperate.StopHand(2);
                    break;
            }
        }

        internal static bool GetHandRelease(int handIndex)
        {
            return kinectOperate.GetHandRelease(handIndex);
        }

        /// <summary>
        /// 获取手锁定状态
        /// </summary>
        /// <param name="handIndex"></param>
        /// <returns></returns>
        public static bool IsLockHand(int handIndex)
        {
            return kinectOperate.GetLockHand(handIndex);
        }
        #endregion

        #region Kinect骨骼节点的位置
        /// <summary>
        /// 获取到手坐标(没有纵深)
        /// </summary>
        /// <param name="handIndex"></param>
        /// <returns></returns>
        public static Vector3 GetOverlayHandPos(int handIndex)
        {
            return KinectCapture.instance.GetOverlayHandPos(handIndex);
        }

        /// <summary>
        /// 获取用户是否处于最佳操作范围且在当前操作窗口
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        internal static bool IsUserNear(long userID)
        {
            // 是否在当前窗口
            bool isCurrentWindow = KinectEnableController.EnableKinect();
            KinectManager manager = KinectManager.Instance;
            Vector3 userPos = manager.GetUserPosition(userID);
            //Debug.Log(userPos);
            bool isNear = userPos.x > -0.500 && userPos.x < 0.500 && userPos.z > 1.000 && userPos.z < 1.800 && userPos.y > 0;

            //if (KinectConfig.handStartStatus == KinectActiveHandStadus.Two)
            //{
            //    Vector3 hipRight = manager.GetJointKinectPosition(userID, (int)KinectInterop.JointType.HipRight);
            //    Vector3 handRight = manager.GetJointKinectPosition(userID, (int)KinectInterop.JointType.HandRight);

            //    if (handRight.y < hipRight.y + KinectConfig.HandHipDistance)
            //    {
            //        kinectOperate.StopHand(0);
            //    }
            //    else
            //    {
            //        kinectOperate.StartHand(0);
            //    }
            //    Vector3 hipLeft = manager.GetJointKinectPosition(userID, (int)KinectInterop.JointType.HipLeft);
            //    Vector3 handLeft = manager.GetJointKinectPosition(userID, (int)KinectInterop.JointType.HandLeft);

            //    if (handLeft.y < hipRight.y + KinectConfig.HandHipDistance)
            //    {
            //        kinectOperate.StopHand(1);
            //    }
            //    else
            //    {
            //        kinectOperate.StartHand(1);
            //    }
            //}

            return isNear && isCurrentWindow;
            //return isNear;
        }
        #endregion

        /// <summary>
        /// 获取场景中用户个数
        /// </summary>
        /// <returns></returns>
        public static int GetUsersCount()
        {
            return kinectOperate.kinectUserManager.GetUsersCount();
        }

        //初始化Kinect手势操作
        public static void InstantiationHand(KinectHandFunction left, KinectHandFunction right)
        {
            kinectOperate.InstantiationHand(left, right);
        }

        // 手动刷新一次Kinect的坐标
        internal static void RunUpdateOnce()
        {
            KinectCapture.RunUpdateOnce();
        }

        #region 鼠标手势坐标信息

        internal static Vector3 GetScreenHandPos(int handIndex)
        {
            return Camera.main.WorldToScreenPoint(GetOverlayHandPos(handIndex));
        }
        #endregion

    }
}

