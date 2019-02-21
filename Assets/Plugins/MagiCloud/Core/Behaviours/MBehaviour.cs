using System.Collections.Generic;
using System;

namespace MagiCloud.Core
{

    /// <summary>
    /// 事件行为
    /// </summary>
    public struct EventBehaviour
    {
        private List<Action> Actions;

        public void AddListener(Action action)
        {
            if (Actions == null) Actions = new List<Action>();

            if (action == null) return;

            if (Actions.Contains(action)) return;

            Actions.Add(action);
        }

        public void RemoveListener(Action action)
        {
            if (Actions == null) return;

            if (!Actions.Contains(action)) return;

            Actions.Remove(action);
        }

        public void RemoveListenerAll()
        {
            if (Actions == null) return;

            Actions.Clear();
        }

        public bool IsHandler(Action action)
        {
            if (Actions == null) Actions = new List<Action>();

            return Actions.Contains(action);
        }

        public void SendListener()
        {
            if (Actions == null) return;

            for (int i = 0; i < Actions.Count; i++)
            {
                if (Actions[i] != null)
                    Actions[i].Invoke();
            }
        }
    }

    /// <summary>
    /// 框架行为基类
    /// </summary>
    public class MBehaviour
    {
        private bool isEnable;

        private ExecutionPriority priority = ExecutionPriority.Mid;

        /// <summary>
        /// 执行顺序
        /// </summary>
        private float executionOrder;

        internal EventBehaviour onAwake = new EventBehaviour();
        internal EventBehaviour onEnable = new EventBehaviour();
        internal EventBehaviour onStart = new EventBehaviour();
        internal EventBehaviour onDisable = new EventBehaviour();
        internal EventBehaviour onDestroy = new EventBehaviour();
        internal EventBehaviour onUpdate = new EventBehaviour();

        public MBehaviour(ExecutionPriority priority = ExecutionPriority.Mid, float executionOrder = 0, bool isActive = true)
        {
            this.priority = priority;
            this.executionOrder = executionOrder;

            this.IsActive = isActive;
            this.isEnable = isActive;

            MBehaviourController.AddBehaviour(this);
        }

        internal bool IsAwake { get; set; }
        internal bool IsStart { get; set; }
        
        /// <summary>
        /// 初始激活值
        /// </summary>
        internal bool IsActive { get; set; }

        /// <summary>
        /// 是否激活，用于外部处理Enable
        /// </summary>
        public bool IsEnable {
            //get {
            //    return isEnable;
            //}
            //set {

            //    //如果为Null,说明已经被销毁了
            //    if (MBehaviourController.Instance == null) return;

            //    MBehaviourController.Instance.ExcuteDelay(() =>
            //    {
            //        if (!IsActive)
            //        {
            //            isEnable = false;
            //            return;
            //        }

            //        if (isEnable == value) return;

            //        isEnable = value;

            //        if (isEnable)
            //        {
            //            this.OnExcuteAwake();

            //            onEnable.SendListener();

            //            this.OnExcuteStart();
            //        }
            //        else
            //        {
            //            //this.OnExcuteDisable();
            //            onDisable.SendListener();
            //        }
            //    });
            //}
            get {
                return isEnable;
            }
            set {
                isEnable = value;
            }
        }

        /// <summary>
        /// 用于第一次设置isEnable
        /// </summary>
        internal void SetEnable()
        {
            isEnable = true;
        }

        /// <summary>
        /// 执行时间顺序
        /// </summary>
        public float ExecutionOrder {
            get {
                return executionOrder;
            }
        }

        /// <summary>
        /// 初始化执行优先级
        /// </summary>
        public ExecutionPriority ExecutionPriority {
            get { return priority; }
        }

    }
}

