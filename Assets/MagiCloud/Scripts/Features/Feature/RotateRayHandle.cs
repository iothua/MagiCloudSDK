using MagiCloud.KGUI;
using UnityEngine;
using MagiCloud.Core.Events;

namespace MagiCloud.Features
{
    /// <summary>
    /// 旋转射线处理
    /// </summary>
    public class RotateRayHandle
    {
        public int handIndex;
        
        public HandOperaType operaType;

        public Ray objectRay, uiRay;

        public bool IsGrabObject = false;
        public bool IsButtonPress = false;

        public RotateRayHandle(int handIndex)
        {
            this.handIndex = handIndex;
        }

        public void OnEnable()
        {
            //KGUI的射线
            EventHandUIRay.AddListener(Instance_EventUIRay);
        }

        public void OnDistable()
        {
            //KGUI的射线
            EventHandUIRay.RemoveListener(Instance_EventUIRay);
        }

        private void Instance_EventUIRay(Ray ray, int handIndex)
        {
            if (handIndex != this.handIndex)
                return;

            this.uiRay = ray;
        }

        void OnGrabObject(GameObject target, int handIndex)
        {
            if (this.handIndex != handIndex) return;

            IsGrabObject = true;
        }

        void OnIdleObject(GameObject target, int handIndex)
        {
            if (this.handIndex != handIndex) return;

            IsGrabObject = false;
        }

        void OnPressedUI(KGUI_Base @base, int handIndex)
        {
            if (this.handIndex != handIndex) return;

            IsButtonPress = true;
        }

        void OnReleaseUI(KGUI_Base @base, int handIndex)
        {
            if (this.handIndex != handIndex) return;

            IsButtonPress = false;
        }

        /// <summary>
        /// 射线检测
        /// </summary>
        /// <param name="objectRay"></param>
        /// <param name="uiRay"></param>
        /// <returns></returns>
        public bool ObjectRayDetection()
        {
            if (RayHandle(objectRay, (1 << MOperateManager.layerObject) | (1 << MOperateManager.layerRay))) return true;

            return false;
        }

        /// <summary>
        /// UI射线检测
        /// </summary>
        /// <returns></returns>
        public bool UIRayDetection()
        {
            if (RayHandle(uiRay, KGUI_Utility.layerMask))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 射线处理
        /// </summary>
        /// <param name="ray"></param>
        /// <returns></returns>
        public bool RayHandle(Ray ray, LayerMask layerMask)
        {
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100000, layerMask))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
