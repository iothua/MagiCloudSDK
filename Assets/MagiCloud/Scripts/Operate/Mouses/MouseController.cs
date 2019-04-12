using System;
using UnityEngine;
using System.Collections.Generic;
using MagiCloud.Core.MInput;
using MagiCloud.Core.Events;
using MagiCloud.Core;
using MagiCloud.Features;
using Utility;

namespace MagiCloud.Operate
{


    /// <summary>
    /// 鼠标控制端
    /// </summary>
    public class MouseController :MonoBehaviour, IHandController
    {
        public class ObservedMode
        {
            private bool IsObserved;
            private bool IsDown;
            Touch lastTouch1;
            Touch lastTouch2;
            /// <summary>
            /// 按下时
            /// </summary>
            public void OnDown()
            {
                IsDown = true;
                IsObserved = false;
            }

            /// <summary>
            /// 抬起时
            /// </summary>
            /// <param name="IsRotate"></param>
            public void OnUp(bool IsRotate)
            {
                IsDown = false;

                //已经存在旋转，并且在集合中记录
                if (IsObserved)
                {
                    if (IsRotate)
                        EventCameraRotate.SendListener(Vector3.zero);
                    else
                        EventCameraZoom.SendListener(0);
                }

                IsObserved = false;
            }

            /// <summary>
            /// 具体实现，是旋转还是缩放
            /// </summary>
            /// <param name="inputHand"></param>
            /// <param name="IsRotate"></param>
            public void OnAchieve(MInputHand inputHand,bool IsRotate)
            {
                ////如果按下右键
                //if (Input.GetMouseButtonDown(0))
                //{
                //    IsDown = true;
                //    IsObserved = false;
                //}

                //if (Input.GetMouseButtonUp(0))
                //{
                //    IsDown = false;

                //    //已经存在旋转，并且在集合中记录
                //    if (IsObserved)
                //    {
                //        if (IsRotate)
                //            EventCameraRotate.SendListener(Vector3.zero);
                //        else
                //            EventCameraZoom.SendListener(0);
                //    }

                //    IsObserved = false;
                //    inputHand.HandStatus = MInputHandStatus.Idle;
                //}

                //按住右键旋转
                if (IsDown)
                {
                    //向量的模大于2.0时
                    if (!IsObserved  && inputHand.ScreenVector.magnitude > 2.0f)
                    {
                        //将动作记录到集合中
                        inputHand.HandStatus = IsRotate ? MInputHandStatus.Rotate : MInputHandStatus.Zoom;

                        IsObserved = true;
                    }

                    //已经存在旋转，并且在集合中记录
                    if (IsObserved)
                    {
                        if (IsRotate)
                        {
                            Vector3 vector = inputHand.ScreenVector;
                            EventCameraRotate.SendListener(vector);
                        }
                        else
                        {
                            EventCameraZoom.SendListener(inputHand.ScreenVector.x / 1200);
                        }
                    }
                }
            }

            public void OnZoom()
            {
                if (Input.touchCount>=2)
                {
                    Touch touch1 = Input.GetTouch(0);
                    Touch touch2 = Input.GetTouch(1);
                    if (touch2.phase==TouchPhase.Began)
                    {
                        lastTouch1=touch1;
                        lastTouch2=touch2;
                        return;
                    }
                    float lastDis = Vector2.Distance(lastTouch1.position,lastTouch2.position);
                    float dis = Vector2.Distance(touch1.position,touch2.position);
                    float offset = (dis-lastDis)*0.001f;
                    //float dir = 1;
                    //if (offset<0)
                    //    dir=-1;
                    //offset=MathHelper.Damping(offset,10)*dir;
                    EventCameraZoom.SendListener(offset);
                    lastTouch1=touch1;
                    lastTouch2=touch2;
                }
                else
                {
                    float offset = Input.GetAxis("Mouse ScrollWheel");

                    //  offset =MathHelper.Damping(offset,10);
                    EventCameraZoom.SendListener(offset);
                }
            }


        }

        private MBehaviour behaviour;

        [Header("手图标")]
        public HandIcon handSprite;//手图标

        [Header("图标大小")]
        public Vector2 handSize;//图标大小

        private bool isPlaying = false;

        private IOperateObject operateObject;

        private Vector3 offset;
        private bool isEnable;
        private MOperate operate;

        //观察模式的具体实现
        private ObservedMode observedMode = new ObservedMode();

        /// <summary>
        /// 是否触摸
        /// </summary>
        public bool IsTouching = false;
        /// <summary>
        /// 是否鼠标
        /// </summary>
        public bool IsMousing = false;

        /// <summary>
        /// 输入端
        /// </summary>
        public Dictionary<int,MInputHand> InputHands
        {
            get; set;
        }

        /// <summary>
        /// 是否启动
        /// </summary>
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

                if (isEnable)
                {
                    behaviour = new MBehaviour(ExecutionPriority.Highest,-900);
                    behaviour.OnUpdate_MBehaviour(OnMouseUpdate);

                    enabled = true;
                    operate.OnEnable();
                }
                else
                {

                    enabled = false;
                    operate.OnDisable();

                    behaviour.OnExcuteDestroy();
                }
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
            behaviour = new MBehaviour(ExecutionPriority.Highest,-900,enabled);

