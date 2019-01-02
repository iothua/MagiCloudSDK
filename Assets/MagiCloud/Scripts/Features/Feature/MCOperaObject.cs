using System;
using MagiCloud.Core;
using UnityEngine;

namespace MagiCloud.Features
{
    /// <summary>
    /// 物体操作
    /// </summary>
    public class MCOperaObject : MonoBehaviour,IOperateObject
    {
        [SerializeField, Header("当射线照射该物体时，赋予谁被抓取，不赋值默认为本身")]
        public GameObject grabObject;
        [SerializeField]
        protected bool IsRayLayer = true;
        
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
