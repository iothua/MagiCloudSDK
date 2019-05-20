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
        public GameObject UIObj => uiObject;
        private bool isEnter;
        public bool IsButtonPress;

        private bool isEnable;

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
                InputHand.HandStatus = Core.MInputHandStatus.Pressed; //设置为UI按下
                IsButtonPress = true;

                currentButton.OnDown(handIndex);
            }
        }

        public bool OnUIRay(Ray ray)
        {
            //如果不是松手或者握拳，返回false
            if (!(InputHand.IsIdleStatus ||
                InputHand.IsGripStatus))
            {
                return false;
            }

            //如果不是长按并且握拳，返回false
            if (!IsButtonPress && InputHand.IsGripStatus)
            {
                //MLog.WriteLog(InputHand.HandIndex + "握拳");
                return false;
            }

            if (!IsEnable) return false;

            RaycastHit hit;

            //持续按下
            if (IsButtonPress && currentButton != null)
            {
                currentButton.OnDownStay(InputHand.HandIndex);
            }

            if (Physics.Raycast(ray,out hit,10000,1 << MOperateManager.layerUI))
            {
                //1、如果照射到了，将此碰撞体加0.5f
                //2、如果是握拳时，碰撞体范围还是以0.5f为算，如果默认，则以0.5为计算

                if (uiObject == null)
                {
                    uiObject = hit.collider.gameObject;
                    EventHandUIRayEnter.SendListener(uiObject,InputHand.HandIndex);
                }
                else if (uiObject != hit.collider.gameObject)
                {
                    EventHandUIRayExit.SendListener(uiObject,InputHand.HandIndex);

                    uiObject = hit.collider.gameObject;
                    EventHandUIRayEnter.SendListener(uiObject,InputHand.HandIndex);
                }
                else
                { }

                if (hit.collider.gameObject.CompareTag("button"))
                {
                    if (!IsButtonPress)
                    {
                        if (rayObject == null)
                        {
                            //移入
                            currentButton = hit.collider.GetComponent<IButton>();
                            if (currentButton == null) return true;

                            OnButtonEnter(hit.collider.gameObject);

                        }
                        else if (currentObject == hit.collider.gameObject)
                        {
                            //如果相等，计算下

                            //currentButton.Collider.IsShake = false;

                            //RaycastHit detectionHit;
                            ////如果在扫描时，不是这个物体则进行清空
                            //if (Physics.Raycast(ray,out detectionHit,10000,1 << MOperateManager.layerUI))
                            //{
                            //    if (currentObject != detectionHit.collider.gameObject)
                            //    {
                            //        ClearButton();
                            //        return true;
                            //    }
                            //    else
                            //    {
                            //        currentButton.Collider.IsShake = true;
                            //        return true;
                            //    }
                            //}
                            //else
                            //{
                            //    NotUIRay();
                            //    return false;
                            //}
                        }
                        else
                        {
                            ClearButton();

                            //移入
                            currentButton = hit.collider.GetComponent<IButton>();
                            if (currentButton == null) return true;

                            OnButtonEnter(hit.collider.gameObject);
                        }
                    }
                    else
                    {
                        //MLog.WriteLog(InputHand.HandIndex + "握拳：" + hit.collider.gameObject);

                        rayObject = hit.collider.gameObject;
                        //如果握拳，则进行处理
                    }
                }
                else
                {
                    NotUIRay();
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
                EventHandUIRayExit.SendListener(uiObject,InputHand.HandIndex);

                uiObject = null;
            }

            rayObject = null;
            if (IsButtonPress) return;

            ClearButton();

            //SetButton(null);
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
                    currentButton.OnUpRange(handIndex,false);

                //SetButton(null);
                ClearButton();

                currentButton = null;
                currentObject = null;
                IsScroll = false;
                rayObject = null;

                return;
            }

            if (currentButton != null)
                currentButton.OnUpRange(handIndex,true);


            if (currentButton != null && currentObject == rayObject && !IsScroll)
            {
                currentButton.OnClick(handIndex);
            }

            IsScroll = false;
        }

        void OnButtonEnter(GameObject hitObject)
        {
            //currentButton.Collider.IsShake = true;

            rayObject = hitObject;

            currentButton.OnEnter(InputHand.HandIndex);

            currentObject = rayObject;
        }

        /// <summary>
        /// 清空Button信息
        /// </summary>
        void ClearButton()
        {
            if (currentButton == null) return;

            try
            {
                currentButton.OnExit(InputHand.HandIndex);

                //清除原来的大小
                //currentButton.Collider.IsShake = false;

                currentButton = null;
                currentObject = null;
                rayObject = null;
            }
            catch
            {
                currentButton = null;
                currentObject = null;
                rayObject = null;
            }
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
