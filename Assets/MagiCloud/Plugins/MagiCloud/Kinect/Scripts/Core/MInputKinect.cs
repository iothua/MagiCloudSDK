using UnityEngine;
using System.Collections.Generic;

namespace MagiCloud.Kinect
{
    /*
    1、思路，实例化MInputKinect。
    2、开始用户实时检测，并且识别用户的手启用状态
    3、当手势启用时，开启手势识别监控，当人离开时，则关闭监控
    */




    /// <summary>
    /// 手势激活状态
    /// </summary>
    public enum KinectHandStatus
    {
        /// <summary>
        /// 识别手势状态
        /// </summary>
        Identify,
        /// <summary>
        /// 手势启动
        /// </summary>
        Enable,
        /// <summary>
        /// 手势禁用
        /// </summary>
        Disable
    }

    /// <summary>
    /// 单双手状态
    /// </summary>
    public enum KinectHandModel
    {
        /// <summary>
        /// 不启动手
        /// </summary>
        None,
        /// <summary>
        /// 启动一只/启动左手，右手禁用。反之
        /// </summary>
        One,
        /// <summary>
        /// 双手都启动
        /// </summary>
        Two
    }

    /// <summary>
    /// Kinect信息获取端
    /// </summary>
    public class MInputKinect : MonoBehaviour
    {

        /// <summary>
        /// Kinect人物手势识别是否举手
        /// </summary>
        public class RayHandControl
        {
            public long userID;
            public bool IsActive { get; set; }

            public bool IsUser(long id)
            {
                return id == userID;
            }
        }

        /// <summary>
        /// 单手模型控制端
        /// </summary>
        public class OneHandControl
        {
            
            public void StartDetectHand(long userID, UserManager userManager,KinectHandInteract rightHand,KinectHandInteract leftHand)
            {
                KinectManager kinectManager = KinectManager.Instance;

                //这里的代码是从Kinect SDK的 InteractionManager.cs 复制过来的，用于判断用户是否举起一只手。最终用到的是isLeftHandInteracting和isRightHandInteracting两个bool值。

                leftHand.OnHandInteracting(userID);
                rightHand.OnHandInteracting(userID);

                //判断条件：1.当前没有操作用户。2.当前用户不是操作用户。3.后来加的，只有检测区域的用户才识别。
                if ((UserID == 0 || UserID != userID) && userManager.IsUserNear(userID))
                {
                    OnDetechHand(userID, userManager, rightHand, leftHand);
                    return;
                }

                if (UserID == userID)
                {
                    OnChangeHand(userID, userManager, rightHand, leftHand);
                    return;
                }

            }

            private void OnDetechHand(long userID, UserManager userManager, KinectHandInteract rightHand, KinectHandInteract leftHand)
            {
                if (rightHand.IsHandInteracting)
                {
                    HandStatus = KinectHandStatus.Enable;
                    userManager.StopOtherUsers(userID);
                    SetUserID(userID);
                    StartHand(0);

                    KinectManager.Instance.SetPrimaryUserID(userID);

                    return;
                }

                if (leftHand.IsHandInteracting)
                {
                    HandStatus = KinectHandStatus.Enable;
                    userManager.StopOtherUsers(userID);
                    SetUserID(userID);
                    StartHand(1);

                    KinectManager.Instance.SetPrimaryUserID(userID);

                    return;
                }
            }

            private void OnChangeHand(long userID, UserManager userManager, KinectHandInteract rightHand, KinectHandInteract leftHand)
            {
                if (!rightHand.IsEnable)
                {
                    if (leftHand.IsHandInteracting)
                    {
                        StopHand(0);
                        StartHand(1);
                        leftHand.IsEnable = true;
                    }
                    else
                    {
                        leftHand.IsEnable = false;
                    }
                }

                if (!leftHand.IsEnable)
                {
                    if (rightHand.IsHandInteracting)
                    {
                        StopHand(1);
                        StartHand(0);
                        rightHand.IsEnable = true;
                    }
                    else
                    {
                        rightHand.IsEnable = false;
                    }
                }

                if ((!rightHand.IsHandInteracting && !leftHand.IsHandInteracting)
                    || !userManager.IsUserNear(userID))
                {
                    //操作用户ID设置为0
                    SetUserIDNull();

                    //将手停止
                    //StopHand(2);
                    HandStatus = KinectHandStatus.Identify;

                    userManager.StartUsers();

                    rightHand.IsEnable = false;
                    leftHand.IsEnable = false;

                    return;
                }

                //如果正在识别中的用户不在检测区域，则检测区域的用户开启检测并允许抢夺控制权
                if (!userManager.IsUserNear(userID))
                {
                    userManager.StartNearUsers();
                }
                else
                {
                    userManager.StopOtherUsers(userID);
                }
            }
        }

