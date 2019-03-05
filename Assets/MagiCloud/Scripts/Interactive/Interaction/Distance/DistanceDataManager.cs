using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace MagiCloud.Interactive.Distance
{
    /// <summary>
    /// 距离数据管理
    /// </summary>
    [SerializeField]
    public class DistanceDataManager
    {
        public DistanceInteraction sendData; //主动点对应的距离对象信息

        public List<DistanceInteraction> Distances;//被动点的所有集合,包括主动点对象

        public List<DistanceInteraction> Distanceing;

        private DistanceInteraction tempDistance = null;
        private float tempValue = 0;

        public DistanceDataManager()
        {
            //sendData = new DistanceInteraction();
            Distances = new List<DistanceInteraction>();
            Distanceing = new List<DistanceInteraction>();
        }

        /// <summary>
        /// 加入距离
        /// </summary>
        /// <param name="distance"></param>
        public void AddDistance(DistanceInteraction distance)
        {
            switch (distance.distanceData.interactionType)
            {
                case InteractionType.Receive:
                    if (Distances.Any(obj => obj.Equals(distance)))
                        return;

                    Distances.Add(distance);

                    break;
                case InteractionType.Send:
                case InteractionType.Pour:
                case InteractionType.All:
                    sendData = distance;
                    break;
            }
        }

        /// <summary>
        /// 加入距离
        /// </summary>
        /// <param name="distance"></param>
        /// <param name="target"></param>
        public void AddDistance(DistanceInteraction distance,DistanceInteraction target)
        {
            if (sendData != distance) return;

            if (distance.distanceData.interactionType != target.distanceData.interactionType) return;

            if (Distances.Contains(target)) return;

            Distances.Add(target);
        }

        /// <summary>
        /// 移除距离
        /// </summary>
        /// <param name="distance"></param>
        public void RemoveDistance(DistanceInteraction distance)
        {
            var data = Distances.Find(obj => obj.Equals(distance));
            Distances.Remove(data);
        }

        /// <summary>
        /// 根据传递过来的具体距离对象，来获取到从内存读取距离信息的对象
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public DistanceInteraction GetDistanceData(DistanceInteraction data)
        {
            return sendData.Equals(data) ? sendData : Distances.Find(obj => obj.Equals(data));
        }

        /// <summary>
        /// 计算距离
        /// </summary>
        public void OnComputeDistance()
        {

            if (!sendData.FeaturesObjectController.IsEnable)
                return;

            if (sendData.distanceData.IsShort)
            {
                

                foreach (var receive in Distances)
                {
                    if (receive == null) continue;

                    if (!receive.FeaturesObjectController.IsEnable) continue;

                    float distanceValue;

                    if (OnDistance(receive, sendData, out distanceValue))
                    {

                        if (tempDistance == null)
                        {
                            tempDistance = receive;
                            tempValue = distanceValue;

                            OnEnter(receive);
                        }
                        else
                        {
                            if (tempValue > distanceValue && tempDistance != receive)
                            {

                                Debug.Log("距离值：" + distanceValue + "    距离对象：" + tempDistance);
                                //先执行退出，然后在执行移入
                                OnExit(tempDistance);

                                tempDistance = receive;
                                tempValue = distanceValue;

                                OnEnter(receive);
                                continue;
                            }

                            OnEnter(receive);
                        }
                    }
                    else
                    {
                        OnExit(receive);
                        if (tempDistance == receive)
                        {
                            tempDistance = null;
                            distanceValue = 0;
                        }
                    }
                }
            }
            else
            {
                foreach (var receive in Distances)
                {
                    if (receive == null) continue;

                    if (!receive.FeaturesObjectController.IsEnable) continue;

                    float distanceValue;

                    //如果在距离范围内
                    if (OnDistance(receive, sendData, out distanceValue))
                    {
                        OnEnter(receive);
                    }
                    else
                    {
                        OnExit(receive);
                    }
                }
            }

        }

        void OnEnter(DistanceInteraction receive)
        {
            switch (sendData.distanceData.detectType)
            {
                //并且关系
                case InteractionDetectType.And:

                    //如果都没有移入，则
                    if (!InteractionDistanceController.IsEnter(sendData, receive))
                    {
                        //判断两者的条件是否都可以进行交互。
                        if (receive.IsCanInteraction(sendData) &&
                            sendData.IsCanInteraction(receive))
                        {
                            InteractionDistanceController.OnEnter(sendData, receive);

                            if (!Distanceing.Contains(receive))
                                Distanceing.Add(receive);
                        }
                    }

                    if (InteractionDistanceController.IsEnter(sendData, receive))
                    {
                        if (!Distanceing.Contains(receive))
                            Distanceing.Add(receive);
                        InteractionDistanceController.OnStay(sendData, receive);
                    }

                    break;
                case InteractionDetectType.Receive:

                    //如果是接收端，那么只需要计算接收端的IsEnter和receiveData看是否可以进行交互。
                    if (!InteractionDistanceController.IsEnter(sendData, receive)
                        && receive.IsCanInteraction(sendData))
                    {
                        InteractionDistanceController.OnEnter(sendData, receive);

                        if (!Distanceing.Contains(receive))
                            Distanceing.Add(receive);
                    }

                    if (InteractionDistanceController.IsEnter(sendData, receive))
                    {
                        if (!Distanceing.Contains(receive))
                            Distanceing.Add(receive);
                        InteractionDistanceController.OnStay(sendData, receive);
                    }

                    break;

                case InteractionDetectType.Send:

                    //如果是发送端为主，那么只需要计算发射端的IsEnter和sendData看是否可以进行交互
                    if (!InteractionDistanceController.IsEnter(sendData, receive)
                        && sendData.IsCanInteraction(receive))
                    {

                        InteractionDistanceController.OnEnter(sendData, receive);

                        if (!Distanceing.Contains(receive))
                            Distanceing.Add(receive);
                    }

                    if (InteractionDistanceController.IsEnter(sendData, receive))
                    {
                        if (!Distanceing.Contains(receive))
                            Distanceing.Add(receive);
                        InteractionDistanceController.OnStay(sendData, receive);
                    }

                    break;

                default:
                    break;
            }
        }

        void OnExit(DistanceInteraction receive)
        {
            //要加一层判断，否则一直执行是不好的
            InteractionDistanceController.OnExit(sendData, receive);

            if (Distanceing.Count == 0) return;

            if (Distanceing.Contains(receive))
                Distanceing.Remove(receive);
        }

        bool OnDistance(DistanceInteraction receiveDistance,DistanceInteraction sendDistance,out float distanceValue)
        {
            switch (receiveDistance.distanceData.distanceShape)
            {
                case DistanceShape.Sphere:
                    return Utilitys.Distance(receiveDistance.Position,sendDistance.Position,receiveDistance.distanceData.distanceType,out distanceValue) <= receiveDistance.distanceData.distanceValue;
                case DistanceShape.Cube:
                    return Utilitys.CubeDistance(receiveDistance.Position,receiveDistance.distanceData.Size,sendDistance.Position,receiveDistance.distanceData.distanceType,out distanceValue);
                default:
                    distanceValue = -1;
                    return false;
            }

        }

        /// <summary>
        /// 计算释放
        /// </summary>
        public void OnComputeRelesae(bool isAuto=false )
        {
            if (Distanceing.Count == 0)
            {
                sendData.OnInteractionNotRelease();
                return;
            }

            bool isNotRelease = false;

            foreach (var receive in Distanceing.ToList())
            {
                if (receive == null) continue;

                //检测是否有正在交互中，如果没有，则执行notRelease释放。
                if (!InteractionDistanceController.IsEnter(sendData,receive))
                {
                    isNotRelease = true;
                    continue;
                }

                isNotRelease = false;

                switch (sendData.distanceData.detectType)
                {
                    case InteractionDetectType.And:

                        if (receive.IsCanInteraction(sendData) &&
                                sendData.IsCanInteraction(receive))
                        {
                            InteractionDistanceController.OnRelease(sendData,receive,isAuto);
                        }

                        break;
                    case InteractionDetectType.Receive:

                        if (receive.IsCanInteraction(sendData))
                        {
                            InteractionDistanceController.OnRelease(sendData,receive,isAuto);
                        }

                        break;
                    case InteractionDetectType.Send:

                        if (sendData.IsCanInteraction(receive))
                        {
                            InteractionDistanceController.OnRelease(sendData,receive,isAuto);
                        }

                        break;
                    default:
                        break;
                }
            }

            //Debug.Log("SendData IsGrab");
            sendData.IsGrab = false;
            sendData.HandIndex = -1;

            //如果不存在有交互的，就进行无释放。
            if (isNotRelease)
                sendData.OnInteractionNotRelease();
        }
    }
}
