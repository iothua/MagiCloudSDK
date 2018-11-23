using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MagiCloud.Core.Events.Handlers
{
    public class EventVector3
    {
        private List<Vector3Handler> Values;

        public void AddListener(ExecutionPriority priority, Action<Vector3> action)
        {
            if (Values == null)
                Values = new List<Vector3Handler>();

            if (IsHandler(action)) return;

            Values.Add(new Vector3Handler(priority, action));

            Sort();
        }

        public void Sort()
        {
            Values = Values.OrderBy(obj => obj.Priority).ToList();
        }

        public bool IsHandler(Action<Vector3> action)
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
        public void RemoveListener(Action<Vector3> action)
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

        public void SendListener(Vector3 lerp)
        {
            if (Values == null) return;
            foreach (var item in Values)
            {
                item.Action(lerp);
            }
        }
    }
}
