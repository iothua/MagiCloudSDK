using MagiCloud.Common;
using System;
using System.Collections;
using UnityEngine;
using Utility;

namespace MagiCloud.Operate.OperateFSM
{
    public class OperateController :MonoBehaviour
    {
        [SerializeField]
        private string[] operateStateTypeNames;
        [SerializeField]
        private string startOperateStateTypeName;
        FsmSystem fsmSystem;
        OperateSystem operateSystem;
        OperateStateBase startOperateState;
        private void Awake()
        {
            fsmSystem=new FsmSystem();
            fsmSystem.Init();
            operateSystem=new OperateSystem();
        }

        private IEnumerator Start()
        {

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
            operateSystem.Initialize(fsmSystem,states);
            yield return new WaitForEndOfFrame();
            operateSystem.Start(startOperateState.GetType());
        }


        private void Update()
        {
          
            if (MSwitchManager.CurrentMode!=OperateModeType.Tool)
                fsmSystem.Update();
        }
        private void OnDestroy()
        {
            fsmSystem.Shutdown();
        }
    }
}
