/*
    1、该脚本主要负责获取到抓取和松开手势的信号
    2、主要原理是判断指尖和掌心这两个关节点位置间的距离


    单手模式：
    1）当一只手识别到后，就不识别另一只手，并且禁止识别。除非这只手放下后，在开启识别
    2）相应的事件处理，也要触发（这是重点）
    双手模式：
    1）可识别双手，当一只手以及识别，放下时，该手要属于禁止状态，相应的事件需要触发。
    2）当手抬起时，则进行识别。
    3）
 */


using System.Collections;
using UnityEngine;

namespace MagiCloud.Kinect
{
    public class KinectGestureListener : MonoBehaviour, KinectGestures.GestureListenerInterface
    {
        /// <summary>
        /// 手区域
        /// </summary>
        public enum HandArea
        {
            UP,
            SPREAD,
            MIDDLE,
            MIDDLESIDE,
            OUT
        }

        /// <summary>
        /// 手状态
        /// </summary>
        public enum HandState
        {
            Grip,
            Release,
            Lasso
        }

        /// <summary>
        /// 手相关坐标问题
        /// </summary>
        public class HandPosition
        {
            /// <summary>
            /// 手坐标（物体的World）
            /// </summary>
            public Vector3 hand_OverLay;
            /// <summary>
            /// 肩坐标
            /// </summary>
            public Vector3 shoulder_OverLay;

            /// <summary>
            /// 手坐标（Kinect骨骼）
            /// </summary>
            public Vector3 HandPos;

            /// <summary>
            /// 指尖坐标
            /// </summary>
            public Vector3 TipPos;

            /// <summary>
            /// 是否握拳
            /// </summary>
            public bool IsGrip;
            /// <summary>
            /// 是否释放
            /// </summary>
            public bool IsRelease;

            /// <summary>
            /// 是否报错
            /// </summary>
            public bool IsLasso;

            /// <summary>
            /// 是否锁住手
            /// </summary>
            public bool IsLockHand;

            private bool isHandActive;
            /// <summary>
            /// 是否激活手
            /// </summary>
            public bool IsHandActive
            {
                get {
                    return isHandActive;
                }
                set {

                    if (isHandActive == value) return;
                    isHandActive = value;

                    //发送事件
                    if(isHandActive)
                    {
                        Core.Events.EventHandStart.SendListener(HandIndex);
                    }
                    else
                    {
                        Core.Events.EventHandStop.SendListener(HandIndex);
                    }
                }
            }

            public int HandIndex = 0;

            ///// <summary>
            ///// 延时时长
            ///// </summary>
            //public float Timer = 0;
            /// <summary>
            /// 当前手坐标
            /// </summary>
            public Vector3 currentHand = Vector3.zero;
            /// <summary>
            /// 上一帧手坐标
            /// </summary>
            public Vector3 lashHand = Vector3.zero;
            /// <summary>
            /// 手是否移动
            /// </summary>
            public bool IsHandMoveing;

            public HandArea handArea = HandArea.UP;
            public HandState handState = HandState.Release;

            public KinectGestureListener KinectGesture { get; set; }

            public void ResetGesturesData()
            {
                IsHandMoveing = false;
                IsLockHand = false;
                handArea = HandArea.UP;
            }

            public void HandSpeedLimit()
            {
                currentHand = HandPos;
                float deltaHand = Mathf.Abs(Vector3.Distance(currentHand, lashHand));
                float handSpeed = deltaHand / Time.deltaTime;

                lashHand = HandPos;

                IsHandMoveing = handSpeed > 1.0f;
            }

            ///// <summary>
            ///// 启动握拳手势
            ///// </summary>
            ///// <param name="holdTime"></param>
            //public void StartGripGesture(float holdTime)
            //{
            //    if (Timer < holdTime)
            //    {
            //        Timer += Time.deltaTime;
            //    }
            //    else
            //    {
            //        Timer = 0;
            //        IsHandActive = true;
            //    }
            //}

