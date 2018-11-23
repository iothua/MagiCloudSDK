using System;
using System.Collections.Generic;
using System.Linq;
using MagiCloud.Core.Events.Handlers;

namespace MagiCloud.Core.Events
{
    /// <summary>
    /// 相机缩放
    /// </summary>
    public static class EventCameraZoom
    {
        private readonly static EventFloat Value = new EventFloat();

        public static void AddListener(Action<float> action, ExecutionPriority priority = ExecutionPriority.Mid)
        {
            Value.AddListener(priority, action);
        }

        public static bool IsHandler(Action<float> action)
        {
            return Value.IsHandler(action);
        }

        public static void RemoveListener(Action<float> action)
        {
            Value.RemoveListener(action);
        }

        public static void RemoveListenerAll()
        {
            Value.RemoveListenerAll();
        }

        public static void SendListener(float lerp)
        {
            Value.SendListener(lerp);
        }
    }
}
