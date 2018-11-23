using UnityEngine;
using System.Collections;

namespace MagiCloud.Features
{
    /// <summary>
    /// 操作物体，通过FeaturesObjectController生成物体时添加
    /// </summary>
    [RequireComponent(typeof(BoxCollider))]
    public class OperaObject : MonoBehaviour
    {
        [HideInInspector]
        public FeaturesObjectController FeaturesObject;

        private void Awake()
        {
            if (FeaturesObject == null)
                FeaturesObject = GetComponentInParent<FeaturesObjectController>();

            this.hideFlags = HideFlags.HideInInspector;
        }
    }
}

