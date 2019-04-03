using UnityEngine;

namespace MagiCloud.RotateAndZoomTool
{

    public static class RotateAndZoomManager
    {
        /// <summary>
        /// 相机绕点旋转时候的水平轴方向上旋转的角度限制
        /// </summary>
        public static Vector2 Limit_CameraRotateAroundCenter_HorizontalAxis = new Vector2(float.MinValue,float.MaxValue);

        /// <summary>
        /// 相机绕点旋转时候的垂直轴方向上旋转的角度限制
        /// </summary>
        public static Vector2 Limit_CameraRotateAroundCenter_VerticalAxis = new Vector2(float.MinValue,float.MaxValue);


        /// <summary>
        /// 相机自身旋转时候的水平轴方向上旋转的角度限制
        /// </summary>
        public static Vector2 Limit_CameraRotateSelf_HorizontalAxis = new Vector2(-360,360);

        /// <summary>
        /// 相机自身旋转时候的垂直轴方向上旋转的角度限制
        /// </summary>
        public static Vector2 Limit_CameraRotateSelf_VerticalAxis = new Vector2(-85,85);

        /// <summary>
        /// 相机缩放系数
        /// </summary>
        public static float Speed_CameraZoom = 20;

        /// <summary>
        /// 旋转速度 水平轴
        /// </summary>
        public static float Speed_CameraRotateAroundCenter_HorizontalAxis = 0.1f;

        /// <summary>
        /// 旋转速度 垂直轴
        /// </summary>
        public static float Speed_CameraRotateAroundCenter_VerticalAxis = 0.1f;

        /// <summary>
        /// 缩放暂停或者重启
        /// </summary>
        private static bool isPauseOrReStart_isZoom = false;

        /// <summary>
        /// 相机绕点旋转暂停或者重启
        /// </summary>
        private static bool isPauseOrReStart_CameraRotateAroundCenter = false;

        /// <summary>
        /// 相机自身转动开启后中间的暂停和重启
        /// </summary>
        private static bool isPauseOrReStart_CameraSelfRotate = false;

        /// <summary>
        /// 相机开启绕点转是否初始化完毕
        /// </summary>
        private static bool isDone_StartCameraAroundCenter_Initialization = false;

        /// <summary>
        /// 缩放暂停或者重启
        /// </summary>
        public static bool IsPauseOrReStart_CameraZoom
        {
            get
            {
                return isPauseOrReStart_isZoom;
            }

            set
            {
                CameraZoom.Instance.IsEnable = value;

                isPauseOrReStart_isZoom = value;
            }
        }

        /// <summary>
        /// 相机绕点旋转暂停或者重启
        /// </summary>
        public static bool IsPauseOrReStart_CameraRotateAroundCenter
        {
            get
            {
                return isPauseOrReStart_CameraRotateAroundCenter;
            }

            set
            {
                CameraRotate.Instance.IsRotateCameraWithCenterEnable = value;

                isPauseOrReStart_CameraRotateAroundCenter = value;
            }
        }

        /// <summary>
        /// 相机绕点旋转暂停或者重启
        /// </summary>
        public static bool IsPauseOrReStart_CameraSelfRotate
        {
            get
            {
                return isPauseOrReStart_CameraSelfRotate;
            }

            set
            {
                CameraRotate.Instance.IsSelfRotateCameraEnable = value;

                isPauseOrReStart_CameraSelfRotate = value;
            }
        }

        #region UI控制显示开关

        /// <summary>
        /// 相机开启绕点转是否初始化完毕
        /// </summary>
        public static bool IsDone_StartCameraAroundCenter_Initialization
        {
            get
            {
                return isDone_StartCameraAroundCenter_Initialization;
            }

            set
            {
                isDone_StartCameraAroundCenter_Initialization = value;
            }
        }
        #endregion



        #region 相机旋转开启与关闭
        /// <summary>
        /// 相机围绕某个中心旋转
        /// </summary>
        /// <param name="center"></param>
        public static void StartCameraAroundCenter(Transform center,float duration = 0.5f)
        {
            CameraRotate.Instance.StartCameraRotateWithCenter(center,duration);
        }

        /// <summary>
        /// 相机围绕某个中心旋转,可初始化相机位置
        /// </summary>
        /// <param name="center">围绕中心点</param>
        /// <param name="pos">初始化相机位置</param>
        /// <param name="qua">初始化相机角度</param>
        public static void StartCameraAroundCenter(Transform center,Vector3 pos,Quaternion qua,float duration = 0.5f)
        {
            CameraRotate.Instance.StartCameraRotateWithCenter(center,pos,qua,duration);
        }

        /// <summary>
        /// 关闭相机围绕某个中心旋转
        /// </summary>
        public static void StopCameraAroundCenter()
        {
            CameraRotate.Instance.StopCameraRotateWithCenter();
        }



        /// <summary>
        /// 开启相机按自身轴转
        /// </summary>
        public static void StartCameraSelfRotate()
        {
            StopCameraAroundCenter();
            CameraRotate.Instance.StartCameraSelfRotate();
        }


        /// <summary>
        /// 关闭相机按自身轴转
        /// </summary>
        public static void StopCameraSelfRotate()
        {
            CameraRotate.Instance.StopCameraSelfRotate();
        }

        #endregion

        #region 相机缩放开启与关闭

        /// <summary>
        /// 开启相机缩放
        /// </summary>
        /// <param name="center">缩放中心</param>
        /// <param name="mindistance">距离中心最近距离</param>
        /// <param name="maxdistance">距离中心最远距离</param>
        public static void StartCameraZoom(Transform center,float mindistance,float maxdistance)
        {
            CameraZoom.Instance.StartCameraZoomWithCenter(center,mindistance,maxdistance);
        }

        /// <summary>
        /// 关闭相机缩放
        /// </summary>
        public static void StopCameraZoom()
        {
            CameraZoom.Instance.StopCameraZoom();
        }


        /// <summary>
        /// 旋转和缩放管理重置 参数和处理对象的数据
        /// </summary>
        public static void RotateAndZoomReset()
        {

            //角度限制的重置
            Limit_CameraRotateAroundCenter_HorizontalAxis = new Vector2(0,0);
            Limit_CameraRotateAroundCenter_VerticalAxis = new Vector2(-85,85);
            Limit_CameraRotateSelf_HorizontalAxis = new Vector2(-360,360);
            Limit_CameraRotateSelf_VerticalAxis = new Vector2(-85,85);

            //缩放系数
            Speed_CameraZoom = 20;

            //暂停参数重置
            IsPauseOrReStart_CameraRotateAroundCenter = false;
            IsPauseOrReStart_CameraSelfRotate = false;
            IsPauseOrReStart_CameraZoom = false;

            //停止旋转和缩放
            StopCameraAroundCenter();
            StopCameraSelfRotate();
            StopCameraZoom();

            //旋转处理对象的清除
            CameraRotate.Instance.rotateCore = null;
            CameraRotate.Instance.dragMouseOrbit = null;

        }


        #endregion
    }
}
