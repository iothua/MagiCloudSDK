using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using MagiCloud.Core;
using MagiCloud.Core.Events;
using MagiCloud.Core.UI;

namespace MagiCloud.KGUI
{
    public enum PanelType
    {
        UI,
        Object
    }

    [System.Serializable]
    public class TransformSize
    {
        public Transform transform;
        public Vector2 panelSize;
    }

    /// <summary>
    /// 接收射线面板，不做任何处理
    /// </summary>
    public class KGUI_Panel :KGUI_Base
    {
        private bool IsDown = false;
        private int handIndex = -1;

        // [Title("容器的类型")]
        // [EnumToggleButtons, EnumPaging]
        public PanelType panelType = PanelType.UI;
        public float extentValue = 10;//幅度值

        // [ShowIf("panelType",PanelType.UI)]
        //[ListDrawerSettings(ShowIndexLabels = true,ShowPaging = true)]

        public List<RectTransform> OtherRects;

        [Space(10)]
        //[ShowIf("panelType",PanelType.Object)]
        //  [InfoBox("手动输入容器大小")]
        public Vector2Int panelSize;//容器大小

        // [ShowIf("panelType",PanelType.Object)]

        // [ListDrawerSettings(ShowIndexLabels = true,ListElementLabelName = "transform")]
        public List<TransformSize> OtherSizes;

        [Header("按下移动方向")]
        public ButtonEvent onDirectionX;//X轴方向
        public ButtonEvent onDirectionY;//Y轴方向


        [Header("移入与移出")]
        public ButtonEvent onEnter;
        public ButtonEvent onExit;
        public ButtonEvent onStay;
        [Header("按下与抬起")]
        public PanelEvent onDown;
        public PanelEvent onUp;

        [HideInInspector]
        public bool IsEnter;
        private int enterHandIndex = -1;//移入时的手

        private float? screenX; //临时X轴坐标
        private float? screenY; //临时Y轴坐标


        private bool isEnable = false;

        private bool IsButtonPressed = false;

        [HideInInspector]
        public int eventCount = 0;

        //private void Start()
        //{
        //    IsEnable = true;
        //}

        private void OnEnable()
        {
            if (!IsEnable)
                IsEnable = true;
        }

        private void OnDisable()
        {

            if (IsEnable)
                IsEnable = false;

            //Debug.Log("设置为false ：" + name);
        }

        public bool IsEnable
        {
            get
            {
                return isEnable;
            }
            set
            {

                isEnable = value;

                //if (isEnable)
                //    onExit?.Invoke(this.handIndex);

                IsButtonPressed = false;
                IsDown = false;
                handIndex = -1;
                screenX = null;
                screenY = null;
                IsEnter = false;

                if (isEnable)
                {

                    EventHandGrip.AddListener(OnButtonPressed,ExecutionPriority.High);
                    EventHandIdle.AddListener(OnButtonRelease,ExecutionPriority.High);
                    //KinectEventHandGrip.AddListener(EventLevel.A, OnButtonPressed);
                    //KinectEventHandIdle.AddListener(EventLevel.A, OnButtonRelease);

                }
                else
                {
                    EventHandGrip.RemoveListener(OnButtonPressed);
                    EventHandIdle.RemoveListener(OnButtonRelease);

                    //KinectEventHandGrip.RemoveListener(OnButtonPressed);
                    //KinectEventHandIdle.RemoveListener(OnButtonRelease);
                }
            }
        }

        private void Update()
        {
            if (!enabled) return;
            if (!Active) return;
            if (!IsEnable) return;

            OnEnterHandle(0);//右手
            OnEnterHandle(1);//左手

            if (!IsDown || handIndex == -1) return;
            //屏幕坐标
            var screenPoint = MOperateManager.GetHandScreenPoint(handIndex);

            //将屏幕坐标传递出去
            OnExecute(screenPoint);
        }

        /// <summary>
        /// 移入移出处理
        /// </summary>
        public void OnEnterHandle(int handIndex)
        {
            if (IsAreaContains(handIndex))
            {

                onStay?.Invoke(handIndex);
                //如果以及有手移入了，在移入则什么都不处理
                if (IsEnter) return;

                onEnter?.Invoke(handIndex);

                IsEnter = true;
                enterHandIndex = handIndex;
            }
            else
            {
                if (!IsEnter) return;

                if (enterHandIndex != handIndex)
                    return;

                onExit?.Invoke(handIndex);

                IsEnter = false;
                enterHandIndex = -1;
            }
        }

