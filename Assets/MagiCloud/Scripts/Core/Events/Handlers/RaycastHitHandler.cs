using System;
using UnityEngine;

namespace MagiCloud.Core.Events.Handlers
{
    public struct RaycastHitHandler
    {
        public ExecutionPriority Priority { get; set; }
        public Action<RaycastHit, int> Action { get; set; }

        public RaycastHitHandler(ExecutionPriority priority, Action<RaycastHit, int> action)
        {
            this.Priority = priority;
            this.Action = action;
        }
    }
}
