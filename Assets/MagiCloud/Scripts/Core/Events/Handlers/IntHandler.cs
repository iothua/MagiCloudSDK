using System;

namespace MagiCloud.Core.Events.Handlers
{
    public struct IntHandler
    {
        public ExecutionPriority Priority { get; private set; }
        public Action<int> Action { get; private set; }

        public IntHandler(ExecutionPriority priority, Action<int> action)
        {
            this.Priority = priority;
            this.Action = action;
        }
    }
}
