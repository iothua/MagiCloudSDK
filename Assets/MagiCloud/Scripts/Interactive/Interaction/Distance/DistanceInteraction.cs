using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using MagiCloud.Interactive.Actions;
using System.Collections.Generic;

namespace MagiCloud.Interactive.Distance
{
    [System.Serializable]
    public class EventDistanceInteraction :UnityEvent<DistanceInteraction> { }

    public class EventDistanceInteractionRelease :UnityEvent<DistanceInteraction,InteractionReleaseStatus> { }

    /// <summary>
    /// 距离交互(挂在物体中)
    /// </summary>
    [ExecuteInEditMode]
    public class DistanceInteraction :MonoBehaviour
    {
        public DistanceData distanceData;

        #region 事件

        //靠近、停留、离开、放下、中断(OnBreak)
        public EventDistanceInteraction OnEnter, OnStay, OnExit;

        public EventDistanceInteraction OnRelease; //交互距离后交互释放
        public EventDistanceInteractionRelease OnStatusRelease; //交互后，第二次释放
        public UnityEvent OnNotRelease;//没有交互时的释放

        #endregion

        private Features.FeaturesObjectController featuresObject;
        /// <summary>
        /// 功能对象
        /// </summary>
        public Features.FeaturesObjectController FeaturesObjectController
        {
            get
            {

                if (featuresObject == null)
                {
                    featuresObject = gameObject.GetComponentInParent<Features.FeaturesObjectController>();
                }

                return featuresObject;
            }
        }

        /// <summary>
        /// 外部交互对象
        /// </summary>
        //public InteractionBase ExternalInteraction { get; set; }
        public IExternalInteraction ExternalInteraction { get; set; }

        /// <summary>
        /// 初始距离检测，在距离内则进行交互
        /// </summary>
        public bool AutoDetection = true;
        public bool HasDetected { get; set; }
        public bool ActiveParent;
        public bool ActiveShadow;

        public InteractionParent interactionParent; //父子关系
        public InteractionShadow interactionShadow; //虚影关系

        /// <summary>
        /// 是否被抓取
        /// </summary>
        public bool IsGrab { get; set; }

        public int HandIndex = -1;

        public Vector3 Position
        {
            get
            {
                return transform.position;
            }
        }

        protected virtual void Awake()
        {

            if (OnEnter == null)
                OnEnter = new EventDistanceInteraction();

            if (OnStay == null)
                OnStay = new EventDistanceInteraction();

            if (OnExit == null)
                OnExit = new EventDistanceInteraction();

            if (OnRelease == null)
                OnRelease = new EventDistanceInteraction();

            if (OnStatusRelease == null)
                OnStatusRelease = new EventDistanceInteractionRelease();

            if (OnNotRelease == null)
                OnNotRelease = new UnityEvent();

            //加入时，检索一次
            if (distanceData == null)
            {
                distanceData = new DistanceData();
            }

            if (ActiveShadow)
            {
                AddShadow();
            }
            else
            {
                RemoveShadow();
            }

            if (ActiveParent)
            {
                AddParent();
            }
            else
            {
                RemoveParent();
            }
        }

        protected virtual void OnEnable()
        {
            //distanceData.IsEnabel = true;
            //统一调用，去匹配数据，还需要一个数据，每隔一段时间校验一次，用于匹配执行顺序等情况
            DistanceStorage.AddDistanceData(this);
        }

        protected virtual void Start()
        {
            if (Application.isPlaying)
            {
                //目前只支持send端初始交互
                if (distanceData.interactionType == InteractionType.Send && !IsGrab)
                    StartCoroutine(AutoInteraction(0.01f));
            }
        }

        /// <summary>
        /// 自动交互
        /// </summary>
        /// <returns></returns>
        public IEnumerator AutoInteraction(float delay)
        {
            //关闭一些动作，只是单纯的数据初始化
            if (FeaturesObjectController == null) yield break;

            if (InteractiveController.Instance == null) yield break;

            if (distanceData.interactionType != InteractionType.Send) yield break;

            yield return new WaitForSeconds(delay);
            //初始交互
            InteractiveController.Instance.Search.OnStartInteraction(FeaturesObjectController.gameObject,false,0,true);
            yield return new WaitForSeconds(0.15f);
            InteractiveController.Instance.Search.OnStopInteraction(FeaturesObjectController.gameObject,true);
        }

        protected virtual void OnDisable()
        {
            //distanceData.IsEnabel = false;
            DistanceStorage.DeleteDistanceData(this);
        }

        public virtual bool IsCanInteraction(DistanceInteraction distanceInteraction)
        {
            return true;
        }

        /// <summary>
        /// 移入
        /// </summary>
        public virtual void OnDistanceEnter(DistanceInteraction distanceInteraction)
        {
            if (OnEnter != null)
                OnEnter.Invoke(distanceInteraction);
        }

