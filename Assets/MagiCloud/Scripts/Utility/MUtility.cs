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
        /// <summary>
        /// 发射线点
        /// </summary>
        public static Camera markCamera;
        /// <summary>
        /// 主相机
        /// </summary>
        public static Camera mainCamera;

        public static Camera UICamera;

        public static Core.OperatePlatform CurrentPlatform;

        /// <summary>
        /// 世界坐标转屏幕坐标(标记相机为准)
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public static Vector3 MarkWorldToScreenPoint(Vector3 point)
        {
            if (markCamera == null)
                throw new Exception("markCamera相机对象为Null");

            return markCamera.WorldToScreenPoint(point);
        }

        /// <summary>
        /// 屏幕坐标转世界坐标(标记相机为准)
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public static Vector3 MarkScreenToWorldPoint(Vector3 point)
        {
            if (markCamera == null)
                throw new Exception("markCamera相机对象为Null");

            return markCamera.ScreenToWorldPoint(point);
        }

        /// <summary>
        /// 屏幕坐标转世界坐标(主相机为准)
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public static Vector3 MainWorldToScreenPoint(Vector3 point)
        {
            if (mainCamera == null)
                throw new Exception("mainCamera相机对象为Null");

            return mainCamera.WorldToScreenPoint(point);
        }

        /// <summary>
        /// 屏幕坐标转世界坐标(主相机为准)
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public static Vector3 MainScreenToWorldPoint(Vector3 point)
        {
            if (mainCamera == null)
                throw new Exception("mainCamera相机对象为Null");

            return mainCamera.ScreenToWorldPoint(point);
        }

        /// <summary>
        /// 屏幕坐标转Ray值
        /// </summary>
        /// <param name="screenPoint"></param>
        /// <returns></returns>
        public static Ray ScreenPointToRay(Vector3 screenPoint)
        {
            if (markCamera == null)
                throw new Exception("markCamera相机对象为Null");

            return markCamera.ScreenPointToRay(screenPoint);
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