            ///// <summary>
            ///// 停止握拳手势
            ///// </summary>
            //public void StopGripGesture()
            //{
            //    Timer = 0;
            //    IsHandActive = false;
            //}

            public void SetLasso()
            {
                IsLasso = true;
                IsGrip = false;
                IsRelease = false;
            }

            public bool CheckGrip()
            {
                if (IsLockHand) return false;

                if (IsGrip)
                {
                    if (!isStartGrip)
                        KinectGesture.StartCoroutine(DelayBoolGrip());

                    return true;
                }
                return false;
            }

            public bool CheckRelease()
            {
                if (IsHandMoveing) return false;

                if (IsRelease)
                {
                    if (!isStartRelease)
                        KinectGesture.StartCoroutine(DelayBoolRelease());
                }

                return false;
            }

            public bool CheckLasso()
            {
                if (IsLasso)
                {
                    IsLasso = false;
                    return true;
                }
                return false;
            }

            private bool isStartGrip = false;
            private bool isStartRelease = false;

            IEnumerator DelayBoolGrip()
            {
                isStartGrip = true;
                yield return new WaitForEndOfFrame();
                IsGrip = false;
                isStartGrip = false;
            }

            IEnumerator DelayBoolRelease()
            {
                isStartRelease = true;
                yield return new WaitForEndOfFrame();
                IsRelease = false;
                isStartRelease = false;
            }

        }

        //public float holdTime = 0.2f;
        private KinectManager kinectManager;

        public HandPosition rightHandPosition, leftHandPosition;

        private MInputKinect.UserManager userManager;

        public void OnInitialize(MInputKinect.UserManager userManager)
        {
            this.userManager = userManager;

            rightHandPosition = new HandPosition() { KinectGesture = this, HandIndex = 0 };
            leftHandPosition = new HandPosition() { KinectGesture = this, HandIndex = 1 };
        }

        /// <summary>
        /// 双手检测手势
        /// </summary>
        public void TwoModelGestures()
        {
            if (KinectConfig.CheckMisjudgment)
            {
                ResetGesturesData();
                DetectGestures();
            }
            else
            {
                LockHand();
                HandSpeedLimit();
                DivideArea();
                DetectGestures();
            }
        }

        /// <summary>
        /// 单手检测手势
        /// </summary>
        public void OneModelGestures()
        {
            ResetGesturesData();
            DetectGestures();
        }

        /// <summary>
        /// 重置手势数据
        /// </summary>
        private void ResetGesturesData()
        {
            leftHandPosition.ResetGesturesData();
            rightHandPosition.ResetGesturesData();
        }

        /// <summary>
        /// 更新关节位置
        /// </summary>
        public void UpdateJointsPos()
        {
            kinectManager = KinectManager.Instance;

            rightHandPosition.HandPos = KinectCapture.Instance.GetKinectHandPos(0);
            rightHandPosition.TipPos = KinectCapture.Instance.GetKinectTipPos(0);
            rightHandPosition.hand_OverLay = KinectCapture.Instance.GetOverlayHandPos(0);
            rightHandPosition.shoulder_OverLay = KinectCapture.Instance.GetOverlayShoulderPos(0);

            leftHandPosition.HandPos = KinectCapture.Instance.GetKinectHandPos(1);
            leftHandPosition.TipPos = KinectCapture.Instance.GetKinectTipPos(1);
            leftHandPosition.hand_OverLay = KinectCapture.Instance.GetOverlayHandPos(1);
            leftHandPosition.shoulder_OverLay = KinectCapture.Instance.GetOverlayShoulderPos(1);
        }

        /// <summary>
        /// 根据手势当前状态判断相对手势的信号
        /// 如果是握下状态则只检测是否松开，反之则只检测是否握下
        /// </summary>
        private void DetectGestures()
        {
            if (rightHandPosition.IsHandActive)
                DetectRightGestures();

            if (leftHandPosition.IsHandActive)
                DetectLeftGestures();
        }

