using UnityEngine;

namespace MagiCloud.Kinect
{
    /// <summary>
    /// Kinect初始化并操作
    /// </summary>
    public class KinectOperate : MonoBehaviour
    {
        #region 变量

        private KinectManager kinectManager;                //Kinect管理对象
        private InteractionManager interactionManager;      //互动管理对象
        private KinectGestureListener kinectGestureListener;
        private SimpleGestureListener simpleGestureListener;
        private KinectTransfer kinectTransfer;
        private MInputKinect mInputKinect;

        public KinectUserManager kinectUserManager { get; private set; }

        public KinectHandFunction LeftHandFunction;
        public KinectHandFunction RightHandFunction;

        #endregion

        #region 私有方法

        private void OnEnable()
        {
            kinectManager = KinectConfig.mainCamera.gameObject.AddComponent<KinectManager>();

            kinectManager.computeUserMap = true;
            kinectManager.computeColorMap = true;

            interactionManager = KinectConfig.mainCamera.gameObject.AddComponent<InteractionManager>();
            kinectUserManager = gameObject.GetComponentInChildren<KinectUserManager>();

            kinectGestureListener = KinectConfig.mainCamera.gameObject.AddComponent<KinectGestureListener>();
            simpleGestureListener = KinectConfig.mainCamera.gameObject.AddComponent<SimpleGestureListener>();

            kinectManager.gestureListeners.Add(kinectGestureListener);
            kinectManager.gestureListeners.Add(simpleGestureListener);

            kinectGestureListener.OnInitialize(this);

            kinectTransfer = new KinectTransfer(this);
            mInputKinect = new MInputKinect(kinectGestureListener,this);
        }
        #endregion

        public void InstantiationHand(KinectHandFunction left, KinectHandFunction right)
        {
            LeftHandFunction = left;
            RightHandFunction = right;

            right.OnInit(kinectGestureListener, left, 0);
            left.OnInit(kinectGestureListener, right, 1);
        }

        #region 公用方法
        public bool GetHandGrip(int handIndex)
        {
            return kinectGestureListener.GetHandGrip(handIndex);
        }

        public bool GetHandRelease(int handIndex)
        {
            return kinectGestureListener.GetHandRelease(handIndex);
        }

        public bool GetLockHand(int handIndex)
        {
            return kinectGestureListener.GetLockHand(handIndex);
        }

        /// <summary>
        /// 启动手（开启实验操作）
        /// </summary>
        /// <param name="handIndex"></param>
        public void StartHand(int handIndex)
        {
            if (handIndex == 0)
            {
                if (!RightHandFunction.IsHand)
                {
                    //开启手的控制
                    RightHandFunction.StartHand();
                }
            }
            else
            {
                if (!LeftHandFunction.IsHand)
                {
                    LeftHandFunction.StartHand();
                }
            }
        }

        /// <summary>
        /// 停止手
        /// </summary>
        /// <param name="handIndex"></param>
        public void StopHand(int handIndex)
        {
            if (handIndex == 0)
            {
                if (RightHandFunction.IsHand)
                {
                    RightHandFunction.StopHand();
                }
            }
            else if(handIndex == 1)
            {
                if (LeftHandFunction.IsHand)
                {
                    LeftHandFunction.StopHand();
                }
            }
            else
            {
                if (RightHandFunction.IsHand && LeftHandFunction.IsHand)
                {
                    RightHandFunction.StopHand();
                    LeftHandFunction.StopHand();
                }
            }
        }

        public bool IsHandActive(int handIndex)
        {
            if (handIndex == 0)
            {
                return RightHandFunction.IsHand;
            }
            else if(handIndex == 1)
            {
                return LeftHandFunction.IsHand;
            }
            else
            {
                return RightHandFunction.IsHand && LeftHandFunction.IsHand;
            }
        }

        #endregion
    }
}

