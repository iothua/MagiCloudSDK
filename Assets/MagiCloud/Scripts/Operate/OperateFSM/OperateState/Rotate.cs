using MagiCloud.Common;
using MagiCloud.Core;
using MagiCloud.Core.Events;
using MagiCloud.RotateAndZoomTool;
using UnityEngine;

namespace MagiCloud.Operate.OperateFSM
{
    /// <summary>
    /// 旋转状态
    /// </summary>
    public class Rotate :OperateStateBase
    {

        internal override void OnEnter(IFsm<OperateSystem> fSM)
        {
            base.OnEnter(fSM);
            MSwitchManager.CurrentMode=OperateModeType.Rotate;
            //开启旋转惯性
            CameraRotate.Instance.inertia=true;
            KGUI.UIShieldController.ShieldDownward(0);
        }
        internal override void OnUpdate(IFsm<OperateSystem> fSM)
        {
            base.OnUpdate(fSM);
            //在Kinect模式下，双手握拳激活缩放
            if (Platform==OperatePlatform.Kinect)
            {
                if (LeftGrip&&RightGrip)
                    ChangeState(fSM,typeof(Zoom));
            }
            else
            {
                if (IsTwoTouch)
                    ChangeState(fSM,typeof(Zoom));
            }
          
            if (LeftIdle&&RightIdle)
                ChangeState(fSM,typeof(Idle));
        }
        internal override void OnLeave(IFsm<OperateSystem> fSM,bool v)
        {
            base.OnLeave(fSM,v);
            EventCameraRotate.SendListener(Vector3.zero);
        }
    }

}
