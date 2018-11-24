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
    public class MInitialize : MonoBehaviour
    {
        public OperatePlatform CurrentPlatform;

        private MBehaviour behaviour;

        private HighlightingRenderer highlighting;

        private void Awake()
        {
            behaviour = new MBehaviour(ExecutionPriority.Highest, -1000, enabled);

            behaviour.OnAwake(() =>
            {
                MUtility.CurrentPlatform = CurrentPlatform;

                SwitchPlatform(Application.platform);
            });

            behaviour.OnDestroy(() =>
            {
                DestoryPlatform(Application.platform);
            });

            DontDestroyOnLoad(gameObject);

            MBehaviourController.AddBehaviour(behaviour);
        }

        void AddHighlighting()
        {
            highlighting = MUtility.MainCamera.gameObject.GetComponent<HighlightingRenderer>() ?? MUtility.MainCamera.gameObject.AddComponent<HighlightingRenderer>();

            if (highlighting != null)
            {
                highlighting.blurIntensity = 1;
                highlighting.blurSpread = 0;
                highlighting.blurMinSpread = 1;
                highlighting.iterations = 2;
                highlighting.downsampleFactor = 1;
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
            if (behaviour != null)
                behaviour.OnExcuteDestroy();
        }
    }
}
