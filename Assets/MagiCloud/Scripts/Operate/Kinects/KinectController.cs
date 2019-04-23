using UnityEngine;
using MagiCloud.Core;
using MagiCloud.Core.Events;
using MagiCloud.Kinect;
using MagiCloud.Core.MInput;
using MagiCloud.Features;
using System.Collections.Generic;
using System;

namespace MagiCloud.Operate
{
    /// <summary>
    /// Kinect控制端
    /// 1、支持单双手操作
    /// 2、同时兼容鼠标操作
    ///     1）当识别到人物手时，鼠标禁止
    ///     2）当未识别到人的手时，鼠标开启
    /// 
    /// 思路：
    /// 1、当手势未识别到手时（双手没有一只手激活时，鼠标控制端启动）
    /// 2、当手势识别时，开启手势端。
    /// 3、当手势端有一个未开启的时候，需要禁用相应的操作，但是事件是否需要考虑待定。
    /// 4、比如握拳时，消失。在显示时，这时候需要做什么处理。
    /// 5、单双手的缩放，也需要优化。
    /// 
    /// 旋转思路：
    /// 1、检测是否支持旋转（条件：一只手握拳，另一只手松开，并且处于Idle/Grip状态下）
    /// 2、当开启状态后，设置IsRotateing为True，这时将缩放和另一只旋转的全部关闭判断
    /// 3、此时只检测这旋转情况，除非该手的旋转停止。
    /// 
    /// 缩放思路：
    /// 1、检测是否支持缩放（两只手同时满足Grip）
    /// 2、当开启状态后，设置IsZooming为ture，这时将旋转与抓取等都关闭判断
    /// 3、此时只检测缩放操作，除非停止操作。
    /// 
    /// </summary>
    public class KinectController :MonoBehaviour, IHandController
    {
        private bool isEnable = false;

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

