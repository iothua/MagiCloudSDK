/***
 * 
 *    Title: Kinect捕捉
 *           
 *    Description: 
 *           保存需要用到的Kinect数据
 *           主要是要用到的关节点位置:
 *           关节点位置分为两种：一种是世界坐标的位置，通过kinectManager.GetJointKinectPosition获得，是人相对于(0,0,0)相机的位置。
 *                              主要用到手心和指尖的坐标来判断握下和松开手势。
 *                              
 *                              另一种是通过色彩图的像素点位置计算过来的，Z值是不会改变的。这种方法得到的坐标一定会和真实人物的坐标对上，
 *                              所以转化为屏幕坐标可以用来发射线或者给UI手图标赋值。
 *                  
 */

using UnityEngine;

namespace MagiCloud.Kinect
{
    /// <summary>
    /// Kinect骨骼信息
    /// </summary>
    public class KinectCapture
    {
        public static KinectCapture Instance;

        private static KinectManager kinectManager;

        /// <summary>
        /// Kinect人物骨骼-右手坐标
        /// </summary>
        private static Vector3 KinectJointPosHandRight;     // 右手
        /// <summary>
        /// Kinect人物骨骼-左手坐标
        /// </summary>
        private static Vector3 KinectJointPosHandLeft;
        /// <summary>
        /// Kinect人物骨骼-右指尖
        /// </summary>
        private static Vector3 KinectJointPosTipRight;      // 右指尖
        /// <summary>
        /// Kinect人物骨骼-左指尖
        /// </summary>
        private static Vector3 KinectJointPosTipLeft;
        /// <summary>
        /// Kinect人物骨骼-右肩
        /// </summary>
        private static Vector3 KinectJointPosShoulderRight;     // 右肩
        /// <summary>
        /// Kinect人物骨骼-左肩
        /// </summary>
        private static Vector3 KinectJointPosShoulderLeft;

        /// <summary>
        /// Kinect人物骨骼-髋关节中心
        /// </summary>
        private static Vector3 KinectJointPosSpineBase; // 髋关节中心

        /// <summary>
        /// 左手物体对象
        /// </summary>
        private static GameObject overlayHandLeft;
        /// <summary>
        /// 右手物体对象
        /// </summary>
        private static GameObject overlayHandRight;
        /// <summary>
        /// 左肩物体对象
        /// </summary>
        private static GameObject overlayShoulderLeft;
        /// <summary>
        /// 右肩物体对象
        /// </summary>
        private static GameObject overlayShoulderRight;
        /// <summary>
        /// 右指尖物体对象
        /// </summary>
        private static GameObject overlayTipRight;
        /// <summary>
        /// 左指尖物体对象
        /// </summary>
        private static GameObject overlayTipLeft;

        /// <summary>
        /// 髋关节中心 物体对象
        /// </summary>
        private static GameObject overlaySpineBase;

        /// <summary>
        /// 右手关节坐标（对应World）
        /// </summary>
        private static Vector3 OverlayJointPosHandRight;
        /// <summary>
        /// 左手关节坐标（对应World）
        /// </summary>
        private static Vector3 OverlayJointPosHandLeft;

        /// <summary>
        /// 右肩关节坐标（对应World）
        /// </summary>
        private static Vector3 OverlayJointPosShoulderRight;
        /// <summary>
        /// 左肩关节坐标（对应World）
        /// </summary>
        private static Vector3 OverlayJointPosShoulderLeft;
        /// <summary>
        /// 右指尖关节坐标（对应World）
        /// </summary>
        private static Vector3 OverlayJointPosTipRight;
        /// <summary>
        /// 左指尖关节坐标（对应World）
        /// </summary>
        private static Vector3 OverlayJointPosTipLeft;

        /// <summary>
        /// 髋关节中心关节坐标（对应World）
        /// </summary>
        private static Vector3 OverlayJointPosSpineBase;

        /// <summary>
        /// 上一帧右手坐标
        /// </summary>
        private static Vector3 lastHandPosRight;