        /// <summary>
        /// 检测右手信息
        /// </summary>
        void DetectRightGestures()
        {
            switch (rightHandPosition.handState)
            {
                case HandState.Grip:
                    SwitchRightGestureRelease();
                    if (rightHandPosition.IsRelease)
                    {
                        rightHandPosition.handState = HandState.Release;
                    }

                    SwitchRightLasso();
                    if (rightHandPosition.IsLasso)
                    {
                        rightHandPosition.handState = HandState.Lasso;
                    }

                    break;
                case HandState.Release:
                    SwitchRightGestureGrip();
                    if (rightHandPosition.IsGrip)
                    {
                        rightHandPosition.handState = HandState.Grip;
                    }

                    SwitchRightLasso();
                    if (rightHandPosition.IsLasso)
                    {
                        rightHandPosition.handState = HandState.Lasso;
                    }

                    break;
                case HandState.Lasso:
                    SwitchRightGestureRelease();
                    if (rightHandPosition.IsRelease)
                    {
                        rightHandPosition.handState = HandState.Release;
                    }

                    SwitchRightGestureGrip();
                    if (rightHandPosition.IsGrip)
                    {
                        rightHandPosition.handState = HandState.Grip;
                    }

                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 检测左手信息
        /// </summary>
        void DetectLeftGestures()
        {
            switch (leftHandPosition.handState)
            {
                case HandState.Grip:
                    SwitchLeftGestureRelease();
                    if (leftHandPosition.IsRelease)
                    {
                        leftHandPosition.handState = HandState.Release;
                    }

                    SwitchLeftLasso();
                    if (leftHandPosition.IsLasso)
                    {
                        leftHandPosition.handState = HandState.Lasso;
                    }

                    break;
                case HandState.Release:

                    SwitchLeftGestureGrip();
                    if (leftHandPosition.IsGrip)
                    {
                        leftHandPosition.handState = HandState.Grip;
                    }

                    SwitchLeftLasso();
                    if (leftHandPosition.IsLasso)
                    {
                        leftHandPosition.handState = HandState.Lasso;
                    }

                    break;
                case HandState.Lasso:
                    SwitchLeftGestureRelease();
                    if (leftHandPosition.IsRelease)
                    {
                        leftHandPosition.handState = HandState.Release;
                    }

                    SwitchLeftGestureGrip();
                    if (leftHandPosition.IsGrip)
                    {
                        leftHandPosition.handState = HandState.Grip;
                    }

                    break;
                default:
                    break;
            }
        }

        #region 右手处理

        private void SwitchRightGestureGrip()
        {
            if (kinectManager.IsRightHandConfidenceHigh(MInputKinect.UserID))
            {
                GripBySystem(ref rightHandPosition.IsGrip, 0);
                return;
            }

            switch (rightHandPosition.handArea)
            {
                case HandArea.UP:
                    GripBySystem(ref rightHandPosition.IsGrip, 0);

                    break;
                case HandArea.SPREAD:
                    GripBySystem(ref rightHandPosition.IsGrip, 0);

                    break;
                case HandArea.MIDDLESIDE:
                case HandArea.MIDDLE:
                    GripByGesture1(rightHandPosition.HandPos, rightHandPosition.TipPos, ref rightHandPosition.IsGrip, ref rightHandPosition.IsRelease);

                    break;
                case HandArea.OUT:
                    GripByGesture2(rightHandPosition.HandPos, rightHandPosition.TipPos, ref rightHandPosition.IsGrip, ref rightHandPosition.IsRelease);

                    break;
                default:
                    break;
            }
        }

        private void SwitchRightGestureRelease()
        {
            if (kinectManager.IsRightHandConfidenceHigh(MInputKinect.UserID))
            {
                ReleaseBySystem(ref rightHandPosition.IsGrip, ref rightHandPosition.IsRelease, 0);
                return;
            }

            switch (rightHandPosition.handArea)
            {
                case HandArea.UP:
                case HandArea.SPREAD:
                    ReleaseBySystem(ref rightHandPosition.IsGrip, ref rightHandPosition.IsRelease, 0);
                    break;
                case HandArea.MIDDLESIDE:
                    ReleaseBySystem(ref rightHandPosition.IsGrip, ref rightHandPosition.IsRelease, 0);

                    break;
                case HandArea.MIDDLE:
                case HandArea.OUT:
                    ReleaseByGesture2(rightHandPosition.HandPos, rightHandPosition.TipPos, ref rightHandPosition.IsGrip, ref rightHandPosition.IsRelease);
                    break;
                default:
                    break;
            }
        }

        private void SwitchRightLasso()
        {
            if (kinectManager.GetRightHandState(MInputKinect.UserID) == KinectInterop.HandState.Lasso)
            {
                rightHandPosition.SetLasso();
            }
        }

        #endregion

        #region 左手处理

        private void SwitchLeftGestureGrip()
        {
            if (kinectManager.IsLeftHandConfidenceHigh(MInputKinect.UserID))
            {
                GripBySystem(ref leftHandPosition.IsGrip, 1);
                return;
            }

            switch (leftHandPosition.handArea)
            {
                case HandArea.UP:
                    GripBySystem(ref leftHandPosition.IsGrip, 1);
                    break;
                case HandArea.SPREAD:
                    GripBySystem(ref leftHandPosition.IsGrip, 1);

                    break;
                case HandArea.MIDDLESIDE:
                case HandArea.MIDDLE:
                    GripByGesture1(leftHandPosition.HandPos, leftHandPosition.TipPos, ref leftHandPosition.IsGrip, ref leftHandPosition.IsRelease);

                    break;
                case HandArea.OUT:
                    GripByGesture2(leftHandPosition.HandPos, leftHandPosition.TipPos, ref leftHandPosition.IsGrip, ref leftHandPosition.IsRelease);
                    break;
                default:
                    break;
            }
        }

        private void SwitchLeftGestureRelease()
        {
            if (kinectManager.IsLeftHandConfidenceHigh(MInputKinect.UserID))
            {
                ReleaseBySystem(ref leftHandPosition.IsGrip, ref leftHandPosition.IsRelease, 1);
                return;
            }

            switch (leftHandPosition.handArea)
            {
                case HandArea.UP:
                case HandArea.SPREAD:
                    ReleaseBySystem(ref leftHandPosition.IsGrip, ref leftHandPosition.IsRelease, 1);
                    break;
                case HandArea.MIDDLESIDE:
                    ReleaseBySystem(ref leftHandPosition.IsGrip, ref leftHandPosition.IsRelease, 1);
                    break;
                case HandArea.MIDDLE:
                case HandArea.OUT:
                    ReleaseByGesture2(leftHandPosition.HandPos, leftHandPosition.TipPos, ref leftHandPosition.IsGrip, ref leftHandPosition.IsRelease);
                    break;
                default:
                    break;
            }
        }

        private void SwitchLeftLasso()
        {
            if (kinectManager.GetLeftHandState(MInputKinect.UserID) == KinectInterop.HandState.Lasso)
            {
                leftHandPosition.SetLasso();
            }
        }

        #endregion

        #region 抓取判断

        /// <summary>
        /// 释放手势
        /// </summary>
        /// <param name="hand"></param>
        /// <param name="handTip"></param>
        /// <param name="IsGrip"></param>
        /// <param name="IsRelease"></param>
        private void ReleaseByGesture1(Vector3 hand, Vector3 handTip, ref bool IsGrip, ref bool IsRelease)
        {
            if (Mathf.Abs(Vector3.Distance(hand, handTip)) > 0.085f)
            {
                IsRelease = true;
                IsGrip = false;
            }
            else
            {
                IsRelease = false;
            }
        }

        /// <summary>
        /// 释放手势2
        /// </summary>
        /// <param name="hand"></param>
        /// <param name="handTip"></param>
        /// <param name="IsGrip"></param>
        /// <param name="IsRelease"></param>
        private void ReleaseByGesture2(Vector3 hand, Vector3 handTip, ref bool IsGrip, ref bool IsRelease)
        {
            if (Mathf.Abs(Vector3.Distance(hand, handTip)) > 0.078f)
            {
                IsRelease = true;
                IsGrip = false;
            }
            else
            {
                IsRelease = false;
            }

        }

        /// <summary>
        /// 握拳手势1
        /// </summary>
        /// <param name="hand"></param>
        /// <param name="handTip"></param>
        /// <param name="IsGrip"></param>
        /// <param name="IsRelease"></param>
        void GripByGesture1(Vector3 hand, Vector3 handTip, ref bool IsGrip,
            ref bool IsRelease)
        {
            /*
            //等价于
            if(Mathf.Abs(Vector3.Distance(hand, handTip)) < 0.050f)
            {
                IsGrip = true;
            }
            */

            IsGrip |= Mathf.Abs(Vector3.Distance(hand, handTip)) < 0.050f;
        }

        /// <summary>
        /// 握拳手势2
        /// </summary>
        /// <param name="hand"></param>
        /// <param name="handTip"></param>
        /// <param name="IsGrip"></param>
        /// <param name="IsRelease"></param>
        void GripByGesture2(Vector3 hand, Vector3 handTip, ref bool IsGrip,
            ref bool IsRelease)
        {
            IsGrip |= Mathf.Abs(Vector3.Distance(hand, handTip)) < 0.067f;
        }

        private bool GetHandOpen(int handIndex)
        {
            return handIndex==0? kinectManager.GetRightHandState(MInputKinect.UserID) == KinectInterop.HandState.Open:
                kinectManager.GetLeftHandState(MInputKinect.UserID) == KinectInterop.HandState.Open;
        }

        private void GripBySystem(ref bool IsGrip, int handIndex)
        {
            IsGrip = handIndex == 0 ? kinectManager.GetRightHandState(MInputKinect.UserID) == KinectInterop.HandState.Closed
                : kinectManager.GetLeftHandState(MInputKinect.UserID) == KinectInterop.HandState.Closed;
        }

        private void ReleaseBySystem(ref bool IsGrip, ref bool IsRelease, int handIndex)
        {
            if (GetHandOpen(handIndex))
            {
                IsRelease = true;
                IsGrip = false;
            }
            else
            {
                IsRelease = false;
            }
        }

        private void DivideArea()
        {

            if (rightHandPosition.IsHandActive)
            {

                //右手
                if (rightHandPosition.hand_OverLay.y >= rightHandPosition.shoulder_OverLay.y)
                {
                    rightHandPosition.handArea = HandArea.UP;
                }
                else if (rightHandPosition.hand_OverLay.x < leftHandPosition.shoulder_OverLay.x)
                {
                    rightHandPosition.handArea = HandArea.OUT;
                }
                else if (rightHandPosition.hand_OverLay.x - 1.2f < rightHandPosition.shoulder_OverLay.x)
                {
                    rightHandPosition.handArea = HandArea.MIDDLE;
                }
                else if (rightHandPosition.hand_OverLay.x - 4f < rightHandPosition.shoulder_OverLay.x &&
                    rightHandPosition.hand_OverLay.y - rightHandPosition.shoulder_OverLay.y < -2f)
                {
                    rightHandPosition.handArea = HandArea.MIDDLESIDE;
                }
                else
                {
                    rightHandPosition.handArea = HandArea.SPREAD;
                }

            }

            if (leftHandPosition.IsHandActive)
            {
                //左手
                if (leftHandPosition.hand_OverLay.y >= leftHandPosition.shoulder_OverLay.y)
                {
                    leftHandPosition.handArea = HandArea.UP;
                }
                else if (leftHandPosition.hand_OverLay.x > rightHandPosition.shoulder_OverLay.x)
                {
                    leftHandPosition.handArea = HandArea.OUT;
                }
                else if (leftHandPosition.hand_OverLay.x + 1.2f < leftHandPosition.shoulder_OverLay.x)
                {
                    leftHandPosition.handArea = HandArea.MIDDLE;
                }
                else if (leftHandPosition.hand_OverLay.x + 4f < leftHandPosition.shoulder_OverLay.x &&
                    leftHandPosition.hand_OverLay.y - leftHandPosition.shoulder_OverLay.y < -2f)
                {
                    leftHandPosition.handArea = HandArea.MIDDLESIDE;
                }
                else
                {
                    leftHandPosition.handArea = HandArea.SPREAD;
                }
            }
        }

        #endregion

        /// <summary>
        /// 检测手移动速度
        /// </summary>
        private void HandSpeedLimit()
        {
            if (rightHandPosition.IsHandActive)
                rightHandPosition.HandSpeedLimit();

            if (leftHandPosition.IsHandActive)
                leftHandPosition.HandSpeedLimit();
        }

        #region 检测手是否离胸前太近
        //根据安全距离锁定手势(锁定手势:无法进行抓取操作)
        private void LockHand()
        {
            Vector3 vHandRight = KinectCapture.Instance.GetKinectHandPos(0);
            Vector3 vShoulderRight = KinectCapture.Instance.GetKinectShoulderPos(0);
            Vector3 vHandLeft = KinectCapture.Instance.GetKinectHandPos(1);
            Vector3 vShoulderLeft = KinectCapture.Instance.GetKinectShoulderPos(1);

            if (rightHandPosition.IsHandActive)
            {
                //右手判定
                if ((vHandRight.x - vShoulderLeft.x > -0.22f && vHandRight.x - vShoulderLeft.x < 0 &&
                    vHandRight.y - vShoulderLeft.y > -0.22f && vHandRight.y - vShoulderLeft.y < 0 &&
                    vShoulderRight.z - vHandRight.z < 0.23f) ||
                    (vHandRight.x - vShoulderRight.x < 0 && vHandRight.x - vShoulderRight.x > -1.2f &&
                    vHandRight.y - vShoulderRight.y < 0 && vHandRight.z - vShoulderRight.z > -0.13f))
                {
                    rightHandPosition.IsLockHand = true;
                }
                else
                {
                    rightHandPosition.IsLockHand = false;
                }
            }

            if (leftHandPosition.IsHandActive)
            {
                //左手判定
                if ((vHandLeft.x - vShoulderRight.x > 0 && vHandLeft.x - vShoulderRight.x < 0.22f &&
                     vHandLeft.y - vShoulderRight.y > -0.22f && vHandLeft.y - vShoulderRight.y < 0 &&
                     vShoulderRight.z - vHandRight.z < 0.23f) ||
                    (vHandLeft.x - vShoulderLeft.x < 1.2f && vHandLeft.x - vShoulderLeft.x > 0 &&
                    vHandLeft.y - vShoulderLeft.y < 0 && vHandLeft.z - vShoulderLeft.z > -0.13f))
                {
                    leftHandPosition.IsLockHand = true;
                }
                else
                {
                    leftHandPosition.IsLockHand = false;
                }
            }
        }

        #endregion

        #region 暂时注释

        //#region 0.3f开启和关闭抓取手势

        ///// <summary>
        ///// 启动该握拳手势
        ///// </summary>
        ///// <param name="handIndex"></param>
        //public void StartGripGesture(int handIndex)
        //{
        //    if (handIndex == 1)
        //    {
        //        leftHandPosition.StartGripGesture(holdTime);
        //    }
        //    else
        //    {
        //        rightHandPosition.StartGripGesture(holdTime);
        //    }
        //}

        ///// <summary>
        ///// 停止握拳手势
        ///// </summary>
        ///// <param name="handIndex"></param>
        //public void StopGripGesture(int handIndex)
        //{
        //    if (handIndex == 1)
        //    {
        //        leftHandPosition.StopGripGesture();
        //    }
        //    else
        //    {
        //        rightHandPosition.StopGripGesture();
        //    }
        //}

        ///// <summary>
        ///// 获取手势激活状态
        ///// </summary>
        ///// <param name="handIndex"></param>
        ///// <returns></returns>
        //public bool GetHandActive(int handIndex)
        //{
        //    return handIndex == 1 ? leftHandPosition.IsHandActive : rightHandPosition.IsHandActive;
        //}

        //#endregion

        #endregion

        /// <summary>
        /// 获取手势激活状态
        /// </summary>
        /// <param name="handIndex"></param>
        /// <returns></returns>
        public bool GetHandActive(int handIndex)
        {
            return handIndex == 1 ? leftHandPosition.IsHandActive : rightHandPosition.IsHandActive;
        }

        #region 接口方法

        /// <summary>
        /// 手势取消
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="userIndex"></param>
        /// <param name="gesture"></param>
        /// <param name="joint"></param>
        /// <returns></returns>
        public bool GestureCancelled(long userId, int userIndex, KinectGestures.Gestures gesture, KinectInterop.JointType joint)
        {
            return true;
        }

        /// <summary>
        /// 手势完成
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="userIndex"></param>
        /// <param name="gesture"></param>
        /// <param name="joint"></param>
        /// <param name="screenPos"></param>
        /// <returns></returns>
        public bool GestureCompleted(long userId, int userIndex, KinectGestures.Gestures gesture, KinectInterop.JointType joint, Vector3 screenPos)
        {
            return true;
        }

        /// <summary>
        /// 手势进行中
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="userIndex"></param>
        /// <param name="gesture"></param>
        /// <param name="progress"></param>
        /// <param name="joint"></param>
        /// <param name="screenPos"></param>
        public void GestureInProgress(long userId, int userIndex, KinectGestures.Gestures gesture, float progress, KinectInterop.JointType joint, Vector3 screenPos)
        {

        }

        //添加手势
        public void UserDetected(long userId, int userIndex)
        {
            userManager.AddUser(userId);
        }

        //移除手势
        public void UserLost(long userId, int userIndex)
        {
            userManager.LostUser(userId);
        }

        #endregion

        #region 公用方法

        /// <summary>
        /// 握拳状态
        /// </summary>
        /// <param name="handIndex"></param>
        /// <returns></returns>
        public bool GetHandGrip(int handIndex)
        {
            return handIndex == 1 ? leftHandPosition.CheckGrip() : rightHandPosition.CheckGrip();
        }

        /// <summary>
        /// 释放状态
        /// </summary>
        /// <param name="handIndex"></param>
        /// <returns></returns>
        public bool GetHandRelease(int handIndex)
        {
            return handIndex == 1 ? leftHandPosition.CheckRelease() : rightHandPosition.CheckRelease();
        }

        /// <summary>
        /// 警告
        /// </summary>
        /// <param name="handIndex"></param>
        /// <returns></returns>
        public bool GetHandLasso(int handIndex)
        {
            return handIndex == 1 ? leftHandPosition.CheckLasso() : rightHandPosition.CheckLasso();
        }

        /// <summary>
        /// 手锁住状态
        /// </summary>
        /// <param name="handIndex"></param>
        /// <returns></returns>
        public bool GetLockHand(int handIndex)
        {
            return handIndex == 1 ? leftHandPosition.IsLockHand : rightHandPosition.IsLockHand;
        }

        /// <summary>
        /// 设置手势隐藏与显示
        /// </summary>
        /// <param name="handIndex">手编号</param>
        /// <param name="isActive">状态 <c>true</c> is active.</param>
        public void SetHandActive(int handIndex,bool isActive)
        { 
            switch(handIndex)
            {
                case 0:
                    rightHandPosition.IsHandActive = isActive;
                    break;
                case 1:
                    leftHandPosition.IsHandActive = isActive;
                    break;
                default:
                    rightHandPosition.IsHandActive = isActive;
                    leftHandPosition.IsHandActive = isActive;
                    break;
            }
        }

        #endregion
    }
}
