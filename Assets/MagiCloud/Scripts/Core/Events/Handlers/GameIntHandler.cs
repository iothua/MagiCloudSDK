using System;
using UnityEngine;

namespace MagiCloud.Core.Events.Handlers
{
    public struct GameIntHandler
    {
        public ExecutionPriority Priority { get; private set; }
        public Action<GameObject, int> Action { get; private set; }

        public GameIntHandler(ExecutionPriority priority, Action<GameObject, int> action)
        {
            Priority = priority;
            this.Action = action;
        }
    }
}