        /// <summary>
        /// 右手-坐标（当前帧与上一帧的插值）
        /// </summary>
        public static Vector3 deltaHandPosRight;
        /// <summary>
        /// 上一帧的坐标
        /// </summary>
        private static Vector3 lastHandPosLeft;
        /// <summary>
        /// 左手-坐标（当前帧与上一帧的插值）
        /// </summary>
        public static Vector3 deltaHandPosLeft;

        /// <summary>
        /// 是否更新手势坐标
        /// </summary>
        private static bool HasUpdatedPos;

        #region 内部
        public KinectCapture()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            #region 创建关节物体
            var joinObject = new GameObject("joinObject-关节");

            joinObject.transform.SetParent(MInputKinect.InputKinect.transform);

            overlayHandLeft = new GameObject("leftOverlayObject-左手");
            overlayHandRight = new GameObject("rightOverlayObject-右手");
            overlayShoulderRight = new GameObject("rightShoulder-右肩");
            overlayShoulderLeft = new GameObject("leftShoulder-左肩");
            overlayTipLeft = new GameObject("leftOverlayTipObject-左指尖");
            overlayTipRight = new GameObject("rightOverlayTipObject-右指尖");
            overlaySpineBase = new GameObject("overlaySpineBaseObject-髋关节中心");

            overlayHandRight.transform.position = Vector3.down * 10;
            overlayHandLeft.transform.position = Vector3.down * 10;

            overlayHandLeft.transform.SetParent(joinObject.transform);
            overlayHandRight.transform.SetParent(joinObject.transform);
            overlayShoulderRight.transform.SetParent(joinObject.transform);
            overlayShoulderLeft.transform.SetParent(joinObject.transform);
            overlayTipRight.transform.SetParent(joinObject.transform);
            overlayTipLeft.transform.SetParent(joinObject.transform);
            overlaySpineBase.transform.SetParent(joinObject.transform);

            #endregion
        }

        public void OnUpdate()
        {

            if (MInputKinect.UserID == 0)
            {
                ResetOverlayJoint();
                HasUpdatedPos = false;
                return;
            }

            kinectManager = KinectManager.Instance;

            SetOverlayJoint();
            SetKinectJoint();

            deltaHandPosRight = KinectJointPosHandRight - lastHandPosRight;
            lastHandPosRight = KinectJointPosHandRight;
            deltaHandPosLeft = KinectJointPosHandLeft - lastHandPosLeft;
            lastHandPosLeft = KinectJointPosHandLeft;

            HasUpdatedPos = true;
        }

        /// <summary>
        /// 设置关节点坐标
        /// </summary>
        private static void SetOverlayJoint()
        {
            OverlayObjectMove((int)KinectInterop.JointType.HandRight, overlayHandRight);
            OverlayObjectMove((int)KinectInterop.JointType.HandLeft, overlayHandLeft);
            OverlayObjectMove((int)KinectInterop.JointType.ShoulderRight, overlayShoulderRight);
            OverlayObjectMove((int)KinectInterop.JointType.ShoulderLeft, overlayShoulderLeft);
            OverlayObjectMove((int)KinectInterop.JointType.HandTipRight, overlayTipRight);
            OverlayObjectMove((int)KinectInterop.JointType.HandTipLeft, overlayTipLeft);

            OverlayObjectMoveSpineBase((int)KinectInterop.JointType.SpineBase, overlaySpineBase);

            OverlayJointPosHandLeft = overlayHandLeft.transform.position;
            OverlayJointPosHandRight = overlayHandRight.transform.position;
            OverlayJointPosShoulderLeft = overlayShoulderLeft.transform.position;
            OverlayJointPosShoulderRight = overlayShoulderRight.transform.position;
            OverlayJointPosTipRight = overlayTipRight.transform.position;
            OverlayJointPosTipLeft = overlayTipLeft.transform.position;
            OverlayJointPosSpineBase = overlaySpineBase.transform.position;

        }

        private static void ResetOverlayJoint()
        {
            OverlayJointPosHandRight = Vector3.down * 100;
            OverlayJointPosHandLeft = Vector3.down * 100;
        }