        /// <summary>
        /// 双手模型控制端
        /// </summary>
        public class TwoHandControl
        {
            public void OnUpdate(UserManager userManager)
            {
                if (UserID == 0)
                {
                    userManager.StartUsers();
                }
            }

            public void StartJointRay(long userID, UserManager userManager)
            {
                //userID是当前用户，UserID是操作中的用户
                if (userManager.IsUserNear(UserID)) return;

                //如果当前用户在检测区域中并且不是操作中的用户，则设置当前用户为操作用户
                if (userManager.IsUserNear(userID) && userID != UserID)
                {
                    SetUserID(userID);
                    HandStatus = KinectHandStatus.Enable;

                    MLog.WriteLog(userID + "被检测到，开启双手操作——StartJointRay()");

                    //StartHand(2);
                }

                if (!userManager.IsUserNear(userID) && userID == UserID)
                {
                    SetUserIDNull();

                    MLog.WriteLog(userID + "被检测到，停止双手操作——StartJointRay()");

                    StopHand(2);

                    HandStatus = KinectHandStatus.Identify;
                    userManager.StartNearUsers();
                }
            }
        }

        /// <summary>
        /// 手势用户管理端
        /// </summary>
        public class UserManager
        {
            //最大用户数
            private List<long> usersID = new List<long>();
            private List<RayHandControl> usersControl = new List<RayHandControl>();

            private OneHandControl oneHandControl;
            private TwoHandControl twoHandControl;

            public KinectHandInteract leftHandInteract, rightHandInteract;

            public UserManager()
            {
                oneHandControl = new OneHandControl();
                twoHandControl = new TwoHandControl();

                rightHandInteract = new KinectHandInteract(0);
                leftHandInteract = new KinectHandInteract(1);
            }

            public int UserCount {
                get {
                    return usersID.Count;
                }
            }

            public void OnUpdate()
            {
                DeleteMissedUser();

                for (int i = 0; i < usersControl.Count; i++)
                {
                    if (usersControl[i].IsActive)
                    {
                        switch (HandModel)
                        {
                            case KinectHandModel.One:

                                switch (HandStatus)
                                {
                                    case KinectHandStatus.Identify:
                                        oneHandControl.StartDetectHand(usersControl[i].userID, this, rightHandInteract, leftHandInteract);
                                        break;
                                    case KinectHandStatus.Enable:
                                        oneHandControl.StartDetectHand(usersControl[i].userID, this, rightHandInteract, leftHandInteract);
                                        break;
                                    default:
                                        break;
                                }

                                break;
                            case KinectHandModel.Two:

                                twoHandControl.OnUpdate(this);
                                twoHandControl.StartJointRay(usersControl[i].userID, this);

                                break;
                            default:
                                break;
                        }
                    }
                }
            }

            private void DeleteMissedUser()
            {
                if ((!usersID.Contains(UserID)) && UserID != 0)
                {
                    MLog.WriteLog("执行双手停止操作，方法名：DeleteMissedUser");

                    StopHand(2);

                    SetUserIDNull();
                    HandStatus = KinectHandStatus.Identify;
                }
            }

            /// <summary>
            /// 添加用户
            /// </summary>
            /// <param name="ID"></param>
            public void AddUser(long ID)
            {
                usersID.Add(ID);

                //实例化一个ID
                RayHandControl handControl = new RayHandControl() { userID = ID };
                usersControl.Add(handControl);

                if (HandStatus != KinectHandStatus.Enable)
                {
                    StartUsers();
                }
            }

