using System;
using System.Collections;

namespace Loxodon.Framework.Bundles
{
    public interface ITask:IDisposable
    {
        int Priority { get; }

        long StartTime { get; }

        bool IsDone { get; }

        IEnumerator GetRoutin();
    }   
}
