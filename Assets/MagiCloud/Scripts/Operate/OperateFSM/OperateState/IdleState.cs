using MagiCloud.Common;
using MagiCloud.Core;
using MagiCloud.Core.MInput;
using UnityEngine;

namespace MagiCloud.Operate.OperateFSM
{
    /// <summary>
    /// 正常状态
    /// </summary>
    public class Idle :OperateStateBase
    {
        internal override void OnEnter(IFsm<OperateSystem> fSM)
        {
            MSwitchManager.CurrentMode=OperateModeType.Move;
            KGUI.UIShieldController.UnAllShileldAssign();
            base.OnEnter(fSM);

        }
        internal override void OnUpdate(IFsm<OperateSystem> fSM)
        {
            base.OnUpdate(fSM);
            //检查到鼠标左键点击或者手势握拳，进入到握拳状态
            if (IsUI) return;
            if (LeftGrip||RightGrip)
                ChangeState(fSM,typeof(Grip));
            if (Platform==OperatePlatform.Mouse&&ActiveZoom)
            {
                if (Input.GetAxis("Mouse ScrollWheel")!=0)
                    ChangeState(fSM,typeof(Zoom));
            }
        }
    }

}
