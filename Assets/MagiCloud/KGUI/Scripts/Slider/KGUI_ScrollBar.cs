using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using MagiCloud.Core.Events;

namespace MagiCloud.KGUI
{
    /*
    1、滚动条，分UI滚动和物体滚动
    2、UI滚动，根据设置的值，来进行计算
    3、物体滚动，计算出物体的最大移动范围和最小移动范围
         
    */


    /// <summary>
    /// KGUI滚动条
    /// </summary>
    [RequireComponent(typeof(BoxCollider))]
    public class KGUI_ScrollBar : KGUI_ButtonBase
    {
        public Axis KguiAxis = Axis.X;

        public Horizontal horizontal = Horizontal.LeftToRight;
        public Vertical vertical = Vertical.TopToBottom;

        public float? minValue, maxValue;
        public RectTransform handleRect;

        private bool IsDown = false;

        private int handIndex = -1; //同一时间段，只能是一个一只手操作滚动

        private float sumValue;

        public float _value = 0;
        public float _size = 0;

        public bool IsFullHandle = false;

        public EventFloat OnValueChanged;

        public UnityEvent OnRelease;

        public float Value {
            get {
                return _value;
            }

            set {

                _value = Mathf.Clamp(value, 0, 1);

                SetChangingValue(_value);

                if (OnValueChanged != null)
                {
                    OnValueChanged.Invoke(_value);
                }
            }
        }

        public float Size {
            get {
                return _size;
            }
            set {
                if (!IsFullHandle) return;
                _size = Mathf.Clamp(value, 0, 1);

                SetSize(_size);
            }
        }

        public override bool IsEnable {
            get {
                return base.IsEnable;
            }

            set {
                base.IsEnable = value;

                if (!value)
                    gameObject.SetActive(false);
                else
                {
                    gameObject.SetActive(true);
                    SetRectData();//进行刷新
                }
            }
        }

        protected override void OnStart()
        {
            base.OnStart();

            EventHandGrip.AddListener(OnButtonRelease, Core.ExecutionPriority.High);

            //KinectEventHandIdle.AddListener(EventLevel.A, OnButtonRelease);

            SetRectData();//进行刷新

            SetChangingValue(_value);
        }

        protected override void OnDestroy()
        {
            EventHandGrip.RemoveListener(OnButtonRelease);
            //KinectEventHandIdle.RemoveListener(OnButtonRelease);
        }

        private void Update()
        {

            if (IsDown && handIndex != -1 && IsEnable && enabled)
            {
                //屏幕坐标
                Vector3 screenPoint = MOperateManager.GetHandScreenPoint(handIndex);

                //将屏幕坐标传递出去
                OnExecute(screenPoint);
            }
        }

        public override void OnDown(int handIndex)
        {
            if (!enabled) return;
            base.OnDown(handIndex);

            if (this.handIndex != -1 && this.handIndex != handIndex) return;

            this.handIndex = handIndex;

            IsDown = true;
        }

        void OnButtonRelease(int handIndex)
        {
            if (!enabled) return;

            //如果当前手的值不等于-1，并且释放时，跟当前手不符，则直接跳过
            if (this.handIndex != -1 && this.handIndex != handIndex) return;

            if (IsDown)
            {
                if (OnRelease != null)
                    OnRelease.Invoke();
            }
            
            IsDown = false;
            handIndex = -1;

        }