            /// <summary>
            /// 移除用户
            /// </summary>
            /// <param name="ID"></param>
            public void LostUser(long ID)
            {
                usersID.Remove(ID);

                foreach (var item in usersControl)
                {
                    if (item.IsUser(ID))
                    {
                        if (UserID == ID)
                        {
                            MLog.WriteLog("执行移除用户操作：LostUser");
                            StopHand(2);
                            SetUserIDNull();

                            HandStatus = KinectHandStatus.Identify;
                        }

                        usersControl.Remove(item);
                        return;
                    }
                }
            }

            /// <summary>
            /// 当一个用户开始激活手后，除去这个用户的其他用户会被屏蔽
            /// </summary>
            public void StopOtherUsers(long ID)
            {
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
                    if (IsUserNear(usersControl[i].userID))
                    {
                        usersControl[i].IsActive = true;
                    }
                }
            }

            /// <summary>
            /// 检测用户是否在区域内
            /// </summary>
            /// <param name="userID"></param>
            /// <returns></returns>
            public bool IsUserNear(long userID)
            {
                Vector3 userPos = KinectManager.Instance.GetUserPosition(userID);

                bool isNear = userPos.x > -0.500 && userPos.x < 0.500 && userPos.z > 1.000 && userPos.z < 1.800 && userPos.y > 0;

                if (HandModel == KinectHandModel.Two && isNear)
                {
                    OnDetectHand(rightHandInteract,userID);
                    OnDetectHand(leftHandInteract, userID);
                }

                return isNear;
            }

            private void OnDetectHand(KinectHandInteract handInteract,long userID)
            {
                handInteract.OnHandInteracting(userID);

                if (handInteract.IsHandInteracting)
                {
                    if (!IsHandActive(handInteract.handIndex))
                    {
                        MLog.WriteLog(userID + "用户检测到手启动——OnDetectHand()，手编号：" + handInteract.handIndex);
                        StartHand(handInteract.handIndex);
                    }
                }
                else
                {
                    if (IsHandActive(handInteract.handIndex))
                    {
                        MLog.WriteLog(userID + "用户检测到手停止——OnDetectHand()，手编号：" + handInteract.handIndex);
                        StopHand(handInteract.handIndex);
                    }
                }

            }
        }

        #region 用户ID管理
        public static long UserID;

        public static void SetUserIDNull()
        {
            UserID = 0;
        }

        public static void SetUserID(long id)
        {
            UserID = id;
        }

        #endregion

        /// <summary>
        /// 手模式
        /// </summary>
        public static KinectHandModel HandModel { get; set; }

        /// <summary>
        /// 手状态
        /// </summary>
        public static KinectHandStatus HandStatus { get; internal set; }

        public static MInputKinect InputKinect;

        /// <summary>
        /// 手势用户管理器
        /// </summary>
        public UserManager userManager { get; private set; } 

        private bool isEnable;
        private KinectCapture kinectCapture; //手势关节信息
        private KinectGestureListener kinectGestureListener; //手势监听

        private KinectHandInteract leftHandInteract, rightHandInteract;

        private void Awake()
        {
            InputKinect = this;

            //实例化组件
            var kinectManager = KinectConfig.mainCamera.gameObject.AddComponent<KinectManager>();
            kinectManager.computeUserMap = true;
            kinectManager.computeColorMap = true;

            kinectGestureListener = KinectConfig.mainCamera.gameObject.AddComponent<KinectGestureListener>();
            kinectManager.gestureListeners.Add(kinectGestureListener);

            leftHandInteract = new KinectHandInteract(1);
            rightHandInteract = new KinectHandInteract(0);

            kinectCapture = new KinectCapture();

            userManager = new UserManager();

            kinectGestureListener.OnInitialize(userManager);

            DontDestroyOnLoad(gameObject);
        }

        public bool IsEnable {
            get {
                return isEnable;
            }
            set {
                isEnable = value;
                enabled = value;

            }
        }

        private void Update()
        {
            kinectCapture.OnUpdate();
            kinectGestureListener.UpdateJointsPos();//更新关节位置
            userManager.OnUpdate();

            if (HandStatus != KinectHandStatus.Enable) return;

            switch (HandModel)
            {
                case KinectHandModel.None:
                    break;
                case KinectHandModel.One:
                    kinectGestureListener.OneModelGestures();
                    break;
                case KinectHandModel.Two:
                    kinectGestureListener.TwoModelGestures();
                    break;
            }
        }

