using UnityEngine;
using MagiCloud.Core.Events;

namespace MagiCloud.Core.MInput
{
    /// <summary>
    /// 手势(光标)
    /// </summary>
    public class MInputHand
    {
        private bool isPressed;

        private bool isError;

        private bool isEnable;

        private Vector3 currentPoint = Vector3.zero;//当前帧
        private Vector3? lastPoint;//上一帧

        private Vector3 lerpPoint = Vector3.zero; //差值帧

        private MInputHandStatus handStatus = MInputHandStatus.Idle;

        public IHandUI HandUI { get; private set; }

        /// <summary>
        /// 手势状态
        /// </summary>
        public MInputHandStatus HandStatus
        {
            get { return handStatus; }
            set
            {

                if (handStatus == value) return;

                handStatus = value;

                if (HandUI != null)
                    HandUI.SetHandIcon(handStatus);
            }
        }

        /// <summary>
        /// 手平台（以便多平台操作）
        /// </summary>
        public OperatePlatform Platform { get; private set; }

        /// <summary>
        /// 手编号
        /// </summary>
        public int HandIndex { get; set; }

        /// <summary>
        /// 是否按下
        /// </summary>
        public bool IsPressed
        {
            get
            {
                return isPressed;
            }
        }

        /// <summary>
        /// 是否激活
        /// </summary>
        public bool IsEnable
        {
            get
            {
                return isEnable;
            }
            set
            {

                if (!value)
                    SetIdle();

                isEnable = value;

                isPressed = false;
                isError = false;

                currentPoint = Vector3.zero;
                lastPoint = null;
                lerpPoint = Vector3.zero;

                if (HandUI != null)
                    HandUI.IsEnable = value;
            }
        }

        /// <summary>
        /// 屏幕坐标
        /// </summary>
        public Vector3 ScreenPoint
        {
            get
            {
                return currentPoint;
            }
        }

        /// <summary>
        /// 屏幕向量（当前帧与上一帧的差值）
        /// </summary>
        public Vector3 ScreenVector
        {
            get
            {
                return lerpPoint;
            }
        }

        public MInputHand(int handIndex,IHandUI handUI,OperatePlatform platform)
        {
            HandIndex = handIndex;
            HandUI = handUI;

            Platform = platform;
        }

        /// <summary>
        /// 每帧执行
        /// </summary>
        /// <param name="inputPoint">输入端世界坐标值</param>
        public void OnUpdate(Vector3 inputPoint)
        {
            if (!isEnable) return;

            currentPoint = inputPoint;

            if (lastPoint == null)
                lastPoint = currentPoint;

            //移动坐标
            if (HandUI != null)
                HandUI.MoveHand(ScreenPoint);
            //获取到射线
            Ray ray = MUtility.ScreenPointToRay(ScreenPoint);

            Ray uiRay = MUtility.UIScreenPointToRay(ScreenPoint);

            //发送射线信息
            EventHandRay.SendListener(ray,HandIndex);
            EventHandUIRay.SendListener(uiRay,HandIndex);
            EventHandRays.SendListener(ray,uiRay,HandIndex);

            //算直接坐标的插值
            lerpPoint = currentPoint - lastPoint.Value;

            //记录上一帧
            lastPoint = currentPoint;
        }

        /// <summary>
        /// 释放
        /// </summary>
        public void SetGrip()
        {
            if (!isEnable) return;

            if (!isError)
                if (isPressed) return;

            isPressed = true;
            isError = false;

            HandStatus = MInputHandStatus.Grip;
            lerpPoint=currentPoint;
            EventHandGrip.SendListener(HandIndex);
        }

        /// <summary>
        /// 抬起
        /// </summary>
        public void SetIdle()
        {
            if (!isEnable) return;

            if (!isError)
                if (!isPressed) return;

            isPressed = false;
            isError = false;

            HandStatus = MInputHandStatus.Idle;

            EventHandIdle.SendListener(HandIndex);
        }

        public void SetLasso()
        {
            if (!isEnable) return;
            isError = true;
            HandStatus = MInputHandStatus.Error;
        }

        /// <summary>
        /// 设置手势大小
        /// </summary>
        /// <param name="icon"></param>
        /// <param name="size"></param>
        public void SetHandIcon(Sprite icon,Vector2? size = null)
        {
            if (HandUI == null) return;

            HandUI.SetHandIcon(icon,size);
        }

        /// <summary>
        /// 设置默认图标
        /// </summary>
        public void SetNormalIcon()
        {
            if (HandUI == null) return;

            HandUI.SetNormalIcon();
        }

        /// <summary>
        /// 是否处于默认状态
        /// </summary>
        /// <value><c>true</c> if is idle status; otherwise, <c>false</c>.</value>
        public bool IsIdleStatus
        {
            get
            {
                return HandStatus == MInputHandStatus.Idle;
            }
        }

        /// <summary>
        /// 是否处于握拳的状态（Grip/Grab/Grabing/Pressed）
        /// </summary>
        /// <value><c>true</c> if is grip status; otherwise, <c>false</c>.</value>
        public bool IsGripStatus
        {
            get
            {
                return HandStatus == MInputHandStatus.Grip || HandStatus == MInputHandStatus.Grab
                || HandStatus == MInputHandStatus.Grabing || HandStatus == MInputHandStatus.Pressed;
            }
        }

        /// <summary>
        /// 是否处于错误状态（Error/Invalid）
        /// </summary>
        /// <value><c>true</c> if is error status; otherwise, <c>false</c>.</value>
        public bool IsErrorStatus
        {
            get
            {
                return HandStatus == MInputHandStatus.Error || HandStatus == MInputHandStatus.Invalid;
            }
        }

        /// <summary>
        /// 是否处于旋转状态
        /// </summary>
        /// <value><c>true</c> if is rotate; otherwise, <c>false</c>.</value>
        public bool IsRotateStatus
        {
            get
            {
                return HandStatus == MInputHandStatus.Rotate;
            }
        }

        /// <summary>
        /// 是否处于缩放状态
        /// </summary>
        /// <value><c>true</c> if is zoom status; otherwise, <c>false</c>.</value>
        public bool IsZoomStatus
        {
            get
            {
                return HandStatus == MInputHandStatus.Zoom;
            }
        }

        /// <summary>
        /// 是否处于旋转/缩放状态
        /// </summary>
        /// <value><c>true</c> if is rotate zoom status; otherwise, <c>false</c>.</value>
        public bool IsRotateZoomStatus
        {
            get
            {
                return HandStatus == MInputHandStatus.Rotate || HandStatus == MInputHandStatus.Zoom;
            }
        }
    }
}

