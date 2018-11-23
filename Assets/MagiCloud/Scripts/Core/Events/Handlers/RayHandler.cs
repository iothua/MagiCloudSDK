using System;
using System.Collections.Generic;
using UnityEngine;

namespace MagiCloud.Core.Events.Handlers
{
    public class RayHandler
    {
        public ExecutionPriority Priority { get; private set; }
        public Action<Ray, int> Action { get; private set; }

        public RayHandler(ExecutionPriority priority, Action<Ray, int> action)
        {
            Priority = priority;
            this.Action = action;
        }
    }
}
