using System;
using System.Collections.Generic;
using UnityEngine;


namespace MagiCloud.Interactive.Distance
{
    /// <summary>
    /// 距离交互信息
    /// </summary>
    public class InteractionDistanceInfo
    {
        public DistanceData sendKey;
        public DistanceData receiveValue;

        //距离状态
        public DistanceStatus distanceStatus = DistanceStatus.Exit;

        public InteractionDistanceInfo(DistanceData key, DistanceData value, DistanceStatus distanceStatus)
        {
            this.sendKey = key;
            this.receiveValue = value;
            this.distanceStatus = distanceStatus;
        }

        /// <summary>
        /// 设置距离状态
        /// </summary>
        /// <param name="distanceStatus"></param>
        public void SetDistanceStatus(DistanceStatus distanceStatus)
        {
            this.distanceStatus = distanceStatus;
        }

    }


    /// <summary>
    /// 交互距离控制端
    /// </summary>
    public static class InteractionDistanceController
    {
        private static List<InteractionDistanceInfo> DistanceInfos = new List<InteractionDistanceInfo>();

        /// <summary>
        /// 移入
        /// </summary>
        /// <param name="send"></param>
        /// <param name="receive"></param>
        public static void OnEnter(DistanceData send,DistanceData receive)
        {
            //在次之前，还需要判断一次，否则一直执行循环查找，会很费资源，这么处理不恰当
            if (send.OnlyDistance != null || !receive.OnCheck()) return;

            //Debug.Log("进来-发送端：" + send.Interaction.FeaturesObjectController + " 接收端：" + receive.Interaction.FeaturesObjectController);

            InteractionDistanceInfo distanceInfo;

            if (IsContains(send, receive, out distanceInfo))
            {
                distanceInfo.SetDistanceStatus(DistanceStatus.Enter);
            }
            else
            {
                distanceInfo = new InteractionDistanceInfo(send, receive, DistanceStatus.Enter);
                DistanceInfos.Add(distanceInfo);
            }

            receive.OnEnter(send);
            send.OnEnter(receive);
        }

        /// <summary>
        /// 移出
        /// </summary>
        /// <param name="send"></param>
        /// <param name="receive"></param>
        public static void OnExit(DistanceData send, DistanceData receive)
        {
            //在次之前，还需要判断一次，否则一直执行循环查找，会很费资源，这么处理不恰当

            //To…Do

            InteractionDistanceInfo distanceInfo;

            if (IsContains(send, receive, out distanceInfo))
            {
                DistanceInfos.Remove(distanceInfo);

                receive.OnExit(send);
                send.OnExit(receive);

                //Debug.Log("离开了");
            }
        }

        /// <summary>
        /// 停留
        /// </summary>
        /// <param name="send"></param>
        /// <param name="receive"></param>
        public static void OnStay(DistanceData send, DistanceData receive)
        {
            receive.OnStay(send);
            send.OnStay(receive);
        }

        /// <summary>
        /// 放下，完成
        /// </summary>
        /// <param name="send"></param>
        /// <param name="receive"></param>
        public static void OnRelease(DistanceData send, DistanceData receive)
        {
            InteractionDistanceInfo distanceInfo;

            if (IsContains(send, receive, out distanceInfo))
            {
                distanceInfo.SetDistanceStatus(DistanceStatus.Complete);

                receive.OnRelesae(send);
                send.OnRelesae(receive);
            }
        }

        /// <summary>
        /// 是否存在
        /// </summary>
        /// <param name="send"></param>
        /// <param name="receive"></param>
        /// <returns></returns>
        static bool IsContains(DistanceData send, DistanceData receive,out InteractionDistanceInfo distanceInfo)
        {
            var distance = DistanceInfos.Find(obj => obj.sendKey.Equals(send) && obj.receiveValue.Equals(receive));

            distanceInfo = distance;

            return distance != null;
        }

        /// <summary>
        /// 两者之间是否已经靠近
        /// </summary>
        /// <param name="send"></param>
        /// <param name="receive"></param>
        /// <returns></returns>
        public static bool IsEnter(DistanceData send, DistanceData receive)
        {
            InteractionDistanceInfo distanceInfo;

            IsContains(send, receive, out distanceInfo);

            if (distanceInfo == null) return false;

            return distanceInfo.distanceStatus == DistanceStatus.Enter
                || distanceInfo.distanceStatus == DistanceStatus.Complete;
        }
    }
}
