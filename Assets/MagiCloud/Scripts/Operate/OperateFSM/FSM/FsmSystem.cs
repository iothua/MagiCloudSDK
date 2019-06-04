using System;
using System.Collections.Generic;

namespace MagiCloud.Common
{
    /// <summary>
    /// 状态机管理系统
    /// </summary>
    public class FsmSystem :IFsmSystem
    {
        private Dictionary<string,FsmBase> _fsms;
        private List<FsmBase> _runningFsm;
        public FsmSystem()
        {

        }

        public int Count { get { return _fsms==null ? 0 : _fsms.Count; } }
        public void Update()
        {
            _runningFsm.Clear();
            if (_fsms.Count<=0) return;
            foreach (var item in _fsms)
                _runningFsm.Add(item.Value);
            foreach (var item in _runningFsm)
            {
                if (item.IsDestroyed)
                    continue;
                item.Updtate();
            }
        }

        public bool HasFsm<T>() where T : class
        {
            var type = typeof(T);
            if (type==null) return false;
            return HasFsm(type.FullName);
        }

        public IFsm<T> CreatFsm<T>(T owner,params State<T>[] states) where T : class
        {
            return CreatFsm(string.Empty,owner,states);
        }

        public bool HasFsm(Type type)
        {
            if (type==null)
                throw new ArgumentNullException(type.FullName);

            return HasFsm(type.FullName);
        }



        public IFsm<T> GetFsm<T>() where T : class
        {
            return (IFsm<T>)GetFsm(typeof(T).FullName);
        }

        public FsmBase GetFsm(Type type)
        {
            if (type==null)
                throw new ArgumentNullException(type.FullName);
            return GetFsm(type.FullName);
        }



        public FsmBase[] GetAllFsms()
        {
            int i = 0;
            var fsms = new FsmBase[_fsms.Count];
            foreach (var item in _fsms)
                fsms[i]=item.Value;
            return fsms;
        }


        public void GetAllFsms(List<FsmBase> fsms)
        {
            if (fsms==null)
                throw new ArgumentNullException("fsms");
            fsms.Clear();
            foreach (var item in _fsms)
                fsms.Add(item.Value);
        }

        public IFsm<T> CreatFsm<T>(string name,T owner,params State<T>[] states) where T : class
        {
            if (HasFsm<T>(name))
                throw new NotImplementedException();
            var fsm = new Fsm<T>(name,owner,states);
            var key = name==string.Empty ? typeof(T).FullName : typeof(T).FullName+"."+name;
            _fsms.Add(key,fsm);
            return fsm;
        }

        public bool DestoryFsm<T>() where T : class
        {
            return DestoryFsm(typeof(T).FullName);
        }
        public bool DestoryFsm<T>(string name) where T : class
        {
            var key = name==string.Empty ? typeof(T).FullName : typeof(T).FullName+"."+name;
            return DestoryFsm(key);
        }
        public bool DestoryFsm(Type type)
        {
            if (type==null)
                throw new ArgumentNullException(type.FullName);
            return DestoryFsm(type.FullName);
        }
        public bool DestoryFsm(Type type,string name)
        {
            if (type==null)
                throw new ArgumentNullException(type.FullName);
            var key = name==string.Empty ? type.FullName : type.FullName+"."+name;
            return DestoryFsm(key);
        }

        private bool HasFsm(string fullName)
        {
            return _fsms.ContainsKey(fullName);
        }

        private FsmBase GetFsm(string fullname)
        {
            FsmBase fsm = null;
            if (_fsms.TryGetValue(fullname,out fsm))
                return fsm;
            return null;
        }

        private bool DestoryFsm(string fullName)
        {
            FsmBase fsm = null;
            if (_fsms.TryGetValue(fullName,out fsm))
            {
                fsm.ShutDown();
                return _fsms.Remove(fullName);
            }
            return false;
        }

        public bool HasFsm<T>(string name) where T : class
        {
            var key = name==string.Empty ? typeof(T).FullName : typeof(T).FullName+"."+name;
            return HasFsm(key);
        }

        public bool HasFsm(Type type,string name)
        {
            var key = name==string.Empty ? type.FullName : type.FullName+"."+name;
            return HasFsm(key);
        }

        public IFsm<T> GetFsm<T>(string name) where T : class
        {
            var key = name==string.Empty ? typeof(T).FullName : typeof(T).FullName+"."+name;
            return (IFsm<T>)GetFsm(key);
        }

        public FsmBase GetFsm(Type type,string name)
        {
            var key = name==string.Empty ? type.FullName : type.FullName+"."+name;
            return GetFsm(key);
        }

        public void Init()
        {
            _fsms=new Dictionary<string,FsmBase>();
            _runningFsm=new List<FsmBase>();
        }


        public void Shutdown()
        {

            foreach (var item in _fsms)
            {
                item.Value.ShutDown();
            }
            _fsms.Clear();
            _runningFsm.Clear();

        }
    }


}
