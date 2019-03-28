using System;
using System.Collections.Generic;

namespace MagiCloud.Common
{
    public interface IFsmSystem
    {
        int Count { get; }

        bool HasFsm<T>() where T : class;
        bool HasFsm(Type type);
        bool HasFsm<T>(string name) where T : class;
        bool HasFsm(Type type,string name);
        IFsm<T> GetFsm<T>() where T : class;
        FsmBase GetFsm(Type type);
        IFsm<T> GetFsm<T>(string name) where T : class;
        FsmBase GetFsm(Type type,string name);
        FsmBase[] GetAllFsms();
        void GetAllFsms(List<FsmBase> fsms);
        IFsm<T> CreatFsm<T>(T owner,params State<T>[] states) where T : class;
        IFsm<T> CreatFsm<T>(string name,T owner,params State<T>[] states) where T : class;
        bool DestoryFsm<T>() where T : class;
    }
}