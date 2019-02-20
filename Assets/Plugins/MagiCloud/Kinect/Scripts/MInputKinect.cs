using UnityEngine;

namespace MagiCloud.Kinect
{
    /// <summary>
    /// Kinect信息获取端
    /// </summary>
    public class MInputKinect
    {
        private static KinectGestureListener kinectGestureListener;
        private static KinectOperate kinectOperate;

        public MInputKinect(KinectGestureListener gestureListener, KinectOperate operate)
        {
            kinectOperate = operate;
            kinectGestureListener = gestureListener;
        }

        /// <summary>
        /// 手势是否激活
        /// </summary>
        /// <param name="handIndex">0为右手，1为左手，2为双手</param>
        /// <returns></returns>
        public static bool IsHandActive(int handIndex)
        {
            return kinectOperate.IsHandActive(handIndex);
        }

        /// <summary>
        /// 手势屏幕坐标
        /// </summary>
        /// <param name="handIndex"></param>
        /// <returns></returns>
        public static Vector3 ScreenHandPostion(int handIndex)
        {
            return KinectConfig.mainCamera.WorldToScreenPoint(KinectCapture.instance.GetOverlayHandPos(handIndex));
        }

        /// <summary>
        /// 手势握下
        /// </summary>
        /// <param name="handIndex"></param>
        /// <returns></returns>
        public static bool HandGrip(int handIndex)
        {
            return kinectGestureListener.GetHandGrip(handIndex);
        }

        /// <summary>
        /// 手势释放
        /// </summary>
        /// <param name="hanIndex"></param>
        /// <returns></returns>
        public static bool HandRelease(int hanIndex)
        {
            return kinectGestureListener.GetHandRelease(hanIndex);
        }

        public static void SetKinectActiveHand(KinectActiveHandStadus status)
        {
            KinectConfig.SetHandStartStatus(status);
        }

    }
}