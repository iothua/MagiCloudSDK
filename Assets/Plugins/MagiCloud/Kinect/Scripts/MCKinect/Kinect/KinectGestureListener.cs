//该脚本主要负责获取到抓取和松开手势的信号
//主要原理是判断指尖和掌心这两个关节点位置间的距离

using System.Collections;
using UnityEngine;

namespace MagiCloud.Kinect
{
    /// <summary>
    /// Kinect手势监听
    /// </summary>
    public class KinectGestureListener : MonoBehaviour, KinectGestures.GestureListenerInterface
    {

        //用于划分区域的关节位置(无关Z轴)
        private Vector3 rightHand_OverLay;
        private Vector3 rightShoulder_OverLay;
        private Vector3 leftHand_OverLay;
        private Vector3 leftShoulder_OverLay;

        //用于判断抓取的关节位置
        private Vector3 rightHandPos;
        private Vector3 rightTipPos;
        private Vector3 leftHandPos;
        private Vector3 leftTipPos;

        private bool rightGrip;
        private bool rightRelease;
        private bool leftGrip;
        private bool leftRelease;

        private bool rightLasso;
        private bool leftLasso;

        bool isLockHandRight;
        bool isLockHandLeft;

        bool isRightHandActive;
        bool isLeftHandActive;
        float rightTimer = 0;
        float leftTimer = 0;
        float holdTime = 0.2f;      //延时多少秒以后才开始监听手势


        //计算手移动的速度，太快的话不执行松开判断
        Vector3 currentHandRight = Vector3.zero;
        Vector3 lastHandRight = Vector3.zero;
        bool isHandMovingRight;
        Vector3 currentHandLeft = Vector3.zero;
        Vector3 lastHandLeft = Vector3.zero;
        bool isHandMovingLeft;

        //private static KinectGestureListener gestureListener;

        //private UIHandMoveControl kinectHand;
        private KinectOperate kinectOperate;
        private KinectManager kinectManager;

        enum HandArea
        {
            UP, SPREAD, MIDDLE, MIDDLESIDE, OUT
        }

        HandArea RightHandArea = HandArea.UP;
        HandArea LeftHandArea = HandArea.UP;

        enum HandState
        {
            Grip, Release, Lasso
        }

        HandState RightHandState = HandState.Release;
        HandState LeftHandState = HandState.Release;

        public void OnInitialize(KinectOperate kinectOperate)
        {
            this.kinectOperate = kinectOperate;
        }

        private void Update()
        {
            if (KinectConfig.GetHandStartStatus() == KinectActiveHandStadus.None) return;
            if (KinectConfig.GetKinectHandActiveStatus() != KinectHandStatus.Enable) return;
            UpdateJointsPos();

            switch (KinectConfig.GetHandStartStatus())
            {
                case KinectActiveHandStadus.Two:
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
                    break;
                case KinectActiveHandStadus.One:
                    ResetGesturesData();
                    DetectGestures();
                    break;
                default:
                    break;
            }
        }
        
        //重置手势的参数
        void ResetGesturesData()
        {
            isHandMovingLeft = false;
            isHandMovingRight = false;
            isLockHandRight = false;
            isLockHandLeft = false;
            //kinectHand.SetUIHandColor(0, Color.white);
            //kinectHand.SetUIHandColor(1, Color.white);
            RightHandArea = HandArea.UP;
            LeftHandArea = HandArea.UP;
        }

        //更新关节点位置
        void UpdateJointsPos()
        {
            kinectManager = KinectManager.Instance;
            rightHandPos = KinectCapture.instance.GetKinectHandPos(0);
            rightTipPos = KinectCapture.instance.GetKinectTipPos(0);
            leftHandPos = KinectCapture.instance.GetKinectHandPos(1);
            leftTipPos = KinectCapture.instance.GetKinectTipPos(1);

            rightHand_OverLay = KinectCapture.instance.GetOverlayHandPos(0);
            rightShoulder_OverLay = KinectCapture.instance.GetOverlayShoulderPos(0);
            leftHand_OverLay = KinectCapture.instance.GetOverlayHandPos(1);
            leftShoulder_OverLay = KinectCapture.instance.GetOverlayShoulderPos(1);
        }

