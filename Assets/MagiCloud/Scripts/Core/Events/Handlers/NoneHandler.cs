using System;

namespace MagiCloud.Core.Events.Handlers
{
    public struct NoneHandler
    {
        public ExecutionPriority Priority { get; private set; }
        public Action Action { get; private set; }

        public NoneHandler(ExecutionPriority priority, Action action)
        {
            this.Priority = priority;
            this.Action = action;
        }
    }
}
