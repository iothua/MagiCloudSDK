using System;
using System.Collections.Generic;
using UnityEngine;

namespace MagiCloud.Interactive.Distance
{
    /*
    1、思路
        1）要有一个交互控制端吗InteractionDistanceController？
        2）当进行Enter处理时，将此信息加入到InteractionDistance集合中，离开时，从集合中移除。
        3）Distance不应该只是IsEnter，应该加一个状态Exit,Enter,Complete。
    */

    /// <summary>
    /// 距离信息
    /// </summary>
    [Serializable]
    public class DistanceData
    {
        //private bool isEnable = false;
        private DistanceInteraction distanceInteraction;
        //距离的TagID
        public string TagID = "距离交互";

        /// <summary>
        /// 距离值
        /// </summary>
        public float distanceValue = 0.05f;

        /// <summary>
        /// 如果是在All状态下，也就是两者都可以交互，那么距离检测是以自己为标准，还是对方。
        /// 如果两个都是true或false，则采用默认的看谁先抓取，或者随机去找一个
        /// </summary>
        public bool IsSelf;

        //距离类型
        public DistanceType distanceType = DistanceType.D2D;

        //交互类型
        public InteractionType interactionType = InteractionType.Send;

        /// <summary>
        /// 交互检测优先级
        /// </summary>
        public InteractionDetectType detectType = InteractionDetectType.Receive;//以接收点为主

        //距离形状
        public DistanceShape distanceShape = DistanceShape.Sphere;

        //当距离形状状态为Cube时，该属性生效。Cube的范围大小
        public Vector3 Size = new Vector3(0.1f, 0.1f, 0.1f);

        /// <summary>
        /// 交互本身交互
        /// </summary>
        public bool IsGrabOwn = true;

        /// <summary>
        /// 是唯一的吗
        /// </summary>
        public bool IsOnly;

        /// <summary>
        /// 最大值-1为无限
        /// </summary>
        public int maxCount = -1;
    }
}