            InputHands = new Dictionary<int,MInputHand>();

            //初始化手的种类
            var handUI = MHandUIManager.CreateHandUI(transform,handSprite,handSize);
            var inputHand = new MInputHand(0,handUI,OperatePlatform.Mouse);
            handUI.name = "Mouse-Hand";

            InputHands.Add(0,inputHand);

            isPlaying = true;

            //注册操作者相关事件
            operate = MOperateManager.AddOperateHand(inputHand,this);
            //注册方法
            operate.OnGrab = OnGrabObject;
            operate.OnSetGrab = SetGrabObject;

            IsEnable = true;
        }

        /// <summary>
        /// 鼠标控制端的妹帧执行
        /// </summary>
        void OnMouseUpdate()
        {
            if (!IsEnable) return;

            //将他的屏幕坐标传递出去
            InputHands[0].OnUpdate(Input.mousePosition);

            #region 鼠标/触摸检测

            //触摸
            if (!IsMousing&&Input.touchCount >= 1)
            {
                InputHands[0].SetGrip();
                IsTouching = true;

                observedMode.OnDown();
            }

            if (Input.GetMouseButtonDown(0))
            {
                //鼠标
                if (!IsTouching&& InputHands[0].IsIdleStatus)
                {
                    InputHands[0].SetGrip();
                    IsMousing = true;

                    observedMode.OnDown();
                }
            }

            if (IsTouching && Input.touchCount == 0)
            {
                InputHands[0].SetIdle();
                IsTouching = false;

                ObservedModeUpHandler();
            }

            if (Input.GetMouseButtonUp(0))
            {
                if (IsMousing && !InputHands[0].IsErrorStatus)
                {
                    InputHands[0].SetIdle();
                    IsMousing = false;

                    ObservedModeUpHandler();
                }
            }

           


            #endregion

            //不同模式中的不同操作
            switch (MSwitchManager.CurrentMode)
            {
                case OperateModeType.Rotate:

                    observedMode.OnAchieve(InputHands[0],true);
                    break;
                case OperateModeType.Zoom:

                    //observedMode.OnAchieve(InputHands[0], false);
                    observedMode.OnZoom();
                    break;
                case OperateModeType.Tool:
                    break;
                default:
                    break;
            }

            if (operateObject != null)
            {
                switch (InputHands[0].HandStatus)
                {
                    case MInputHandStatus.Grabing:

                        //需要处理偏移量
                        var screenDevice = MUtility.MainWorldToScreenPoint(operateObject.GrabObject.transform.position);
                        Vector3 screenMouse = InputHands[0].ScreenPoint;
                        Vector3 vPos = MUtility.MainScreenToWorldPoint(new Vector3(screenMouse.x,screenMouse.y,screenDevice.z));

                        Vector3 position = vPos - offset;

                        EventUpdateObject.SendListener(operateObject.GrabObject,position,operateObject.GrabObject.transform.rotation,InputHands[0].HandIndex);

                        break;
                    case MInputHandStatus.Idle:

                        this.operateObject = null;
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// 观察模式时，释放处理
        /// </summary>
        private void ObservedModeUpHandler()
        {
            switch (MSwitchManager.CurrentMode)
            {
                case OperateModeType.Rotate:
                    observedMode.OnUp(true);

                    break;
                case OperateModeType.Zoom:
                    observedMode.OnUp(false);

                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 移动设置
        /// </summary>
        /// <param name="operate"></param>
        /// <param name="handIndex"></param>
        void OnGrabObject(IOperateObject operate,int handIndex)
        {
            if (handIndex != InputHands[0].HandIndex) return;

            offset = MUtility.GetOffsetPosition(InputHands[0].ScreenPoint,operate.GrabObject);

            this.operateObject = operate;
        }

        /// <summary>
        /// 设置物体被抓取
        /// </summary>
        /// <param name="operate"></param>
        /// <param name="handIndex"></param>
        /// <param name="cameraRelativeDistance"></param>
        void SetGrabObject(IOperateObject operate,int handIndex,float cameraRelativeDistance)
        {
            if (handIndex != InputHands[0].HandIndex) return;

            //Vector3 screenDevice = MUtility.MainWorldToScreenPoint(operate.GrabObject.transform.position);
            Vector3 screenpoint = InputHands[0].ScreenPoint;
            operateObject = operate;

            Vector3 screenMainCamera = MUtility.MainWorldToScreenPoint(MUtility.MainCamera.transform.position
                + MUtility.MainCamera.transform.forward * cameraRelativeDistance);

            Vector3 position = MUtility.MainScreenToWorldPoint(new Vector3(screenpoint.x,screenpoint.y,screenMainCamera.z));

            offset = Vector3.zero;

            operateObject.GrabObject.transform.position = position;
        }

        private void OnDestroy()
        {
            behaviour.OnExcuteDestroy();
        }

        public void StartOnlyHand()
        {
            //不做任何处理
        }

        public void StartMultipleHand()
        {
            //不用做任何处理
        }
    }
}
