using System;
using UnityEngine;
using UnityEngine.Events;

namespace MagiCloud.UISystem
{
    public class UIBase :MonoBehaviour, IUIBase
    {
        private int priority;
        private bool isAvailable;
        private bool isVisible;
        private int parentLayer;
        private int id;
        private int depth;
        private bool pauseCoveredUI;
        private IUIGroup ownerGroup;
        public UnityEvent onEnableUI;
        public UnityEvent onDisableUI;
        public UnityEvent onShowUI;
        public UnityEvent onHideUI;
        #region 属性
        public int ID => id;
        public string Name => name;
        public int Depth => depth;
        public Transform Node { get; private set; }
        public IUIGroup OwnerGroup => ownerGroup;
        public int Priority { get => priority; set => priority=value; }
        /// <summary>
        /// 是否可用
        /// </summary>
        public bool IsAvailable { get => isAvailable; }
        /// <summary>
        /// 是否可见
        /// </summary>
        public bool IsVisible
        {
            get { return isVisible&&isAvailable; }
            set
            {
                if (!isAvailable) return;
                if (isVisible==value) return;
                isVisible=value;
                SetVisible(value);
            }
        }

        public bool Pause { get; set; }
        public bool IsCovered { get; set; }
        /// <summary>
        /// 是否暂停被覆盖的UI
        /// </summary>
        public virtual bool PauseCoveredUI => pauseCoveredUI;
        #endregion


        public virtual void OnInit(int id,IUIGroup group,bool pauseCoveredUI)
        {
            this.id=id;
            ownerGroup=group;
            depth=0;
            this.pauseCoveredUI=pauseCoveredUI;
            parentLayer=gameObject.layer;
            if (Node==null)
                Node=transform;
        }


        private void SetVisible(bool visible)
        {
            gameObject.SetActive(visible);
        }

        /// <summary>
        /// 禁用ui
        /// </summary>
        public virtual void OnDisableUI()
        {
            //  IsVisible=false;
            isAvailable=false;
            onDisableUI?.Invoke();
        }
        /// <summary>
        /// 覆盖
        /// </summary>
        public virtual void OnCovered()
        {

        }

        public virtual void OnDepthChanged(int groupDepth,int depth)
        {
            this.depth=depth;
            //var ren = GetComponentInChildren<Renderer>();
            //if (ren!=null) ren.sortingOrder=depth;
            transform.SetSiblingIndex(0);
        }

        /// <summary>
        /// 启用UI
        /// </summary>
        public virtual void OnEnableUI()
        {
            isAvailable=true;
            onEnableUI?.Invoke();
        }





        /// <summary>
        /// 打开界面
        /// </summary>
        public virtual void OnOpen()
        {
            isAvailable=true;
            IsVisible=true;
            onShowUI?.Invoke();
        }
        /// <summary>
        /// 关闭界面
        /// </summary>
        public virtual void OnClose()
        {
            gameObject.SetLayer(parentLayer);
            IsVisible=false;
            isAvailable=false;
            onHideUI?.Invoke();
        }

        public virtual void OnUpdate()
        {

        }

        /// <summary>
        /// 取消覆盖
        /// </summary>
        public virtual void OnReveal()
        {
        }


    }
}
