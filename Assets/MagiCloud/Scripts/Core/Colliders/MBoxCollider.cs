using UnityEngine;

namespace MagiCloud
{

    [RequireComponent(typeof(BoxCollider))]
    public class MBoxCollider : MonoBehaviour
    {
        public bool IsEnable {
            get {
                return BoxCollider.enabled;
            }
            set {
                BoxCollider.enabled = value;
            }
        }

        private bool isShake = false;

        /// <summary>
        /// 是否开启防抖
        /// </summary>
        public bool IsShake {
            get {
                return isShake;
            }
            set {
                if (isShake == value) return;

                isShake = value;

                if (isShake)
                {
                    BoxCollider.size += offsetValue;
                }
                else
                {
                    BoxCollider.size -= offsetValue;
                }
            }
        }

        [HideInInspector]
        public BoxCollider BoxCollider;

        protected Vector3 offsetValue;//偏移值

        protected virtual void Awake()
        {
            if (BoxCollider == null)
                BoxCollider = GetComponent<BoxCollider>() ?? gameObject.AddComponent<BoxCollider>();

            hideFlags = HideFlags.HideInInspector; //这个脚本要隐藏

            SetOffsetValue();
        }

        /// <summary>
        /// 设置偏移量
        /// </summary>
        protected virtual void SetOffsetValue()
        {
            switch (gameObject.layer)
            {
                case MOperateManager.layerObject:
                case MOperateManager.layerRay:
                    offsetValue = new Vector3(0.5f, 0.5f, 0.5f);
                    break;
                case MOperateManager.layerUI:
                    offsetValue = new Vector3(30, 30, 0);
                    break;
            }
        }
    }
}
