using MagiCloud.RotateAndZoomTool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MagiCloud.Features
{
    /*
    1、抓取或UI时，旋转关闭掉
    2、设置不抓取物体时，也支持摄像机围绕物体旋转
    */



    /// <summary>
    /// 旋转控制端
    /// </summary>
    public class RotateManager
    {
        private static bool _isActiveCameraZoom;
        private static bool _isActiveCameraRotate;
        private static bool _isActiveCameraAroundCenter;

        ///// <summary>
        ///// 启动摄像机缩放
        ///// </summary>
        //public static bool IsEnableCameraZoom;

        ///// <summary>
        ///// 启动摄像机自身旋转
        ///// </summary>
        //public static bool IsEnableCameraRotate;


        /// <summary>
        /// 是否启动相机缩放
        /// </summary>
        public static bool IsActiveCameraZoom
        {
            get
            {
                return _isActiveCameraZoom;
            }
            set
            {

                if (_isActiveCameraZoom == value) return;

                _isActiveCameraZoom = value;

                RotateAndZoomManager.IsPauseOrReStart_CameraZoom = value;
            }
        }

        /// <summary>
        /// 是否启动相机旋转
        /// </summary>
        public static bool IsActiveCameraRotate
        {
            get
            {
                return _isActiveCameraRotate;
            }
            set
            {

                if (_isActiveCameraRotate == value) return;

                _isActiveCameraRotate = value;

                if (_isActiveCameraRotate)
                {
                    RotateAndZoomManager.IsPauseOrReStart_CameraSelfRotate = true;
                }
                else
                {
                    RotateAndZoomManager.IsPauseOrReStart_CameraSelfRotate = false;
                }
            }

        }

        /// <summary>
        /// 是否激活摄像机绕中心点旋转
        /// </summary>
        public static bool IsActiveCameraAroundCenter
        {
            get
            {
                return RotateAndZoomManager.IsPauseOrReStart_CameraRotateAroundCenter;
            }
            set
            {
                RotateAndZoomManager.IsPauseOrReStart_CameraRotateAroundCenter = value;
            }
        }

        /// <summary>
        /// 摄像机旋转是否开启
        /// </summary>
        public static bool IsCameraRotate { get; set; }


        /// <summary>
        /// 相机绕点旋转时候的水平轴方向上旋转的角度限制
        /// </summary>
        public static Vector2 Limit_CameraRotateAroundCenter_HorizontalAxis
        {
            get
            {
                return RotateAndZoomManager.Limit_CameraRotateAroundCenter_HorizontalAxis;
            }

            set
            {
                RotateAndZoomManager.Limit_CameraRotateAroundCenter_HorizontalAxis = value;
            }
        }

        /// <summary>
        /// 相机绕点旋转时候的垂直轴方向上旋转的角度限制
        /// </summary>
        public static Vector2 Limit_CameraRotateAroundCenter_VerticalAxis
        {
            get
            {
                return RotateAndZoomManager.Limit_CameraRotateAroundCenter_VerticalAxis;
            }

            set
            {
                RotateAndZoomManager.Limit_CameraRotateAroundCenter_VerticalAxis = value;
            }
        }


        /// <summary>
        /// 相机自身旋转时候的水平轴方向上旋转的角度限制
        /// </summary>
        public static Vector2 Limit_CameraRotateSelf_HorizontalAxis
        {
            get
            {
                return RotateAndZoomManager.Limit_CameraRotateSelf_HorizontalAxis;
            }

            set
            {
                RotateAndZoomManager.Limit_CameraRotateSelf_HorizontalAxis = value;
            }
        }

        /// <summary>
        /// 相机自身旋转时候的垂直轴方向上旋转的角度限制
        /// </summary>
        public static Vector2 Limit_CameraRotateSelf_VerticalAxis
        {
            get
            {
                return RotateAndZoomManager.Limit_CameraRotateSelf_VerticalAxis;
            }

            set
            {
                RotateAndZoomManager.Limit_CameraRotateSelf_VerticalAxis = value;
            }
        }

        #region 相机旋转开启与关闭

        /// <summary>
        /// 相机围绕某个中心旋转
        /// </summary>
        /// <param name="center"></param>
        public static void StartCameraAroundCenter(Transform center, Vector3 position = default(Vector3), Quaternion quaternion = default(Quaternion),float duration =0.5f)
        {
            if (position.Equals(default(Vector3)) && quaternion.Equals(default(Quaternion)))
            {
                RotateAndZoomManager.StartCameraAroundCenter(center, duration);
            }
            else
            {
                RotateAndZoomManager.StartCameraAroundCenter(center, position, quaternion, duration);
            }

        }

        /// <summary>
        /// 关闭相机围绕某个中心旋转
        /// </summary>
        public static void StopCameraAroundCenter()
        {
            RotateAndZoomManager.StopCameraAroundCenter();
        }

        /// <summary>
        /// 开启相机按自身轴转
        /// </summary>
        public static void StartCameraSelfRotate()
        {
            if (IsCameraRotate) return;

            RotateAndZoomManager.StartCameraSelfRotate();
            IsCameraRotate = true;
        }


        /// <summary>
        /// 关闭相机按自身轴转
        /// </summary>
        public static void StopCameraSelfCenter()
        {
            if (!IsCameraRotate) return;
            RotateAndZoomManager.StopCameraSelfRotate();
            IsCameraRotate = false;
        }

        #endregion

        #region 物体旋转开启与关闭

        /// <summary>
        /// 开启物体旋转
        /// </summary>
        /// <param name="go">旋转的物体</param>
        /// <param name="space">旋转相对坐标系</param>
        /// <param name="axisLimit">限制于某轴</param>
        /// <param name="minangle">限制于某轴上最小角度</param>
        /// <param name="maxangle">限制于某轴上最大角度</param>
        //public static void StartGoRotate(GameObject go, Space space = Space.World, AxisLimits axisLimit = AxisLimits.None, float minangle = -360, float maxangle = 360)
        //{
        //    RotateAndZoomManager.StartGoRotate(go, space, axisLimit, minangle, maxangle);
        //}

        /// <summary>
        /// 关闭物体旋转
        /// </summary>
        //public static void StopGoRotate(GameObject go)
        //{
        //    RotateAndZoomManager.StopGoRotate(go);
        //}

        #endregion


        #region 相机缩放开启与关闭

        /// <summary>
        /// 开启相机缩放
        /// </summary>
        /// <param name="center">缩放中心</param>
        /// <param name="mindistance">距离中心最近距离</param>
        /// <param name="maxdistance">距离中心最远距离</param>
        public static void StartCameraZoom(Transform center, float mindistance, float maxdistance)
        {
            RotateAndZoomManager.StartCameraZoom(center, mindistance, maxdistance);
        }

        /// <summary>
        /// 关闭相机缩放
        /// </summary>
        public static void StopCameraZoom()
        {
            RotateAndZoomManager.StopCameraZoom();
        }

        #endregion

        #region 旋转和缩全部重置

        public static void RotateAndZoomReset()
        {
            RotateAndZoomManager.RotateAndZoomReset();
        }

        #endregion

        /// <summary>
        /// 暂停旋转
        /// </summary>
        public static void PauseAll()
        {
            IsActiveCameraAroundCenter = false;
            IsActiveCameraRotate = false;
            IsActiveCameraZoom = false;
        }

        /// <summary>
        /// 启用旋转（只针对已经开启过的）
        /// </summary>
        public static void StartAll()
        {
            IsActiveCameraAroundCenter = true;
            IsActiveCameraRotate = true;
            IsActiveCameraZoom = true;
        }
    }
}