        //根据手势当前状态判断相对手势的信号
        //如果是握下状态则只检测是否松开，反之则只检测是否握下
        void DetectGestures()
        {
            switch (RightHandState)
            {
                case HandState.Grip:
                    //Debug.Log("<color=blue>" + "<size=20>" + "Grip" + "</size>" + "</color>");
                    SwitchRightGestureRelease();
                    if (rightRelease)
                    {
                        RightHandState = HandState.Release;
                    }
                    SwitchRightLasso();
                    if (rightLasso)
                    {
                        RightHandState = HandState.Lasso;
                        //rightGrip = false;
                    }
                    break;
                case HandState.Release:
                    //Debug.Log("<color=blue>"+ "<size=20>" + "Release" + "</size>" + "</color>");
                    SwitchRightGestureGrip();
                    if (rightGrip)
                    {
                        RightHandState = HandState.Grip;
                    }
                    SwitchRightLasso();
                    if (rightLasso)
                    {
                        RightHandState = HandState.Lasso;
                        //rightLasso = false;
                    }
                    break;
                case HandState.Lasso:
                    //Debug.Log("<color=blue>" +"<size=20>"+ "Lasso" +"</size>"+ "</color>");
                    SwitchRightGestureRelease();
                    if (rightRelease)
                    {
                        //wasRightRelease = false;
                        RightHandState = HandState.Release;
                    }
                    SwitchRightGestureGrip();
                    if (rightGrip)
                    {
                        RightHandState = HandState.Grip;
                        //rightLasso = false;
                    }
                    break;
                default:
                    break;
            }

            switch (LeftHandState)
            {
                case HandState.Grip:
                    //Debug.Log("<color=yellow>" + "<size=20>" + "Grip" + "</size>" + "</color>");
                    SwitchLeftGestureRelease();
                    if (leftRelease)
                    {
                        LeftHandState = HandState.Release;
                    }
                    SwitchLeftLasso();
                    if (leftLasso)
                    {
                        LeftHandState = HandState.Lasso;
                        //leftLasso = false;
                    }
                    break;
                case HandState.Release:
                    //Debug.Log("<color=yellow>" + "<size=20>" + "Release" + "</size>" + "</color>");
                    SwitchLeftGestureGrip();
                    if (leftGrip)
                    {
                        LeftHandState = HandState.Grip;
                    }
                    SwitchLeftLasso();
                    if (leftLasso)
                    {
                        LeftHandState = HandState.Lasso;
                    }
                    break;
                case HandState.Lasso:
                    //Debug.Log("<color=yellow>" + "<size=20>" + "Lasso" + "</size>" + "</color>");
                    SwitchLeftGestureRelease();
                    if (leftRelease)
                    {
                        //wasLeftRelease = false;
                        LeftHandState = HandState.Release;
                    }

                    SwitchLeftGestureGrip();
                    if (leftGrip)
                    {
                        LeftHandState = HandState.Grip;
                        leftLasso = false;
                    }
                    break;
                default:
                    break;
            }
        }

        #region 握和松的手势判断
        public bool IsRightGrip()
        {
            if (isLockHandRight)
            {
                return false;
            }


            //if (rightGrip)
            //{
            //    rightGrip = false;
            //    return true;
            //}
            //else
            //{
            //    return false;
            //}


            if (rightGrip)
            {
                if (!isStartRG)
                    StartCoroutine(DelayBoolRG());
                //rightGrip = false;
                return true;
            }
            return false;
        }

        bool isStartRG = false;
        IEnumerator DelayBoolRG()
        {
            isStartRG = true;
            yield return new WaitForEndOfFrame();
            rightGrip = false;
            isStartRG = false;
        }

        public bool IsLeftGrip()
        {
            if (isLockHandLeft)
            {
                return false;
            }

            //if (!wasLeftGrip && leftGrip)
            //{
            //    wasLeftGrip = leftGrip;
            //    return true;
            //}
            //else
            //{
            //    return false;
            //}

            if (leftGrip)
            {
                //leftGrip = false;
                if (!isStartLG)
                    StartCoroutine(DelayBoolLG());
                return true;
            }
            return false;
        }

