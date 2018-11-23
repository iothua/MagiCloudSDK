using MagiCloud.Core.Events.Handlers;
using System;
using UnityEngine;

namespace MagiCloud.Core.Events
{
    public static class EventHandRayTarget
    {
        private readonly static EventRaycastHit Value = new EventRaycastHit();

        public static void AddListener(Action<RaycastHit, int> action, ExecutionPriority priority = ExecutionPriority.Mid)
        {
            Value.AddListener(priority, action);
        }

        public static bool IsHandler(Action<RaycastHit, int> action)
        {
            return Value.IsHandler(action);
        }

        public static void RemoveListener(Action<RaycastHit, int> action)
        {
            Value.RemoveListener(action);
        }

        public static void SendListener(RaycastHit hit, int handIndex)
        {
            Value.SendListener(hit, handIndex);
        }
    }
}
