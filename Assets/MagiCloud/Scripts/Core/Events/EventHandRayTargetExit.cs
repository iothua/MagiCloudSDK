using MagiCloud.Core.Events.Handlers;
using System;
using UnityEngine;


namespace MagiCloud.Core.Events
{
    public static class EventHandRayTargetExit
    {
        private readonly static EventIntGameKey Value = new EventIntGameKey();

        public static void AddListener(GameObject key, Action<int> action, ExecutionPriority priority = ExecutionPriority.Mid)
        {
            Value.AddListener(key, priority, action);
        }

        public static bool IsHandlerKey(GameObject key)
        {
            return Value.IsHandlerKey(key);
        }

        public static bool IsHandler(GameObject key, Action<int> action)
        {
            return Value.IsHandler(key, action);
        }

        public static void RemoveListener(GameObject key)
        {
            Value.RemoveListener(key);
        }

        public static void RemoveListenerAll()
        {
            Value.RemoveAllListener();
        }

        public static void RemoveListener(GameObject key, Action<int> action)
        {
            Value.RemoveListener(key, action);
        }

        public static void SendListener(GameObject key, int handIndex)
        {
            Value.SendListener(key, handIndex);
        }

        /// <summary>
        /// 添加自身抓取事件
        /// </summary>
        /// <param name="key">自身Object</param>
        /// <param name="action">委托</param>
        /// <param name="priority">优先级</param>
        public static void AddRayTargetEnter(this GameObject key, Action<int> action, ExecutionPriority priority = ExecutionPriority.Mid)
        {
            AddListener(key, action, priority);
        }

        /// <summary>
        /// 移除自身抓取委托方法
        /// </summary>
        /// <param name="key">自身Object</param>
        /// <param name="action">委托</param>
        public static void RemoveRayTargetEnter(this GameObject key, Action<int> action)
        {
            RemoveListener(key, action);
        }

        /// <summary>
        /// 移除该抓取物体下的所有注册委托
        /// </summary>
        /// <param name="key"></param>
        public static void RemoveRayTargetEnter(this GameObject key)
        {
            RemoveListener(key);
        }
    }
}
