using MagiCloud.Common;
using MagiCloud.Core;

namespace MagiCloud.Operate.OperateFSM
{
    /// <summary>
    /// 握拳状态
    /// </summary>
    public class Grip :OperateStateBase
    {
        internal override void OnUpdate(IFsm<OperateSystem> fSM)
        {
            base.OnUpdate(fSM);
            //抓取时有物体进入移动状态，否则进入旋转状态
            if (LeftGrab||RightGrab)
                ChangeState(fSM,typeof(Move));
            //握拳进入旋转状态
            if ((LeftGrip||RightGrip)&&ActiveRotate)
                ChangeState(fSM,typeof(Rotate));
            if (LeftIdle&&RightIdle)
                ChangeState(fSM,typeof(Idle));
        }
    }

}
