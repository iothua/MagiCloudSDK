using System;
using UnityEngine;

namespace MagiCloud.Core.Events.Handlers
{
    public struct Vector3Handler
    {
        public ExecutionPriority Priority { get; private set; }
        public Action<Vector3> Action { get; private set; }

        public Vector3Handler(ExecutionPriority priority, Action<Vector3> action)
        {
            this.Priority = priority;
            this.Action = action;
        }
    }
}
