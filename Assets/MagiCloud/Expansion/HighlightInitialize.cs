using HighlightingSystem;
using UnityEngine;

namespace MagiCloud.Core
{
    public class HighlightInitialize :MonoBehaviour
    {
        private HighlightingRenderer highlighting;
        private void Awake()
        {
            SwitchPlatform(Application.platform);
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
        private void OnDestroy()
        {
            DestoryPlatform(Application.platform);
        }
    }
}
