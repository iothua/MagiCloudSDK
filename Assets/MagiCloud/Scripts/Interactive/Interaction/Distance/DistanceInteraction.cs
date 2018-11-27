using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using MagiCloud.Interactive.Actions;

namespace MagiCloud.Interactive.Distance
{
    [System.Serializable]
    public class EventDistanceInteraction : UnityEvent<DistanceInteraction> { }

    public class EventDistanceInteractionRelease : UnityEvent<DistanceInteraction, InteractionReleaseStatus> { }

    /// <summary>
    /// 距离交互(挂在物体中)
    /// </summary>
    [ExecuteInEditMode]
    public class DistanceInteraction : MonoBehaviour
    {
        public DistanceData distanceData;

        //靠近、停留、离开、放下、中断(OnBreak)
        public EventDistanceInteraction OnEnter, OnStay, OnExit;

        public EventDistanceInteraction OnRelease; //交互距离后交互释放
        public EventDistanceInteractionRelease OnStatusRelease; //交互后，第二次释放
        public UnityEvent OnNotRelease;//没有交互时的释放

        /// <summary>
        /// 功能对象
        /// </summary>
        public Features.FeaturesObjectController FeaturesObjectController { get; set; }

        /// <summary>
        /// 外部交互对象
        /// </summary>
        //public InteractionBase ExternalInteraction { get; set; }
        public IExternalInteraction ExternalInteraction { get; set; }

        /// <summary>
        /// 初始距离检测，在距离内则进行交互
        /// </summary>
        public bool AutoDetection = true;

        public bool ActiveParent;
        public bool ActiveShadow;
        public InteractionParent interactionParent; //父子关系
        public InteractionShadow interactionShadow; //虚影关系

        /// <summary>
        /// 是否被抓取
        /// </summary>
        public bool IsGrab { get; set; }

        private void Awake()
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

            distanceData.Interaction = this;

            if (FeaturesObjectController == null)
            {
                FeaturesObjectController = gameObject.GetComponent<Features.FeaturesObjectController>();

                if (FeaturesObjectController == null)
                    FeaturesObjectController = gameObject.GetComponentInParent<Features.FeaturesObjectController>();
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
            distanceData.IsEnabel = true;
            //统一调用，去匹配数据，还需要一个数据，每隔一段时间校验一次，用于匹配执行顺序等情况
        }

        private void Start()
        {
            if (Application.isPlaying)
            {
                //目前只支持send端初始交互
                if (distanceData.interactionType == InteractionType.Send)
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
            InteractiveController.Instance.Search.OnStartInteraction(FeaturesObjectController.gameObject, false, true);
            yield return new WaitForSeconds(0.15f);
            InteractiveController.Instance.Search.OnStopInteraction(FeaturesObjectController.gameObject);
        }

        protected virtual void OnDisable()
        {
            distanceData.IsEnabel = false;
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
                interactionParent.OnClose(this, distanceInteraction);
            }

            if (ActiveShadow && interactionShadow != null)
            {
                interactionShadow.OnClose(this, distanceInteraction);
            }
        }

        /// <summary>
        /// 停留
        /// </summary>
        public virtual void OnDistanceStay(DistanceInteraction distanceInteraction)
        {
            if (ActiveShadow && interactionShadow != null)
            {
                interactionShadow.OnOpen(this, distanceInteraction);
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
            if (OnStatusRelease != null)
            {
                OnStatusRelease.Invoke(distanceInteraction, status);
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
                interactionShadow = new InteractionShadow();

            return interactionShadow;
        }

        /// <summary>
        /// 移除虚影
        /// </summary>
        public void RemoveShadow()
        {
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
                        Gizmos.DrawSphere(transform.position, distanceData.distanceValue);

                        break;
                    case DistanceShape.Cube:

                        Gizmos.color = Color.yellow;
                        Gizmos.DrawCube(transform.position, distanceData.Size);

                        break;
                }

                
            }
#endif

        }

        #endregion

    }
}