        public void SetOverlayJointPosHandRight(Vector3 mousePosition)
        {
            float xNorm = mousePosition.x / Screen.width;
            float yNorm = mousePosition.y / Screen.height;

            OverlayJointPosHandRight = KinectConfig.mainCamera.ViewportToWorldPoint(new Vector3(xNorm, yNorm, 10));
        }

        private static void SetKinectJoint()
        {
            KinectJointPosHandRight = kinectManager.GetJointKinectPosition(MInputKinect.UserID, (int)KinectInterop.JointType.HandRight);
            KinectJointPosHandLeft = kinectManager.GetJointKinectPosition(MInputKinect.UserID, (int)KinectInterop.JointType.HandLeft);
            //KinectJointPosHandRight = kinectManager.GetJointPosition(KinectConfig.UserID, (int)KinectInterop.JointType.HandRight);
            //KinectJointPosHandLeft = kinectManager.GetJointPosition(KinectConfig.UserID, (int)KinectInterop.JointType.HandLeft);
            KinectJointPosTipRight = kinectManager.GetJointKinectPosition(MInputKinect.UserID, (int)KinectInterop.JointType.HandTipRight);
            KinectJointPosTipLeft = kinectManager.GetJointKinectPosition(MInputKinect.UserID, (int)KinectInterop.JointType.HandTipLeft);
            KinectJointPosShoulderRight = kinectManager.GetJointKinectPosition(MInputKinect.UserID, (int)KinectInterop.JointType.ShoulderRight);
            KinectJointPosShoulderLeft = kinectManager.GetJointKinectPosition(MInputKinect.UserID, (int)KinectInterop.JointType.ShoulderLeft);
            KinectJointPosSpineBase = kinectManager.GetJointKinectPosition(MInputKinect.UserID, (int)KinectInterop.JointType.SpineBase);
        }

        private static void OverlayObjectMove(int JointIndex, GameObject OverlayObject)
        {
            if (kinectManager.IsUserDetected())
            {
                long userId = MInputKinect.UserID;
                if (kinectManager.IsJointTracked(userId, JointIndex))
                {
                    Vector3 posJoint = kinectManager.GetJointKinectPosition(userId, JointIndex);

                    if (posJoint != Vector3.zero)
                    {
                        Vector2 posDepth = kinectManager.MapSpacePointToDepthCoords(posJoint);
                        ushort depthValue = kinectManager.GetDepthForPixel((int)posDepth.x, (int)posDepth.y);

                        if (depthValue > 0)
                        {
                            Vector2 posColor = kinectManager.MapDepthPointToColorCoords(posDepth, depthValue);

                            float xNorm = posColor.x / kinectManager.GetColorImageWidth();
                            float yNorm = 1.0f - posColor.y / kinectManager.GetColorImageHeight();

                            //左下0.0
                            xNorm = (xNorm * KinectConfig.ImageZoom - (KinectConfig.ImageZoom - 1) / 2 * 1) - 0.04f + KinectConfig.MRImageMoveDistanceX;
                            yNorm = yNorm * KinectConfig.ImageZoom - (KinectConfig.ImageZoom - 1) / 2 * 1 + KinectConfig.MRImageMoveDistanceY;

                            if (OverlayObject)
                            {
                                Vector3 vPosOverlay = KinectConfig.mainCamera.ViewportToWorldPoint(new Vector3(xNorm, yNorm, 10));
                                OverlayObject.transform.position = Vector3.Lerp(OverlayObject.transform.position, vPosOverlay, 20 * Time.deltaTime);
                                //OverlayObject.transform.position = vPosOverlay;
                            }
                        }
                    }
                }
            }
        }