        bool isStartLG = false;
        IEnumerator DelayBoolLG()
        {
            isStartLG = true;
            yield return new WaitForEndOfFrame();
            leftGrip = false;
            isStartLG = false;
        }

        public bool IsRightRelease()
        {
            if (isHandMovingRight)
            {
                return false;
            }

            //if (rightRelease)
            //{
            //    rightRelease = false;
            //    return true;
            //}
            //else
            //{
            //    return false;
            //}

            if (rightRelease)
            {
                //rightRelease = false;
                if (!isStartRR)
                    StartCoroutine(DelayBoolRR());
                return true;
            }
            return false;
        }
        bool isStartRR = false;
        IEnumerator DelayBoolRR()
        {
            isStartRR = true;
            yield return new WaitForEndOfFrame();
            rightRelease = false;
            isStartRR = false;
        }

        public bool IsLeftRelease()
        {
            if (isHandMovingLeft)
            {
                return false;
            }

            //if (!wasLeftRelease && leftRelease)
            //{
            //    wasLeftRelease = leftRelease;
            //    return true;
            //}
            //else
            //{
            //    return false;
            //}

            if (leftRelease)
            {
                //leftRelease = false;
                if (!isStartLR)
                    StartCoroutine(DelayBoolLR());
                return true;
            }
            return false;
        }
        bool isStartLR = false;
        IEnumerator DelayBoolLR()
        {
            isStartLR = true;
            yield return new WaitForEndOfFrame();
            leftRelease = false;
            isStartLR = false;
        }

        public bool IsRightLasso()
        {
            if (rightLasso)
            {
                rightLasso = false;
                return true;
            }
            return false;
        }

        public bool IsLeftLasso()
        {
            if (leftLasso)
            {
                leftLasso = false;
                return true;
            }
            return false;
        }

        void SwitchRightGestureGrip()
        {
            if (kinectManager.IsRightHandConfidenceHigh(KinectConfig.UserID))
            {
                GripBySystem(ref rightGrip, 0);
                return;
            }

            switch (RightHandArea)
            {
                case HandArea.UP:
                    //Debug.Log("Up");
                    GripBySystem(ref rightGrip, 0);
                    break;
                case HandArea.SPREAD:
                    //Debug.Log("Spread");
                    GripBySystem(ref rightGrip, 0);
                    break;
                case HandArea.MIDDLESIDE:
                case HandArea.MIDDLE:
                    //Debug.Log("Middle");
                    GripByGesture1(rightHandPos, rightTipPos, ref rightGrip, ref rightRelease);
                    break;
                case HandArea.OUT:
                    //Debug.Log("Out");
                    GripByGesture2(rightHandPos, rightTipPos, ref rightGrip, ref rightRelease);
                    break;
                default:
                    break;
            }
        }
        void SwitchRightGestureRelease()
        {
            if (kinectManager.IsRightHandConfidenceHigh(KinectConfig.UserID))
            {
                ReleaseBySystem(ref rightGrip, ref rightRelease, 0);
                return;
            }

            switch (RightHandArea)
            {
                case HandArea.UP:
                case HandArea.SPREAD:
                    ReleaseBySystem(ref rightGrip, ref rightRelease, 0);
                    break;
                case HandArea.MIDDLESIDE:
                    ReleaseBySystem(ref rightGrip, ref rightRelease, 0);
                    break;
                case HandArea.MIDDLE:
                case HandArea.OUT:
                    ReleaseByGesture2(rightHandPos, rightTipPos, ref rightGrip, ref rightRelease);
                    break;
                default:
                    break;
            }
        }
        void SwitchRightLasso()
        {
          if (kinectManager.GetRightHandState(KinectConfig.UserID) == KinectInterop.HandState.Lasso)
          {
              rightLasso = true;
                rightGrip = false;
                rightRelease = false;
          }
        }       

