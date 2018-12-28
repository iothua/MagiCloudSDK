using MagiCloud.KGUI;
using MagiCloud.Core.MInput;
using UnityEngine;
using MagiCloud.Core.Events;
using MagiCloud.Core.UI;

namespace MagiCloud
{
    /// <summary>
    /// UI控制端
    /// </summary>
    public class UIOperate
    {

        public bool IsScroll = false;

        private IButton currentButton;
        public GameObject currentObject;//当前抓取物体
        public GameObject rayObject;//射线照射物体
        private GameObject uiObject; //只要照射UI

        private bool isEnter;
        public bool IsButtonPress;

        private bool isEnable;

        /// <summary>
        /// 是否激活
        /// </summary>
        public bool IsEnable {
            get {
                return isEnable;
            }
            set {
                if (isEnable == value) return;
                isEnable = value;

                if (isEnable)
                {
                    EventHandGrip.AddListener(OnButtonPress);
                    EventHandIdle.AddListener(OnButtonRelease);
                }
                else
                {
                    EventHandGrip.RemoveListener(OnButtonPress);
                    EventHandIdle.RemoveListener(OnButtonRelease);
                }

                OnReset();
            }
        }

        /// <summary>
        /// 输入端
        /// </summary>
        public MInputHand InputHand { get; set; }

        public UIOperate(MInputHand inputHand)
        {
            this.InputHand = inputHand;
        }

        public void SetScroll()
        {
            IsScroll = true;
        }

        private void OnButtonPress(int handIndex)
        {
            if (handIndex != InputHand.HandIndex) return;

            if (currentButton != null)
            {
                currentButton.OnDown(handIndex);
                IsButtonPress = true;

            }
        }

        public bool OnUIRay(Ray ray)
        {
            if (!IsEnable) return false;

            RaycastHit hit;

            //持续按下
            if (IsButtonPress && currentButton != null)
            {
                currentButton.OnDownStay(InputHand.HandIndex);
            }

            if (Physics.Raycast(ray, out hit, 10000, 1 << MOperateManager.layerUI))
            {
                //1、如果照射到了，将此碰撞体加0.5f
                //2、如果是握拳时，碰撞体范围还是以0.5f为算，如果默认，则以0.5为计算

                if (uiObject == null)
                {
                    uiObject = hit.collider.gameObject;
                    EventHandUIRayEnter.SendListener(uiObject, InputHand.HandIndex);
                }
                else if (uiObject != hit.collider.gameObject)
                {
                    EventHandUIRayExit.SendListener(uiObject, InputHand.HandIndex);

                    uiObject = hit.collider.gameObject;
                    EventHandUIRayEnter.SendListener(uiObject, InputHand.HandIndex);
                }
                else
                { }

                if (hit.collider.gameObject.CompareTag("button"))
                {
                    //照射到的物体
                    rayObject = hit.collider.gameObject;

                    if (IsButtonPress || MOperateManager.GetHandStatus(InputHand.HandIndex) != Core.MInputHandStatus.Idle)
                        return true;

                    SetButton(rayObject);
                }
                else
                {
                    //如果没有照射到button，表示没有在UI上了。
                    rayObject = null;
                    if (IsButtonPress) return true;

                    SetButton(null);
                }

                return true;
            }
            else
            {
                NotUIRay();

                return false;
            }
        }

        /// <summary>
        /// Not UI射线处理
        /// </summary>
        void NotUIRay()
        {
            if (uiObject != null)
            {
                EventHandUIRayExit.SendListener(uiObject, InputHand.HandIndex);

                uiObject = null;
            }

            rayObject = null;
            if (IsButtonPress) return;
            SetButton(null);
        }


        private void OnButtonRelease(int handIndex)
        {

            if (handIndex != InputHand.HandIndex) return;

            if (!IsButtonPress)
            {
                IsScroll = false;
                return;
            }

            IsButtonPress = false;

            if (currentButton != null)
                currentButton.OnUp(handIndex);

            if (currentObject != rayObject)
            {
                if (currentButton != null)
                    currentButton.OnUpRange(handIndex, false);

                SetButton(null);

                currentButton = null;
                currentObject = null;
                IsScroll = false;

                return;
            }

            if (currentButton != null)
                currentButton.OnUpRange(handIndex, true);


            if (currentButton != null && currentObject == rayObject && !IsScroll)
            {
                currentButton.OnClick(handIndex);
            }

            IsScroll = false;
        }

        public void SetButton(GameObject target)
        {

            if (target == currentObject) return;

            if (target == null)
            {
                ClearButton();
                return;
            }

            if (target.CompareTag("button"))
            {
                ClearButton();

                currentButton = target.GetComponent<IButton>();
                if (currentButton == null) return;

                currentButton.OnEnter(InputHand.HandIndex);

                currentObject = target;

            }
            else
            {
                ClearButton();
            }
        }

        /// <summary>
        /// 清空Button信息
        /// </summary>
        public void ClearButton()
        {
            if (currentButton != null)
            {
                currentButton.OnExit(InputHand.HandIndex);
            }

            currentButton = null;
            currentObject = null;
        }

        /// <summary>
        /// 重置
        /// </summary>
        public void OnReset()
        {
            ClearButton();
            IsButtonPress = false;
        }

    }
}
