using UnityEngine;
using MagiCloud.Core.Events.Handlers;
using System;

namespace MagiCloud.Core.Events
{
    public static class EventHandUIRayExit
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
    }
}
