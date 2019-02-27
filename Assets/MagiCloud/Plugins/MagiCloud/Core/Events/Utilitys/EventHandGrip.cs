using MagiCloud.Core.Events.Handlers;
using System;
namespace MagiCloud.Core.Events
{
    /// <summary>
    /// 按下（手势/鼠标）
    /// </summary>
    public static class EventHandGrip
    {
        private readonly static EventInt Values = new EventInt();

        public static void AddListener(Action<int> action, ExecutionPriority priority = ExecutionPriority.Mid)
        {
            Values.AddListener(priority, action);
        }

        public static bool IsHandler(Action<int> action) {
            return Values.IsHandler(action);
        }

        public static void RemoveListener(Action<int> action){
            Values.RemoveListener(action);
        }

        public static void RemoveListenerAll()
        {
            Values.RemoveListenerAll();
        }

        public static void SendListener(int handIndex)
        {
            Values.SendListener(handIndex);
        }
    }
}
