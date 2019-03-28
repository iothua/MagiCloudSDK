using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MagiCloud.Common
{
    /// <summary>
    /// 状态机管理
    /// </summary>
    public class Fsm<T> :FsmBase, IFsm<T> where T : class
    {
        private readonly T _owner;
        private readonly Dictionary<string,State<T>> _states;
        private readonly Dictionary<string,Variable> _datas;

        private State<T> _curState;
        private bool _isDestroyed;
        private float _curStateTime;
        private bool _isRunning = false;


        public Fsm(string name,T owner,params State<T>[] states) : base(name)
        {
            _owner=owner;
            _states=new Dictionary<string,State<T>>();
            _datas=new Dictionary<string,Variable>();
            foreach (var item in states)
            {
                string stateName = item.GetType().FullName;
                if (!_states.ContainsKey(stateName))
                {
                    _states.Add(stateName,item);
                    item.OnInit(this);
                }
            }
            _curStateTime=0;
            _curState=null;
            _isDestroyed=false;
        }

        public override Type OwnerType => _owner.GetType();


        public override bool IsRunning => _isRunning;

        public override int StateCount
        {
            get
            {
                if (_states==null)
                    return 0;
                return _states.Count;
            }
        }

        public override bool IsDestroyed => _isDestroyed;

        public override string CurStateName => _curState?.GetType().FullName;


        public override float CurStateTime => _curStateTime;

        public T Owner => _owner;


        public State<T> CurState => _curState;

        public State<T>[] GetAllStates()
        {
            return _states.Values.ToArray();
        }

        public void GetAllStates(List<State<T>> states)
        {
            if (states==null) return;
            states.Clear();
            foreach (var item in _states)
                states.Add(item.Value);
        }

        public TData GetData<TData>(string name) where TData : Variable
        {
            return (TData)GetData(name);
        }

        public TState GetState<TState>() where TState : State<T>
        {
            State<T> state = null;
            if (_states.TryGetValue(typeof(TState).FullName,out state))
                return state as TState;
            return null;
        }

        public State<T> GetState(Type stateType)
        {
            if (stateType==null)
                throw new ArgumentNullException(nameof(stateType));
            if (!typeof(State<T>).IsAssignableFrom(stateType))
                throw new Exception(stateType.FullName +"已经存在");
            State<T> state = null;
            if (_states.TryGetValue(stateType.FullName,out state))
                return state;
            return null;
        }

        public bool HasData(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("message",nameof(name));
            return _datas.ContainsKey(name);
        }

        public bool HasState<TState>() where TState : State<T>
        {
            return _states.ContainsKey(typeof(TState).FullName);
        }

        public bool HasState(Type stateType)
        {
            if (stateType==null)
                throw new ArgumentNullException(nameof(stateType));
            return _states.ContainsKey(stateType.FullName);
        }

        public void OnEvent(object sender,int eventId)
        {
            if (_curState!=null)
                _curState.OnEvent(this,sender,eventId,null);
        }

        public void OnEvent(object sender,int eventId,object userData)
        {
            if (_curState!=null)
                _curState.OnEvent(this,sender,eventId,userData);
        }

        public bool RemoveData(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("message",nameof(name));
            return _datas.Remove(name);
        }

        public void SetData<TData>(string name,TData data) where TData : Variable
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("message",nameof(name));
            _datas[name]=data;
        }

        public void Start<TState>() where TState : State<T>
        {
            if (IsRunning) return;
            State<T> state = GetState<TState>();
            if (state==null) return;
            _curStateTime=0;
            _curState=state;
            _curState.OnEnter(this);
        }

        public void Start(Type stateType)
        {
            if (IsRunning) return;
            if (stateType==null)
                throw new ArgumentNullException(nameof(stateType));
            if (!typeof(State<T>).IsAssignableFrom(stateType)) return;
            State<T> state = GetState(stateType);
            if (state==null) return;
            _curStateTime=0;
            _curState=state;
            _curState.OnEnter(this);
        }

        internal override void ShutDown()
        {
            if (_curState!=null)
            {
                _curState.OnLeave(this,true);
                _curState=null;
                _curStateTime=0f;
            }
            foreach (var item in _states)
                item.Value.OnDestroy(this);
            _states.Clear();
            _datas.Clear();
            _isDestroyed=true;
        }

        internal void ChangeState<TState>() where TState : State<T>
        {
            ChangeState(typeof(TState));
        }
        internal void ChangeState(Type stateType)
        {
            if (_curState==null) return;
            var state = GetState(stateType);
            if (state==null) return;
            _curState.OnLeave(this,false);
            _curStateTime=0;
            _curState=state;
            _curState.OnEnter(this);
        }

        internal override void Updtate()
        {
            if (_curState==null) return;
            _curStateTime+=Time.deltaTime;
            _curState.OnUpdate(this);
        }

        public Variable GetData(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("message",nameof(name));
            Variable data = null;
            if (_datas.TryGetValue(name,out data))
                return data;
            return null;
        }

        public void SetData(string name,Variable data)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("message",nameof(name));
            _datas[name]=data;
        }
    }
}
