using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MagiCloud.Core.Events.Handlers
{
    /// <summary>
    /// 无参数事件
    /// </summary>
    public class EventNone
    {
        private Dictionary<object, List<NoneHandler>> Values;

        public void AddListener(object key, ExecutionPriority priority, Action action)
        {
            if (Values == null)
                Values = new Dictionary<object, List<NoneHandler>>();

            if (IsHandler(key, action)) return;

            if (IsHandlerKey(key))
            {
                var values = Values[key];

                values.Add(new NoneHandler(priority, action));

                Values[key] = values.OrderBy(obj => obj.Priority).ToList();
            }
            else
            {
                var values = new List<NoneHandler>
                {
                    new NoneHandler(priority, action)
                };

                Values.Add(key, values);
            }
        }

        public bool IsHandlerKey(object key)
        {
            if (Values == null) return false;

            return Values.ContainsKey(key);
        }

        public bool IsHandler(object key, Action action)
        {
            if (!IsHandlerKey(key)) return false;

            foreach (var item in Values[key])
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
        public void RemoveListener(object key, Action action)
        {
            if (Values == null) return;
            if (!IsHandlerKey(key)) return;

            var values = Values[key];

            foreach (var item in values)
            {
                if (item.Action.Equals(action))
                {
                    values.Remove(item);
                    Values[key] = values;
                    return;
                }
            }
        }

        /// <summary>
        /// 移除指定key
        /// </summary>
        /// <param name="key"></param>
        public void RemoveListener(object key)
        {
            if (Values == null) return;
            if (!IsHandlerKey(key)) return;

            Values.Remove(key);
        }

        /// <summary>
        /// 移除全部
        /// </summary>
        public void RemoveListenerAll()
        {
            if (Values == null) return;

            Values.Clear();
        }

        public void SendListener()
        {
            if (Values == null)
            {
                return;
            }

            foreach (var value in Values)
            {
                foreach (var item in value.Value)
                {
                    item.Action();
                }
            }
        }

        public void SendListener(object key)
        {
            if (Values == null || !IsHandlerKey(key) || Values[key].Count == 0)
            {
                return;
            }

            for (int i = 0; i < Values[key].Count; i++)
            {
                Values[key][i].Action();
            }

            //foreach (var item in Values[key])
            //{
            //    item.Action();
            //}

        }
    }
}

