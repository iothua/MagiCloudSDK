using UnityEngine;
using System.Collections;
using MagiCloud.Features;

namespace MagiCloud.Features
{

    /// <summary>
    /// 操作物体，通过FeaturesObjectController生成物体时添加
    /// </summary>
    [RequireComponent(typeof(MBoxCollider))]
    public class OperaObject : MonoBehaviour
    {
        [HideInInspector]
        public FeaturesObjectController FeaturesObject;

        private MBoxCollider _boxCollider;

        public MBoxCollider BoxCollider {
            get {
                if (_boxCollider == null)
                    _boxCollider = GetComponent<MBoxCollider>()??gameObject.AddComponent<MBoxCollider>();

                return _boxCollider;
            }
        }

        private void Awake()
        {
            if (FeaturesObject == null)
                FeaturesObject = GetComponentInParent<FeaturesObjectController>();

            this.hideFlags = HideFlags.HideInInspector;
        }
    }
}