                if (isEnable)
                {
                    enabled = true;

                    inputKinect.IsEnable = true;

                    behaviour = new MBehaviour(ExecutionPriority.Highest,-900);
                    behaviour.OnUpdate_MBehaviour(OnKinectUpdate);

                    //注册手势启动/停止事件
                    EventHandStart.AddListener(HandStart);
                    EventHandStop.AddListener(HandStop);
                }
                else
                {

                    inputKinect.IsEnable = false;

                    if (leftHandOperate != null)
                        leftHandOperate.Operate.OnDisable();

                    if (rightHandOperate != null)
                        rightHandOperate.Operate.OnDisable();


                    enabled = false;
                    behaviour.OnExcuteDestroy();

                    //移除手势启动/停止事件
                    EventHandStart.RemoveListener(HandStart);
                    EventHandStop.RemoveListener(HandStop);
                }
            }
        }

        /// <summary>
        /// 手操作相关数据
        /// </summary>
        [Serializable]
        public class HandOperate
        {
            /// <summary>
            /// 手图标
            /// </summary>
            public HandIcon handIcon;
            /// <summary>
            /// 手势操作端
            /// </summary>
            /// <value>The operate.</value>
            public MOperate Operate { get; set; }
            /// <summary>
            /// 针对手对物体的操作
            /// </summary>
            /// <value>The operate object.</value>
            public IOperateObject OperateObject { get; set; }
            /// <summary>
            /// 针对手与物体抓取时的偏移量
            /// </summary>
            /// <value>The offset.</value>
            public Vector3 Offset { get; set; }
            /// <summary>
            /// 计算最后一帧时的手坐标
            /// </summary>
            /// <value>The last hand position.</value>
            public Vector3 LastHandPos { get; set; }

            /// <summary>
            /// 手编号
            /// </summary>
            /// <value>The index of the hand.</value>
            public int HandIndex;

            /// <summary>
            /// 旋转
            /// </summary>
            public bool IsActioning = false;

            /// <summary>
            /// 绑定相关抓取事件
            /// </summary>
            public void BindGrab()
            {
                Operate.OnGrab += OnGrabObject;
                Operate.OnSetGrab += SetGrabObject;
                Operate.OnEnable();
            }

            /// <summary>
            /// 抓取设置
            /// </summary>
            /// <param name="operate">Operate.</param>
            /// <param name="handIndex">Hand index.</param>
            public void OnGrabObject(IOperateObject operate,int handIndex)
            {
                if (HandIndex != handIndex) return;
                Offset = MUtility.GetOffsetPosition(Operate.InputHand.ScreenPoint,operate.GrabObject);
                OperateObject = operate;
            }


            /// <summary>
            /// 设置物体被抓取
            /// </summary>
            /// <param name="operate">Operate.</param>
            /// <param name="handIndex">Hand index.</param>
            public void SetGrabObject(IOperateObject operate,int handIndex,float cameraRelativeDistance)
            {
                if (HandIndex != handIndex) return;
                Vector3 screenPoint = Operate.InputHand.ScreenPoint;
                OperateObject = operate;

                Vector3 screenMainCamera = MUtility.MainWorldToScreenPoint(MUtility.MainCamera.transform.position
                + MUtility.MainCamera.transform.forward * cameraRelativeDistance);

                Vector3 position = MUtility.MainScreenToWorldPoint(new Vector3(screenPoint.x,screenPoint.y,screenMainCamera.z));
                Offset = Vector3.zero;

                OperateObject.GrabObject.transform.position = position;
            }

            /// <summary>
            /// 针对OperateObject的处理
            /// </summary>
            public void OnOperateObjectHandle()
            {
                if (OperateObject==null) return;
                switch (Operate.InputHand.HandStatus)
                {
                    case MInputHandStatus.Grabing:

                        var screenDevice = MUtility.MainWorldToScreenPoint(OperateObject.GrabObject.transform.position);

                        var screenMouse = Operate.InputHand.ScreenPoint;
                        Vector3 vpos = MUtility.MainScreenToWorldPoint(new Vector3(screenMouse.x,screenMouse.y,screenDevice.z));

                        Vector3 position = vpos - Offset;

                        EventUpdateObject.SendListener(OperateObject.GrabObject,position,OperateObject.GrabObject.transform.rotation,HandIndex);

                        break;
                    case MInputHandStatus.Idle:

                        OperateObject = null;

                        break;
                }
            }

            /// <summary>
            /// 是否支持旋转
            /// </summary>
            /// <returns><c>true</c>, if rotate was ised, <c>false</c> otherwise.</returns>
            /// <param name="handOperate">Hand operate.</param>
            public bool IsAction(HandOperate handOperate)
            {
                return Operate.InputHand.HandStatus == MInputHandStatus.Grip &&
                    handOperate.Operate.InputHand.HandStatus == MInputHandStatus.Idle;
            }

            /// <summary>
            /// 手势动作的判定，用户来实现旋转和缩放
            /// </summary>
            /// <param name="handOperate">Hand operate.</param>
            public void OnGestureAction(HandOperate handOperate,bool isRotate)
            {
                switch (MSwitchManager.CurrentMode)
                {
                    case OperateModeType.Move:
                        break;
                    case OperateModeType.Rotate:
                        EventCameraRotate.SendListener(Operate.InputHand.ScreenVector);
                        break;
                    case OperateModeType.Zoom:
                        // EventCameraZoom.SendListener(Operate.InputHand.ScreenVector.x / 1200);
                        break;
                    case OperateModeType.Tool:
                        break;
                    default:
                        break;
                }
                #region old
                ////如果左手以及处于旋转状态，则没必要进行下一步了
                //if (!handOperate.IsActioning)
                //{
                //    if (IsAction(handOperate) && Operate.InputHand.ScreenVector.magnitude > 5)
                //    {
                //        if (isRotate)
                //            Operate.InputHand.HandStatus = MInputHandStatus.Rotate;
                //        else
                //            Operate.InputHand.HandStatus = MInputHandStatus.Zoom;

                //        IsActioning = true;
                //    }

                //    if (isRotate)
                //    {
                //        if (Operate.InputHand.IsRotateStatus)
                //        {
                //            EventCameraRotate.SendListener(Operate.InputHand.ScreenVector);
                //        }
                //        else
                //        {
                //            if (IsActioning)
                //            {
                //                EventCameraRotate.SendListener(Vector3.zero);
                //                IsActioning = false;
                //            }
                //        }
                //    }
                //    else
                //    {
                //        if (Operate.InputHand.IsZoomStatus)
                //        {
                //            EventCameraZoom.SendListener(Operate.InputHand.ScreenVector.x / 1200);
                //        }
                //        else
                //        {
                //            if (IsActioning)
                //            {
                //                EventCameraZoom.SendListener(0);
                //                IsActioning = false;
                //            }
                //        }
                //    }
                //} 
                #endregion
            }
        }

        public Vector2 handSize = new Vector2(50,50);

        public HandOperate rightHandOperate = new HandOperate() { HandIndex = 0 };
        public HandOperate leftHandOperate = new HandOperate() { HandIndex = 1 };

        public Dictionary<int,MInputHand> InputHands { get; set; }
        public bool IsPlaying { get; private set; }

        private MBehaviour behaviour;

        [SerializeField]
        private KinectHandModel handModel = KinectHandModel.Two;

        private MouseController mouseController; //鼠标控制端

        private bool IsZooming;
        private bool IsRotating;

        private MInputKinect inputKinect;

        public MInputHand GetInputHand(int handIndex)
        {
            MInputHand hand;

            InputHands.TryGetValue(handIndex,out hand);

            if (hand == null)
                throw new Exception("手势编号错误：" + handIndex);

            return hand;
        }

        /// <summary>
        /// Kinect控制端数据初始化
        /// </summary>
        /// <param name="handModel">Hand model.</param>
        private void KinectInitialize(KinectHandModel handModel)
        {
            InputHands = new Dictionary<int,MInputHand>();

            inputKinect = gameObject.GetComponent<MInputKinect>() ?? gameObject.AddComponent<MInputKinect>();

            MInputKinect.HandModel = handModel;
            IsPlaying = true;

            //实例化右手
            var rightHandUI = MHandUIManager.CreateHandUI(transform,rightHandOperate.handIcon);
            var rightInputHand = new MInputHand(rightHandOperate.HandIndex,rightHandUI,OperatePlatform.Kinect);
            rightHandUI.name = "kinect-Hand-0";
            InputHands.Add(rightHandOperate.HandIndex,rightInputHand);

            //实例化左手
            var leftHandUI = MHandUIManager.CreateHandUI(transform,leftHandOperate.handIcon);
            var leftInputHand = new MInputHand(leftHandOperate.HandIndex,leftHandUI,OperatePlatform.Kinect);
            leftHandUI.name = "Kinect-Hand-1";
            InputHands.Add(leftHandOperate.HandIndex,leftInputHand);

            //右手操作端相关初始化与事件绑定
            rightHandOperate.Operate = MOperateManager.AddOperateHand(rightInputHand,this);
            rightHandOperate.BindGrab();

            //左手操作端相关初始化与事件的绑定
            leftHandOperate.Operate = MOperateManager.AddOperateHand(leftInputHand,this);
            leftHandOperate.BindGrab();

            IsEnable = true;

            mouseController = gameObject.GetComponent<MouseController>() ?? gameObject.AddComponent<MouseController>();
            mouseController.IsEnable = false;

            //检查一次是否激活手
            HandStop(2);//默认也禁止
        }

        /// <summary>
        /// 手停止
        /// </summary>
        /// <param name="handIndex">Hand index.</param>
        private void HandStop(int handIndex)
        {
            switch (handIndex)
            {
                case 0:
                    rightHandOperate.Operate.OnDisable();
                    InputHands[0].SetIdle();

                    break;
                case 1:
                    leftHandOperate.Operate.OnDisable();
                    InputHands[1].SetIdle();

                    break;
                case 2:
                    leftHandOperate.Operate.OnDisable();
                    rightHandOperate.Operate.OnDisable();

                    InputHands[0].SetIdle();
                    InputHands[1].SetIdle();

                    break;
                default:
                    break;
            }

            ChangePlatform();
        }

        /// <summary>
        /// 变更平台
        /// </summary>
        void ChangePlatform()
        {
            if (!MInputKinect.IsHandActive(2))
            {
                if (MUtility.CurrentPlatform != OperatePlatform.Mouse)
                {
                    mouseController.IsEnable = true;
                    MUtility.CurrentPlatform = OperatePlatform.Mouse;

                    //MLog.WriteLog("切换：鼠标平台");
                    //Debug.Log("切换：鼠标平台");
                }
            }
            else
            {
                if (MUtility.CurrentPlatform != OperatePlatform.Kinect)
                {
                    mouseController.IsEnable = false;
                    MUtility.CurrentPlatform = OperatePlatform.Kinect;

                    //MLog.WriteLog("切换：Kinect平台");
                    //Debug.Log("切换：Kinect平台");
                }
            }
        }

        /// <summary>
        /// 手启动
        /// </summary>
        /// <param name="handIndex">Hand index.</param>
        private void HandStart(int handIndex)
        {
            switch (handIndex)
            {
                case 0:
                    rightHandOperate.Operate.OnEnable();
                    break;
                case 1:
                    leftHandOperate.Operate.OnEnable();
                    break;
                case 2:
                    leftHandOperate.Operate.OnEnable();
                    rightHandOperate.Operate.OnEnable();
                    break;
                default:
                    break;
            }

            //根据手势的启用情况，设置他的状态
            ChangePlatform();
        }

        void Awake()
        {
            KinectInitialize(handModel);
            IsEnable = true;
        }

        private OperateModeType operateModeType;
        private Vector2 lastLeftPos;
        private Vector2 lastRightPos;

        void OnKinectUpdate()
        {
            if (!isEnable) return;

            SetHandStatus(0); //右手
            SetHandStatus(1); //左手

            rightHandOperate.OnOperateObjectHandle();
            leftHandOperate.OnOperateObjectHandle();

            //不同模式中的不同操作
            switch (MSwitchManager.CurrentMode)
            {
                //不用区分是左手旋转还是右手旋转，双手使用时会切换到缩放模式
                case OperateModeType.Rotate:
                    if (GetInputHand(0).HandStatus==MInputHandStatus.Grip)
                        rightHandOperate.OnGestureAction(leftHandOperate,true);
                    if (GetInputHand(1).HandStatus==MInputHandStatus.Grip)
                        leftHandOperate.OnGestureAction(rightHandOperate,true);
                    break;
                case OperateModeType.Zoom:
                    Vector2 left = leftHandOperate.Operate.InputHand.ScreenPoint;
                    Vector2 right = rightHandOperate.Operate.InputHand.ScreenPoint;
                    if (operateModeType!=MSwitchManager.CurrentMode)
                    {
                        lastLeftPos=left;
                        lastRightPos=right;
                    }
                    float lastDis = Vector2.Distance(lastRightPos,lastLeftPos);
                    float dis = Vector2.Distance(left,right);
                    float offset = (dis - lastDis) / 1200;
                    EventCameraZoom.SendListener(offset);
                    lastLeftPos=left;
                    lastRightPos=right;
                    //rightHandOperate.OnGestureAction(leftHandOperate,false);
                    //leftHandOperate.OnGestureAction(rightHandOperate,false);
                    break;
                case OperateModeType.Tool:
                    break;
                default:
                    break;
            }
            operateModeType = MSwitchManager.CurrentMode;

        }

        /// <summary>
        /// 设置手的状态
        /// </summary>
        /// <param name="handIndex">Hand index.</param>
        private void SetHandStatus(int handIndex)
        {
            if (MInputKinect.IsHandActive(handIndex))
            {
                //发送抓取事件等相关事件
                InputHands[handIndex].OnUpdate(MInputKinect.ScreenHandPostion(handIndex));

                if (MInputKinect.HandGrip(handIndex))
                {
                    InputHands[handIndex].SetGrip();
                }

                if (MInputKinect.HandRelease(handIndex))
                {
                    InputHands[handIndex].SetIdle();
                }

                if (MInputKinect.HandLasso(handIndex))
                {
                    InputHands[handIndex].SetLasso();
                }
            }
        }

        /// <summary>
        /// 切换多手
        /// </summary>
        public void StartMultipleHand()
        {
            MInputKinect.HandModel = KinectHandModel.Two;
        }

        /// <summary>
        /// 切换单手
        /// </summary>
        public void StartOnlyHand()
        {
            MInputKinect.HandModel = KinectHandModel.One;
        }

        public void DisableHand()
        {
            MInputKinect.HandModel = KinectHandModel.None;
        }
    }
}

