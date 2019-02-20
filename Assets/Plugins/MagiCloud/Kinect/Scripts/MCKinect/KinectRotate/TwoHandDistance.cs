using UnityEngine;

namespace MagiCloud.Kinect
{
    public class TwoHandDistance
    {
        //左手与右手的距离
        float dis;

        //上一帧左手与右手的距离
        float lastdis;

        //放大起始位置
        Vector3 startPos;

        public TwoHandDistance()
        {

            dis = 0;
            lastdis = 0;
        }

        /// <summary>
        /// 双手都握
        /// </summary>
        public void TwoHandGrip()
        {
            dis = Vector3.Distance(MInputKinect.ScreenHandPostion(0), MInputKinect.ScreenHandPostion(1));
            lastdis = dis;
        }

        /// <summary>
        /// 双手都放
        /// </summary>
        public void TwoHandIdle()
        {
            dis = 0;
            lastdis = 0;
        }

        public float ZoomCameraToMoveFloat()
        {

            float moveDistance = 0;
            lastdis = Vector3.Distance(MInputKinect.ScreenHandPostion(0), MInputKinect.ScreenHandPostion(1));

            //上一帧和下一帧的差值大于10才进行缩放
            //if (Mathf.Abs(dis - lastdis) > 1f)
            {
                moveDistance = lastdis - dis;
            }
            dis = lastdis;
            return moveDistance;
        }
    }
}