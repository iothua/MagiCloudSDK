using MagiCloud.Core.Events.Handlers;
using System;
using UnityEngine;

namespace MagiCloud.Core.Events
{
    public class EventHandGrabObject
    {
        private readonly static EventGameInt Value = new EventGameInt();

        public static void AddListener(Action<GameObject, int> action, ExecutionPriority priority = ExecutionPriority.Mid)
        {
            Value.AddListener(priority, action);
        }

        public static bool IsHandler(Action<GameObject, int> action)
        {
            return Value.IsHandler(action);
        }

        public static void RemoveListener(Action<GameObject, int> action)
        {
            Value.RemoveListener(action);
        }

        public static void RemoveListenerAll()
        {
            Value.RemoveListenerAll();
        }

        public static void SendListener(GameObject target, int handIndex)
        {
            Value.SendListener(target, handIndex);
        }
    }

}
