using System;
using System.Collections.Generic;

namespace MCKinect.Events
{
    public struct HandStartEventHandler
    {
        public int handIndex;
        public Action action;
        public EventLevel Level;

        public HandStartEventHandler(EventLevel level, int handIndex, Action action)
        {
            this.handIndex = handIndex;
            this.action = action;
            this.Level = level;
        }
    }

    /// <summary>
    /// 手势启动时处理
    /// </summary>
    public static class KinectEventHandStart
    {
        private static List<HandStartEventHandler> Values;

        public static void AddListener(EventLevel level, int handIndex, Action action)
        {
            if (Values == null)
                Values = new List<HandStartEventHandler>();

            if (IsHandler(action)) return;

            Values.Add(new HandStartEventHandler(level, handIndex, action));
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
