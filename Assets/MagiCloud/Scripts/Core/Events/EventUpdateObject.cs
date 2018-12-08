using MagiCloud.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MagiCloud.Core.Events
{
    /// <summary>
    /// 抓取物体自定义移动事件
    /// </summary>
    public struct UpdateObjectEventHandler
    {
        public ExecutionPriority Level { get; private set; }

        public Action<GameObject, Vector3, Quaternion, int> action { get; private set; }

        public UpdateObjectEventHandler(ExecutionPriority level, Action<GameObject, Vector3, Quaternion, int> action)
        {
            Level = level;
            this.action = action;
        }
    }

    /// <summary>
    /// 抓取物体自定义移动事件
    /// </summary>
    public static class EventUpdateObject
    {
        private static Dictionary<GameObject, List<UpdateObjectEventHandler>> Values;

        public static void AddListener(GameObject key, Action<GameObject, Vector3, Quaternion, int> action, ExecutionPriority level = ExecutionPriority.Mid)
        {
            if (Values == null)
                Values = new Dictionary<GameObject, List<UpdateObjectEventHandler>>();

            if (IsHandler(key, action)) return;

            if (IsHandlerKey(key))
            {
                List<UpdateObjectEventHandler> values = Values[key];
                values.Add(new UpdateObjectEventHandler(level, action));

                Values[key] = values;
            }
            else
            {
                List<UpdateObjectEventHandler> values = new List<UpdateObjectEventHandler>();
                values.Add(new UpdateObjectEventHandler(level, action));

                Values.Add(key, values);
            }
        }

        public static bool IsHandlerKey(GameObject key)
        {
            if (Values == null) return false;

            return Values.ContainsKey(key);
        }

        public static bool IsHandler(GameObject key, Action<GameObject, Vector3, Quaternion, int> action)
        {
            if (!IsHandlerKey(key)) return false;

            foreach (var item in Values[key])
            {
                if (item.action.Equals(action)) return true;
            }
            return false;
        }

        public static void RemoveListener(GameObject key)
        {
            if (Values == null) return;
            if (!IsHandlerKey(key)) return;

            Values.Remove(key);
        }

        public static void RemoveListenerAll()
        {
            if (Values == null) return;
            Values.Clear();
        }

        public static void RemoveListener(GameObject key, Action<GameObject, Vector3, Quaternion, int> action)
        {
            if (Values == null) return;
            if (!IsHandler(key, action)) return;

            List<UpdateObjectEventHandler> values = Values[key];

            foreach (var item in values)
            {
                if (item.action.Equals(action))
                {
                    values.Remove(item);
                    Values[key] = values;
                    return;
                }
            }
        }

        public static void SendListener(GameObject key, Vector3 position, Quaternion rotation, int handIndex)
        {
            if (Values == null || !IsHandlerKey(key) || Values[key].Count == 0)
            {
                key.transform.position = position;
                key.transform.rotation = rotation;

                return;
            }

            foreach (var item in Values[key])
            {
                item.action(key, position, rotation, handIndex);
            }

            for (int i = 0; i < Values[key].Count; i++)
            {
                Values[key][i].action(key, position, rotation, handIndex);
            }
        }

        public static void AddUpdateObject(this GameObject key, Action<GameObject, Vector3, Quaternion, int> action)
        {
            AddListener(key, action);
        }

        public static void RemoveUpdateObject(this GameObject key, Action<GameObject, Vector3, Quaternion, int> action)
        {
            RemoveListener(key, action);
        }
    }
}
