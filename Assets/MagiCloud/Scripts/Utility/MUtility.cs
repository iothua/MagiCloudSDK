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
    public sealed class MUtility
    {
        private static Camera _markCamera;
        private static Camera _mainCamera;
        private static Camera _uiCamera;

        /// <summary>
        /// 发射线点
        /// </summary>
        public static Camera MarkCamera
        {
            get{
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
            get{
                if (_mainCamera == null)
                    _mainCamera = Camera.main;

                return _mainCamera;
            }
        }

        public static Camera UICamera
        {
            get{
                if (_uiCamera == null)
                    _uiCamera = GameObject.FindGameObjectWithTag("UICamera").GetComponent<Camera>();

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
            if (MarkCamera == null)
                throw new Exception("markCamera相机对象为Null");

            return MarkCamera.ScreenPointToRay(screenPoint);
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
    }
}
