using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace MagiCloud.Interactive.Distance
{
    /// <summary>
    /// 距离存储,及在编辑器中添加初始数据
    /// </summary>
    public static class DistanceStorage
    {

        public static List<DistanceInteraction> AllDistances = new List<DistanceInteraction>();//全部的距离检测点
        public static List<DistanceDataManager> dataManagers = new List<DistanceDataManager>();

        /// <summary>
        /// 校验距离检测（每隔一段时间）
        /// </summary>
        public static void CheckDistanceData()
        {
            /*
             1、获取到所有的发送点
             2、获取到所有的接送点
             3、在进行重新读取
             */

            List<DistanceDataManager> managers = new List<DistanceDataManager>();

            foreach (var item in Enum.GetValues(typeof(InteractionType)))
            {
                AddManagerDistances(managers, AllDistances.FindAll(obj => obj.distanceData.interactionType.Equals((InteractionType)item)));
            }

            dataManagers = managers;
        }

        /// <summary>
        /// 从管理器中添加距离检测
        /// </summary>
        /// <param name="managers"></param>
        /// <param name="distances"></param>
        private static void AddManagerDistances(List<DistanceDataManager> managers,List<DistanceInteraction> distances)
        {
            foreach (var distance in distances)
            {
                AddDistanceData(managers, distance);
            }
        }

        /// <summary>
        /// 添加距离数据
        /// </summary>
        /// <param name="data"></param>
        public static void AddDistanceData(DistanceInteraction data)
        {

            if (data == null) return;

            if (AllDistances.Contains(data)) return;

            AllDistances.Add(data);

            AddDistanceData(dataManagers, data);
            //校验一次
            CheckDistanceData();
        }

        /// <summary>
        /// 添加距离数据
        /// </summary>
        /// <param name="managers"></param>
        /// <param name="data"></param>
        public static void AddDistanceData(List<DistanceDataManager> managers, DistanceInteraction data)
        {
            switch (data.distanceData.interactionType)
            {
                //如果是主动点
                case InteractionType.Send:

                    if (IsExistDistanceData(managers, data)) return;

                    //新增一个主动点管理
                    DistanceDataManager sendDataManager = new DistanceDataManager();

                    sendDataManager.AddDistance(data);//添加到距离管理器中

                    managers.Add(sendDataManager);

                    //遍历下相同的Receive对象，从而来取得相应的对象

                    var receives = GetReceiveDistanceData(data.distanceData.TagID);

                    foreach (var item in receives)
                    {
                        sendDataManager.AddDistance(item);
                    }

                    break;
                case InteractionType.All:
                case InteractionType.Pour:

                    if (IsExistDistanceData(managers, data)) return;

                    //新增一个主动点管理
                    DistanceDataManager allDataManager = new DistanceDataManager();

                    allDataManager.AddDistance(data);//添加到距离管理器中

                    managers.Add(allDataManager);

                    //同时遍历下之前已经相同的TagID以及All，从而来取得联系

                    break;
                //如果是被动点
                case InteractionType.Receive:
                    //如果要加入的是被动点，去找匹配的主动点是否有，如果没有，则不添加，并且提示警告

                    //AddReceiveDistance(data);

                    var distanceDatas = managers.FindAll(obj => obj.sendData.distanceData.TagID.Equals(data.distanceData.TagID));
                    foreach (var item in distanceDatas)
                    {
                        item.AddDistance(data);
                    }

                    break;
                default:
                    //什么都不处理，相当于禁用
                    break;
            }

        }

        /// <summary>
        /// 获取接收距离数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static List<DistanceInteraction> GetReceiveDistanceDatas(DistanceInteraction data)
        {
            List<DistanceInteraction> distances = new List<DistanceInteraction>();
            var manager = GetSendDistanceManager(data);

            if (manager != null)
                distances = manager.Distances;

            return distances;
        }

        /// <summary>
        /// 删除距离数据信息
        /// </summary>
        /// <param name="data"></param>
        public static void DeleteDistanceData(DistanceInteraction data)
        {
            switch (data.distanceData.interactionType)
            {
                case InteractionType.Pour:
                case InteractionType.All:
                case InteractionType.Send:

                    if (!IsExistDistanceData(dataManagers,data))
                        return;

                    var manager = GetSendDistanceManager(data);
                    dataManagers.Remove(manager);

                    break;
                case InteractionType.Receive:

                    //根据这个被动点，获取到所有的主动点信息
                    var managers = GetSendDistaceDataAll(data, InteractionType.Send);

                    //遍历主动点，然后在去查找主动点中所有的被动点信息，将被动点数据从管理端移除掉
                    foreach (var item in managers)
                    {
                        var distanceData = item.GetDistanceData(data);
                        item.RemoveDistance(distanceData);
                    }

                    break;
                default:
                    break;
            }

            AllDistances.Remove(data);
            CheckDistanceData();
            //SaveDistanceData();//保存一次就好
        }

        /// <summary>
        /// 根据到所有的主动点接收数据，适用Send类型以及All类型(但是不包括本身)
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static List<DistanceDataManager> GetSendDistaceDataAll(DistanceInteraction data, InteractionType interactionType)
        {
            if (dataManagers == null) return null;

            return dataManagers.FindAll(obj => obj.sendData != null && obj.sendData.distanceData.TagID.Equals(data.distanceData.TagID)
            && obj.sendData.distanceData.interactionType.Equals(interactionType)
            && !obj.sendData.Equals(data)
                                        && !obj.sendData.FeaturesObjectController.Equals(data.FeaturesObjectController));
        }

        /// <summary>
        /// 根据距离点，获取到指定的发送点
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static List<DistanceDataManager> GetSendDistaceDataKey(DistanceInteraction data)
        {
            if (dataManagers == null) return null;

            return dataManagers.FindAll(obj => obj.sendData.distanceData.TagID.Equals(data.distanceData.TagID) && obj.sendData.Equals(data));
        }

        /// <summary>
        /// 根据类型，获取到所有的主动点信息，适用Send类型以及All类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static List<DistanceDataManager> GetSendDistanceData(InteractionType type)
        {
            if (dataManagers == null) return null;
            return dataManagers.FindAll(obj => obj.sendData.distanceData.interactionType.Equals(type));
        }

        /// <summary>
        /// 根据主动Send的TagID,获取到所有的被动TagID
        /// </summary>
        /// <param name="TagID"></param>
        /// <returns></returns>
        public static List<DistanceInteraction> GetReceiveDistanceData(string TagID)
        {
            List<DistanceInteraction> distances = new List<DistanceInteraction>();

            var managers = dataManagers.FindAll(obj => obj.Distances.Any(d => d.distanceData.TagID.Equals(TagID)));

            foreach (var manager in managers)
            {
                var items = manager.Distances.FindAll(obj => obj.distanceData.TagID.Equals(TagID));

                foreach (var item in items)
                {
                    if (!distances.Any(obj => obj.Equals(item)))
                        distances.Add(item);
                }
            }

            return distances;
        }


        /// <summary>
        /// 根据物体传递传递过来的距离对象，去找到相应的所在管理端
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static DistanceDataManager GetSendDistanceManager(DistanceInteraction data)
        {
            if (dataManagers == null) return null;

            return dataManagers.Find(obj => obj.sendData != null && obj.sendData.Equals(data));//寻找到一个GUID与TadID一致的对象
        }

        /// <summary>
        /// 判断主动点对象是否相同
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool IsExistDistanceData(List<DistanceDataManager> dataManagers, DistanceInteraction data)
        {
            
            if (dataManagers == null) return false;

            //在集合中是否存在此主动交互点
            return dataManagers.Any(obj => obj.sendData.Equals(data));
        }

        /// <summary>
        /// 合并两个集合
        /// </summary>
        /// <param name="dataManagers"></param>
        /// <param name="distances"></param>
        public static void Merge(this List<DistanceDataManager> dataManagers, List<DistanceDataManager> distances)
        {
            /*
             1、遍历目标距离信息，发送端中是否存在，如果存在，则不处理，如果不存在，则将这个对象都加进去
             2、在判断目标距离信息中，发送端下的接收端信息是否存在，不存在则加入，存在则不加入。
             */

            foreach (var distance in distances)
            {
                var manager = dataManagers.Find(obj => obj.sendData.Equals(distance.sendData));

                //如果不存在
                if (manager == null)
                {
                    dataManagers.Add(distance);
                }
                else
                {
                    //判断距离信息
                    foreach (var dis in distance.Distances)
                    {
                        manager.AddDistance(dis);
                    }
                }
            }
        }
    }
}
