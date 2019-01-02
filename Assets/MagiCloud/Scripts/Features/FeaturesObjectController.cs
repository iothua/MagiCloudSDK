using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MagiCloud.Features
{
    /// <summary>
    /// 功能控制端
    /// </summary>
    [ExecuteInEditMode]
    public class FeaturesObjectController :MonoBehaviour
    {
        public bool ActiveSpaceLimit_ = true;               //激活空间限制
        public bool ActiveHighlight;                        //激活高亮
        public bool ActiveShadow;                           //激活虚影
        public bool ActiveLabel;                            //激活标签
        public bool ActiveLimitMove;                        //激活移动限制
        public GameObject shadowModel;                      //显示虚影的模型
        public GameObject highligheModel;                   //边框高亮模型
        public GameObject highlightGameobject;              //高亮物体

        /// <summary>
        /// 限制移动对象
        /// </summary>
        public MCLimitMove LimitMove;

        /// <summary>
        /// 物体旋转对象
        /// </summary>
        public MCObjectRotation ObjectRatation;
        /// <summary>
        /// 相机绕物体旋转
        /// </summary>
        public MCCameraRotateAround cameraRotateAround;

        public MCObjectButton objectButton;

        private GameObject operaObject; //操作物体

        /// <summary>
        /// 标签控制端
        /// </summary>
        public LabelData LabelController;

        private BoxCollider _collider;
        private bool isEnable = true;

        private GameObject interactionObject;
        public GameObject InteractionObject
        {
            get
            {

                if (interactionObject == null)
                {
                    var obj = transform.Find("interactionObject");

                    if (obj == null)
                    {
                        interactionObject = null;
                    }
                    else
                    {
                        interactionObject = obj.gameObject;
                    }

                    if (interactionObject == null)
                    {
                        interactionObject = new GameObject("interactionObject");
                        interactionObject.transform.SetParent(transform);
                        interactionObject.transform.localPosition = Vector3.zero;
                        interactionObject.hideFlags = HideFlags.HideInHierarchy;
                    }
                }

                return interactionObject.gameObject;
            }
        }

        private void Awake()
        {
            if (ActiveSpaceLimit_)
                AddSpaceLimit();
            else
                RemoveSpaceLimit();

            if (ActiveHighlight)
                AddHighlight();
            else
                RemoveHighlight();

            if (ActiveShadow)
                AddShadow();
            else
                RemoveShadow();

            if (ActiveLabel)
                AddLabel();
            else
                RemoveLabel();

            if (ActiveLimitMove)
                AddLimitMove();
            else
                RemoveLimitMove();

            OnOperaStatus();
        }

        /// <summary>
        /// 物体被抓取
        /// </summary>
        public MCCanGrab CanGrab;

        /// <summary>
        /// 空间限制
        /// </summary>
        public SpaceLimit spaceLimit;

        /// <summary>
        /// 高亮功能
        /// </summary>
        public HighlightObject highlightObject;

        /// <summary>
        ///虚影控制端
        /// </summary>
        public ShadowController ShadowController;

        public MCNone None;

        public MCustomize Customize;//自定义对象

        /// <summary>
        /// 物体操作类型
        /// </summary>
        public ObjectOperaType operaType = ObjectOperaType.无;

        //更改前的值
        public ObjectOperaType beforeChange = ObjectOperaType.无;

        /// <summary>
        /// 碰撞体
        /// </summary>
        public BoxCollider Collider
        {
            get
            {
                if (_collider == null)
                    _collider = OperaObject.GetComponent<BoxCollider>();

                return _collider;
            }
        }

        /// <summary>
        /// 激活仪器操作
        /// </summary>
        public bool IsEnable
        {
            get
            {
                return isEnable;
            }
            set
            {
                if (isEnable == value) return;

                isEnable = value;

                Collider.enabled = isEnable;
            }
        }

        /// <summary>
        /// 操作物体对象
        /// </summary>
        public OperaObject Opera;

        public GameObject OperaObject
        {
            get
            {
                if (operaObject == null)
                {
                    var opera = transform.Find("operaObject");
                    if (opera == null)
                        operaObject = null;
                    else
                        operaObject = opera.gameObject;
                }
                if (operaObject == null)
                {
                    operaObject = new GameObject("operaObject");
                    operaObject.transform.SetParent(transform);
                    operaObject.transform.localPosition = Vector3.zero;

                    Opera = operaObject.AddComponent<OperaObject>();
                    Opera.FeaturesObject = this;
                    Opera.hideFlags = HideFlags.HideInInspector;
                }

                if (Opera == null)
                {
                    Opera = operaObject.GetComponent<OperaObject>() ?? operaObject.AddComponent<OperaObject>();
                    Opera.FeaturesObject = this;
                    Opera.hideFlags = HideFlags.HideInInspector;
                }

                return operaObject;
            }
        }

        /// <summary>
        /// 添加“空间限制”
        /// </summary>
        public SpaceLimit AddSpaceLimit()
        {
            if (spaceLimit == null)
                spaceLimit = OperaObject.GetComponent<SpaceLimit>() ?? OperaObject.AddComponent<SpaceLimit>();
            spaceLimit.hideFlags = HideFlags.HideInInspector;
            return spaceLimit;
        }

        /// <summary>
        /// 移除“空间限制”
        /// </summary>
        public void RemoveSpaceLimit()
        {
            if (spaceLimit == null) return;

            DestroyImmediate(spaceLimit);
        }

        /// <summary>
        /// 添加“高亮”
        /// </summary>
        public HighlightObject AddHighlight()
        {
            highlightObject = OperaObject.GetComponent<HighlightObject>() ?? OperaObject.AddComponent<HighlightObject>();
            highlightObject.hideFlags = HideFlags.HideInInspector;
            return highlightObject;
        }

        /// <summary>
        /// 移除“高亮”
        /// </summary>
        public void RemoveHighlight()
        {
            if (highlightObject == null) return;

            DestroyImmediate(highlightObject);
        }

        /// <summary>
        /// 添加“虚影”
        /// </summary>
        public ShadowController AddShadow()
        {
            if (ShadowController == null)
                ShadowController = OperaObject.GetComponent<ShadowController>() ?? OperaObject.AddComponent<ShadowController>();

            ShadowController.hideFlags = HideFlags.HideInInspector;
            return ShadowController;
        }

        /// <summary>
        /// 移除“虚影”
        /// </summary>
        public void RemoveShadow()
        {
            if (ShadowController == null)
                return;

            DestroyImmediate(ShadowController);
        }

        /// <summary>
        /// 添加“标签”
        /// </summary>
        public LabelData AddLabel()
        {

            LabelController = OperaObject.GetComponent<LabelData>() ?? OperaObject.AddComponent<LabelData>();
            LabelController.hideFlags = HideFlags.HideInInspector;
            LabelController.appertaining = gameObject;
            MagiCloud.KGUI.KGUI_LabelController.Instance.GetLabel(LabelController);
            return LabelController;
        }

        /// <summary>
        /// 移除“标签”
        /// </summary>
        public void RemoveLabel()
        {
            if (LabelController == null) return;
            KGUI.KGUI_LabelController.Instance.DestroyByLabelController(LabelController);

            DestroyImmediate(LabelController);
        }

        /// <summary>
        /// 添加“被抓取”
        /// </summary>
        public MCCanGrab AddCanGrab()
        {
            CanGrab = OperaObject.GetComponent<MCCanGrab>() ?? OperaObject.AddComponent<MCCanGrab>();
            CanGrab.hideFlags = HideFlags.HideInInspector;
            return CanGrab;
        }


        /// <summary>
        /// 移除“被抓取”
        /// </summary>
        public void RemoveCanGrab()
        {
            if (CanGrab == null) return;
            DestroyImmediate(CanGrab);
        }

        public MCNone AddNone()
        {
            None = OperaObject.GetComponent<MCNone>() ?? OperaObject.AddComponent<MCNone>();
            None.hideFlags = HideFlags.HideInInspector;
            return None;
        }

        public void RemoveNone()
        {
            if (None == null)
                return;
            DestroyImmediate(None);
        }

        /// <summary>
        /// 添加“自定义”
        /// </summary>
        /// <returns></returns>
        public MCustomize AddCustomize()
        {
            Customize = OperaObject.GetComponent<MCustomize>() ?? OperaObject.AddComponent<MCustomize>();
            Customize.hideFlags = HideFlags.HideInInspector;

            return Customize;
        }

        public void RemoveCustomize()
        {
            if (Customize == null)
                return;

            DestroyImmediate(Customize);
        }

        /// <summary>
        /// 添加“物体自身旋转”
        /// </summary>
        public MCObjectRotation AddSelfRotation()
        {
            ObjectRatation = OperaObject.GetComponent<MCObjectRotation>() ?? OperaObject.AddComponent<MCObjectRotation>();
            //  ObjectRatation.SetOperaType(ObjectOperaType.物体自身旋转);
            ObjectRatation.hideFlags = HideFlags.HideInInspector;
            return ObjectRatation;
        }

        /// <summary>
        /// 移除“旋转”脚本
        /// </summary>
        public void RemoveRotation()
        {
            if (ObjectRatation == null) return;
            DestroyImmediate(ObjectRatation);
        }

        /// <summary>
        /// 添加“摄像机围绕物体旋转”
        /// </summary>
        public MCCameraRotateAround AddCameraCenterObjectRotation()
        {
            cameraRotateAround = OperaObject.GetComponent<MCCameraRotateAround>() ?? OperaObject.AddComponent<MCCameraRotateAround>();
            // ObjectRatation.SetOperaType(ObjectOperaType.摄像机围绕物体旋转);
            cameraRotateAround.hideFlags = HideFlags.HideInInspector;
            return cameraRotateAround;
        }


        /// <summary>
        /// 移除“摄像机围绕物体旋转”
        /// </summary>
        public void RemoveCameraCenterObjectRotation()
        {
            if (cameraRotateAround == null) return;
            DestroyImmediate(cameraRotateAround);
        }

        /// <summary>
        /// 添加物体式按钮
        /// </summary>
        /// <returns></returns>
        public MCObjectButton AddObjectButton()
        {
            objectButton = OperaObject.GetComponent<MCObjectButton>() ?? OperaObject.AddComponent<MCObjectButton>();
            objectButton.hideFlags = HideFlags.HideInInspector;
            return objectButton;
        }

        /// <summary>
        /// 移除物体式按钮
        /// </summary>
        public void RemoveObjectButton()
        {
            if (objectButton==null) return;
            DestroyImmediate(objectButton);
        }

        /// <summary>
        /// 添加“限制移动”
        /// </summary>
        public MCLimitMove AddLimitMove()
        {
            LimitMove = OperaObject.GetComponent<MCLimitMove>() ?? OperaObject.AddComponent<MCLimitMove>();
            LimitMove.hideFlags = HideFlags.HideInInspector;
            return LimitMove;
        }

        /// <summary>
        /// 移除“限制移动”
        /// </summary>
        public void RemoveLimitMove()
        {
            if (LimitMove == null) return;
            DestroyImmediate(LimitMove);
        }

        /// <summary>
        /// 操作状态
        /// </summary>
        /// <param name="operaType"></param>
        public void OnOperaStatus()
        {
            if (operaType == beforeChange)
                return;

            switch (beforeChange)
            {
                case ObjectOperaType.无:
                    RemoveNone();
                    break;
                case ObjectOperaType.能抓取:
                    RemoveCanGrab();
                    break;
                case ObjectOperaType.物体自身旋转:
                    RemoveRotation();
                    break;
                case ObjectOperaType.摄像机围绕物体旋转:
                    RemoveCameraCenterObjectRotation();
                    break;
                case ObjectOperaType.自定义:
                    RemoveCustomize();
                    break;
                case ObjectOperaType.物体式按钮:
                    RemoveObjectButton();
                    break;
                default:
                    break;
            }

            switch (operaType)
            {
                case ObjectOperaType.无:
                    AddNone();
                    break;
                case ObjectOperaType.能抓取:
                    AddCanGrab();
                    break;
                case ObjectOperaType.物体自身旋转:
                    AddSelfRotation();
                    break;
                case ObjectOperaType.摄像机围绕物体旋转:
                    AddCameraCenterObjectRotation();
                    break;
                case ObjectOperaType.自定义:
                    AddCustomize();
                    break;
                case ObjectOperaType.物体式按钮:
                    AddObjectButton();
                    break;
                default:
                    break;
            }

            beforeChange = operaType;
        }

        /// <summary>
        /// 设置状态
        /// </summary>
        /// <param name="operaType"></param>
        public void OnOperaStatus(ObjectOperaType operaType)
        {
            this.operaType = operaType;
            OnOperaStatus();
        }

        public void SetParent(Transform parent)
        {
            transform.SetParent(parent);
        }

        public void SetParent(Transform parent,Vector3 localPosition,Vector3 localRotation)
        {
            transform.SetParent(parent);
            transform.localPosition = localPosition;
            transform.localRotation = Quaternion.Euler(localRotation);
        }

        /// <summary>
        /// 设置碰撞体大小
        /// </summary>
        /// <param name="center">Center.</param>
        /// <param name="size">Size.</param>
        public void SetCollider(Vector3 center,Vector3 size)
        {
            Collider.center = center;
            Collider.size = size;
        }
    }
}


