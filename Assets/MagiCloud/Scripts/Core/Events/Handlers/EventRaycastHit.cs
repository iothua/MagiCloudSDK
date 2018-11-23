using System;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

namespace MagiCloud.Core.Events.Handlers
{
    public class EventRaycastHit
    {
        private List<RaycastHitHandler> Values;

        public void AddListener(ExecutionPriority priority, Action<RaycastHit, int> action)
        {
            if (Values == null)
                Values = new List<RaycastHitHandler>();

            if (IsHandler(action)) return;

            Values.Add(new RaycastHitHandler(priority, action));
            Sort();
        }

        public void Sort()
        {
            Values = Values.OrderBy(obj => obj.Priority).ToList();
        }

        public bool IsHandler(Action<RaycastHit, int> action)
        {
            if (Values == null) return false;

            return Values.Any(obj => obj.Action.Equals(action));
        }

        public void RemoveListener(Action<RaycastHit, int> action)
        {
            if (Values == null) return;

            Values = Values.Where(obj => !obj.Action.Equals(action)).ToList();
        }

        public void SendListener(RaycastHit hit, int handIndex)
        {
            if (Values == null) return;
            foreach (var item in Values)
            {
                item.Action(hit, handIndex);
            }
        }
    }
}