        /// <summary>
        /// 判断其他集合区域是否在范围内
        /// </summary>
        /// <returns></returns>
        private bool OtherRectAreaContains(int hand)
        {
            if (panelType == PanelType.UI)
            {
                if (OtherRects == null) return false;

                foreach (var item in OtherRects)
                {
                    if (MUtility.IsAreaContains(item,hand))
                    {
                        return true;//如果存在，就可以直接返回了
                    }
                }
            }
            else
            {
                if (OtherSizes == null) return false;

                foreach (var item in OtherSizes)
                {
                    var screenPoint = MUtility.UIWorldToScreenPoint(item.transform.position);

                    if (MUtility.IsAreaContains(screenPoint,item.panelSize,hand))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// 判断是否在区域内
        /// </summary>
        /// <param name="handIndex"></param>
        /// <returns></returns>
        private bool IsAreaContains(int handIndex)
        {
            if (panelType == PanelType.UI)
            {

                return MUtility.IsAreaContains(transform,handIndex) || OtherRectAreaContains(handIndex);
            }
            else
            {
                Vector3 screenPoint = MUtility.UIWorldToScreenPoint(transform.position);

                return MUtility.IsAreaContains(screenPoint,panelSize,handIndex) || OtherRectAreaContains(handIndex);
            }
        }

        private void OnButtonRelease(int handIndex)
        {

            //如果当前手的值不等于-1，并且释放时，跟当前手不符，则直接跳过
            if (this.handIndex != -1 && this.handIndex != handIndex) return;

            if (!enabled) return;

            if (!IsButtonPressed) return;

            IsButtonPressed = false;

            onUp?.Invoke(handIndex,IsAreaContains(handIndex));

            //OnUp(handIndex);

            IsDown = false;
            this.handIndex = -1;

            screenY = null;
            screenX = null;

        }

        /// <summary>
        /// 抓取移动执行
        /// </summary>
        /// <param name="handPosition"></param>
        public void OnExecute(Vector3 handPosition)
        {
            if (screenY == null)
                screenY = handPosition.y;

            //往上走
            if (handPosition.y - screenY > extentValue)
            {
                if (onDirectionY != null)
                {
                    onDirectionY.Invoke(1);

                    if (onDirectionY.GetPersistentEventCount() > 0 || eventCount > 0)
                    {
                        MOperateManager.GetUIOperate(handIndex).SetScroll();
                    }
                }
                //如果相减，大于某个值
                screenY = handPosition.y;
            }

            //往下走
            if (handPosition.y - screenY < -extentValue)
            {
                if (onDirectionY != null)
                {
                    onDirectionY.Invoke(-1);

                    if (onDirectionY.GetPersistentEventCount() > 0 || eventCount > 0)
                    {
                        //设置UI滚动
                        MOperateManager.GetUIOperate(handIndex).SetScroll();
                    }
                }

                screenY = handPosition.y;
            }

            if (screenX == null)
                screenX = handPosition.x;

            if (handPosition.x - screenX > extentValue)
            {
                if (onDirectionX != null)
                {
                    onDirectionX.Invoke(1);

                    if (onDirectionY.GetPersistentEventCount() > 0 || eventCount > 0)
                    {
                        MOperateManager.GetUIOperate(handIndex).SetScroll();
                    }
                }

                screenX = handPosition.x;
            }

            if (!(handPosition.x - screenX < -extentValue)) return;
            if (onDirectionX != null)
            {
                onDirectionX.Invoke(-1);

                if (onDirectionY.GetPersistentEventCount() > 0 || eventCount > 0)
                {
                    MOperateManager.GetUIOperate(handIndex).SetScroll();
                }
            }

            screenX = handPosition.x;

        }

        /// <summary>
        /// 按下时，判断是否在区域内
        /// </summary>
        /// <param name="handIndex"></param>
        public void OnButtonPressed(int handIndex)
        {
            if (this.handIndex != -1 && this.handIndex != handIndex) return;
            if (!enabled) return;

            if (IsButtonPressed)
                return;

            IsButtonPressed = true;

            if (IsAreaContains(handIndex))
            {
                this.handIndex = handIndex;

                IsDown = true;

                onDown?.Invoke(handIndex,true);

                //OnDown(handIndex);
            }
            else
            {
                onDown?.Invoke(handIndex,false);
            }

        }

        private void OnDestroy()
        {

            if (IsEnable)
                IsEnable = false;

            onDirectionX.RemoveAllListeners();
            onDirectionY.RemoveAllListeners();

            onEnter.RemoveAllListeners();
            onExit.RemoveAllListeners();

            onDown.RemoveAllListeners();
            onUp.RemoveAllListeners();

            eventCount = 0;
        }

        public void AddDirectionXListeners(UnityEngine.Events.UnityAction<int> action)
        {
            onDirectionX.AddListener(action);
            eventCount++;
        }

        public void RemoveDirectionXListeners(UnityEngine.Events.UnityAction<int> action)
        {
            onDirectionX.RemoveListener(action);
            eventCount--;
        }

        public void AddDirectionYListeners(UnityEngine.Events.UnityAction<int> action)
        {
            onDirectionY.AddListener(action);
            eventCount++;
        }

        public void RemoveDirectionYListeners(UnityEngine.Events.UnityAction<int> action)
        {
            onDirectionY.RemoveListener(action);
            eventCount--;
        }

    }
}

