using MagiCloud.Core.Events.Handlers;
using System;
using UnityEngine;

namespace MagiCloud.Core.Events
{
    public static class EventHandRay
    {
        private readonly static EventRay Value = new EventRay();

        public static void AddListener(Action<Ray, int> action, ExecutionPriority priority = ExecutionPriority.Mid)
        {
            Value.AddListener(priority, action);
        }

        public static bool IsHandler(Action<Ray, int> action)
        {
            return Value.IsHandler(action);
        }

        public static void RemoveListener(Action<Ray, int> action)
        {
            Value.RemoveListener(action);
        }

        public static void RemoveListenerAll()
        {
            Value.RemoveListenerAll();
        }

        public static void SendListener(Ray ray, int handIndex)
        {
            Value.SendListener(ray, handIndex);
        }
    }
}
