using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MagiCloud.Core.Events.Handlers
{
    public class EventRays
    {
        private List<RaysHandler> Values;

        public void AddListener(ExecutionPriority priority, Action<Ray,Ray, int> action)
        {
            if (Values == null)
                Values = new List<RaysHandler>();

            if (IsHandler(action)) return;

            Values.Add(new RaysHandler(priority, action));
        }

        public bool IsHandler(Action<Ray, Ray, int> action)
        {
            if (Values == null) return false;

            return Values.Any(obj => obj.Action.Equals(action));
        }

        public void RemoveListener(Action<Ray, Ray, int> action)
        {
            if (Values == null) return;

            Values = Values.Where(obj => !obj.Action.Equals(action)).ToList();
        }

        public void RemoveListenerAll()
        {
            if (Values == null) return;
            Values.Clear();
        }

        public void SendListener(Ray ray,Ray uiRay, int handIndex)
        {
            if (Values == null) return;

            for (int i = 0; i < Values.Count; i++)
            {
                Values[i].Action(ray, uiRay, handIndex);
            }

            //foreach (var item in Values)
            //{
            //    item.Action(ray, uiRay, handIndex);

            //}
        }
    }
}
