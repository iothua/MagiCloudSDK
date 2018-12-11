using UnityEngine;

namespace Chemistry.Effects
{
    public class EffectBase : MonoBehaviour
    {

        protected virtual void Start()
        {
            OnInitialize();
        }

        public virtual void OnInitialize()
        {

        }

        /// <summary>
        /// 编辑器调用
        /// </summary>
        public virtual void OnInitialize_Editor()
        {
            //从代码中，初始化所有的特效
        }
    }

}