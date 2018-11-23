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

        [Header("发射线相机")]
        public Camera markCamera;
        
        [Space(5),Header("主相机")]
        public Camera mainCamera;

        [Space(5), Header("UI相机")]
        public Camera UICamera;

        private MBehaviour behaviour;

        private HighlightingRenderer highlighting;

        private void Awake()
        {
            behaviour = new MBehaviour(ExecutionPriority.Highest, -1000, enabled);

            behaviour.OnAwake(() =>
            {
                if (markCamera == null)
                {
                    var camera = transform.Find("mark Camera");
                    if (camera != null)
                        markCamera = camera.GetComponent<Camera>();
                }

                if (mainCamera == null)
                    mainCamera = Camera.main;

                MUtility.markCamera = markCamera;
                MUtility.mainCamera = mainCamera;
                MUtility.UICamera = UICamera;

                highlighting = mainCamera.gameObject.GetComponent<HighlightingRenderer>() ?? mainCamera.gameObject.AddComponent<HighlightingRenderer>();

                if (highlighting != null)
                {
                    highlighting.blurIntensity = 1;
                    highlighting.blurSpread = 0;
                    highlighting.blurMinSpread = 1;
                    highlighting.iterations = 2;
                    highlighting.downsampleFactor = 1;
                }
            });

            behaviour.OnDestroy(() =>
            {
                Destroy(highlighting);
            });

            DontDestroyOnLoad(gameObject);

            MBehaviourController.AddBehaviour(behaviour);
        }

        private void OnDestroy()
        {
            if (behaviour != null)
                behaviour.OnExcuteDestroy();
        }
    }
}
