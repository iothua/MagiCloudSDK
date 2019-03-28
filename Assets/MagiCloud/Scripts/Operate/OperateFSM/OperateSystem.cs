using MagiCloud.Common;
using System;

namespace MagiCloud.Operate.OperateFSM
{
    /// <summary>
    /// 操作系统
    /// </summary>
    public class OperateSystem
    {
        private IFsmSystem fsmSystem;
        private IFsm<OperateSystem> fsm;
        public OperateSystem()
        {
            fsmSystem=null;
            fsm=null;
        }

        /// <summary>
        /// 获取当前操作状态
        /// </summary>
        public OperateStateBase CurOperate
        {
            get
            {
                if (fsm==null)
                    throw new Exception("");
                return (OperateStateBase)fsm.CurState;
            }
        }
        /// <summary>
        /// 获取当前操作状态时长
        /// </summary>
        public float CurOperateTime
        {
            get
            {
                if (fsm==null)
                    throw new Exception("");
                return fsm.CurStateTime;
            }
        }
        /// <summary>
        /// 获取操作状态
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public OperateStateBase GetOperate<T>() where T : OperateStateBase
        {
            return fsm.GetState<T>();
        }
        /// <summary>
        /// 获取操作状态
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public OperateStateBase GetOperate(Type type)
        {
            return (OperateStateBase)fsm.GetState(type);
        }
        /// <summary>
        /// 是否存在该操作状态
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public bool HasOperate<T>() where T : OperateStateBase
        {
            return fsm.HasState<T>();
        }

        public bool HasOperate(Type type)
        {
            return fsm.HasState(type);
        }


        public void Start<T>() where T : OperateStateBase
        {
            if (fsm==null) return;
            fsm.Start<T>();
        }

        public void Start(Type type)
        {
            if (fsm==null) return;
            fsm.Start(type);
        }

        public void Initialize(FsmSystem fsmSystem,OperateStateBase[] states)
        {
            if (fsmSystem==null)
                throw new Exception("FsmSystem为null");
            this.fsmSystem=fsmSystem;
            fsm=this.fsmSystem.CreatFsm(this,states);
        }

        public void Update()
        {

        }
    }
}
