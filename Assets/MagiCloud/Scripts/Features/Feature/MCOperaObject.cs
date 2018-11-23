using System;
using MagiCloud.Core;
using UnityEngine;

namespace MagiCloud.Features
{
    /// <summary>
    /// 物体操作
    /// </summary>
    [RequireComponent(typeof(BoxCollider))]
    public class MCOperaObject : MonoBehaviour,IOperateObject
    {
        [SerializeField, Header("当射线照射该物体时，赋予谁被抓取，不赋值默认为本身")]
        public GameObject grabObject;
        [SerializeField]
        protected bool IsRayLayer = true;

        private bool isEnable = true;
        private BoxCollider _boxCollider;

        public BoxCollider BoxCollider {
            get {
                if (_boxCollider == null)
                    _boxCollider = GetComponent<BoxCollider>();

                return _boxCollider;
            }
        }

        public bool IsEnable {
            get {
                return isEnable;
            }
            set {
                if (isEnable == value) return;

                isEnable = value;

                BoxCollider.enabled = isEnable;
            }
        }

        protected virtual void Start()
        {
            gameObject.layer = IsRayLayer ? MOperateManager.layerRay : MOperateManager.layerObject;
        }

        /// <summary>
        /// 获取正在抓取的物体
        /// </summary>
        public GameObject GrabObject {
            get {
                if (grabObject == null)
                    grabObject = gameObject;
                return grabObject;
            }
            set {
                grabObject = value;
            }
        }

        public MInputHandStatus HandStatus {
            get;set;
        }
    }
}
