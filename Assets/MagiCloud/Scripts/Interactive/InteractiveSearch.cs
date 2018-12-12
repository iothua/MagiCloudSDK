using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using MagiCloud.Interactive.Distance;
using System.Linq;

namespace MagiCloud.Interactive
{
    /// <summary>
    /// 距离检索
    /// </summary>
    public class InteractiveSearch
    {
        public Dictionary<GameObject, List<DistanceDataManager>> dataManagers = new Dictionary<GameObject, List<DistanceDataManager>>();

        /// <summary>
        /// 启动交互
        /// </summary>
        public void OnStartInteraction(GameObject target, bool isGrab, bool defaultInteraction = false)
        {
            var interactions = target.GetComponentsInChildren<DistanceInteraction>();

            if (interactions.Count() == 0) return;

            OnSend(target, isGrab, interactions, defaultInteraction);

            OnPourAll(target, isGrab, interactions, InteractionType.Pour);
            OnPourAll(target, isGrab, interactions, InteractionType.All);

        }

        /// <summary>
        /// 发送
        /// </summary>
        /// <param name="target">物体</param>
        /// <param name="isGrab">是否抓取</param>
        /// <param name="interactions">距离互动集合</param>
        void OnSend(GameObject target, bool isGrab, DistanceInteraction[] interactions, bool defaultInteraction = false)
        {
            //获取到所有的发送信息
            var interactionSends = interactions.Where(_ => _.distanceData.interactionType == InteractionType.Send);

            //获取到发送集合
            interactionSends = DistanceSearch(target, interactionSends, isGrab);

            foreach (var interaction in interactionSends)
            {
                //根据指定key获取到接收点
                var managers = DistanceStorage.GetSendDistaceDataKey(interaction);

                List<DistanceDataManager> distanceManagers;
                dataManagers.TryGetValue(target, out distanceManagers);

                if (distanceManagers == null)
                    distanceManagers = new List<DistanceDataManager>();

                foreach (var item in managers)
                {
                    if (distanceManagers.Contains(item))
                        continue;

                    distanceManagers.Add(item);

                }

                //如果是初始交互，则对接收端筛选一次
                if (defaultInteraction)
                {
                    //var distances = new List<DistanceDataManager>();
                    //distances.CopyTo(distanceManagers.ToArray());

                    DistanceDataManager[] distances = new DistanceDataManager[distanceManagers.Count];



                    for (int i = 0; i < distanceManagers.Count; i++)
                    {
                        distances[i] = new DistanceDataManager
                        {
                            sendData = distanceManagers[i].sendData,
                            Distances = new List<DistanceInteraction>()
                        };

                        for (int j = 0; j < distanceManagers[i].Distances.Count; j++)
                        {
                            var distance = distanceManagers[i].Distances[j];
                            if (distance.AutoDetection)
                            {
                                distances[i].AddDistance(distance);
                            }
                        }
                        //var distanceings = new DistanceData[distanceManagers[i].Distanceing.Count];
                        //distanceManagers[i].Distanceing.CopyTo(distanceings);

                        //distances[i].Distanceing = distanceings.ToList();
                    }

                    //distances = distances.Select((obj) =>
                    //{
                    //    obj.Distances = obj.Distances.Where(_ => _.Interaction.AutoDetection).ToList();

                    //    return obj;

                    //}).ToList();

                    //distances = distances.Select((obj) =>
                    //{
                    //    obj.Distances = obj.Distances.Where(_ => _.Interaction.AutoDetection).ToList();

                    //    return obj;

                    //}).ToArray();


                    if (!dataManagers.ContainsKey(target))
                        dataManagers.Add(target, distances.ToList());
                    else
                        dataManagers[target] = distances.ToList();
                }
                else
                {

                    if (!dataManagers.ContainsKey(target))
                        dataManagers.Add(target, distanceManagers);
                    else
                        dataManagers[target] = distanceManagers;
                }

                //if (!dataManagers.ContainsKey(target))
                //    dataManagers.Add(target, distanceManagers);
                //else
                //    dataManagers[target] = distanceManagers;
            }
        }

        /// <summary>
        /// 根据不同的距离类型，赛选合适的距离检测信息
        /// </summary>
        /// <param name="target"></param>
        /// <param name="isGrab"></param>
        /// <param name="interactions"></param>
        /// <param name="interactionType"></param>
        void OnPourAll(GameObject target, bool isGrab, DistanceInteraction[] interactions,
            InteractionType interactionType, bool defaultInteraction = false)
        {
            var interactionPours = interactions.Where(_ => _.distanceData.interactionType == interactionType);

            interactionPours = DistanceSearch(target, interactionPours, isGrab);

            foreach (var interaction in interactionPours)
            {
                //获取到其他的接收点（All、Pour）
                var managers = DistanceStorage.GetSendDistaceDataAll(interaction, interactionType);
                
                //如果没有获取到其他的，则跳过
                if (managers.Count == 0) continue;

                //获取到当前已经存在的距离管理端
                List<DistanceDataManager> distanceManagers;
                dataManagers.TryGetValue(target, out distanceManagers);

                //如果不存在，则实例化新的
                if (distanceManagers == null)
                    distanceManagers = new List<DistanceDataManager>();

                //在当前的距离管理端中，找是否存在的，
                DistanceDataManager distanceManager = distanceManagers.Count == 0 ? new DistanceDataManager() :
                    distanceManagers.Find(obj => obj.sendData.Equals(interaction.distanceData)) ?? new DistanceDataManager();

                distanceManager.sendData = interaction;

                interaction.IsGrab = true;

                foreach (var item in managers)
                {
                    distanceManager.AddDistance(interaction, item.sendData);
                }

                distanceManagers.Add(distanceManager);

                if (!dataManagers.ContainsKey(target))
                    dataManagers.Add(target, distanceManagers);
                else
                    dataManagers[target] = distanceManagers;
            }
        }

        /// <summary>
        /// 距离筛选
        /// </summary>
        /// <param name="target">目标物体</param>
        /// <param name="interactions"></param>
        /// <param name="isGrab"></param>
        /// <returns></returns>
        private IEnumerable<DistanceInteraction> DistanceSearch(GameObject target, IEnumerable<DistanceInteraction> interactions, bool isGrab)
        {
            //在筛选一次，获取到所有的

            return interactions = interactions.Where(obj =>
            {
                if (isGrab && !obj.distanceData.IsGrabOwn)
                {
                    return obj;
                }
                else
                {
                    if (obj.FeaturesObjectController == null) return false;

                    if (obj.FeaturesObjectController.gameObject == target)
                        return obj;
                    else
                    {
                        return false;
                    }
                }
            });
        }

        /// <summary>
        /// 停止交互
        /// </summary>
        public void OnStopInteraction(GameObject target)
        {
            List<DistanceDataManager> managers;
            dataManagers.TryGetValue(target, out managers);

            if (managers == null || managers.Count == 0) return;

            foreach (var send in managers)
            {
                //遍历距离检测，并且触发相应的事件
                send.OnComputeRelesae();
            }

            managers.Clear();

            dataManagers.Remove(target);
        }

        public void OnUpdate()
        {

            if (dataManagers.Count == 0) return;

            //帅选被动点中的主动点，然后实时进行距离检测，判断是否靠近了某段距离
            foreach (var send in dataManagers)
            {
                for (int i = 0; i < send.Value.Count; i++)
                {
                    //遍历距离检测，并且触发相应的事件
                    send.Value[i].OnComputeDistance();
                }
            }
        }
    }
}
