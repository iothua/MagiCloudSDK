using System;
using System.Collections.Generic;

namespace MagiCloud.Common
{
    /// <summary>
    /// 状态机事件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="fsm"></param>
    /// <param name="sender"></param>
    /// <param name="userData"></param>
    public delegate void FsmEventHandler<T>(IFsm<T> fsm,object sender,object userData) where T : class;
    /// <summary>
    /// 状态基类
    /// </summary>
    public abstract class State<T> where T : class
    {
        private readonly Dictionary<int,FsmEventHandler<T>> _events;

        protected State()
        {
            _events=new Dictionary<int,FsmEventHandler<T>>();
        }


        internal virtual void OnEvent(Fsm<T> fsm,object sender,int eventId,object userData)
        {
            FsmEventHandler<T> fsmEvent = null;
            if (_events.TryGetValue(eventId,out fsmEvent))
            {
                fsmEvent?.Invoke(fsm,sender,userData);
            }
        }

        internal virtual void OnInit(IFsm<T> fSM)
        {
        }

        internal virtual void OnUpdate(IFsm<T> fSM)
        {
        }

        internal virtual void OnLeave(IFsm<T> fSM,bool v)
        {
        }

        internal virtual void OnDestroy(IFsm<T> fSM)
        {
        }

        internal virtual void OnEnter(IFsm<T> fSM)
        {
        }

        protected void SubscribeEvent(int eventId,FsmEventHandler<T> handler)
        {
            if (handler==null)
                throw new ArgumentNullException(nameof(handler));
            if (!_events.ContainsKey(eventId))
                _events[eventId]=handler;
            else
                _events[eventId]+=handler;
        }

        protected void UnSubscribeEvent(int eventId,FsmEventHandler<T> handler)
        {
            if (handler==null)
                throw new ArgumentNullException(nameof(handler));
            if (_events.ContainsKey(eventId))
                _events[eventId]-=handler;
        }

        public  void ChangeState(IFsm<T> fsm,Type stateType)
        {
            var temp = (Fsm<T>)fsm;
            if (temp==null) return;
            if (stateType==null)
                throw new ArgumentNullException(nameof(stateType));
            if (!typeof(State<T>).IsAssignableFrom(stateType)) return;
            temp.ChangeState(stateType);
        }
    }
}