        void SwitchLeftGestureGrip()
        {
            if (kinectManager.IsLeftHandConfidenceHigh(KinectConfig.UserID))
            {
                GripBySystem(ref leftGrip, 1);
                return;
            }

            switch (LeftHandArea)
            {
                case HandArea.UP:
                    GripBySystem(ref leftGrip, 1);
                    break;
                case HandArea.SPREAD:
                    GripBySystem(ref leftGrip, 1);
                    break;
                case HandArea.MIDDLESIDE:
                case HandArea.MIDDLE:
                    GripByGesture1(leftHandPos, leftTipPos, ref leftGrip, ref leftRelease);
                    break;
                case HandArea.OUT:
                    GripByGesture2(leftHandPos, leftTipPos, ref leftGrip, ref leftRelease);
                    break;
                default:
                    break;
            }
        }
        void SwitchLeftGestureRelease()
        {
            if (kinectManager.IsLeftHandConfidenceHigh(KinectConfig.UserID))
            {
                ReleaseBySystem(ref leftGrip, ref leftRelease, 1);
                return;
            }

            switch (LeftHandArea)
            {
                case HandArea.UP:
                case HandArea.SPREAD:
                    ReleaseBySystem(ref leftGrip, ref leftRelease, 1);
                    break;
                case HandArea.MIDDLESIDE:
                    ReleaseBySystem(ref leftGrip, ref leftRelease, 1);

                    break;
                case HandArea.MIDDLE:
                case HandArea.OUT:
                    ReleaseByGesture2(leftHandPos, leftTipPos, ref leftGrip, ref leftRelease);
                    break;
                default:
                    break;
            }
        }
        void SwitchLeftLasso()
        {
            if (kinectManager.GetLeftHandState(KinectConfig.UserID) == KinectInterop.HandState.Lasso)
            {
                leftLasso = true;
                leftGrip = false;
                leftRelease = false;
            }
        }


        // 区分区域
        void DivideArea()
        {
            //右手
            if (rightHand_OverLay.y >= rightShoulder_OverLay.y)
            {
                RightHandArea = HandArea.UP;
            }
            else if (rightHand_OverLay.x < leftShoulder_OverLay.x)
            {
                RightHandArea = HandArea.OUT;
            }
            else if (rightHand_OverLay.x - 1.2f < rightShoulder_OverLay.x)
            {
                RightHandArea = HandArea.MIDDLE;
            }
            else if (rightHand_OverLay.x - 4f < rightShoulder_OverLay.x && rightHand_OverLay.y - rightShoulder_OverLay.y < -2f)
            {
                RightHandArea = HandArea.MIDDLESIDE;
            }
            else
            {
                RightHandArea = HandArea.SPREAD;
            }

            //左手
            if (leftHand_OverLay.y >= leftShoulder_OverLay.y)
            {
                LeftHandArea = HandArea.UP;
            }
            else if (leftHand_OverLay.x > rightShoulder_OverLay.x)
            {
                LeftHandArea = HandArea.OUT;
            }
            else if (leftHand_OverLay.x + 1.2f > leftShoulder_OverLay.x)
            {
                LeftHandArea = HandArea.MIDDLE;
            }
            else if (leftHand_OverLay.x + 4f > leftShoulder_OverLay.x && leftHand_OverLay.y - leftShoulder_OverLay.y < -2f)
            {
                LeftHandArea = HandArea.MIDDLESIDE;
            }
            else
            {
                LeftHandArea = HandArea.SPREAD;
            }
        }

        #region 抓取判断
        void ReleaseByGesture1(Vector3 hand, Vector3 handTip, ref bool IsGrip,
            ref bool IsRelease)
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

        void ReleaseByGesture2(Vector3 hand, Vector3 handTip, ref bool IsGrip,
            ref bool IsRelease)
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

        void GripByGesture1(Vector3 hand, Vector3 handTip, ref bool IsGrip,
            ref bool IsRelease)
        {
            if (Mathf.Abs(Vector3.Distance(hand, handTip)) < 0.050f)
            {
                IsGrip = true;
            }
        }

        void GripByGesture2(Vector3 hand, Vector3 handTip, ref bool IsGrip,
            ref bool IsRelease)
        {
            if (Mathf.Abs(Vector3.Distance(hand, handTip)) < 0.067f)
            {
                IsGrip = true;
            }
        }

