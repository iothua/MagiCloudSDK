using System;
using System.Collections.Generic;
using MagiCloud.Core.Events.Handlers;
using UnityEngine;

namespace MagiCloud.Core.Events
{
    /// <summary>
    /// 相机旋转
    /// </summary>
    public static class EventCameraRotate
    {
        private readonly static EventVector3 Value = new EventVector3();

        public static void AddListener(Action<Vector3> action, ExecutionPriority priority = ExecutionPriority.Mid)
        {
            Value.AddListener(priority, action);
        }

        public static bool IsHandler(Action<Vector3> action)
        {
            return Value.IsHandler(action);
        }

        public static void RemoveListener(Action<Vector3> action)
        {
            Value.RemoveListener(action);
        }

        public static void RemoveListenerAll()
        {
            Value.RemoveListenerAll();
        }

        public static void SendListener(Vector3 lerp)
        {
            Value.SendListener(lerp);
        }

    }
}
