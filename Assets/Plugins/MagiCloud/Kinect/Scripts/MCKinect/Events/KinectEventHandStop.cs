using System;
using System.Collections.Generic;
using UnityEngine;

namespace MCKinect.Events
{
    public enum EventLevel
    {
        S,
        A,
        B,
        C
    }

    public struct HandStopEventHandler
    {
        public int handIndex;
        public Action action;
        public EventLevel Level;

        public HandStopEventHandler(EventLevel level, int handIndex, Action action)
        {
            this.handIndex = handIndex;
            this.action = action;
            this.Level = level;
        }
    }

    /// <summary>
    /// 手势丢失时处理
    /// </summary>
    public static class KinectEventHandStop
    {
        private static List<HandStopEventHandler> Values;

        public static void AddListener(EventLevel level, int handIndex, Action action)
        {
            if (Values == null)
                Values = new List<HandStopEventHandler>();

            if (IsHandler(action)) return;

            Values.Add(new HandStopEventHandler(level, handIndex, action));
        }

        public static bool IsHandler(Action action)
        {
            if (Values == null) return false;

            foreach (var item in Values)
            {
                if (item.action.Equals(action)) return true;
            }

            return false;
        }

        public static void RemoveListener(Action action)
        {
            if (Values == null)
                return;

            foreach (var item in Values)
            {
                if (item.action.Equals(action))
                {
                    Values.Remove(item);

                    break;
                }
            }
        }

        public static void SendListener(int handIndex)
        {
            foreach (var item in Enum.GetValues(typeof(EventLevel)))
            {
                SendListener((EventLevel)item, handIndex);
            }
        }

        public static void SendListener(EventLevel level, int handIndex)
        {
            if (Values == null) return;
            foreach (var item in Values)
            {
                if (item.Level.Equals(level) && item.handIndex == handIndex)
                {
                    item.action();
                }
            }
        }
    }
}
