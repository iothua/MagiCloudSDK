using HighlightingSystem;
using System;
using UnityEngine;

namespace MagiCloud.Core
{
    /// <summary>
    /// 操作平台
    /// </summary>
    public enum OperatePlatform
    {
        Kinect,
        Mouse
    }

    /// <summary>
    /// 框架初始化
    /// </summary>
    [DefaultExecutionOrder(-1000)]
    public class MInitialize :MonoBehaviour
    {
        public OperatePlatform CurrentPlatform;

        private MBehaviour behaviour;

        private HighlightingRenderer highlighting;

        [Header("Kinect模式兼容鼠标")]
        public bool CompatibleMouse = true;

        [Header("开启日志记录")]
        public bool IsRecordLog; //记录日志

        private void Awake()
        {
            MUtility.CurrentPlatform = CurrentPlatform;

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

            SwitchPlatform(Application.platform);

            DontDestroyOnLoad(gameObject);
        }

        private void AddHighlighting()
        {
            highlighting = MUtility.MainCamera.gameObject.GetComponent<HighlightingRenderer>() ?? MUtility.MainCamera.gameObject.AddComponent<HighlightingRenderer>();

            if (highlighting != null)
            {
                highlighting.blurIntensity = 0.3f;
                highlighting.blurSpread = 0.25f;
                highlighting.blurMinSpread = 0.65f;
                highlighting.iterations = 5;
                highlighting.downsampleFactor =1;
            }
        }

        void RemoveHighlighting()
        {
            if (highlighting != null)
                Destroy(highlighting);
        }

        /// <summary>
        /// 销毁时，平台处理
        /// </summary>
        void DestoryPlatform(RuntimePlatform platform)
        {
            switch (platform)
            {
                case RuntimePlatform.WindowsPlayer:
                case RuntimePlatform.WindowsEditor:
                    {
                        RemoveHighlighting();
                        break;
                    }
                default:
                    break;
            }
        }

        /// <summary>
        /// 选择平台，根据不同的平台切换不同的高亮处理
        /// </summary>
        /// <param name="platform">Platform.</param>
        void SwitchPlatform(RuntimePlatform platform)
        {
            switch (platform)
            {
                case RuntimePlatform.WindowsPlayer:
                case RuntimePlatform.WindowsEditor:
                    {
                        AddHighlighting();
                        break;
                    }
                default:
                    break;
            }
        }

        private void OnDestroy()
        {

            if (IsRecordLog)
                MLog.WriteLogs();

            DestoryPlatform(Application.platform);
        }
    }
}
