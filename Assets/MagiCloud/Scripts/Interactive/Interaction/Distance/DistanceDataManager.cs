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
        public DistanceData sendData; //主动点对应的距离对象信息

        public List<DistanceData> Distances;//被动点的所有集合,包括主动点对象

        public List<DistanceData> Distanceing;

        public DistanceDataManager()
        {
            sendData = new DistanceData();
            Distances = new List<DistanceData>();
            Distanceing = new List<DistanceData>();
        }

        /// <summary>
        /// 加入距离
        /// </summary>
        /// <param name="distance"></param>
        public void AddDistance(DistanceData distance)
        {
            switch (distance.interactionType)
            {
                case InteractionType.Receive:
                    if (Distances.Any(obj => obj.EqualsObject(distance)))
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
        public void AddDistance(DistanceData distance, DistanceData target)
        {
            if (sendData != distance) return;

            if (distance.interactionType != target.interactionType) return;

            if (Distances.Contains(target)) return;

            Distances.Add(target);
        }

        /// <summary>
        /// 移除距离
        /// </summary>
        /// <param name="distance"></param>
        public void RemoveDistance(DistanceData distance)
        {
            var data = Distances.Find(obj => obj.EqualsObject(distance));
            Distances.Remove(data);
        }

        /// <summary>
        /// 根据传递过来的具体距离对象，来获取到从内存读取距离信息的对象
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public DistanceData GetDistanceData(DistanceData data)
        {
            return sendData.EqualsObject(data) ? sendData : Distances.Find(obj => obj.EqualsObject(data));
        }

        /// <summary>
        /// 计算距离
        /// </summary>
        public void OnComputeDistance()
        {

            foreach (var receive in Distances)
            {
                if (receive.Interaction == null) continue;

                //如果在距离范围内
                if (OnDistance(receive, sendData))
                {
                    switch (sendData.detectType)
                    {
                        //并且关系
                        case InteractionDetectType.And:

                            //如果都没有移入，则
                            if (!InteractionDistanceController.IsEnter(sendData, receive))
                            {
                                //判断两者的条件是否都可以进行交互。
                                if (receive.Interaction.IsCanInteraction(sendData.Interaction) &&
                                    sendData.Interaction.IsCanInteraction(receive.Interaction))
                                {
                                    InteractionDistanceController.OnEnter(sendData, receive);

                                    if (!Distanceing.Contains(receive))
                                        Distanceing.Add(receive);
                                }
                            }

                            if (InteractionDistanceController.IsEnter(sendData, receive))
                            {
                                InteractionDistanceController.OnStay(sendData, receive);
                            }

                            break;
                        case InteractionDetectType.Receive:

                            //如果是接收端，那么只需要计算接收端的IsEnter和receiveData看是否可以进行交互。
                            if (!InteractionDistanceController.IsEnter(sendData, receive)
                                && receive.Interaction.IsCanInteraction(sendData.Interaction))
                            {
                                InteractionDistanceController.OnEnter(sendData, receive);

                                if (!Distanceing.Contains(receive))
                                    Distanceing.Add(receive);
                            }

                            if (InteractionDistanceController.IsEnter(sendData, receive))
                            {

                                InteractionDistanceController.OnStay(sendData, receive);
                            }

                            break;

                        case InteractionDetectType.Send:

                            //如果是发送端为主，那么只需要计算发射端的IsEnter和sendData看是否可以进行交互
                            if (!InteractionDistanceController.IsEnter(sendData, receive)
                                && sendData.Interaction.IsCanInteraction(receive.Interaction))
                            {

                                InteractionDistanceController.OnEnter(sendData, receive);

                                if (!Distanceing.Contains(receive))
                                    Distanceing.Add(receive);
                            }

                            if (InteractionDistanceController.IsEnter(sendData, receive))
                            {
                                InteractionDistanceController.OnStay(sendData, receive);
                            }

                            break;

                        default:
                            break;
                    }

                }
                else
                {
                    //要加一层判断，否则一直执行是不好的
                    InteractionDistanceController.OnExit(sendData, receive);

                    if (Distanceing.Count == 0) continue;

                    if (Distanceing.Contains(receive))
                        Distanceing.Remove(receive);
                }
            }
        }

        bool OnDistance(DistanceData receiveDistance, DistanceData sendDistance)
        {
            switch (receiveDistance.distanceShape)
            {
                case DistanceShape.Sphere:
                    return Utilitys.Distance(receiveDistance.Position, sendDistance.Position, receiveDistance.distanceType) <= receiveDistance.distanceValue;
                case DistanceShape.Cube:
                    return Utilitys.CubeDistance(receiveDistance.Position, receiveDistance.Size, sendDistance.Position, receiveDistance.distanceType);
                default:
                    return false;
            }

        }

        /// <summary>
        /// 计算释放
        /// </summary>
        public void OnComputeRelesae()
        {
            if (Distanceing.Count == 0)
            {
                sendData.OnNotRelease();
                return;
            }

            bool isNotRelease = false;

            foreach (var receive in Distanceing.ToList())
            {
                if (receive.Interaction == null) continue;

                //检测是否有正在交互中，如果没有，则执行notRelease释放。
                if (!InteractionDistanceController.IsEnter(sendData, receive))
                {
                    isNotRelease = true;
                    continue;
                }

                isNotRelease = false;

                switch (sendData.detectType)
                {
                    case InteractionDetectType.And:

                        if (receive.Interaction.IsCanInteraction(sendData.Interaction) &&
                                sendData.Interaction.IsCanInteraction(receive.Interaction))
                        {
                            InteractionDistanceController.OnRelease(sendData, receive);
                        }

                        break;
                    case InteractionDetectType.Receive:
                        
                        if (receive.Interaction.IsCanInteraction(sendData.Interaction))
                        {
                            InteractionDistanceController.OnRelease(sendData, receive);
                        }

                        break;
                    case InteractionDetectType.Send:

                        if (sendData.Interaction.IsCanInteraction(receive.Interaction))
                        {
                            InteractionDistanceController.OnRelease(sendData, receive);
                        }

                        break;
                    default:
                        break;
                }
            }

            sendData.Interaction.IsGrab = false;

            if(isNotRelease)
                sendData.OnNotRelease(); ;
        }
    }
}
