using System;
using System.Collections.Generic;


namespace MagiCloud.Core.Events.Handlers
{
    public struct IntFloatHandler
    {
        public ExecutionPriority Priority { get; private set; }
        public Action<int,float> Action { get; private set; }

        public IntFloatHandler(ExecutionPriority priority, Action<int,float> action)
        {
            this.Priority = priority;
            this.Action = action;
        }
    }
}
