using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace MagiCloud.Core.Events.Handlers
{
    public class EventIntGameKey
    {
        private Dictionary<GameObject, List<IntHandler>> Values;

        public void AddListener(GameObject key, ExecutionPriority priority, Action<int> action)
        {
            if (Values == null)
                Values = new Dictionary<GameObject, List<IntHandler>>();

            if (IsHandler(key, action)) return;

            if (IsHandlerKey(key))
            {
                List<IntHandler> values = Values[key];

                values.Add(new IntHandler(priority, action));

                Values[key] = values.OrderBy(obj => obj.Priority).ToList();
            }
            else
            {
                var values = new List<IntHandler>();
                values.Add(new IntHandler(priority, action));

                Values.Add(key, values);
            }
        }

        public bool IsHandlerKey(GameObject key)
        {
            if (Values == null) return false;

            return Values.ContainsKey(key);
        }

        public bool IsHandler(GameObject key, Action<int> action)
        {
            if (!IsHandlerKey(key)) return false;

            return Values[key].Any(obj => obj.Action.Equals(action));
        }

        public void RemoveListener(GameObject key)
        {
            if (Values == null) return;

            if (!IsHandlerKey(key)) return;

            Values.Remove(key);
        }

        public void RemoveAllListener()
        {
            if (Values == null) return;

            Values.Clear();
        }

        public void RemoveListener(GameObject key, Action<int> action)
        {
            if (Values == null) return;

            if (!IsHandler(key, action)) return;

            var values = Values[key];

            Values[key] = values.Where(obj => !obj.Action.Equals(action)).ToList();
        }

        public void SendListener(GameObject key, int handIndex)
        {
            if (Values == null) return;

            if (!IsHandlerKey(key)) return;

            List<IntHandler> values;

            Values.TryGetValue(key, out values);

            if (values == null) return;
            if (values.Count == 0) return;

            foreach (var item in values)
            {
                item.Action(handIndex);
            }
        }
    }
}