        /// <summary>
        /// 离开
        /// </summary>
        public virtual void OnDistanceExit(DistanceInteraction distanceInteraction)
        {
            if (OnExit != null)
            {
                OnExit.Invoke(distanceInteraction);
            }

            if (ActiveParent && interactionParent != null)
            {
                interactionParent.OnClose(this,distanceInteraction);
            }

            if (ActiveShadow && interactionShadow != null)
            {
                interactionShadow.OnClose(this,distanceInteraction);
            }
        }

        /// <summary>
        /// 停留
        /// </summary>
        public virtual void OnDistanceStay(DistanceInteraction distanceInteraction)
        {
            if (ActiveShadow && interactionShadow != null&&HasDetected)
            {
                interactionShadow.OnOpen(this,distanceInteraction);
            }

            if (OnStay != null)
            {
                OnStay.Invoke(distanceInteraction);
            }
        }

        /// <summary>
        /// 释放
        /// </summary>
        public virtual void OnDistanceRelesae(DistanceInteraction distanceInteraction)
        {
            if (ActiveParent && interactionParent != null)
            {
                interactionParent.OnOpen(this,distanceInteraction);
            }
            if (ActiveShadow && interactionShadow != null)
            {
                interactionShadow.OnClose(this,distanceInteraction);
            }

            if (OnRelease != null)
            {
                OnRelease.Invoke(distanceInteraction);
            }
        }

        /// <summary>
        /// 带状态释放
        /// </summary>
        /// <param name="distanceInteraction"></param>
        /// <param name="status"></param>
        public virtual void OnDistanceRelease(DistanceInteraction distanceInteraction,InteractionReleaseStatus status)
        {
            //Debug.Log("OnDistanceRelease IsGrab");
            IsGrab = false;

            if (OnStatusRelease != null)
            {
                OnStatusRelease.Invoke(distanceInteraction,status);
            }
        }

        /// <summary>
        /// 没有物体与它进行交互时，会进行释放
        /// </summary>
        public virtual void OnDistanceNotInteractionRelease()
        {

            if (OnNotRelease != null)
            {
                OnNotRelease.Invoke();
            }
        }

        #region 编辑器使用

        /// <summary>
        /// 添加虚影
        /// </summary>
        /// <returns></returns>
        public InteractionShadow AddShadow()
        {
            if (interactionShadow == null)
            {
                interactionShadow = new InteractionShadow();
                interactionShadow.Init(transform);
            }

            return interactionShadow;
        }

        /// <summary>
        /// 移除虚影
        /// </summary>
        public void RemoveShadow()
        {
            interactionShadow?.OnDestory();
            interactionShadow = null;
        }

        /// <summary>
        /// 添加子父物体
        /// </summary>
        /// <returns></returns>
        public InteractionParent AddParent()
        {
            if (interactionParent == null)
                interactionParent = new InteractionParent();

            return interactionParent;
        }
        /// <summary>
        /// 移除父子关系
        /// </summary>
        public void RemoveParent()
        {
            interactionParent = null;
        }

        private void OnDrawGizmos()
        {
#if UNITY_EDITOR
            if (UnityEditor.Selection.activeObject == gameObject)
            {
                switch (distanceData.distanceShape)
                {
                    case DistanceShape.Sphere:

                        Gizmos.color = Color.yellow;
                        Gizmos.DrawSphere(transform.position,distanceData.distanceValue);

                        break;
                    case DistanceShape.Cube:

                        Gizmos.color = Color.yellow;
                        Gizmos.DrawCube(transform.position,distanceData.Size);

                        break;
                }


            }
#endif

        }

        #endregion

        #region 距离互动处理

        public DistanceInteraction OnlyDistance { get; set; }
        public List<DistanceInteraction> Distanced { get; set; }
        public List<DistanceInteraction> Distancing { get; set; }

        /// <summary>
        /// 交互移入处理
        /// </summary>
        /// <param name="interaction">Interaction.</param>
        public void OnInteractionEnter(DistanceInteraction interaction)
        {
            switch (distanceData.interactionType)
            {
                case InteractionType.Receive:
                case InteractionType.All:
                case InteractionType.Pour:
                    AddReceiveDistancing(interaction);
                    OnDistanceEnter(interaction);

                    break;
                case InteractionType.Send:
                    OnDistanceEnter(interaction);
                    break;
            }
        }

        /// <summary>
        /// 交互离开处理
        /// </summary>
        /// <param name="interaction">Interaction.</param>
        public void OnInteractionExit(DistanceInteraction interaction)
        {
            switch (distanceData.interactionType)
            {
                case InteractionType.Receive:
                case InteractionType.All:
                case InteractionType.Pour:

                    if (distanceData.IsOnly)
                    {
                        OnlyDistance = null;
                    }
                    else
                    {
                        RemoveReceiveDistancing(interaction);
                        RemoveReceiveDistanced(interaction);
                    }

                    OnDistanceExit(interaction);

                    break;
                case InteractionType.Send:

                    OnlyDistance = null;

                    OnDistanceExit(interaction);

                    break;
            }
        }

        /// <summary>
        /// 交互停留处理
        /// </summary>
        /// <param name="interaction">Interaction.</param>
        public void OnInteractionStay(DistanceInteraction interaction)
        {
            OnDistanceStay(interaction);
        }

