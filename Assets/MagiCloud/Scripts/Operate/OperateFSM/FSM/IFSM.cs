using System;
using System.Collections.Generic;

namespace MagiCloud.Common
{
    /// <summary>
    /// 状态机接口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IFsm<T> where T : class
    {
        string Name { get; }
        T Owner { get; }
        int StateCount { get; }
        bool IsRunning { get; }
        bool IsDestroyed { get; }
        State<T> CurState { get; }
        float CurStateTime { get; }
        void Start<TState>() where TState : State<T>;
        void Start(Type stateType);
        bool HasState<TState>() where TState : State<T>;
        bool HasState(Type stateType);
        TState GetState<TState>() where TState : State<T>;
        State<T> GetState(Type stateType);
        State<T>[] GetAllStates();
        void GetAllStates(List<State<T>> states);
        void OnEvent(object sender,int eventId);
        void OnEvent(object sender,int eventId,object userData);
        bool HasData(string name);
        TData GetData<TData>(string name) where TData : Variable;
        Variable GetData(string name);
        void SetData<TData>(string name,TData data) where TData : Variable;
        void SetData(string name,Variable data);
        bool RemoveData(string name);
    }
}
