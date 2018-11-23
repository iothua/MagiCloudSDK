using UnityEngine;
using MagiCloud.Features;
using MagiCloud.Interactive;
using System;

namespace MagiCloud.Equipments
{
    /// <summary>
    /// 仪器基类
    /// </summary>
    [RequireComponent(typeof(FeaturesObjectController))]
    public class EquipmentBase : MonoBehaviour
    {
        private FeaturesObjectController _featuresObjectController;

        private Transform _modelNode;
        private Transform _effectNode;

        public Action EventDestory;

        /// <summary>
        /// 激活仪器操作
        /// </summary>
        public bool IsEnable {
            get {

                if (FeaturesObject == null) return false;
                return FeaturesObject.IsEnable;
            }
            set {

                if (FeaturesObject == null) return;

                FeaturesObject.IsEnable = value;
            }
        }

        /// <summary>
        /// 模型仪器节点
        /// </summary>
        public Transform ModelNode {

            get {
                if (_modelNode == null)
                {
                    _modelNode = transform.Find("Model");
                }

                return _modelNode;
            }

        }

        /// <summary>
        /// 特效仪器节点
        /// </summary>
        public Transform EffectNode {
            get {
                if (_effectNode == null)
                {
                    _effectNode = transform.Find("Effect");
                }

                return _effectNode;
            }
        }

        /// <summary>
        /// 自身物体
        /// </summary>
        public GameObject SelfGameObject {
            get { return gameObject; }
        }
        
        /// <summary>
        /// 功能控制端
        /// </summary>
        public FeaturesObjectController FeaturesObject {
            get {

                try
                {
                    if (gameObject == null) return null;

                    if (_featuresObjectController == null)
                        _featuresObjectController = gameObject.GetComponent<FeaturesObjectController>();
                }
                catch
                {
                    return null;
                }
                

                return _featuresObjectController;
            }
        }

        /// <summary>
        /// 碰撞体
        /// </summary>
        public Collider Collider {
            get {
                return FeaturesObject.Collider;
            }
        }

        protected virtual void Start()
        {
        }

        protected virtual void Update()
        {
        }

        protected virtual void OnDestroy()
        {
            if (EventDestory != null)
            {
                EventDestory();
            }
        }

        /// <summary>
        /// 初始化仪器
        /// </summary>
        public virtual void OnInitializeEquipment()
        {

        }

        /// <summary>
        /// 初始化仪器（编辑器下调用）
        /// </summary>
        /// <param name="name"></param>
        public virtual void OnInitializeEquipment_Editor(string name)
        {
            gameObject.name = name;
        }


        /// <summary>
        /// 初始化
        /// </summary>
        public virtual void OnInitialize()
        { }

        public virtual void OnDistanceNotRelease()
        {

        }

        public virtual bool IsCanInteraction(InteractionEquipment interaction)
        {
            return true;
        }

        public virtual void OnDistanceEnter(InteractionEquipment interaction)
        {

        }

        public virtual void OnDistanceStay(InteractionEquipment interaction)
        {

        }

        public virtual void OnDistanceExit(InteractionEquipment interaction)
        {

        }

        public virtual void OnDistanceRelease(InteractionEquipment interaction)
        {

        }

    }
}