        /// <summary>
        /// 设置RectTransform基本数据
        /// </summary>
        public void SetRectData()
        {
            if (handleRect == null)
            {
                Debug.LogError("请设置拖动滚动条参数");
                return;
            }
            //获取到屏幕坐标
            Vector3 scrollbarScreen = MUtility.UIWorldToScreenPoint(transform.position);

            RectTransform rectTransform = GetComponent<RectTransform>();

            if (KguiAxis == Axis.X)
            {
                //设置父物体左右两边的值
                float left = scrollbarScreen.x - rectTransform.sizeDelta.x / 2;
                float right = scrollbarScreen.x + rectTransform.sizeDelta.x / 2;

                //设置滚动条的最大可移动值和最小的可移动值
                minValue = left + handleRect.sizeDelta.x / 2;

                maxValue = right - handleRect.sizeDelta.x / 2;
            }

            if (KguiAxis == Axis.Y)
            {
                float top = scrollbarScreen.y + rectTransform.sizeDelta.y / 2;
                float bottom = scrollbarScreen.y - rectTransform.sizeDelta.y / 2;

                minValue = bottom + handleRect.sizeDelta.y / 2;
                maxValue = top - handleRect.sizeDelta.y / 2;
            }

            var box = GetComponent<BoxCollider>();
            box.size = new Vector3(rectTransform.sizeDelta.x, rectTransform.sizeDelta.y, 0);

            sumValue = Mathf.Abs(minValue.Value - maxValue.Value);
        }

        /// <summary>
        /// 抓取移动执行
        /// </summary>
        /// <param name="handPosition"></param>
        public void OnExecute(Vector3 handPosition)
        {
            Vector3 position = handPosition;

            if (minValue == null || maxValue == null)
                SetRectData();
            //Vector3 vpos;

            switch (KguiAxis)
            {
                case Axis.X:
                    
                    position.x = Mathf.Clamp(position.x, minValue.Value, maxValue.Value);

                    var xValue = Mathf.Abs(position.x - minValue.Value) / sumValue;//获取到此时屏幕坐标所占百分比

                    Value = horizontal == Horizontal.RightToLeft ? 1 - xValue : xValue;

                    break;
                case Axis.Y:

                    position.y = Mathf.Clamp(position.y, minValue.Value, maxValue.Value);
                    //Vector3 handle = GetScreenPoint(handleRect.position);

                    var yValue = Mathf.Abs(position.y - minValue.Value) / sumValue;

                    Value = vertical == Vertical.TopToBottom ? 1 - yValue : yValue;

                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 更改属性值
        /// </summary>
        /// <param name="value"></param>
        public void SetChangingValue(float value)
        {
            if (handleRect == null)
            {
                Debug.LogError("未指定当前滚动对象：handleRect属性为Null");
                return;
            }

            if (minValue == null || maxValue == null)
                SetRectData();

            Vector3 position = Vector3.zero;

            float moveValue = 0;

            switch (KguiAxis)
            {
                case Axis.X:
                    //if (horizontal == Horizontal.RightToLeft)
                    //    value = 1 - value;

                    moveValue = minValue.Value + value * sumValue;

                    //在将屏幕坐标转化为世界坐标

                    Vector3 handleScreenX = MUtility.UIWorldToScreenPoint(handleRect.position);

                    position = MUtility.UIScreenToWorldPoint(new Vector3(moveValue, handleScreenX.y, handleScreenX.z));
                    position.y = handleRect.position.y;
                    position.z = handleRect.position.z;

                    break;
                case Axis.Y:
                    //if (vertical == Vertical.TopToBottom)
                    //    value = 1 - value;

                    moveValue = maxValue.Value - value * sumValue;

                    Vector3 handleScreenY = MUtility.UIWorldToScreenPoint(handleRect.position);

                    position = MUtility.UIScreenToWorldPoint(new Vector3(handleScreenY.x, moveValue, handleScreenY.z));
                    position.x = handleRect.position.x;
                    position.z = handleRect.position.z;

                    break;
                default:
                    break;
            }

            handleRect.position = position;
        }

        /// <summary>
        /// 设置大小
        /// </summary>
        /// <param name="value"></param>
        public void SetSize(float value)
        {
            RectTransform rectTransform = GetComponent<RectTransform>();

            switch (KguiAxis)
            {
                case Axis.X:

                    handleRect.sizeDelta = new Vector2(value * rectTransform.sizeDelta.x, handleRect.sizeDelta.y);
                    break;
                case Axis.Y:

                    handleRect.sizeDelta = new Vector2(handleRect.sizeDelta.x, value * rectTransform.sizeDelta.y);
                    break;
                default:
                    break;
            }

            SetRectData();
        }
    }
}
