using MagiCloud.Core.Events.Handlers;
using System;

namespace MagiCloud.Core.Events
{
    public class EventHandStop
    {
        private readonly static EventInt Value = new EventInt();

        public static void AddListener(Action<int> action, ExecutionPriority priority = ExecutionPriority.Mid)
        {
            Value.AddListener(priority, action);
        }
        public static bool IsHandler(Action<int> action)
        {
            return Value.IsHandler(action);
        }

        public static void RemoveListener(Action<int> action)
        {
            Value.RemoveListener(action);
        }

        public static void RemoveListenerAll()
        {
            Value.RemoveListenerAll();
        }

        public static void SendListener(int handIndex)
        {
            Value.SendListener(handIndex);
        }
    }
}
