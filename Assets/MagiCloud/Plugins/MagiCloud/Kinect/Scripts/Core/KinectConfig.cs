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

        private static Camera markCamera;

        /// <summary>
        /// 场景主摄像机
        /// </summary>
        public static Camera mainCamera
        {
            get {

                if (markCamera == null)
                {
                    if (GameObject.FindGameObjectWithTag("MarkCamera"))
                    {
                        markCamera = GameObject.FindGameObjectWithTag("MarkCamera").GetComponent<Camera>();
                    }

                    if (markCamera == null)
                        markCamera = Camera.main;
                }

                return markCamera;
            }
        }

        /// <summary>
        /// 校验误判
        /// </summary>
        public static bool CheckMisjudgment = false;
    }
}
