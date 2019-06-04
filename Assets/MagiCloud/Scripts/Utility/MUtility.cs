using System;
using UnityEngine;

namespace MagiCloud
{
    /// <summary>
    /// 框架初始化
    /// 
    /// 标记相机是用来接受Kinect坐标信息的，所以要保证稳定，主相机不一样，凡是涉及到移动，射线，仍然是主相机发射
    /// 如果没有标记相机，特写的时候手势坐标就会不稳定
    /// </summary>
    public static class MUtility
    {
        private static Camera _markCamera;
        private static Camera _mainCamera;
        private static Camera _uiCamera;

        /// <summary>
        /// 发射线点
        /// </summary>
        public static Camera MarkCamera
        {
            get
            {
                if (_markCamera == null)
                    _markCamera = GameObject.FindGameObjectWithTag("MarkCamera").GetComponent<Camera>();

                return _markCamera;
            }
        }
        /// <summary>
        /// 主相机
        /// </summary>
        public static Camera MainCamera
        {
            get
            {
                if (_mainCamera == null)
                    _mainCamera = Camera.main;

                return _mainCamera;
            }
        }

        public static Camera UICamera
        {
            get
            {
                if (_uiCamera == null)
                {
                    var obj = GameObject.FindGameObjectWithTag("UICamera");
                    _uiCamera =(obj==null ? null : obj.GetComponent<Camera>());
                }
                return _uiCamera;
            }
        }

        public static Core.OperatePlatform CurrentPlatform;

        /// <summary>
        /// 世界坐标转屏幕坐标(标记相机为准)
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public static Vector3 MarkWorldToScreenPoint(Vector3 point)
        {
            if (MarkCamera == null)
                throw new Exception("markCamera相机对象为Null");

            return MarkCamera.WorldToScreenPoint(point);
        }

        /// <summary>
        /// 屏幕坐标转世界坐标(标记相机为准)
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public static Vector3 MarkScreenToWorldPoint(Vector3 point)
        {
            if (MarkCamera == null)
                throw new Exception("markCamera相机对象为Null");

            return MarkCamera.ScreenToWorldPoint(point);
        }

        /// <summary>
        /// 屏幕坐标转世界坐标(主相机为准)
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public static Vector3 MainWorldToScreenPoint(Vector3 point)
        {
            if (MainCamera == null)
                throw new Exception("mainCamera相机对象为Null");

            return MainCamera.WorldToScreenPoint(point);
        }

        /// <summary>
        /// 屏幕坐标转世界坐标(主相机为准)
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public static Vector3 MainScreenToWorldPoint(Vector3 point)
        {
            if (MainCamera == null)
                throw new Exception("mainCamera相机对象为Null");

            return MainCamera.ScreenToWorldPoint(point);
        }

        /// <summary>
        /// 屏幕坐标转Ray值
        /// </summary>
        /// <param name="screenPoint"></param>
        /// <returns></returns>
        public static Ray ScreenPointToRay(Vector3 screenPoint)
        {
            if (MainCamera == null)
                throw new Exception("markCamera相机对象为Null");

            return MainCamera.ScreenPointToRay(screenPoint);
        }

        public static Ray UIScreenPointToRay(Vector3 screenPoint)
        {
            if (UICamera == null)
                throw new Exception("UICamera相机对象为Null");

            return UICamera.ScreenPointToRay(screenPoint);
        }

        public static Vector3 UIScreenToWorldPoint(Vector3 point)
        {
            if (UICamera == null)
                throw new Exception("UICamera相机对象为Null");

            return UICamera.ScreenToWorldPoint(point);
        }

        public static Vector3 UIWorldToScreenPoint(Vector3 point)
        {
            if (UICamera == null)
                throw new Exception("UICamera相机对象为Null");

            return UICamera.WorldToScreenPoint(point);
        }

