using System;
using System.Collections.Generic;
using System.Linq;

namespace MagiCloud.Core.Events.Handlers
{
    /// <summary>
    /// 带Int参数的事件
    /// </summary>
    public class EventInt
    {

        private List<IntHandler> Values;

        public void AddListener(ExecutionPriority priority, Action<int> action)
        {
            if (Values == null)
                Values = new List<IntHandler>();

            if (IsHandler(action)) return;

            Values.Add(new IntHandler(priority, action));

            Sort();
        }

        public void Sort()
        {
            Values = Values.OrderBy(obj => obj.Priority).ToList();
        }

        public bool IsHandler(Action<int> action)
        {
            if (Values == null) return false;

            foreach (var item in Values)
            {
                if (item.Action.Equals(action)) return true;
            }
            return false;
        }

        /// <summary>
        /// 移除指定的action方法
        /// </summary>
        /// <param name="key"></param>
        /// <param name="action"></param>
        public void RemoveListener(Action<int> action)
        {
            if (Values == null)
                return;

            Values = Values.Where(obj => !obj.Action.Equals(action)).ToList();
        }

        /// <summary>
        /// 移除全部
        /// </summary>
        public void RemoveListenerAll()
        {
            if (Values == null) return;

            Values.Clear();
        }

        public void SendListener(int handIndex)
        {
            if (Values == null) return;
            foreach (var item in Values)
            {
                item.Action(handIndex);
            }
        }
    }
}
