using System;
using UnityEngine;

namespace MagiCloud.Core
{

    /// <summary>
    /// 框架初始化
    /// </summary>
    [DefaultExecutionOrder(-1000)]
    public class MInitialize :MonoBehaviour
    {
        public OperatePlatform CurrentPlatform;

        private MBehaviour behaviour;



        [Header("Kinect模式兼容鼠标")]
        public bool CompatibleMouse = true;

        [Header("开启日志记录")]
        public bool IsRecordLog; //记录日志

        [Header("Operate生成器")]
        public OperateCreaterBase operateCreater;

        private void Awake()
        {
            MUtility.CurrentPlatform = CurrentPlatform;
            MOperateManager.operateCreater=operateCreater;
            switch (CurrentPlatform)
            {
                case OperatePlatform.Kinect:
                    var kinectController = Instantiate(Resources.Load<GameObject>("Controller/KinectController"),transform).GetComponent<Operate.KinectController>();
                    kinectController.CompatibleMouse = CompatibleMouse;
                    break;
                case OperatePlatform.Mouse:
                    Instantiate(Resources.Load("Controller/MouseController"),transform);
                    break;
                default:
                    break;
            }



            DontDestroyOnLoad(gameObject);
        }






        private void OnDestroy()
        {

            if (IsRecordLog)
                MLog.WriteLogs();


        }
    }
}
