using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace MagiCloud.Core.Events.Handlers
{
    public class EventGameInt
    {
        private List<GameIntHandler> Values;

        public void AddListener(ExecutionPriority priority, Action<GameObject, int> action)
        {
            if (Values == null)
                Values = new List<GameIntHandler>();

            if (IsHandler(action)) return;

            Values.Add(new GameIntHandler(priority, action));

            Sort();
        }

        public void Sort()
        {
            Values = Values.OrderBy(obj => obj.Priority).ToList();
        }

        public bool IsHandler(Action<GameObject, int> action)
        {
            if (Values == null) return false;

            return Values.Any(obj => obj.Action.Equals(action));
        }

        public void RemoveListener(Action<GameObject, int> action)
        {
            if (Values == null) return;

            Values = Values.Where(obj => !obj.Action.Equals(action)).ToList();
        }

        public void RemoveListenerAll()
        {
            if (Values == null) return;
            Values.Clear();
        }

        public void SendListener(GameObject target, int handIndex)
        {
            if (Values == null) return;

            for (int i = 0; i < Values.Count; i++)
            {
                Values[i].Action(target, handIndex);

            }
        }
    }
}