        /// <summary>
        /// 检测手状态
        /// </summary>
        /// <param name="userID"></param>
        private static void DetectHand(long userID)
        {
            if (userID == 0) return;

            Vector3 hipRight = KinectManager.Instance.GetJointKinectPosition(userID, (int)KinectInterop.JointType.HipRight);
            Vector3 handRight = KinectManager.Instance.GetJointKinectPosition(userID, (int)KinectInterop.JointType.HandRight);

            if (handRight.y < hipRight.y + KinectConfig.HandHipDistance)
            {
                if(IsHandActive(0))
                {
                    MLog.WriteLog(userID + "用户右手检测到放下，停止右手操作——DetectHand()");
                    StopHand(0);
                }

            }
            else
            {
                if(!IsHandActive(0))
                {
                    MLog.WriteLog(userID + "用户检测到右手，开启右手操作——DetectHand()");
                    StartHand(0);
                }

            }

            Vector3 hipLeft = KinectManager.Instance.GetJointKinectPosition(userID, (int)KinectInterop.JointType.HipLeft);
            Vector3 handLeft = KinectManager.Instance.GetJointKinectPosition(userID, (int)KinectInterop.JointType.HandLeft);

            if (handLeft.y < hipLeft.y + KinectConfig.HandHipDistance)
            {
                if(IsHandActive(1))
                {
                    MLog.WriteLog(userID + "用户左手检测到放下，停止左手操作——DetectHand()");
                    StopHand(1);
                }

            }
            else
            {
                if (!IsHandActive(1))
                {
                    MLog.WriteLog(userID + "用户左手检测到开启，开启左手操作——DetectHand()");
                    StartHand(1);
                }
            }
        }


        /// <summary>
        /// 手势是否激活
        /// </summary>
        /// <param name="handIndex">0为右手，1为左手，2为双手</param>
        /// <returns></returns>
        public static bool IsHandActive(int handIndex)
        {
            switch(handIndex)
            {
                case 0:
                case 1:
                    return InputKinect.kinectGestureListener.GetHandActive(handIndex);
                case 2:
                    return InputKinect.kinectGestureListener.GetHandActive(0) || InputKinect.kinectGestureListener.GetHandActive(1);
                default:
                    return false;
            }
        }

        /// <summary>
        /// 手势屏幕坐标
        /// </summary>
        /// <param name="handIndex"></param>
        /// <returns></returns>
        public static Vector3 ScreenHandPostion(int handIndex)
        {
            return KinectConfig.mainCamera.WorldToScreenPoint(KinectCapture.Instance.GetOverlayHandPos(handIndex));
        }

        /// <summary>
        /// 手势握下
        /// </summary>
        /// <param name="handIndex"></param>
        /// <returns></returns>
        public static bool HandGrip(int handIndex)
        {
            return InputKinect.kinectGestureListener.GetHandGrip(handIndex);
        }

        /// <summary>
        /// 手势释放
        /// </summary>
        /// <param name="hanIndex"></param>
        /// <returns></returns>
        public static bool HandRelease(int hanIndex)
        {
            return InputKinect.kinectGestureListener.GetHandRelease(hanIndex);
        }

        /// <summary>
        /// 手势错误
        /// </summary>
        /// <returns><c>true</c>, if lasso was handed, <c>false</c> otherwise.</returns>
        /// <param name="handIndex">Hand index.</param>
        public static bool HandLasso(int handIndex)
        {
            return InputKinect.kinectGestureListener.GetHandLasso(handIndex);
        }

        /// <summary>
        /// 停止手
        /// </summary>
        /// <param name="handIndex"></param>
        internal static void StopHand(int handIndex)
        {
            InputKinect.kinectGestureListener.SetHandActive(handIndex, false);
        }

        /// <summary>
        /// 启动手
        /// </summary>
        /// <param name="handIndex"></param>
        internal static void StartHand(int handIndex)
        {
            RefreshKinectPosition();
            InputKinect.kinectGestureListener.SetHandActive(handIndex, true);
        }

        /// <summary>
        /// 手动刷新Kinect坐标
        /// </summary>
        public static void RefreshKinectPosition()
        {
            KinectCapture.RefreshKinectPosition();
        }
    }
}