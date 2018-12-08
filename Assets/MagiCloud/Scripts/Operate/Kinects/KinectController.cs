using MagiCloud;
using MagiCloud.Core;
using MagiCloud.Core.Events;
using MagiCloud.Core.MInput;
using MagiCloud.Features;
using MagiCloud.Kinect;
using MagiCloud.Operate;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MagiCloud.Operate
{
    [DefaultExecutionOrder(-900)]
    public class KinectController : MonoBehaviour, IHandController
    {
        private MBehaviour behaviour;

        private bool IsZoom = true; //开启缩放
        private bool IsRotate = true; //开启旋转
        private bool isTempRotate = false;
        private bool isTempZoom = false;

        private bool IsRotateDown = false; //旋转是否开启

        [Header("右手松手图标")]
        public HandIcon rightHandSprite;//手图标

        [Header("左手图标")]
        public HandIcon leftHandSprite;//手图标

        [Header("图标大小")]
        public Vector2 handSize = new Vector2(50, 50);//图标大小

        private bool isPlaying = false;

        private KinectHandFunction rightHandObject, leftHandObject;

        // 初始默认双手
        [SerializeField]
        private KinectActiveHandStadus kinectHandStartStatus = KinectActiveHandStadus.Two;
        private KinectHandStatus activeStatus = KinectHandStatus.Identify;

        private IOperateObject rightOperateObject;
        private IOperateObject leftOperateObject;

        private Vector3 leftOffset;
        private Vector3 rightOffset;
        private Vector3 lastHandPos;
        private GameObject userManager;     // 用户识别管理

        // 单手操作记录手编号
        private int handIndex;
        private bool isLeft, isRight;
        private bool isLeftGrip, isRightGrip;

        private TwoHandDistance twoHandDistance;

        public Dictionary<int, MInputHand> InputHands
        {
            get;
            set;
        }

        public bool IsPlaying
        {
            get
            {
                return isPlaying;
            }
        }

        void KinectInitialize()
        {
            userManager = new GameObject("UserManager");
            userManager.AddComponent<KinectUserManager>();
            userManager.AddComponent<KinectHandIdentifyManager>();
            userManager.transform.SetParent(transform);

            gameObject.AddComponent<KinectCapture>();
            gameObject.AddComponent<KinectOperate>();

            //实例化Kinect监测
            rightHandObject = new KinectHandFunction();
            leftHandObject = new KinectHandFunction();

            KinectTransfer.InstantiationHand(leftHandObject, rightHandObject);

            KinectConfig.SetHandStartStatus(kinectHandStartStatus); // 设置单双手操作
            KinectConfig.SetKinectHandActiveStatus(activeStatus);   // 激活手势
        }

        void KinectMInputInitialize(KinectActiveHandStadus kinectActiveHandStadus)
        {
            InputHands = new Dictionary<int, MInputHand>();
            switch (kinectActiveHandStadus)
            {
                case KinectActiveHandStadus.None:
                    throw new Exception("单双手状态未选择");
                case KinectActiveHandStadus.One:
                    //初始化手的种类
                    var handUI = MHandUIManager.CreateHandUI(transform, rightHandSprite, handSize);
                    var inputHand = new MInputHand(0, handUI, OperatePlatform.Kinect);
                    InputHands.Add(0, inputHand);

                    isPlaying = true;

                    //注册操作者相关事件
                    var operate = MOperateManager.AddOperateHand(inputHand, this);
                    //注册方法
                    operate.OnGrab = OnRightGrabObject;
                    operate.OnSetGrab = SetGrabObject;
                    operate.OnEnable();

                    break;
                case KinectActiveHandStadus.Two:
                    //初始化手的种类
                    var rightHandUI = MHandUIManager.CreateHandUI(transform, rightHandSprite, handSize);
                    var rightInputHand = new MInputHand(0, rightHandUI, OperatePlatform.Kinect);
                    InputHands.Add(0, rightInputHand);

                    var leftHandUI = MHandUIManager.CreateHandUI(transform, leftHandSprite, handSize);
                    var leftInputHand = new MInputHand(1, leftHandUI, OperatePlatform.Kinect);
                    InputHands.Add(1, leftInputHand);

                    isPlaying = true;

                    //注册操作者相关事件
                    var rightOperate = MOperateManager.AddOperateHand(rightInputHand,this);
                    //注册方法
                    rightOperate.OnGrab = OnRightGrabObject;
                    rightOperate.OnSetGrab = SetGrabObject;
                    rightOperate.OnEnable();

                    //注册操作者相关事件
                    var leftOperate = MOperateManager.AddOperateHand(leftInputHand,this);
                    //注册方法
                    leftOperate.OnGrab = OnLeftGrabObject;
                    leftOperate.OnSetGrab = SetGrabObject;
                    leftOperate.OnEnable();
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// 获取到指定输入端对象
        /// </summary>
        /// <param name="handIndex"></param>
        /// <returns></returns>
        public MInputHand GetInputHand(int handIndex)
        {
            MInputHand hand;

            InputHands.TryGetValue(handIndex, out hand);

            if (hand == null)
                throw new Exception("手势编号错误：" + handIndex);

            return hand;
        }

        private void Awake()
        {
            KinectInitialize();
            behaviour = new MBehaviour(ExecutionPriority.Highest, -900, enabled);

            KinectMInputInitialize(kinectHandStartStatus);
            //behaviour.OnAwake_MBehaviour(() =>
            //{
            //    KinectMInputInitialize(kinectHandStartStatus);
            //});

            behaviour.OnUpdate_MBehaviour(OnKinectUpdate);

        }

        void OnKinectUpdate()
        {
            switch (kinectHandStartStatus)
            {
                case KinectActiveHandStadus.None:
                    KinectHandNone();
                    break;
                case KinectActiveHandStadus.One:
                    KinectHandTwo();
                    //KinectHandOne();
                    break;
                case KinectActiveHandStadus.Two:
                    KinectHandTwo();
                    break;
                default:
                    break;
            }
        }

        // 双手操作处理
        void KinectHandTwo()
        {
            //将他的屏幕坐标传递出去
            InputHands[0].OnUpdate(MInputKinect.ScreenHandPostion(0));

            if (MInputKinect.HandGrip(0))
            {
                InputHands[0].SetGrip();
            }

            if (MInputKinect.HandRelease(0))
            {
                InputHands[0].SetIdle();
            }

            //将他的屏幕坐标传递出去
            InputHands[1].OnUpdate(MInputKinect.ScreenHandPostion(1));

            if (MInputKinect.HandGrip(1))
                InputHands[1].SetGrip();

            if (MInputKinect.HandRelease(1))
                InputHands[1].SetIdle();



            KinectRotateZoom();



            if (rightOperateObject != null)
            {
                switch (InputHands[0].HandStatus)
                {
                    case MInputHandStatus.Grabing:

                        var screenDevice = MUtility.MainWorldToScreenPoint(rightOperateObject.GrabObject.transform.position);

                        Vector3 screenMouse = InputHands[0].ScreenPoint;
                        Vector3 vPos = MUtility.MainScreenToWorldPoint(new Vector3(screenMouse.x, screenMouse.y, screenDevice.z));

                        //rightOperateObject.GrabObject.transform.position = vPos - rightOffset;

                        Vector3 position = vPos - rightOffset;

                        EventUpdateObject.SendListener(rightOperateObject.GrabObject, position, rightOperateObject.GrabObject.transform.rotation, InputHands[0].HandIndex);

                        //需要处理偏移量

                        break;
                    case MInputHandStatus.Idle:

                        this.rightOperateObject = null;
                        break;
                    default:
                        break;
                }
            }

            if (leftOperateObject != null)
            {
                switch (InputHands[1].HandStatus)
                {
                    case MInputHandStatus.Grabing:

                        var screenDevice = MUtility.MainWorldToScreenPoint(leftOperateObject.GrabObject.transform.position);

                        Vector3 screenMouse = InputHands[1].ScreenPoint;
                        Vector3 vPos = MUtility.MainScreenToWorldPoint(new Vector3(screenMouse.x, screenMouse.y, screenDevice.z));

                        //leftOperateObject.GrabObject.transform.position = vPos - leftOffset;

                        Vector3 position = vPos - leftOffset;

                        EventUpdateObject.SendListener(leftOperateObject.GrabObject, position, leftOperateObject.GrabObject.transform.rotation, InputHands[0].HandIndex);

                        //需要处理偏移量

                        break;
                    case MInputHandStatus.Idle:

                        this.leftOperateObject = null;
                        break;
                    default:
                        break;
                }
            }

            if (kinectHandStartStatus == KinectActiveHandStadus.One)
            {
                if (MInputKinect.IsHandActive(0) && !MInputKinect.IsHandActive(1) && isRight == false)
                {
                    MOperateManager.GetOperateHand(0, OperatePlatform.Kinect).OnEnable();
                    MOperateManager.GetOperateHand(1, OperatePlatform.Kinect).OnDisable();
                    isRight = true;
                    isLeft = false;

                }
                else if (MInputKinect.IsHandActive(1) && !MInputKinect.IsHandActive(0) && isLeft == false)
                {
                    MOperateManager.GetOperateHand(1, OperatePlatform.Kinect).OnEnable();
                    MOperateManager.GetOperateHand(0, OperatePlatform.Kinect).OnDisable();
                    isLeft = true;
                    isRight = false;
                }
            }
        }

        void KinectRotateZoom()
        {
            if (MInputKinect.HandGrip(0))
            {
                handIndex = 0;
                isRightGrip = true;
            }
            else if (MInputKinect.HandRelease(0))
            {
                isRightGrip = false;
            }

            if (MInputKinect.HandGrip(1))
            {
                handIndex = 1;
                isLeftGrip = true;
            }
            else if (MInputKinect.HandRelease(1))
            {
                isLeftGrip = false;
                handIndex = 0;
            }


            if (IsRotate)                                                            // 旋转
            {
                if ((isLeftGrip && !isRightGrip) || (isRightGrip && !isLeftGrip))
                {
                    if (!isTempRotate && ActionConstraint.BindCount == 0 && InputHands[0].ScreenVector.magnitude > 2.0f)
                    {
                        //将动作记录到集合中
                        ActionConstraint.AddBind(ActionConstraint.Camera_Rotate_Action);

                        isTempRotate = true;
                    }

                    //已经存在旋转，并且在集合中记录
                    if (isTempRotate && ActionConstraint.IsBind(ActionConstraint.Camera_Rotate_Action))
                    {
                        EventCameraRotate.SendListener(InputHands[handIndex].ScreenVector);
                    }
                }
                else
                {
                    EventCameraRotate.SendListener(Vector3.zero);
                }

            }


            if (IsZoom)                                                                 // 缩放
            {
                if (isRightGrip && isLeftGrip)
                {
                    if (!isTempZoom)
                    {
                        if (twoHandDistance != null)
                        {
                            twoHandDistance.TwoHandGrip();
                        }
                        isTempZoom = true;
                    }

                    if (twoHandDistance == null)
                    {
                        twoHandDistance = new TwoHandDistance();
                        twoHandDistance.TwoHandGrip();
                    }
                    else
                    {
                        float result = twoHandDistance.ZoomCameraToMoveFloat() / 10000;

                        if (result != 0/* && isTempZoom *//*&& ActionConstraint.IsBind(ActionConstraint.Camera_Zoom_Action)*/)
                        {
                            EventCameraZoom.SendListener(result);

                        }
                    }

                }
                else if (!isRightGrip && !isLeftGrip && twoHandDistance != null)
                {
                    twoHandDistance.TwoHandIdle();
                    EventCameraZoom.SendListener(0);
                    isTempZoom = false;
                }
            }

        }

        // 单手操作处理
        void KinectHandOne()
        {
            if (MInputKinect.IsHandActive(0))
            {
                handIndex = 0;
            }
            else if (MInputKinect.IsHandActive(1))
            {
                handIndex = 1;
            }

            //将他的屏幕坐标传递出去
            InputHands[0].OnUpdate(MInputKinect.ScreenHandPostion(handIndex));

            if (MInputKinect.HandGrip(handIndex))
                InputHands[0].SetGrip();

            if (MInputKinect.HandRelease(handIndex))
                InputHands[0].SetIdle();

            if (rightOperateObject != null)
            {
                switch (InputHands[0].HandStatus)
                {
                    case MInputHandStatus.Grabing:

                        var screenDevice = MUtility.MainWorldToScreenPoint(rightOperateObject.GrabObject.transform.position);

                        Vector3 screenMouse = InputHands[0].ScreenPoint;
                        Vector3 vPos = MUtility.MainScreenToWorldPoint(new Vector3(screenMouse.x, screenMouse.y, screenDevice.z));

                        rightOperateObject.GrabObject.transform.position = vPos - rightOffset;

                        //需要处理偏移量

                        break;
                    case MInputHandStatus.Idle:

                        this.rightOperateObject = null;
                        break;
                    default:
                        break;
                }
            }
        }

        void KinectHandNone() { }

        /// <summary>
        /// 移动设置
        /// </summary>
        /// <param name="operate"></param>
        /// <param name="handIndex"></param>
        void OnRightGrabObject(IOperateObject operate, int handIndex)
        {
            if (handIndex != InputHands[0].HandIndex) return;

            rightOffset = GetOffsetPosition(InputHands[0].ScreenPoint, operate.GrabObject);

            this.rightOperateObject = operate;
        }

        /// <summary>
        /// 移动设置
        /// </summary>
        /// <param name="operate"></param>
        /// <param name="handIndex"></param>
        void OnLeftGrabObject(IOperateObject operate, int handIndex)
        {
            if (handIndex != InputHands[1].HandIndex) return;

            leftOffset = GetOffsetPosition(InputHands[1].ScreenPoint, operate.GrabObject);
            this.leftOperateObject = operate;
        }

        /// <summary>
        /// 计算适口偏移值
        /// </summary>
        /// <param name="mousePosition"></param>
        /// <param name="grabObject"></param>
        /// <returns></returns>
        private Vector3 GetOffsetPosition(Vector3 mousePosition, GameObject grabObject)
        {
            var offset = Vector3.zero;
            Vector3 screenDevice = MUtility.MainWorldToScreenPoint(grabObject.transform.position);
            Vector3 vPos = MUtility.MainScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, screenDevice.z));

            offset = vPos - grabObject.transform.position;

            return offset;
        }

        /// <summary>
        /// 设置物体被抓取
        /// </summary>
        /// <param name="operate"></param>
        /// <param name="handIndex"></param>
        /// <param name="cameraRelativeDistance"></param>
        void SetGrabObject(IOperateObject operate, int handIndex, float cameraRelativeDistance)
        {
            if (handIndex != InputHands[0].HandIndex) return;

            //Vector3 screenDevice = MUtility.MainWorldToScreenPoint(operate.GrabObject.transform.position);
            Vector3 screenpoint = InputHands[0].ScreenPoint;
            rightOperateObject = operate;

            Vector3 screenMainCamera = MUtility.MainWorldToScreenPoint(MUtility.MainCamera.transform.position
                + MUtility.MainCamera.transform.forward * cameraRelativeDistance);

            Vector3 position = MUtility.MainScreenToWorldPoint(new Vector3(screenpoint.x, screenpoint.y, screenMainCamera.z));

            rightOperateObject.GrabObject.transform.position = position;
        }

        public void StartOnlyHand()
        {
            kinectHandStartStatus = KinectActiveHandStadus.One;
            KinectConfig.SetHandStartStatus(kinectHandStartStatus); // 设置单双手操作
        }

        public void StartMultipleHand()
        {
            kinectHandStartStatus = KinectActiveHandStadus.Two;
            KinectConfig.SetHandStartStatus(kinectHandStartStatus); // 设置单双手操作
        }
    }
}