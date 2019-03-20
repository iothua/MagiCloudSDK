using System;
using System.Collections.Generic;

namespace MagiCloud.NetWorks
{
    /// <summary>
    /// 事件池，获取或实例化事件
    /// </summary>
    public class EventPool
    {
        IConnect onnection;
        IMessageDistribution messageDistribution;
        private List<EventBase> events;

        public EventPool(IConnect onnection,IMessageDistribution messageDistribution)
        {
            this.onnection=onnection;
            this.messageDistribution=messageDistribution;
            events=new List<EventBase>();
        }

        /// <summary>
        /// 获取事件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetEvent<T>() where T : EventBase
        {
            Type type = typeof(T);
            foreach (var item in events)
            {
                if (item.GetType()==type)
                    return item as T;
            }
            return CreateEvent(type)as T;
        }

        /// <summary>
        /// 移除事件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public bool RemoveEvent<T>() where T : EventBase
        {
            Type type = typeof(T);
            foreach (var item in events)
            {
                if (item.GetType()==type)
                {
                    item.Remove();
                    events.Remove(item);
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 实例化事件
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private EventBase CreateEvent(Type type)
        {
            EventBase eventBase = (EventBase)Activator.CreateInstance(type);
            if (eventBase==null)
                throw new Exception("实例化类型失败:"+type.FullName);
            events.Add(eventBase);
            eventBase.Init(onnection,messageDistribution);
            return eventBase;
        }
    }


}
