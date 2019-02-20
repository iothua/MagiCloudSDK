using MagiCloud.Core;
using MagiCloud.Core.Events;
using MagiCloud.Core.MInput;
using MagiCloud.Features;
using MagiCloud.Kinect;
using MCKinect.Events;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MagiCloud.Operate
{
    [DefaultExecutionOrder(-900)]
    public class KinectController :MonoBehaviour, IHandController
    {
        private MBehaviour behaviour;

        private bool IsZoom = true; //开启缩放
        private bool IsRotate = true; //开启旋转
        private bool isTempLeftRotate = false;
        private bool isTempRightRotate = false;
        private bool isTempZoom = false;

        private bool IsRotateDown = false; //旋转是否开启

        private bool handStart;
        private bool handEnd;

        [Header("右手松手图标")]
        public HandIcon rightHandSprite;//手图标

        [Header("左手图标")]
        public HandIcon leftHandSprite;//手图标

        [Header("图标大小")]
        public Vector2 handSize = new Vector2(50,50);//图标大小

        private bool isPlaying = false;

        private KinectHandFunction rightHandObject, leftHandObject;

        // 初始默认双手
        [SerializeField]
        private KinectActiveHandStadus kinectActiveHandStatus = KinectActiveHandStadus.Two;
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
        private bool? isLeftUIEnable, isRightUIEnable;
        private bool isLeftRotate, isRightRotate;
        private bool isHandsZoom;

        private TwoHandDistance twoHandDistance;

        private bool isEnable;

        private MOperate operate;
        private MOperate leftOperate;
        private MOperate rightOperate;

        private ZoomState zoomState;

        private enum ZoomState
        {
            rightRotate,
            leftRotate,
            zoom,
            none
        }

        public Dictionary<int,MInputHand> InputHands
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

        public bool IsEnable
        {
            get
            {
                return isEnable;
            }

            set
            {
                if (isEnable == value) return;
                isEnable = value;

                if (kinectActiveHandStatus == KinectActiveHandStadus.Two)
                {
                    if (IsEnable)
                    {
                        leftOperate.OnEnable();
                        rightOperate.OnEnable();
                    }
                    else
                    {
                        leftOperate.OnDisable();
                        rightOperate.OnDisable();
                    }
                }
                else if (kinectActiveHandStatus == KinectActiveHandStadus.One)
                {

                    if (MInputKinect.IsHandActive(0) && !MInputKinect.IsHandActive(1))
                    {
                        if (IsEnable)
                        {
                            rightOperate.OnEnable();
                        }
                        else
                        {
                            rightOperate.OnDisable();
                        }
                    }
                    else if (MInputKinect.IsHandActive(1) && !MInputKinect.IsHandActive(0))
                    {
                        if (IsEnable)
                        {
                            leftOperate.OnEnable();
                        }
                        else
                        {
                            leftOperate.OnDisable();
                        }
                    }

                }

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

            KinectTransfer.InstantiationHand(leftHandObject,rightHandObject);

            KinectConfig.SetHandStartStatus(kinectActiveHandStatus); // 设置单双手操作
            KinectConfig.SetKinectHandActiveStatus(activeStatus);   // 激活手势
        }

        void KinectMInputInitialize(KinectActiveHandStadus kinectActiveHandStadus)
        {
            InputHands = new Dictionary<int,MInputHand>();
            isEnable = true;
            switch (kinectActiveHandStadus)
            {
                case KinectActiveHandStadus.None:
                    throw new Exception("单双手状态未选择");
                case KinectActiveHandStadus.One:
                    //初始化手的种类
                    var handUI = MHandUIManager.CreateHandUI(transform,rightHandSprite,handSize);
                    var inputHand = new MInputHand(0,handUI,OperatePlatform.Kinect);
                    InputHands.Add(0,inputHand);

                    isPlaying = true;

                    //注册操作者相关事件
                    operate = MOperateManager.AddOperateHand(inputHand,this);
                    //注册方法
                    operate.OnGrab = OnRightGrabObject;
                    operate.OnSetGrab = SetRightGrabObject;
                    operate.OnEnable();
                    break;
                case KinectActiveHandStadus.Two:
                    //初始化手的种类
                    var rightHandUI = MHandUIManager.CreateHandUI(transform,rightHandSprite,handSize);
                    var rightInputHand = new MInputHand(0,rightHandUI,OperatePlatform.Kinect);
                    InputHands.Add(0,rightInputHand);

                    var leftHandUI = MHandUIManager.CreateHandUI(transform,leftHandSprite,handSize);
                    var leftInputHand = new MInputHand(1,leftHandUI,OperatePlatform.Kinect);
                    InputHands.Add(1,leftInputHand);

                    isPlaying = true;

                    //注册操作者相关事件
                    rightOperate = MOperateManager.AddOperateHand(rightInputHand,this);
                    //注册方法
                    rightOperate.OnGrab = OnRightGrabObject;
                    rightOperate.OnSetGrab = SetRightGrabObject;
                    rightOperate.OnEnable();

                    //注册操作者相关事件
                    leftOperate = MOperateManager.AddOperateHand(leftInputHand,this);
                    //注册方法
                    leftOperate.OnGrab = OnLeftGrabObject;
                    leftOperate.OnSetGrab = SetLeftGrabObject;
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

            InputHands.TryGetValue(handIndex,out hand);

            if (hand == null)
                throw new Exception("手势编号错误：" + handIndex);

            return hand;
        }

        private void Awake()
        {
            KinectInitialize();
            behaviour = new MBehaviour(ExecutionPriority.Highest,-900,enabled);

            KinectMInputInitialize(kinectActiveHandStatus);

            behaviour.OnUpdate_MBehaviour(OnKinectUpdate);

            KinectEventHandStart.AddListener(EventLevel.A, 0, RightHandStart);
            KinectEventHandStop.AddListener(EventLevel.A, 0, RightHandStop);
            KinectEventHandStart.AddListener(EventLevel.A, 1, LeftHandStart);
            KinectEventHandStop.AddListener(EventLevel.A, 1, LeftHandStop);

        }

        void OnDestroy()
        {
            KinectEventHandStart.RemoveListener( RightHandStart);
            KinectEventHandStop.RemoveListener( RightHandStop);
            KinectEventHandStart.RemoveListener( LeftHandStart);
            KinectEventHandStop.RemoveListener( LeftHandStop);
        }

        private void LeftHandStop()
        {
            //Debug.Log("LeftHandStop");
            MOperateManager.GetOperateHand(1, OperatePlatform.Kinect).OnDisable();
            isLeft = false;
        }

        private void LeftHandStart()
        {
            //Debug.Log("LeftHandStart");
            InputHands[1].SetIdle();
            MOperateManager.GetOperateHand(1, OperatePlatform.Kinect).OnEnable();
            isLeft = true;
        }

        private void RightHandStart()
        {
            //Debug.Log("RightHandStart");
            MOperateManager.GetOperateHand(0, OperatePlatform.Kinect).OnEnable();
            InputHands[0].SetIdle();
            isRight = true;
        }

        private void RightHandStop()
        {
            //Debug.Log("RightHandStop");
            MOperateManager.GetOperateHand(0, OperatePlatform.Kinect).OnDisable();
            isRight = false;
        }

        void OnKinectUpdate()
        {
            if (!isEnable) return;

            switch (kinectActiveHandStatus)
            {
                case KinectActiveHandStadus.None:
                    KinectHandNone();
                    break;
                case KinectActiveHandStadus.One:
                    //KinectHandTwo();
                    KinectHandOne();
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
            if (isRightUIEnable == null)
            {
                MOperateManager.GetOperateHand(0,OperatePlatform.Kinect).OnDisable();
                //Debug.Log(false);
                isRightUIEnable = false;
            }
            if (isLeftUIEnable == null)
            {
                MOperateManager.GetOperateHand(1,OperatePlatform.Kinect).OnDisable();
                isLeftUIEnable = false;
            }

            //将他的屏幕坐标传递出去
            //if (MInputKinect.ScreenHandPostion(0).y> 0)
            {
                InputHands[0].OnUpdate(MInputKinect.ScreenHandPostion(0));

                if (MInputKinect.HandGrip(0))
                {
                    InputHands[0].SetGrip();
                    //Debug.Log("RightGrip");
                }

                if (MInputKinect.HandRelease(0))
                {
                    InputHands[0].SetIdle();
                    //Debug.Log("RightIdle");
                }

                if (isRightUIEnable != null && !isRightUIEnable.Value)
                {
                    MOperateManager.GetOperateHand(0,OperatePlatform.Kinect).OnEnable();
                    isRightUIEnable = true;
                }
            }
            //else
            //{
            //    if (isRightUIEnable != null && isRightUIEnable.Value)
            //    {
            //        MOperateManager.GetOperateHand(0,OperatePlatform.Kinect).OnDisable();
            //        InputHands[0].SetIdle();

            //        isRightUIEnable = false;
            //    }
            //}


            //将他的屏幕坐标传递出去
            //if (MInputKinect.ScreenHandPostion(1).y > 0)
            {

                InputHands[1].OnUpdate(MInputKinect.ScreenHandPostion(1));

                if (MInputKinect.HandGrip(1))
                {
                    InputHands[1].SetGrip();
                    //Debug.Log("LeftGrip");
                }

                if (MInputKinect.HandRelease(1))
                {
                    InputHands[1].SetIdle();
                    //Debug.Log("LeftIdle");
                }

                if (isLeftUIEnable != null && !isLeftUIEnable.Value)
                {
                    MOperateManager.GetOperateHand(1,OperatePlatform.Kinect).OnEnable();
                    InputHands[1].SetIdle();
                    isLeftUIEnable = true;
                }
            }
            //else
            //{
            //    if (isLeftUIEnable != null && isLeftUIEnable.Value)
            //    {
            //        MOperateManager.GetOperateHand(1,OperatePlatform.Kinect).OnDisable();
            //        InputHands[1].SetIdle();
            //        isLeftUIEnable = false;
            //    }
            //}

            if (rightOperateObject != null)
            {
                switch (InputHands[0].HandStatus)
                {
                    case MInputHandStatus.Grabing:
                        if (ActionConstraint.BindCount == 0)// && InputHands[0].ScreenVector.magnitude > 2.0f)
                        {
                            //将动作记录到集合中
                            ActionConstraint.AddBind(ActionConstraint.Grab_Action);
                        }
                        var screenDevice = MUtility.MainWorldToScreenPoint(rightOperateObject.GrabObject.transform.position);

                        Vector3 screenMouse = InputHands[0].ScreenPoint;
                        Vector3 vPos = MUtility.MainScreenToWorldPoint(new Vector3(screenMouse.x,screenMouse.y,screenDevice.z));

                        //rightOperateObject.GrabObject.transform.position = vPos - rightOffset;

                        Vector3 position = vPos - rightOffset;

                        EventUpdateObject.SendListener(rightOperateObject.GrabObject,position,rightOperateObject.GrabObject.transform.rotation,InputHands[0].HandIndex);

                        //需要处理偏移量

                        break;
                    case MInputHandStatus.Idle:
                        if (ActionConstraint.IsBind(ActionConstraint.Grab_Action))
                        {
                            //    EventHandReleaseObject.SendListener(rightOperateObject.GrabObject,InputHands[0].HandIndex);
                            ActionConstraint.RemoveBind(ActionConstraint.Grab_Action);
                        }
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
                        if (ActionConstraint.BindCount == 0)//&& InputHands[1].ScreenVector.magnitude > 2.0f)
                        {
                            //将动作记录到集合中
                            ActionConstraint.AddBind(ActionConstraint.Grab_Action);
                        }

                        var screenDevice = MUtility.MainWorldToScreenPoint(leftOperateObject.GrabObject.transform.position);

                        Vector3 screenMouse = InputHands[1].ScreenPoint;
                        Vector3 vPos = MUtility.MainScreenToWorldPoint(new Vector3(screenMouse.x,screenMouse.y,screenDevice.z));

                        //leftOperateObject.GrabObject.transform.position = vPos - leftOffset;

                        Vector3 position = vPos - leftOffset;

                        EventUpdateObject.SendListener(leftOperateObject.GrabObject,position,leftOperateObject.GrabObject.transform.rotation,InputHands[1].HandIndex);

                        //需要处理偏移量

                        break;
                    case MInputHandStatus.Idle:
                        if (ActionConstraint.IsBind(ActionConstraint.Grab_Action))
                        {
                            //  EventHandReleaseObject.SendListener(leftOperateObject.GrabObject,InputHands[1].HandIndex);
                            ActionConstraint.RemoveBind(ActionConstraint.Grab_Action);
                        }
                        this.leftOperateObject = null;
                        break;
                    default:
                        break;
                }
            }
            KinectRotateZoomTwo(zoomState);
            KinectRotateZoom();

        }

        void KinectRotateZoomOne()
        {


        }

        void KinectRotateZoomTwo(ZoomState zoomState)
        {

            //if (MInputKinect.HandGrip(1) && MInputKinect.HandGrip(0))
            //{
            //    Debug.Log("Grip");
            //}

            switch (zoomState)
            {
                case ZoomState.rightRotate:

                    break;
                case ZoomState.leftRotate:

                    break;
                case ZoomState.zoom:

                    break;
                case ZoomState.none:

                    break;
                default:
                    break;
            }

        }

        void KinectRotateZoom()
        {

            if (MOperateManager.GetHandStatus(0) == MInputHandStatus.Grip)
            {
                handIndex = 0;
                isRightGrip = true;
            }
            else if (MOperateManager.GetHandStatus(0) == MInputHandStatus.Idle)
            {
                isRightGrip = false;
            }

            if (MOperateManager.GetHandStatus(1) == MInputHandStatus.Grip)
            {
                handIndex = 1;
                isLeftGrip = true;
            }
            else if (MOperateManager.GetHandStatus(1) == MInputHandStatus.Idle)
            {
                handIndex = 0;
                isLeftGrip = false;
            }

            if (IsRotate)                                                            // 旋转
            {

                if (isLeftUIEnable != null && isRightUIEnable != null && isLeftUIEnable.Value && !isRightUIEnable.Value)
                {
                    if (isLeftGrip && InputHands[1].ScreenVector.magnitude > 2)
                    {
                        isLeftRotate = true;
                    }
                    else
                    {
                        isLeftRotate = false;
                    }
                }

                if (isLeftUIEnable != null && isRightUIEnable != null && isLeftUIEnable.Value && isRightUIEnable.Value)
                {
                    if (!isRightGrip)
                    {
                        if (isLeftGrip&& InputHands[1].ScreenVector.magnitude > 2)
                        {
                            isLeftRotate = true;
                        }
                        else
                        {
                            isLeftRotate = false;
                        }
                    }
                }

                if (isLeftUIEnable != null && !isLeftUIEnable.Value)
                {
                    isLeftRotate = false;
                }

                if (isLeftRotate)
                {
                    //Debug.Log("isLeftRotate");
                    if (!isTempLeftRotate && ActionConstraint.BindCount == 0&& InputHands[1].ScreenVector.magnitude > 2.0f)
                    {
                        //将动作记录到集合中
                        ActionConstraint.AddBind(ActionConstraint.Left_Camera_Rotate_Action);

                        isTempLeftRotate = true;
                    }

                    //已经存在旋转，并且在集合中记录
                    if (isTempLeftRotate && ActionConstraint.IsBind(ActionConstraint.Left_Camera_Rotate_Action))
                    {
                        EventCameraRotate.SendListener(InputHands[1].ScreenVector);
                    }

                }
                else
                {
                    if (isTempLeftRotate)
                    {
                        //Debug.Log("!!!!!!!!isLeftRotate");
                        if (ActionConstraint.IsBind(ActionConstraint.Left_Camera_Rotate_Action))
                        {
                            EventCameraRotate.SendListener(Vector3.zero);
                            ActionConstraint.RemoveBind(ActionConstraint.Left_Camera_Rotate_Action);
                        }
                        isTempLeftRotate = false;
                    }
                }


                if (isLeftUIEnable != null && isRightUIEnable != null && !isLeftUIEnable.Value && isRightUIEnable.Value)
                {
                    if (isRightGrip&& InputHands[0].ScreenVector.magnitude > 2.0f)
                    {
                        isRightRotate = true;
                    }
                    else
                    {
                        isRightRotate = false;
                    }
                }

                if (isLeftUIEnable != null && isRightUIEnable != null && isLeftUIEnable.Value && isRightUIEnable.Value)
                {
                    if (!isLeftGrip)
                    {

                        if (isRightGrip&& InputHands[0].ScreenVector.magnitude > 2.0f)
                        {
                            isRightRotate = true;
                        }
                        else
                        {
                            isRightRotate = false;
                        }
                    }
                }

                if (isRightUIEnable != null && !isRightUIEnable.Value)
                {
                    isRightRotate = false;
                }

                if (isRightRotate)
                {
                    //Debug.Log("isRightRotate");
                    if (!isTempRightRotate && ActionConstraint.BindCount == 0 && InputHands[0].ScreenVector.magnitude > 2.0f)
                    {
                        //将动作记录到集合中
                        ActionConstraint.AddBind(ActionConstraint.Right_Camera_Rotate_Action);

                        isTempRightRotate = true;
                    }

                    //已经存在旋转，并且在集合中记录
                    if (isTempRightRotate && ActionConstraint.IsBind(ActionConstraint.Right_Camera_Rotate_Action))
                    {
                        EventCameraRotate.SendListener(InputHands[0].ScreenVector);
                    }

                }
                else
                {
                    if (isTempRightRotate)
                    {
                        //Debug.Log("!!!!!!!!isRightRotate");
                        if (ActionConstraint.IsBind(ActionConstraint.Right_Camera_Rotate_Action))
                        {
                            EventCameraRotate.SendListener(Vector3.zero);
                            ActionConstraint.RemoveBind(ActionConstraint.Right_Camera_Rotate_Action);
                        }
                        isTempRightRotate = false;
                    }
                }

            }


            if (IsZoom)                                                                 // 缩放
            {
                if (isLeftUIEnable != null && isRightUIEnable != null && isLeftUIEnable.Value && isRightUIEnable.Value)
                {
                    if (isRightGrip && isLeftGrip)
                    {
                        IsRotate = false;
                        isHandsZoom = true;
                    }
                    else { isHandsZoom = false; IsRotate = true; }
                }
                else { isHandsZoom = false; IsRotate = true; }

                if (isHandsZoom)
                {
                    //Debug.Log("isHandsZoom");
                    if (!isTempZoom && !ActionConstraint.IsBind(ActionConstraint.Camera_Zoom_Action))
                    {
                        ActionConstraint.AddBind(ActionConstraint.Camera_Zoom_Action);
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
                        if (result != 0 && isTempZoom && ActionConstraint.IsBind(ActionConstraint.Camera_Zoom_Action))
                        {
                            EventCameraZoom.SendListener(result);
                        }
                    }
                }
                else
                {
                    if (isTempZoom && !isRightGrip && !isLeftGrip && twoHandDistance != null)
                    {
                        twoHandDistance.TwoHandIdle();
                        if (IsZoom && ActionConstraint.IsBind(ActionConstraint.Camera_Zoom_Action))
                        {
                            EventCameraZoom.SendListener(0);
                        }
                        isTempZoom = false;
                        if (ActionConstraint.IsBind(ActionConstraint.Camera_Zoom_Action))
                            ActionConstraint.RemoveBind(ActionConstraint.Camera_Zoom_Action);
                    }
                }

            }

        }

        // 单手操作处理
        //void KinectHandOne()
        //{
        //    if (MInputKinect.IsHandActive(0))
        //    {
        //        handIndex = 0;
        //    }
        //    else if (MInputKinect.IsHandActive(1))
        //    {
        //        handIndex = 1;
        //    }

        //    //将他的屏幕坐标传递出去
        //    InputHands[0].OnUpdate(MInputKinect.ScreenHandPostion(handIndex));

        //    if (MInputKinect.HandGrip(handIndex))
        //        InputHands[0].SetGrip();

        //    if (MInputKinect.HandRelease(handIndex))
        //        InputHands[0].SetIdle();

        //    if (rightOperateObject != null)
        //    {
        //        switch (InputHands[0].HandStatus)
        //        {
        //            case MInputHandStatus.Grabing:

        //                var screenDevice = MUtility.MainWorldToScreenPoint(rightOperateObject.GrabObject.transform.position);

        //                Vector3 screenMouse = InputHands[0].ScreenPoint;
        //                Vector3 vPos = MUtility.MainScreenToWorldPoint(new Vector3(screenMouse.x, screenMouse.y, screenDevice.z));

        //                rightOperateObject.GrabObject.transform.position = vPos - rightOffset;

        //                //需要处理偏移量

        //                break;
        //            case MInputHandStatus.Idle:

        //                this.rightOperateObject = null;
        //                break;
        //            default:
        //                break;
        //        }
        //    }
        //}

        void KinectHandOne()
        {
            //Debug.Log(" 左手激活状态： " + MInputKinect.IsHandActive(1));
            //Debug.Log(isLeft);
            //if (MInputKinect.IsHandActive(0) /*&& !MInputKinect.IsHandActive(1)*/ && isRight == false)
            //{
            //    MOperateManager.GetOperateHand(0,OperatePlatform.Kinect).OnEnable();
            //    MOperateManager.GetOperateHand(1,OperatePlatform.Kinect).OnDisable(); 
            //    isRight = true;
            //    Debug.Log("右手");
            //}
            //else
            //{
            //    isRight = false;
            //}

            //if (MInputKinect.IsHandActive(1) /*&& !MInputKinect.IsHandActive(0)*/ && isLeft == false)
            //{
            //    Debug.Log("左手");
            //    MOperateManager.GetOperateHand(1,OperatePlatform.Kinect).OnEnable();
            //    MOperateManager.GetOperateHand(0,OperatePlatform.Kinect).OnDisable(); 
            //    isLeft = true;
            //}
            //else
            //{
            //    isLeft = false;
            //}

            if (isRight)
            {
                InputHands[0].OnUpdate(MInputKinect.ScreenHandPostion(0));

                if (MInputKinect.HandGrip(0))
                {
                    InputHands[0].SetGrip();
                }

                if (MInputKinect.HandRelease(0))
                {
                    InputHands[0].SetIdle();
                }

                if (isRightUIEnable != null && !isRightUIEnable.Value)
                {
                    MOperateManager.GetOperateHand(0,OperatePlatform.Kinect).OnEnable();
                    isRightUIEnable = true;
                }
            }
            else
            if (isLeft)
            {
                InputHands[1].OnUpdate(MInputKinect.ScreenHandPostion(1));

                if (MInputKinect.HandGrip(1))
                    InputHands[1].SetGrip();

                if (MInputKinect.HandRelease(1))
                    InputHands[1].SetIdle();

                if (isLeftUIEnable!= null && !isLeftUIEnable.Value)
                {
                    MOperateManager.GetOperateHand(1,OperatePlatform.Kinect).OnEnable();
                    InputHands[1].SetIdle();
                    isLeftUIEnable = true;
                }
            }

            if (isRightUIEnable == null)
            {
                MOperateManager.GetOperateHand(0,OperatePlatform.Kinect).OnDisable();
                isRightUIEnable = false;
            }
            if (isLeftUIEnable == null)
            {
                MOperateManager.GetOperateHand(1,OperatePlatform.Kinect).OnDisable();
                isLeftUIEnable = false;
            }

            if (rightOperateObject != null)
            {
                switch (InputHands[0].HandStatus)
                {
                    case MInputHandStatus.Grabing:
                        //if (ActionConstraint.BindCount==0)
                        //{
                        //    ActionConstraint.AddBind(ActionConstraint.Grab_Action);
                        //}
                        var screenDevice = MUtility.MainWorldToScreenPoint(rightOperateObject.GrabObject.transform.position);

                        Vector3 screenMouse = InputHands[0].ScreenPoint;
                        Vector3 vPos = MUtility.MainScreenToWorldPoint(new Vector3(screenMouse.x,screenMouse.y,screenDevice.z));

                        //rightOperateObject.GrabObject.transform.position = vPos - rightOffset;

                        Vector3 position = vPos - rightOffset;

                        EventUpdateObject.SendListener(rightOperateObject.GrabObject,position,rightOperateObject.GrabObject.transform.rotation,InputHands[0].HandIndex);

                        //需要处理偏移量

                        break;
                    case MInputHandStatus.Idle:
                        //if (ActionConstraint.BindCount>0)
                        //{
                        //    ActionConstraint.RemoveBind(ActionConstraint.Grab_Action);
                        //}
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
                        //if (ActionConstraint.BindCount==0)
                        //{
                        //    ActionConstraint.AddBind(ActionConstraint.Grab_Action);
                        //}
                        var screenDevice = MUtility.MainWorldToScreenPoint(leftOperateObject.GrabObject.transform.position);

                        Vector3 screenMouse = InputHands[1].ScreenPoint;
                        Vector3 vPos = MUtility.MainScreenToWorldPoint(new Vector3(screenMouse.x,screenMouse.y,screenDevice.z));

                        //leftOperateObject.GrabObject.transform.position = vPos - leftOffset;

                        Vector3 position = vPos - leftOffset;

                        EventUpdateObject.SendListener(leftOperateObject.GrabObject,position,leftOperateObject.GrabObject.transform.rotation,InputHands[0].HandIndex);

                        //需要处理偏移量

                        break;
                    case MInputHandStatus.Idle:
                        //if (ActionConstraint.BindCount>0)
                        //{
                        //    ActionConstraint.RemoveBind(ActionConstraint.Grab_Action);
                        //}
                        this.leftOperateObject = null;
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
        void OnRightGrabObject(IOperateObject operate,int handIndex)
        {
            if (handIndex != InputHands[0].HandIndex) return;

            rightOffset = GetOffsetPosition(InputHands[0].ScreenPoint,operate.GrabObject);

            this.rightOperateObject = operate;
        }

        /// <summary>
        /// 移动设置
        /// </summary>
        /// <param name="operate"></param>
        /// <param name="handIndex"></param>
        void OnLeftGrabObject(IOperateObject operate,int handIndex)
        {
            if (handIndex != InputHands[1].HandIndex) return;

            leftOffset = GetOffsetPosition(InputHands[1].ScreenPoint,operate.GrabObject);
            this.leftOperateObject = operate;
        }

        /// <summary>
        /// 计算适口偏移值
        /// </summary>
        /// <param name="mousePosition"></param>
        /// <param name="grabObject"></param>
        /// <returns></returns>
        private Vector3 GetOffsetPosition(Vector3 mousePosition,GameObject grabObject)
        {
            var offset = Vector3.zero;
            Vector3 screenDevice = MUtility.MainWorldToScreenPoint(grabObject.transform.position);
            Vector3 vPos = MUtility.MainScreenToWorldPoint(new Vector3(mousePosition.x,mousePosition.y,screenDevice.z));

            offset = vPos - grabObject.transform.position;

            return offset;
        }

        /// <summary>
        /// 设置物体被抓取
        /// </summary>
        /// <param name="operate"></param>
        /// <param name="handIndex"></param>
        /// <param name="cameraRelativeDistance"></param>
        void SetRightGrabObject(IOperateObject operate,int handIndex,float cameraRelativeDistance)
        {
            if (handIndex != InputHands[0].HandIndex) return;

            //Vector3 screenDevice = MUtility.MainWorldToScreenPoint(operate.GrabObject.transform.position);
            Vector3 screenpoint = InputHands[0].ScreenPoint;
            rightOperateObject = operate;

            Vector3 screenMainCamera = MUtility.MainWorldToScreenPoint(MUtility.MainCamera.transform.position
                + MUtility.MainCamera.transform.forward * cameraRelativeDistance);

            Vector3 position = MUtility.MainScreenToWorldPoint(new Vector3(screenpoint.x,screenpoint.y,screenMainCamera.z));

            rightOffset = Vector3.zero;

            rightOperateObject.GrabObject.transform.position = position;
        }

        void SetLeftGrabObject(IOperateObject operate,int handIndex,float cameraRelativeDistance)
        {
            if (handIndex != InputHands[1].HandIndex) return;

            //Vector3 screenDevice = MUtility.MainWorldToScreenPoint(operate.GrabObject.transform.position);
            Vector3 screenpoint = InputHands[1].ScreenPoint;
            rightOperateObject = operate;

            Vector3 screenMainCamera = MUtility.MainWorldToScreenPoint(MUtility.MainCamera.transform.position
                + MUtility.MainCamera.transform.forward * cameraRelativeDistance);

            Vector3 position = MUtility.MainScreenToWorldPoint(new Vector3(screenpoint.x,screenpoint.y,screenMainCamera.z));

            leftOffset = Vector3.zero;

            rightOperateObject.GrabObject.transform.position = position;
        }

        public void StartOnlyHand()
        {
            kinectActiveHandStatus = KinectActiveHandStadus.One;
            KinectConfig.SetHandStartStatus(kinectActiveHandStatus); // 设置单手操作

            //Debug.Log("右手： " + InputHands[0].ScreenPoint+ MInputKinect.IsHandActive(0) +  "                 左手： " + InputHands[1].ScreenPoint + MInputKinect.IsHandActive(1));

            //if (MInputKinect.IsHandActive(0) && !MInputKinect.IsHandActive(1))
            //{
            //    MOperateManager.GetOperateHand(0,OperatePlatform.Kinect).OnEnable();
            //    MOperateManager.GetOperateHand(1,OperatePlatform.Kinect).OnDisable();

            //}
            //else if (MInputKinect.IsHandActive(1) && !MInputKinect.IsHandActive(0))
            //{
            //    MOperateManager.GetOperateHand(1,OperatePlatform.Kinect).OnEnable();
            //    MOperateManager.GetOperateHand(0,OperatePlatform.Kinect).OnDisable();
            //}
        }

        public void StartMultipleHand()
        {
            kinectActiveHandStatus = KinectActiveHandStadus.Two;
            KinectConfig.SetHandStartStatus(kinectActiveHandStatus); // 设置双手操作

            //MOperateManager.GetOperateHand(1,OperatePlatform.Kinect).OnEnable();
            //MOperateManager.GetOperateHand(0,OperatePlatform.Kinect).OnEnable();
        }

    }
}