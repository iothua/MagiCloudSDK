using MagiCloud.Common;
using MagiCloud.Core;
using MagiCloud.RotateAndZoomTool;
using UnityEngine;

namespace MagiCloud.Operate.OperateFSM
{
    /// <summary>
    /// 操作状态基类
    /// </summary>
    public class OperateStateBase :State<OperateSystem>
    {
        internal override void OnInit(IFsm<OperateSystem> fSM)
        {
            base.OnInit(fSM);
        }
        internal override void OnEnter(IFsm<OperateSystem> fSM)
        {
            base.OnEnter(fSM);
        }
        internal override void OnUpdate(IFsm<OperateSystem> fSM)
        {
            // Debug.Log(MSwitchManager.CurrentMode);
            base.OnUpdate(fSM);
        }
        internal override void OnLeave(IFsm<OperateSystem> fSM,bool v)
        {
            base.OnLeave(fSM,v);
        }
        internal override void OnDestroy(IFsm<OperateSystem> fSM)
        {
            base.OnDestroy(fSM);
        }
        //握拳
        public bool LeftGrip => MOperateManager.GetHandStatus(0)==MInputHandStatus.Grip;
        public bool RightGrip => MOperateManager.GetHandStatus(1)==MInputHandStatus.Grip;

        //松手
        public bool LeftIdle => MOperateManager.GetHandStatus(0)==MInputHandStatus.Idle;
        public bool RightIdle => MOperateManager.GetHandStatus(1)==MInputHandStatus.Idle;

        //抓取物体
        public bool LeftGrab => MOperateManager.GetHandStatus(0)==MInputHandStatus.Grab;
        public bool RightGrab => MOperateManager.GetHandStatus(1)==MInputHandStatus.Grab;

        public bool IsTwoTouch => Input.touchCount>=2;
        //平台
        public OperatePlatform Platform => MOperateManager.GetOperateHand(0).InputHand.Platform;

        public bool ActiveRotate { get { return CameraRotate.Instance.IsRotateCameraWithCenterEnable; } }
        public bool ActiveZoom { get { return CameraZoom.Instance.IsZoomInitialization; } }

        public bool IsUI
        {
            get
            {
                bool left = MOperateManager.GetUIOperate(0).UIObj!=null;
                bool right = MOperateManager.GetUIOperate(1)==null ? false : MOperateManager.GetUIOperate(1).UIObj!=null;
                return left||right;
            }
        }

    }
}
