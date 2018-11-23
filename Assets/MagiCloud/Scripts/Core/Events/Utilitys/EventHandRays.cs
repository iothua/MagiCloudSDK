using MagiCloud.Core.Events.Handlers;
using System;
using UnityEngine;

namespace MagiCloud.Core.Events
{
    public static class EventHandRays
    {
        private readonly static EventRays Value = new EventRays();

        public static void AddListener(Action<Ray, Ray, int> action, ExecutionPriority priority = ExecutionPriority.Mid)
        {
            Value.AddListener(priority, action);
        }

        public static bool IsHandler(Action<Ray, Ray, int> action)
        {
            return Value.IsHandler(action);
        }

        public static void RemoveListener(Action<Ray, Ray, int> action)
        {
            Value.RemoveListener(action);
        }

        public static void RemoveListenerAll()
        {
            Value.RemoveListenerAll();
        }

        public static void SendListener(Ray ray, Ray uiRay, int handIndex)
        {
            Value.SendListener(ray, uiRay, handIndex);
        }
    }
}
