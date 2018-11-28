using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using MagiCloud.Cameras;
using MagiCloud.Core;
using MagiCloud.Utility.Lighting;

namespace MagiCloud.Equipments
{
    
    public class ExperimentNotification : MonoBehaviour
    {
        public static bool IsExperimentHome;
        public string ExperimentName;

        public LightingData lightingData;
        public LightingData normalLighting;

        public bool IsSetLighting;

        public UnityEvent onAwakeEvent, onEnableEvent, onStartEvent, onDisableEvent, onDestoryEvent;

        private CameraProperty normalCamera;
        public bool isSetCamera;
        public CameraProperty setCamera; //设置相机位置

        private MBehaviour behaviour;

        private void Awake()
        {
            behaviour = new MBehaviour(ExecutionPriority.High, -100, enabled);

            behaviour.OnAwake(() =>
            {
                normalCamera = new CameraProperty(MUtility.MainCamera);

                if (isSetCamera)
                {
                    setCamera.SetCameraProperty(MUtility.MainCamera);
                }

                if (onAwakeEvent != null)
                    onAwakeEvent.Invoke();
                IsExperimentHome = true;
            });

            behaviour.OnEnable(() =>
            {
                if (onEnableEvent != null)
                    onEnableEvent.Invoke();
            });

            behaviour.OnStart(() =>
            {
                if (IsSetLighting && lightingData != null)
                    SystemParameters.SetLighting(lightingData);

                if (onStartEvent != null)
                    onStartEvent.Invoke();
            });

            behaviour.OnDisable(() =>
            {
                if (onDisableEvent != null)
                    onDisableEvent.Invoke();
            });

            behaviour.OnDestroy(() =>
            {
                if (onDestoryEvent != null)
                    onDestoryEvent.Invoke();

                if (normalLighting != null)
                {
                    SystemParameters.SetLighting(normalLighting);
                }
            });
        }

        private void OnEnable()
        {
            behaviour.IsEnable = true;
        }

        private void OnDisable()
        {
            behaviour.IsEnable = false;
        }

        private void OnDestroy()
        {
            behaviour.OnExcuteDestroy();
        }
    }
}

