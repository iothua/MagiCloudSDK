using System;

namespace MagiCloud.Common
{
    /// <summary>
    /// 状态机基类
    /// </summary>
    public abstract class FsmBase
    {
        private readonly string _name;

        public FsmBase() : this(null)
        {
        }

        protected FsmBase(string name)
        {
            _name=name??string.Empty;
        }

        public string Name => _name;
        public abstract Type OwnerType { get; }
        public abstract int StateCount { get; }
        public abstract bool IsRunning { get; }
        public abstract bool IsDestroyed { get; }
        public abstract string CurStateName { get; }
        public abstract float CurStateTime { get; }
        internal abstract void Updtate();
        internal abstract void ShutDown();
    }
}
