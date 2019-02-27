using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace MagiCloud.Core.Events.Handlers
{
    public class EventRay
    {
        private List<RayHandler> Values;

        public void AddListener(ExecutionPriority priority, Action<Ray, int> action)
        {
            if (Values == null)
                Values = new List<RayHandler>();

            if (IsHandler(action)) return;

            Values.Add(new RayHandler(priority, action));
        }

        public bool IsHandler(Action<Ray, int> action)
        {
            if (Values == null) return false;

            return Values.Any(obj => obj.Action.Equals(action));
        }

        public void RemoveListener(Action<Ray, int> action)
        {
            if (Values == null) return;

            Values = Values.Where(obj => !obj.Action.Equals(action)).ToList();
        }

        public void RemoveListenerAll()
        {
            if (Values == null) return;
            Values.Clear();
        }

        public void SendListener(Ray ray,int handIndex)
        {
            if (Values == null) return;
            for (int i = 0; i < Values.Count; i++)
            {
                Values[i].Action(ray, handIndex);
            }

            //foreach (var item in Values)
            //{
            //    item.Action(ray, handIndex);

            //}
        }
    }
}
