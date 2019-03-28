using MagiCloud.Common;
using MagiCloud.Core.Events;

namespace MagiCloud.Operate.OperateFSM
{
    /// <summary>
    /// 缩放状态
    /// </summary>
    public class Zoom :OperateStateBase
    {
        internal override void OnEnter(IFsm<OperateSystem> fSM)
        {
            base.OnEnter(fSM);
            MSwitchManager.CurrentMode=OperateModeType.Zoom;
            KGUI.UIShieldController.ShieldDownward(0);
        }
        internal override void OnUpdate(IFsm<OperateSystem> fSM)
        {
            base.OnUpdate(fSM);
            if (!IsTwoTouch)
            {
                if ((LeftIdle||RightIdle))
                    ChangeState(fSM,typeof(Idle));
            }
        }
        internal override void OnLeave(IFsm<OperateSystem> fSM,bool v)
        {
            base.OnLeave(fSM,v);
            EventCameraZoom.SendListener(0);
        }
    }

}
