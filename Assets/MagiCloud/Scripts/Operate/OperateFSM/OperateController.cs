using MagiCloud.Common;
using MagiCloud.RotateAndZoomTool;
using System;
using System.Collections;
using UnityEngine;
using Utility;

namespace MagiCloud.Operate.OperateFSM
{
    public class OperateController :MonoBehaviour
    {
        [SerializeField, Header("状态脚本名称，需要带上命名空间")]
        private string[] operateStateTypeNames;
        [SerializeField, Header("指定初始状态")]
        private string startOperateStateTypeName;
        FsmSystem fsmSystem;
        OperateSystem operateSystem;
        OperateStateBase startOperateState;
        private void Awake()
        {
            //状态机管理系统
            fsmSystem=new FsmSystem();
            fsmSystem.Init();
            operateSystem=new OperateSystem();
        }

        private IEnumerator Start()
        {
            //实例化状态
            MSwitchManager.OnInitializeMode(OperateModeType.Move | OperateModeType.Rotate | OperateModeType.Zoom);
            var states = new OperateStateBase[operateStateTypeNames.Length];
            for (int i = 0; i < operateStateTypeNames.Length; i++)
            {
                Type type = AssemblyUtility.GetTypeByName(operateStateTypeNames[i]);
                if (type==null) continue;
                states[i]=(OperateStateBase)Activator.CreateInstance(type);
                if (states[i]==null) continue;
                if (startOperateStateTypeName==operateStateTypeNames[i])
                {
                    startOperateState=states[i];
                }
            }
            if (startOperateState==null)
                yield break;
            //启动状态机
            operateSystem.Initialize(fsmSystem,states);
            yield return new WaitForEndOfFrame();
            operateSystem.Start(startOperateState.GetType());
        }


        private void Update()
        {
            if (MSwitchManager.CurrentMode!=OperateModeType.Tool)
            {
                if (CameraRotate.Instance.IsRotateCameraWithCenterEnable||CameraZoom.Instance.IsZoomInitialization)
                    fsmSystem.Update();
                else
                {
                    if (MSwitchManager.CurrentMode!=OperateModeType.Move&&operateSystem.CurOperate!=null)
                    {
                        operateSystem.CurOperate.ChangeState(fsmSystem.GetFsm<OperateSystem>(),typeof(Idle));
                    }
                }
            }


        }
        private void OnDestroy()
        {
            fsmSystem.Shutdown();
        }
    }


}
