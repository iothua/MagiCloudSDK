using System;
using UnityEngine;

namespace MagiCloud.Core.Events.Handlers
{
    public class RaysHandler
    {
        public ExecutionPriority Priority { get; private set; }
        public Action<Ray, Ray,int> Action { get; private set; }

        public RaysHandler(ExecutionPriority priority, Action<Ray, Ray, int> action)
        {
            Priority = priority;
            this.Action = action;
        }
    }
}
