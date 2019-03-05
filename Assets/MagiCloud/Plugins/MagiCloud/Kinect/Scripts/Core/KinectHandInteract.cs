using UnityEngine;

namespace MagiCloud.Kinect
{
    /// <summary>
    /// Kinect关于手势的激活算法情况
    /// </summary>
    public class KinectHandInteract
    {
        /// <summary>
        /// 手信息
        /// </summary>
        public class HandInfo
        {
            public Vector3 handPos = Vector3.zero;
            public Vector3 screenPos = Vector3.zero;
            public Vector3 IboxLeftBotBack = Vector3.zero;
            public Vector3 IboxRightTopFront = Vector3.zero;
            public bool IsIboxValid = false;
            public bool IsHandInteracting = false;

            public bool IsEnable = false;
        }

        public int handIndex;

        private HandInfo handInfo;

        public KinectHandInteract(int handIndex)
        {
            this.handIndex = handIndex;
            handInfo = new HandInfo();
        }

        public bool IsEnable {
            get {
                return handInfo.IsEnable;
            }
            set {
                handInfo.IsEnable = value;
            }
        }

        public bool IsHandInteracting {
            get {
                return handInfo.IsHandInteracting;
            }
        }

        public bool OnHandInteracting(long userID)
        {
            KinectManager kinectManager = KinectManager.Instance;

            if (handIndex == 1)
            {
                //左手处理
                handInfo.IsIboxValid = kinectManager.GetLeftHandInteractionBox(userID, ref handInfo.IboxLeftBotBack, ref handInfo.IboxRightTopFront, handInfo.IsIboxValid);

                handInfo.handPos = kinectManager.GetJointPosition(userID, (int)KinectInterop.JointType.HandLeft);

                handInfo.IsHandInteracting = (handInfo.handPos.x >= (handInfo.IboxLeftBotBack.x - 1.0f)) && (handInfo.handPos.x <= (handInfo.IboxRightTopFront.x + 0.5f)) &&
                    (handInfo.handPos.y >= (handInfo.IboxLeftBotBack.y - 0.1f)) && (handInfo.handPos.y <= (handInfo.IboxRightTopFront.y + 0.7f)) &&
                    (handInfo.IboxLeftBotBack.z >= handInfo.handPos.z) && (handInfo.IboxRightTopFront.z * 0.8f <= handInfo.handPos.z);
                
            }
            else
            {
                //右手处理
                handInfo.IsIboxValid = kinectManager.GetRightHandInteractionBox(userID, ref handInfo.IboxLeftBotBack, ref handInfo.IboxRightTopFront, handInfo.IsIboxValid);

                handInfo.handPos = kinectManager.GetJointPosition(userID, (int)KinectInterop.JointType.HandRight);

                handInfo.IsHandInteracting = (handInfo.handPos.x >= (handInfo.IboxLeftBotBack.x - 0.5f)) && (handInfo.handPos.x <= (handInfo.IboxRightTopFront.x + 1.0f)) &&
                    (handInfo.handPos.y >= (handInfo.IboxLeftBotBack.y - 0.1f)) && (handInfo.handPos.y <= (handInfo.IboxRightTopFront.y + 0.7f)) &&
                    (handInfo.IboxLeftBotBack.z >= handInfo.handPos.z) && (handInfo.IboxRightTopFront.z * 0.8f <= handInfo.handPos.z);
            }

            return handInfo.IsHandInteracting;
        }


    }
}