        void ReleaseBySystem(ref bool IsGrip, ref bool IsRelease, int handIndex)
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

        bool GetHandOpen(int handIndex)
        {
            if (handIndex == 0)
            {
                return kinectManager.GetRightHandState(KinectConfig.UserID) == KinectInterop.HandState.Open;
            }
            else
            {
                return kinectManager.GetLeftHandState(KinectConfig.UserID) == KinectInterop.HandState.Open;
            }
        }

        void GripBySystem(ref bool IsGrip, int handIndex)
        {
            if (handIndex == 0)
            {
                if (kinectManager.GetRightHandState(KinectConfig.UserID) == KinectInterop.HandState.Closed)
                {
                    IsGrip = true;
                }
            }
            else
            {
                if (kinectManager.GetLeftHandState(KinectConfig.UserID) == KinectInterop.HandState.Closed)
                {
                    IsGrip = true;
                }
            }
        }

        #endregion


        #endregion

        #region 检测手是否离胸前太近
        //根据安全距离锁定手势(锁定手势:无法进行抓取操作)
        void LockHand()
        {
            Vector3 vHandRight = KinectCapture.instance.GetKinectHandPos(0);
            Vector3 vShoulderRight = KinectCapture.instance.GetKinectShoulderPos(0);
            Vector3 vHandLeft = KinectCapture.instance.GetKinectHandPos(1);
            Vector3 vShoulderLeft = KinectCapture.instance.GetKinectShoulderPos(1);
            //if (vHandRight.x - vShoulderRight.x < 0.22f && vHandRight.y - vShoulderRight.y < 0.22f && vShoulderRight.z - vHandRight.z < 0.23f)
            if ((vHandRight.x - vShoulderLeft.x > -0.22f && vHandRight.x - vShoulderLeft.x < 0 && 
                vHandRight.y - vShoulderLeft.y > -0.22f && vHandRight.y - vShoulderLeft.y < 0 && 
                vShoulderRight.z - vHandRight.z < 0.23f) ||
                (vHandRight.x - vShoulderRight.x < 0 && vHandRight.x - vShoulderRight.x > -1.2f && 
                vHandRight.y - vShoulderRight.y < 0 && vHandRight.z - vShoulderRight.z > -0.13f))
            {
                isLockHandRight = true;
            }
            else
            {
                isLockHandRight = false;
            }

            //if (vHandLeft.x - vShoulderLeft.x > -0.22f && vHandLeft.y - vShoulderLeft.y < 0.22f && vShoulderLeft.z - vHandLeft.z < 0.23f)
            if ((vHandLeft.x - vShoulderRight.x > 0 && vHandLeft.x - vShoulderRight.x < 0.22f &&
                 vHandLeft.y - vShoulderRight.y > -0.22f && vHandLeft.y - vShoulderRight.y < 0 &&
                 vShoulderRight.z - vHandRight.z < 0.23f) ||
                (vHandLeft.x - vShoulderLeft.x < 1.2f && vHandLeft.x - vShoulderLeft.x > 0 &&
                vHandLeft.y - vShoulderLeft.y < 0 && vHandLeft.z - vShoulderLeft.z > -0.13f))
            {
                isLockHandLeft = true;
            }
            else
            {
                isLockHandLeft = false;
            }
        }

        #endregion

        #region 0.3开启和关闭抓取手势
        //需求： 为了减少误抓取的情况。 当手放在可抓取的物体上经过0.5秒后在开始监听抓取手势。如果手没照射到物体，则停止监听。

        /// <summary>
        /// 当物体高亮时会调用这个方法。
        /// 计时达到时间后，将激活手势监听的Bool值设为true
        /// </summary>
        /// <param name="handIndex">区分左右手</param>
        public void StartGripGesture(int handIndex)
        {
            if (handIndex == 0)
            {
                if (rightTimer < holdTime)
                {
                    rightTimer += Time.deltaTime;
                }
                else
                {
                    rightTimer = 0;
                    isRightHandActive = true;
                }
            }
            else if (handIndex == 1)
            {
                if (leftTimer < holdTime)
                {
                    leftTimer += Time.deltaTime;
                }
                else
                {
                    leftTimer = 0;
                    isLeftHandActive = true;
                }
            }
        }