        private static void OverlayObjectMoveSpineBase(int JointIndex, GameObject OverlayObject)
        {
            if (kinectManager.IsUserDetected())
            {
                long userId = MInputKinect.UserID;
                if (kinectManager.IsJointTracked(userId, JointIndex))
                {
                    Vector3 posJoint = kinectManager.GetJointKinectPosition(userId, JointIndex);

                    if (posJoint != Vector3.zero)
                    {
                        Vector2 posDepth = kinectManager.MapSpacePointToDepthCoords(posJoint);
                        ushort depthValue = kinectManager.GetDepthForPixel((int)posDepth.x, (int)posDepth.y);

                        if (depthValue > 0)
                        {
                            Vector2 posColor = kinectManager.MapDepthPointToColorCoords(posDepth, depthValue);

                            float xNorm = posColor.x / kinectManager.GetColorImageWidth();
                            float yNorm = 1.0f - posColor.y / kinectManager.GetColorImageHeight();

                            //左下0.0
                            xNorm = (xNorm * KinectConfig.ImageZoom - (KinectConfig.ImageZoom - 1) / 2 * 1) - 0.03f;
                            yNorm = yNorm * KinectConfig.ImageZoom - (KinectConfig.ImageZoom - 1) / 2 * 1;
                            if (OverlayObject)
                            {
                                Vector3 vPosOverlay = KinectConfig.mainCamera.ViewportToWorldPoint(new Vector3(xNorm, yNorm, 10));
                                OverlayObject.transform.position = Vector3.Lerp(OverlayObject.transform.position, vPosOverlay, 20 * Time.deltaTime);
                                //OverlayObject.transform.position = vPosOverlay;
                            }
                        }
                    }
                }
            }
        }
        #endregion


        #region 公用方法
        /// <summary>
        /// 在开启用户操作后，手动更新第一帧的位置。
        /// 解决了StartHand时手会从上一次消失的位置跳过来的问题
        /// </summary>
        public static void RefreshKinectPosition()
        {
            //如果已经更新过了，则不需要执行
            if (HasUpdatedPos) return;

            kinectManager = KinectManager.Instance;
            SetOverlayJoint();
            SetKinectJoint();

            HasUpdatedPos = true;
        }

        /// <summary>
        /// 获取手3D坐标
        /// </summary>
        /// <param name="handIndex"></param>
        /// <returns></returns>
        public Vector3 GetKinectHandPos(int handIndex)
        {

            return handIndex == 1 ? KinectJointPosHandLeft : KinectJointPosHandRight;
        }

        /// <summary>
        /// 获取肩3D坐标
        /// </summary>
        /// <param name="handIndex"></param>
        /// <returns></returns>
        public Vector3 GetKinectShoulderPos(int handIndex)
        {

            return handIndex == 1 ? KinectJointPosShoulderLeft : KinectJointPosShoulderRight;
        }

        /// <summary>
        /// 获取指尖3D坐标
        /// </summary>
        /// <param name="handIndex"></param>
        /// <returns></returns>
        public Vector3 GetKinectTipPos(int handIndex)
        {
            return handIndex == 1 ? KinectJointPosTipLeft : KinectJointPosTipRight;
        }

        public Vector3 GetOverlayHandPos(int handIndex)
        {

            return handIndex == 1 ? OverlayJointPosHandLeft : OverlayJointPosHandRight;
        }

        public Vector3 GetOverlaySpineBasePos()
        {
            return OverlayJointPosSpineBase;
        }

        public Vector3 GetOverlayShoulderPos(int handIndex)
        {
            return handIndex == 1 ? OverlayJointPosShoulderLeft : OverlayJointPosShoulderRight;
        }

        public Vector3 GetOverlayTipPos(int handIndex)
        {
            return handIndex == 1 ? OverlayJointPosTipLeft : OverlayJointPosTipRight;
        }

        /// <summary>
        /// 这个方法是后来新加的，主要是因为相机位置不是(0,0,0)了，所以GetHandPos不是那么准确。
        /// 因此提供手每一帧改变的变量，在物体需要跟随的手移动时，只需让物体每一帧加上这个变量就可以了。
        /// </summary>
        /// <param name="handIndex"></param>
        /// <returns></returns>
        public static Vector3 GetDeltaHandPos(int handIndex)
        {
            return handIndex == 1 ? deltaHandPosLeft : deltaHandPosRight;
        }

        #endregion

    }
}