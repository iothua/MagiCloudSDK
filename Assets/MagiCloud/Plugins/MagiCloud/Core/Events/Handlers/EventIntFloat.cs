using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MagiCloud.Core.Events.Handlers
{
    public class EventIntFloat
    {
        private List<IntFloatHandler> Values;

        public void AddListener(ExecutionPriority priority, Action<int,float> action)
        {
            if (Values == null)
                Values = new List<IntFloatHandler>();

            if (IsHandler(action)) return;

            Values.Add(new IntFloatHandler(priority, action));

            Sort();
        }

        public void Sort()
        {
            Values = Values.OrderBy(obj => obj.Priority).ToList();
        }

        public bool IsHandler(Action<int, float> action)
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
        public void RemoveListener(Action<int, float> action)
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

        public void SendListener(int handIndex,float lerpValue)
        {
            if (Values == null) return;

            for (int i = 0; i < Values.Count; i++)
            {
                Values[i].Action(handIndex, lerpValue);
            }

            //foreach (var item in Values)
            //{
            //    item.Action(handIndex, lerpValue);
            //}
        }
    }
}
