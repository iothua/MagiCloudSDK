using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Loxodon.Framework.Execution;

namespace Loxodon.Framework.Bundles
{
    public class PriorityTaskExecutor : ITaskExecutor
    {
        private const int DEFAULT_MAX_TASK_COUNT = 6;
        private readonly static WaitForSeconds waitForSeconds = new WaitForSeconds(0.1f);
        private readonly static ComparerImpl<ITask> comparer = new ComparerImpl<ITask>();

        private bool running = false;
        private int maxTaskCount;
        private List<ITask> pendingQueue;
        private List<ITask> runningQueue;       

        public PriorityTaskExecutor() : this(SystemInfo.processorCount * 2)
        {
        }

        public PriorityTaskExecutor(int maxTaskCount)
        {
            this.maxTaskCount = Mathf.Max(maxTaskCount > 0 ? maxTaskCount : SystemInfo.processorCount * 2, DEFAULT_MAX_TASK_COUNT);
            this.pendingQueue = new List<ITask>();
            this.runningQueue = new List<ITask>();
        }

        public virtual void Execute(ITask task)
        {
            this.pendingQueue.Add(task);
            if (!this.running)
                Executors.RunOnCoroutineNoReturn(this.DoTask());
        }

        protected virtual IEnumerator DoTask()
        {
            this.running = true;
            this.pendingQueue.Sort(comparer);
            while (this.pendingQueue.Count > 0)
            {
                ITask task = pendingQueue[0];
                pendingQueue.RemoveAt(0);
                Executors.RunOnCoroutineNoReturn(task.GetRoutin());
                runningQueue.Add(task);

                while (runningQueue.Count >= maxTaskCount || (pendingQueue.Count <= 0 && runningQueue.Count > 0))
                {
                    yield return waitForSeconds;

                    for (int i = runningQueue.Count - 1; i >= 0; i--)
                    {
                        var t = runningQueue[i];
                        if (t.IsDone)
                        {
                            runningQueue.RemoveAt(i);
                            t.Dispose();
                        }
                    }

                    if (pendingQueue.Count > 0 && runningQueue.Count < maxTaskCount)
                        this.pendingQueue.Sort(comparer);
                }
            }
            this.running = false;
        }

        class ComparerImpl<T> : IComparer<T> where T : ITask
        {
            public int Compare(T x, T y)
            {
                if (y.Priority == x.Priority)
                    return x.StartTime.CompareTo(y.StartTime);
                return y.Priority.CompareTo(x.Priority);
            }
        }
    }
}
