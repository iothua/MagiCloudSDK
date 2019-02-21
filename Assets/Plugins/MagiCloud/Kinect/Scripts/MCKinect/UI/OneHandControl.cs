/***
 * 
 *    Title: UI界面的手控制激活
 *           
 *    Description: 
 *           只要有人举起手就设为主ID
 *           放下手后主ID设回0
 *           
 *                  
 */

using UnityEngine;

namespace MagiCloud.Kinect
{
    public class OneHandControl : MonoBehaviour
    {

        private Vector3 leftHandPos = Vector3.zero;
        private Vector3 leftHandScreenPos = Vector3.zero;
        private Vector3 leftIboxLeftBotBack = Vector3.zero;
        private Vector3 leftIboxRightTopFront = Vector3.zero;
        private bool isleftIboxValid = false;
        private bool isLeftHandInteracting = false;

        private Vector3 rightHandPos = Vector3.zero;
        private Vector3 rightHandScreenPos = Vector3.zero;
        private Vector3 rightIboxLeftBotBack = Vector3.zero;
        private Vector3 rightIboxRightTopFront = Vector3.zero;
        private bool isRightIboxValid = false;
        private bool isRightHandInteracting = false;

        private bool isEnableRight;
        private bool isEnableLeft;

        public void StartDetectHand(long userID, KinectUserManager kinectUser)
        {
            KinectManager kinectManager = KinectManager.Instance;

            //这里的代码是从Kinect SDK的 InteractionManager.cs 复制过来的，用于判断用户是否举起一只手。最终用到的是isLeftHandInteracting和isRightHandInteracting两个bool值。
            isleftIboxValid = kinectManager.GetLeftHandInteractionBox(userID, ref leftIboxLeftBotBack, ref leftIboxRightTopFront, isleftIboxValid);
            leftHandPos = kinectManager.GetJointPosition(userID, (int)KinectInterop.JointType.HandLeft);
            isLeftHandInteracting = (leftHandPos.x >= (leftIboxLeftBotBack.x - 1.0f)) && (leftHandPos.x <= (leftIboxRightTopFront.x + 0.5f)) &&
                    (leftHandPos.y >= (leftIboxLeftBotBack.y - 0.1f)) && (leftHandPos.y <= (leftIboxRightTopFront.y + 0.7f)) &&
                    (leftIboxLeftBotBack.z >= leftHandPos.z) && (leftIboxRightTopFront.z * 0.8f <= leftHandPos.z);

            isRightIboxValid = kinectManager.GetRightHandInteractionBox(userID, ref rightIboxLeftBotBack, ref rightIboxRightTopFront, isRightIboxValid);
            rightHandPos = kinectManager.GetJointPosition(userID, (int)KinectInterop.JointType.HandRight);
            isRightHandInteracting = (rightHandPos.x >= (rightIboxLeftBotBack.x - 0.5f)) && (rightHandPos.x <= (rightIboxRightTopFront.x + 1.0f)) &&
                        (rightHandPos.y >= (rightIboxLeftBotBack.y - 0.1f)) && (rightHandPos.y <= (rightIboxRightTopFront.y + 0.7f)) &&
                        (rightIboxLeftBotBack.z >= rightHandPos.z) && (rightIboxRightTopFront.z * 0.8f <= rightHandPos.z);


            //判断条件：1.当前没有操作用户。2.当前用户不是操作用户。 3.后来加的，只有检测区域的用户才识别。
            if ((KinectConfig.UserID == 0 || KinectConfig.UserID != userID) && KinectTransfer.IsUserNear(userID))
            {
                OnDetectHand(userID, kinectUser);
            }
            else if (KinectConfig.UserID == userID)
            {
                OnChangeHand(userID, kinectUser);
            }

        }

        //第一次检测用户
        void OnDetectHand(long userID, KinectUserManager kinectUser)
        {
            if (isLeftHandInteracting)
            {
                //设置手势已经启动
                KinectConfig.SetKinectHandActiveStatus(KinectHandStatus.Enable);
                //其他用户停止检测
                kinectUser.StopOtherUsers(userID);
                //设置当前用户为操作用户
                KinectConfig.SetUserID(userID);
                //现实UI手图标
                KinectTransfer.StartHand(1);
                //设置主用户(换手用)
                KinectManager.Instance.SetPrimaryUserID(userID);
            }
            else if (isRightHandInteracting)
            {
                KinectConfig.SetKinectHandActiveStatus(KinectHandStatus.Enable);
                kinectUser.StopOtherUsers(userID);
                KinectConfig.SetUserID(userID);
                KinectTransfer.StartHand(0);
                KinectManager.Instance.SetPrimaryUserID(userID);
            }
        }

        //用户已经激活手了，检测用户是否有换手的操作
        void OnChangeHand(long userID, KinectUserManager kinectUser)
        {
            //切换左右手
            if (!isEnableRight)
            {
                if (isLeftHandInteracting)
                {
                    KinectTransfer.StopHand(0);
                    KinectTransfer.StartHand(1);
                    isEnableLeft = true;
                }
                else
                {
                    isEnableLeft = false;
                }
            }
            if (!isEnableLeft)
            {
                if (isRightHandInteracting)
                {
                    KinectTransfer.StopHand(1);
                    KinectTransfer.StartHand(0);
                    isEnableRight = true;
                }
                else
                {
                    isEnableRight = false;
                }
            }

            //如果左右手都放下了或者用户不在检测区域，则当前用户取消操控
            if ((!isRightHandInteracting&& !isLeftHandInteracting)|| !KinectTransfer.IsUserNear(userID))
            {
                //操作用户ID设置为0
                KinectConfig.SetUserIDNull();

                //把UI手图标隐藏
                KinectTransfer.StopHand(2);
                //设置当前为可识别状态
                KinectConfig.SetKinectHandActiveStatus(KinectHandStatus.Identify);

                //开启所有用户的检测
                kinectUser.StartUsers();

                isEnableLeft = false;
                isEnableRight = false;
                return;
            }

            //如果正在识别中的用户不在检测区域，则检测区域的用户开启检测并允许抢夺控制权
            if (!KinectTransfer.IsUserNear(userID))
            {
                kinectUser.StartNearUsers();
            }
            else
            {
                kinectUser.StopOtherUsers(userID);
            }

        }

    }
}

