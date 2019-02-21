using MCKinect.Events;
using UnityEngine;

namespace MagiCloud.Kinect
{
    /// <summary>
    /// 手势初始化及手势开启关闭控制
    /// </summary>
    public class KinectHandFunction
    {
        //true 表示启动手，false 表示关闭手
        protected bool isHand = false;

        //0 表示右手，1 表示左手
        protected int handIndex;

        //临时坐标，用于处理平滑
        protected Vector3 tempPos = Vector3.zero;

        protected KinectGestureListener kinectGestureListener;

        // 另一只手的HandFunction
        protected KinectHandFunction otherHandFunction;

        public bool IsHand { get { return isHand; } }

        public void StartHand()
        {
            KinectTransfer.RunUpdateOnce();
            isHand = true;
            KinectEventHandStart.SendListener(handIndex);
        }

        public void StopHand()
        {
            isHand = false;
            KinectEventHandStop.SendListener(handIndex);

        }

        public void OnInit(KinectGestureListener kinectGestureListener, KinectHandFunction otherHand, int handIndex)
        {
            this.kinectGestureListener = kinectGestureListener;
            otherHandFunction = otherHand;
            this.handIndex = handIndex;
        }

    }
}
