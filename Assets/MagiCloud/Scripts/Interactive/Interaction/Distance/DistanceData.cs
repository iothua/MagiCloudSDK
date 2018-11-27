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
        #region 属性

        private bool isEnable = false;

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
        public DistanceType distanceType = DistanceType.DScreen;

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
        /// 是否激活
        /// </summary>
        public bool IsEnabel {
            get {
                return isEnable;
            }
            set {

                isEnable = value;

                if (isEnable)
                {
                    DistanceStorage.AddDistanceData(this);
                }
                else
                {
                    DistanceStorage.DeleteDistanceData(this);
                }
            }
        }

        /// <summary>
        /// 最大值-1为无限
        /// </summary>
        public int maxCount = -1;

        /// <summary>
        /// 交互对象
        /// </summary>
        public DistanceInteraction Interaction;

        /// <summary>
        /// 唯一对象(交互后的)
        /// </summary>
        public DistanceData OnlyDistance { get; set; }

        /// <summary>
        /// 多个对象集合(交互后的)
        /// </summary>
        public List<DistanceData> Distanced { get; set; }

        /// <summary>
        /// 距离交互中……
        /// </summary>
        public List<DistanceData> Distanceing { get; set; }

        /// <summary>
        /// 距离检测
        /// </summary>
        public Vector3 Position
        {
            get
            {
                return Interaction != null ? Interaction.transform.position : Vector3.zero;
            }
        }

        #endregion

        public bool EqualsObject(DistanceData distance)
        {
            return distance != null && TagID.Equals(distance.TagID) && Interaction.Equals(distance.Interaction);
        }

        /// <summary>
        /// 移入
        /// </summary>
        public void OnEnter(DistanceData target)
        {
            //如果已经有物体交互成功了，并且满足了某个条件，则不允许进行交互了

            switch (interactionType)
            {
                case InteractionType.Receive:
                case InteractionType.All:
                case InteractionType.Pour:

                    //将此信息，加入到正在交互中
                    AddReceiveDistancing(target);

                    Interaction.OnDistanceEnter(target.Interaction);

                    break;
                case InteractionType.Send:

                    Interaction.OnDistanceEnter(target.Interaction);

                    break;
            }

        }
        /// <summary>
        /// 离开
        /// </summary>
        public void OnExit(DistanceData target)
        {
            switch (interactionType)
            {
                case InteractionType.Receive:
                case InteractionType.All:
                case InteractionType.Pour:

                    if (IsOnly)
                    {
                        OnlyDistance = null;
                    }
                    else
                    {
                        RemoveReceiveDistancing(target);
                        RemoveReceiveDistanced(target);
                    }

                    Interaction.OnDistanceExit(target.Interaction);

                    break;
                case InteractionType.Send:

                    OnlyDistance = null;

                    Interaction.OnDistanceExit(target.Interaction);

                    break;
            }
        }

        /// <summary>
        /// 停留
        /// </summary>
        public void OnStay(DistanceData target)
        {

            if (Interaction == null) return;

            Interaction.OnDistanceStay(target.Interaction);
        }

        /// <summary>
        /// 松手
        /// </summary>
        public void OnRelesae(DistanceData target)
        {

            switch (interactionType)
            {
                case InteractionType.Receive:
                case InteractionType.All:
                case InteractionType.Pour:

                    if (IsOnly)
                    {
                        if (OnlyDistance == target)
                        {
                            Interaction.OnDistanceRelesae(target.Interaction);
                            Interaction.OnDistanceRelease(target.Interaction, InteractionReleaseStatus.Inside);
                            return;
                        }
                    }
                    else
                    {
                        if (Distanced != null && Distanced.Contains(target))
                        {
                            Interaction.OnDistanceRelesae(target.Interaction);
                            Interaction.OnDistanceRelease(target.Interaction, InteractionReleaseStatus.Inside);
                            return;
                        }
                    }

                    if (!OnCheck()) return;

                    Interaction.OnDistanceRelesae(target.Interaction);
                    Interaction.OnDistanceRelease(target.Interaction, InteractionReleaseStatus.Once);

                    if (IsOnly)
                    {
                        OnlyDistance = target;

                        return;
                    }
                    else
                    {
                        AddReceiveDistanced(target);
                    }

                    break;
                case InteractionType.Send:

                    if (!OnCheck())
                    {
                        Interaction.OnDistanceRelesae(target.Interaction);
                        Interaction.OnDistanceRelease(target.Interaction, InteractionReleaseStatus.Inside);

                        return;
                    }

                    Interaction.OnDistanceRelesae(target.Interaction);
                    Interaction.OnDistanceRelease(target.Interaction, InteractionReleaseStatus.Once);

                    AddSendDistance(target);

                    break;
            }
        }

        /// <summary>
        /// 没有执行交互时释放
        /// </summary>
        public void OnNotRelease()
        {
            if (Interaction == null) return;

            Interaction.OnDistanceNotInteractionRelease();
            Interaction.OnDistanceRelease(null, InteractionReleaseStatus.None);
        }

        /// <summary>
        /// 校验是否可以进行交互
        /// </summary>
        /// <returns></returns>
        public bool OnCheck()
        {
            switch (interactionType)
            {
                case InteractionType.Receive:
                case InteractionType.All:
                case InteractionType.Pour:

                    if (IsOnly)
                    {
                        if (OnlyDistance != null) return false;
                    }
                    else
                    {
                        if (maxCount == 0) return false;
                    }

                    break;
                case InteractionType.Send:

                    if (OnlyDistance != null) return false;

                    break;
            }


            return true;
        }

        /// <summary>
        /// 往接收端中添加发送端信息
        /// </summary>
        /// <param name="send"></param>
        public void AddReceiveDistanced(DistanceData send)
        {
            if (Distanced == null)
                Distanced = new List<DistanceData>();

            //移除正在交互的
            if (Distanceing.Contains(send))
                Distanceing.Remove(send);

            if (Distanced.Contains(send)) return;

            Distanced.Add(send);

            if (maxCount == -1)
                return;

            maxCount--;
        }

        /// <summary>
        /// 往接收端中添加发送距离信息
        /// </summary>
        /// <param name="send"></param>
        public void AddReceiveDistancing(DistanceData send)
        {
            if (Distanceing == null)
                Distanceing = new List<DistanceData>();

            if (Distanceing.Contains(send))
                return;

            Distanceing.Add(send);
        }


        /// <summary>
        /// 往发送端中添加接收端信息，一个发送端只能接收一个接收端
        /// </summary>
        /// <param name="receive"></param>
        public void AddSendDistance(DistanceData receive)
        {
            OnlyDistance = receive;
        }

        public void RemoveReceiveDistancing(DistanceData send)
        {
            if (Distanceing == null) return;

            if (!Distanceing.Contains(send)) return;

            Distanceing.Remove(send);
        }

        public void RemoveReceiveDistanced(DistanceData send)
        {
            if (Distanced == null) return;

            if (!Distanced.Contains(send)) return;

            Distanced.Remove(send);

            if (maxCount != -1)
                maxCount++;
        }

    }
}
