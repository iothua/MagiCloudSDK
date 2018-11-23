using System;
using System.Collections.Generic;
using System.Linq;

namespace MagiCloud.Core.Events.Handlers
{
    public struct FloatHandler
    {
        public ExecutionPriority Priority { get; private set; }
        public Action<float> Action { get; private set; }

        public FloatHandler(ExecutionPriority priority, Action<float> action)
        {
            this.Priority = priority;
            this.Action = action;
        }
    }
}