        /// <summary>
        /// 交互松手处理
        /// </summary>
        /// <param name="interaction">Target.</param>
        public void OnInteractionRelease(DistanceInteraction interaction,bool isAuto = false)
        {

            switch (distanceData.interactionType)
            {
                case InteractionType.Receive:
                case InteractionType.All:
                case InteractionType.Pour:

                    if (distanceData.IsOnly)
                    {
                        if (OnlyDistance == interaction)
                        {
                            OnDistanceRelesae(interaction);
                            if (isAuto)
                                OnDistanceRelease(interaction,InteractionReleaseStatus.IsAuto);
                            else
                                OnDistanceRelease(interaction,InteractionReleaseStatus.Inside);
                            return;
                        }
                    }
                    else
                    {
                        if (Distanced != null && Distanced.Contains(interaction))
                        {
                            OnDistanceRelesae(interaction);
                            if (isAuto)
                                OnDistanceRelease(interaction,InteractionReleaseStatus.IsAuto);
                            else
                                OnDistanceRelease(interaction,InteractionReleaseStatus.Inside);
                            return;
                        }
                    }

                    if (!OnInteractionCheck()) return;

                    OnDistanceRelesae(interaction);
                    if (isAuto)
                        OnDistanceRelease(interaction,InteractionReleaseStatus.IsAuto);
                    else
                        OnDistanceRelease(interaction,InteractionReleaseStatus.Once);

                    if (distanceData.IsOnly)
                    {
                        OnlyDistance = interaction;

                        return;
                    }
                    else
                    {
                        AddReceiveDistanced(interaction);
                    }

                    break;
                case InteractionType.Send:

                    if (!OnInteractionCheck())
                    {
                        OnDistanceRelesae(interaction);
                        if (isAuto)
                            OnDistanceRelease(interaction,InteractionReleaseStatus.IsAuto);
                        else
                            OnDistanceRelease(interaction,InteractionReleaseStatus.Inside);

                        return;
                    }

                    OnDistanceRelesae(interaction);
                    if (isAuto)
                        OnDistanceRelease(interaction,InteractionReleaseStatus.IsAuto);
                    else
                        OnDistanceRelease(interaction,InteractionReleaseStatus.Once);

                    AddSendDistance(interaction);

                    break;
            }
        }

        /// <summary>
        /// 没有执行交互时释放
        /// </summary>
        public void OnInteractionNotRelease()
        {
            OnDistanceNotInteractionRelease();
            OnDistanceRelease(null,InteractionReleaseStatus.None);
        }

        /// <summary>
        /// 校验是否可以进行交互
        /// </summary>
        /// <returns></returns>
        public bool OnInteractionCheck()
        {
            switch (distanceData.interactionType)
            {
                case InteractionType.Receive:
                case InteractionType.All:
                case InteractionType.Pour:

                    if (distanceData.IsOnly)
                    {
                        if (OnlyDistance != null) return false;
                    }
                    else
                    {
                        if (distanceData.maxCount == 0) return false;
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
        public void AddReceiveDistanced(DistanceInteraction send)
        {
            if (Distanced == null)
                Distanced = new List<DistanceInteraction>();

            //移除正在交互的
            if (Distancing.Contains(send))
                Distancing.Remove(send);

            if (Distanced.Contains(send)) return;

            Distanced.Add(send);

            if (distanceData.maxCount == -1)
                return;

            distanceData.maxCount--;
        }


        /// <summary>
        /// 添加接收端信息
        /// </summary>
        /// <param name="send">Send.</param>
        public void AddReceiveDistancing(DistanceInteraction send)
        {
            if (Distancing == null)
                Distancing = new List<DistanceInteraction>();

            if (Distancing.Contains(send))
                return;

            Distancing.Add(send);
        }

        /// <summary>
        /// 往发送端中添加接收信息，
        /// </summary>
        /// <param name="receive">Receive.</param>
        public void AddSendDistance(DistanceInteraction receive)
        {
            OnlyDistance = receive;
        }

        /// <summary>
        /// 移除接收距离交互信息
        /// </summary>
        /// <param name="send">Send.</param>
        public void RemoveReceiveDistancing(DistanceInteraction send)
        {
            if (Distancing == null) return;
            if (!Distancing.Contains(send)) return;
            Distancing.Remove(send);
        }

        /// <summary>
        /// 移除已经交互的距离信息
        /// </summary>
        /// <param name="send">Send.</param>
        public void RemoveReceiveDistanced(DistanceInteraction send)
        {
            if (Distanced == null) return;
            if (!Distanced.Contains(send)) return;

            Distanced.Remove(send);

            if (distanceData.maxCount != -1)
                distanceData.maxCount++;
        }



        #endregion

        public override bool Equals(object other)
        {
            if (other == null) return false;
            var distanceInteraction = (DistanceInteraction)other;
            if (distanceInteraction == null) return false;

            return distanceData.TagID.Equals(distanceInteraction.distanceData.TagID) && this == distanceInteraction;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

    }
}