        /// <summary>
        /// 停止手势监听
        /// </summary>
        /// <param name="handIndex">区分左右手</param>
        public void StopGripGesture(int handIndex)
        {
            if (handIndex == 0)
            {
                rightTimer = 0;
                isRightHandActive = false;
            }
            else if (handIndex == 1)
            {
                leftTimer = 0;
                isLeftHandActive = false;
            }
        }

        public bool GetHandActive(int handIndex)
        {
            if (handIndex == 0)
            {
                return isRightHandActive;
            }
            else if (handIndex == 1)
            {
                return isLeftHandActive;
            }
            return false;
        }

        #endregion

        #region 检测手移动速度

        void HandSpeedLimit()
        {
            currentHandRight = rightHandPos;
            currentHandLeft = leftHandPos;
            float deltaHandRight = Mathf.Abs(Vector3.Distance(currentHandRight, lastHandRight));
            float deltaHandLeft = Mathf.Abs(Vector3.Distance(currentHandLeft, lastHandLeft));
            float handSpeedRight = deltaHandRight / Time.deltaTime;
            float handSpeedLeft = deltaHandLeft / Time.deltaTime;
            lastHandRight = rightHandPos;
            lastHandLeft = leftHandPos;
            if (handSpeedRight > 1f)
            {
                isHandMovingRight = true;
            }
            else
            {
                isHandMovingRight = false;
            }
            if (handSpeedLeft > 1f)
            {
                isHandMovingLeft = true;
            }
            else
            {
                isHandMovingLeft = false;
            }
        }

        #endregion

        #region 用户检测和丢失
        /// <summary>
        /// 用户检测
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="userIndex"></param>
        public void UserDetected(long userId, int userIndex)
        {
            kinectOperate.kinectUserManager.AddUser(userId);
        }

        /// <summary>
        /// 用户丢失
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="userIndex"></param>
        public void UserLost(long userId, int userIndex)
        {
            kinectOperate.kinectUserManager.LostUser(userId);
        }
        #endregion

        #region 公开方法
        /// <summary>
        /// 检测手是否被抓取
        /// </summary>
        /// <param name="handIndex">0：右手 1：左手</param>
        /// <returns></returns>
        public bool GetHandGrip(int handIndex)
        {
            if (handIndex == 0)
            {
                return IsRightGrip();
            }
            else if (handIndex == 1)
            {
                return IsLeftGrip();
            }
            return false;
        }

        /// <summary>
        /// 检测手是否被释放
        /// </summary>
        /// <param name="handIndex">0：右手 1：左手</param>
        /// <returns></returns>
        public bool GetHandRelease(int handIndex)
        {
            if (handIndex == 0)
            {
                //print(IsRightRelease());
                return IsRightRelease();
            }
            else if (handIndex == 1)
            {
                return IsLeftRelease();
            }
            return false;
        }

        public bool GetHandLasso(int handIndex)
        {
            if (handIndex == 0)
            {
                return IsRightLasso();
            }
            else if (handIndex == 1)
            {
                return IsLeftLasso();
            }
            return false;
        }

        public bool GetLockHand(int handIndex)
        {
            if (handIndex == 0)
                return isLockHandRight;
            else
                return isLockHandLeft;
        }
        #endregion

        #region 没有用到
        //这是接口的东西，一开始是有用到的，后来改为完全自定义的手势就没用了

        //手势取消
        public bool GestureCancelled(long userId, int userIndex, KinectGestures.Gestures gesture, KinectInterop.JointType joint)
        {
            return true;
        }

        //手势完成
        public bool GestureCompleted(long userId, int userIndex, KinectGestures.Gestures gesture, KinectInterop.JointType joint, Vector3 screenPos)
        {
            return true;
        }

        //手势进行中
        public void GestureInProgress(long userId, int userIndex, KinectGestures.Gestures gesture, float progress, KinectInterop.JointType joint, Vector3 screenPos)
        {
        }
        #endregion

    }
}
