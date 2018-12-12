using UnityEngine;
using MagiCloud.Features;
using MagiCloud.Interactive;
using System;
using System.Collections.Generic;

#if UNITY_EDITOR

using UnityEditor;
using Sirenix.OdinInspector;

#endif

namespace MagiCloud.Equipments
{


    /// <summary>
    /// 仪器基类
    /// </summary>
    [RequireComponent(typeof(FeaturesObjectController))]
    public class EquipmentBase : MonoBehaviour
    {
        private FeaturesObjectController _featuresObjectController;

        
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
        /// <param name="equipmentName"></param>
        public virtual void OnInitializeEquipment_Editor(string equipmentName)
        {
            gameObject.name = equipmentName;
        }

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

        public virtual void OnDistanceRelease(InteractionEquipment interaction, InteractionReleaseStatus status)
        {

        }

        #region 节点
        private Transform _modelNode;
        private Transform _effectNode;
        //private Transform _liquidNode;


        /// <summary>
        /// 模型仪器节点
        /// </summary>
        public Transform ModelNode {

            get {
                if (_modelNode == null)
                {
                    _modelNode = transform.Find("Model");
                    if (_modelNode == null)
                    {
                        _modelNode = new GameObject("Model").transform;
                        _modelNode.SetParent(transform);
                        _modelNode.localPosition = Vector3.zero;
                        _modelNode.localRotation = Quaternion.identity;
                        _modelNode.localScale = Vector3.one;
                    }
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
                    if (_effectNode == null)
                    {
                        _effectNode = new GameObject("Effect").transform;
                        _effectNode.SetParent(transform);
                    }
                }

                return _effectNode;
            }
        }

        ///// <summary>
        ///// 液体节点
        ///// </summary>
        //public Transform LiquidNode {
        //    get {

        //        if (_liquidNode == null)
        //        {
        //            _liquidNode = transform.Find("Liquid");

        //            if (_liquidNode == null)
        //            {
        //                _liquidNode = new GameObject("Liquid").transform;
        //                _liquidNode.SetParent(transform);
        //            }
        //        }

        //        return _liquidNode;
        //    }
        //}
        /// <summary>
        /// 自身物体
        /// </summary>
        public GameObject SelfGameObject {
            get { return gameObject; }
        }

        #endregion

        #region 注释
        /*
#if UNITY_EDITOR

        /// <summary>
        /// 仪器生成类型
        /// </summary>
        public enum GenerateType
        {
            /// <summary>
            /// 模型
            /// </summary>
            Model,
            /// <summary>
            /// 特效
            /// </summary>
            Effect,
            /// <summary>
            /// 液体
            /// </summary>
            Liquid,
            /// <summary>
            /// 子物体
            /// </summary>
            Child
        }

        [System.Serializable]
        public class GenerateNode
        {
            public string NodeName;
            public GenerateType generateType;
            public Vector3 localPosition = Vector3.zero;
            public Vector3 localRotation = Vector3.zero;
            public Vector3 localScale = Vector3.one;

            [InlineEditor(InlineEditorModes.GUIAndPreview)]
            public GameObject target;



            /// <summary>
            /// 生成
            /// </summary>
            [Button(ButtonStyle.CompactBox, Expanded = true)]
            public void OnGenerate(EquipmentBase equipmentBase)
            {
                GameObject go = null;

                switch (generateType)
                {
                    case GenerateType.Child:

                        if (equipmentBase.transform.Find(NodeName))
                        {
                            Debug.LogError("已经存在相同物体，不可在创建");
                            return;
                        }

                        go = Instantiate(target, equipmentBase.transform);
                        
                        break;
                    case GenerateType.Effect:
                        if (equipmentBase.EffectNode.Find(NodeName))
                        {
                            Debug.LogError("已经存在相同特效，不可在创建");
                            return;
                        }
                        go = Instantiate(target, equipmentBase.EffectNode.transform);
                        break;
                    case GenerateType.Liquid:
                        if (equipmentBase.LiquidNode.Find(NodeName))
                        {
                            Debug.LogError("已经存在相同液体模型，不可在创建");
                            return;
                        }

                        go = Instantiate(target, equipmentBase.LiquidNode.transform);

                        break;
                    case GenerateType.Model:
                        if (equipmentBase.ModelNode.Find(NodeName))
                        {
                            Debug.LogError("已经存在相同模型，不可在创建");
                            return;
                        }

                        go = Instantiate(target, equipmentBase.LiquidNode.transform);

                        break;
                }

                if (go == null) return;

                go.name = NodeName;

                go.transform.localPosition = localPosition;
                go.transform.localRotation = Quaternion.Euler(localRotation);
                go.transform.localScale = localScale;

            }
        }

        /// <summary>
        /// 生成
        /// </summary>
        [Button(ButtonSizes.Large, ButtonStyle.Box, Name = "创建模型")]
        public virtual void OnGenerate(string NodeName, GenerateType generateType,
            Vector3 localPosition, Vector3 localRotation, Vector3 localScale,
            GameObject target)
        {
            GameObject go = null;

            switch (generateType)
            {
                case GenerateType.Child:

                    go = Instantiate(target, transform);

                    break;
                case GenerateType.Effect:
                    
                    go = Instantiate(target, EffectNode);
                    break;
                case GenerateType.Liquid:
                    
                    go = Instantiate(target, LiquidNode);

                    break;
                case GenerateType.Model:
                    
                    go = Instantiate(target, ModelNode);

                    break;
            }

            if (go == null) return;

            go.name = NodeName;

            go.transform.localPosition = localPosition;
            go.transform.localRotation = Quaternion.Euler(localRotation);
            go.transform.localScale = localScale;

        }
#endif
    */

        #endregion
    }
}
