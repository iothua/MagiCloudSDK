using System;
using UnityEngine;
using UnityEngine.Events;

namespace MagiCloud.UIFrame
{
    /// <summary>
    /// UI基类
    /// </summary>
    [RequireComponent(typeof(UI_Action))]
    public class UI_Base :MonoBehaviour
    {
        /// <summary>
        /// UI类型
        /// </summary>
        public UIType type = UIType.Canvas;

        /// <summary>
        /// 唯一的TagID,用于UIManager管理UI时索引检索
        /// </summary>
        public string TagID;//窗口名称
        public int Priority => 0;

        /// <summary>
        /// 是否可用
        /// </summary>
        private bool available;
        public bool Available => available;

        private bool visible;
        public bool Visible
        {
            get { return available&&visible; }
            set
            {
                if (!available) return;
                if (visible==value) return;
                visible=value;
                SetVisible(value);
            }
        }

        protected internal virtual void SetVisible(bool value)
        {
        }
    
        /// <summary>
        /// 显示当前窗口时的事件处理
        /// </summary>
        public UnityEvent OnShow;

        /// <summary>
        /// 关闭当前窗口时的事件处理
        /// </summary>
        public UnityEvent OnHide;

        /// <summary>
        /// 操作
        /// </summary>
        public UIOperate Operate { get; set; }

        /// <summary>
        /// UI动作
        /// </summary>
        public UI_Action uiAction;

        ///// <summary>
        ///// 坐标
        ///// </summary>
        //public Vector3 Position = Vector3.zero;

        /// <summary>
        /// 当前窗口是否打开
        /// </summary>
        [Header("显示状态，默认为false，运行后自动隐藏")]
        public bool IsShow;

        protected virtual void Awake()
        {
            OnInitialize();
        }

        protected virtual void Start()
        {
        }

        protected virtual void OnEnable()
        { }
        protected virtual void OnDisable()
        { }

        public virtual void OnInitialize()
        {

            if (uiAction == null)
                uiAction = GetComponent<UI_Action>();

            UIManager.Instance.AddUI(this);
        }

        protected virtual void OnDestroy()
        {
            if (UIManager.Instance != null)
                UIManager.Instance.RemoveUI(this);
        }

        protected bool _IsEnable;

        /// <summary>
        /// UI激活状态
        /// </summary>
        protected virtual bool IsEnable
        {
            get
            {
                return _IsEnable;
            }
            set
            {
                _IsEnable = value;
            }
        }


        /// <summary>
        /// 显示当前窗口时的处理
        /// </summary>
        public virtual void OnOpen()
        {
            uiAction.OnShowExcute();

            if (OnShow != null)
                OnShow.Invoke();

            IsShow = true;

            //UIManager.AddAction(uiAction, uiAction.OnShowExcute, end: () =>
            //{

            //    if (OnShow != null)
            //        OnShow.Invoke();

            //    IsShow = true;

            //});
        }


        /// <summary>
        /// 关闭当前窗口时的处理
        /// </summary>
        public virtual void OnClose()
        {
            uiAction.OnHideExcute();

            if (OnHide != null)
                OnHide.Invoke();

            IsShow = false;

            //UIManager.AddAction(uiAction, uiAction.OnHideExcute,end:()=> {

            //    if (OnHide != null)
            //        OnHide.Invoke();

            //    IsShow = false;

            //});

        }
    }
}