        /// <summary>
        /// 判断坐标是否在指定物体内
        /// </summary>
        /// <param name="position">物体屏幕坐标</param>
        /// <param name="size">物体的大小</param>
        /// <param name="handPotion">手屏幕坐标</param>
        /// <returns></returns>
        public static bool ScreenPointContains(Vector2 position,Vector2 size,Vector2 handPotion)
        {
            //X轴比较
            float xMin = position.x - size.x / 2;
            float xMax = position.x + size.x / 2;

            float yMin = position.y - size.y / 2;
            float yMax = position.y + size.y / 2;

            return handPotion.x.FloatContains(xMin,xMax) && handPotion.y.FloatContains(yMin,yMax);
        }

        /// <summary>
        /// 比较float值是否在指定范围内
        /// </summary>
        /// <param name="value">指定值</param>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        /// <returns>在范围内，返回True，否则返回false</returns>
        public static bool FloatContains(this float value,float min,float max)
        {
            return value >= min && value <= max;

        }

        /// <summary>
        /// 是否存在区域内
        /// </summary>
        /// <param name="transform">指定物体</param>
        /// <param name="handIndex"></param>
        /// <returns></returns>
        public static bool IsAreaContains(Transform transform,int handIndex)
        {
            //if (!KinectTransfer.IsHandActive(handIndex)) return false;

            try
            {

                //获取到此时手的屏幕坐标屏幕坐标
                Vector3 screenHandPoint = MOperateManager.GetHandScreenPoint(handIndex);

                Vector3 screenPoint = MUtility.UIWorldToScreenPoint(transform.position);

                //根据自身此时的屏幕坐标，去算区域

                RectTransform rectTransform = transform.GetComponent<RectTransform>();

                return ScreenPointContains(screenPoint,rectTransform.sizeDelta,screenHandPoint);
            }
            catch (Exception)
            {
                return false;
                //throw new Exception("手势可能没激活，如果是在编辑器上遇到此问题，不用理会");
            }
        }

        /// <summary>
        /// 是否存在区域内
        /// </summary>
        /// <param name="screenPoint">屏幕坐标</param>
        /// <param name="size">大小</param>
        /// <param name="handIndex">手势</param>
        /// <returns></returns>
        public static bool IsAreaContains(Vector2 screenPoint,Vector2 size,int handIndex)
        {
            //if (!KinectTransfer.IsHandActive(handIndex)) return false;

            //获取到此时手的屏幕坐标屏幕坐标
            Vector3 screenHandPoint = MOperateManager.GetHandScreenPoint(handIndex);

            //根据自身此时的屏幕坐标，去算区域

            return ScreenPointContains(screenPoint,size,screenHandPoint);
        }

        /// <summary>
        /// 返回是否在屏幕内
        /// </summary>
        /// <param name="thingAttach"></param>
        /// <returns></returns>
        public static bool AttachThingPosInCamera(Transform thingAttach,Vector2 xlimits,Vector2 ylimits)
        {
            Transform camTransform = MUtility.MainCamera.transform;

            Vector3 dir = (thingAttach.position - camTransform.position).normalized;

            float dot = Vector3.Dot(camTransform.forward,dir);     //判断物体是否在相机前面  

            Vector2 screenPos = MUtility.MainWorldToScreenPoint(thingAttach.position);

            if (screenPos.x < xlimits.y &&
                screenPos.x > xlimits.x &&
                screenPos.y < ylimits.y &&
                screenPos.y > ylimits.x
               && dot > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        /// 计算适口偏移值
        /// </summary>
        /// <returns>The offset position.</returns>
        /// <param name="handPosition">Hand position.</param>
        /// <param name="grabObject">Grab object.</param>
        public static Vector3 GetOffsetPosition(Vector3 handPosition,GameObject grabObject)
        {

            var offset = Vector3.zero;
            Vector3 screenDevice = MUtility.MainWorldToScreenPoint(grabObject.transform.position);
            Vector3 vPos = MUtility.MainScreenToWorldPoint(new Vector3(handPosition.x,handPosition.y,screenDevice.z));

            offset = vPos - grabObject.transform.position;

            return offset;
        }
    }
}
