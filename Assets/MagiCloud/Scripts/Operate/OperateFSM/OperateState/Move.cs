using MagiCloud.Common;

namespace MagiCloud.Operate.OperateFSM
{
    /// <summary>
    /// 移动状态
    /// </summary>
    public class Move :OperateStateBase
    {
        internal override void OnEnter(IFsm<OperateSystem> fSM)
        {
            base.OnEnter(fSM);

        }
        internal override void OnUpdate(IFsm<OperateSystem> fSM)
        {
            base.OnUpdate(fSM);
            if (LeftIdle&&RightIdle)
                ChangeState(fSM,typeof(Idle));
        }
    }

}
