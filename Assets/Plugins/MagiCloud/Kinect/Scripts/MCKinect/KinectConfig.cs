using UnityEngine;

namespace MagiCloud.Kinect
{
    /// <summary>
    /// Kinect参数配置和公用方法
    /// </summary>
    public static class KinectConfig
    {
        /// <summary>
        /// 背景图像缩放比例
        /// </summary>
        public static float ImageZoom = 1.45f;

        /// <summary>
        /// 手和髋距离
        /// </summary>
        public static float HandHipDistance = -0.04f;

        /// <summary>
        /// 屏幕坐标匹配物体世界坐标位移系数
        /// </summary>
        public static float CoefficientScreenToWorld = 0.00001f;

        public static float MRImageMoveDistanceX = 0;
        public static float MRImageMoveDistanceY = 0.1f;

        ///// <summary>
        ///// UI手尺寸
        ///// </summary>
        //public static int UIHandSize = 110;

        /// <summary>
        /// 场景主摄像机
        /// </summary>
        public static Camera mainCamera
        {
            get {
                if (GameObject.FindGameObjectWithTag("MarkCamera"))
                {
                    return GameObject.FindGameObjectWithTag("MarkCamera").GetComponent<Camera>();
                }

                return Camera.main;
            }
        }

        /// <summary>
        /// 校验误判
        /// </summary>
        public static bool CheckMisjudgment = false;
        
        /// <summary>
        /// Kinect手势激活状态
        /// </summary>
        public static KinectHandStatus kinectHandActive;

        public static void SetKinectHandActiveStatus(KinectHandStatus status) 
        {
            kinectHandActive = status;
        }
        public static KinectHandStatus GetKinectHandActiveStatus()
        {
            return kinectHandActive;
        }

        /// <summary>
        /// 场景状态
        /// </summary>
        public static KinectActiveHandStadus handStartStatus;

        public static void SetHandStartStatus(KinectActiveHandStadus status)
        {
            handStartStatus = status;
        }

        public static KinectActiveHandStadus GetHandStartStatus()
        {
            return handStartStatus;
        }

        #region 用户ID的管理
        //获取用户ID，令用户ID返回初始状态，设置用户ID。三个分开。方便对ID的管理。
        private static long userID;
        public static long UserID { get { return userID; } }

        public static void SetUserIDNull()
        {
            userID = 0;
        }
        public static void SetUserID(long id)
        {
            userID = id;
        }
        #endregion

        public static Vector3 CameraOriginPosition = Vector3.zero;

    }
}
