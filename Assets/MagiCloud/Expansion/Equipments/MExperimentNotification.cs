using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using MagiCloud.Cameras;
using MagiCloud.Core;
using MagiCloud.Utility.Lighting;

namespace MagiCloud.Equipments
{
    
    public class MExperimentNotification : MonoBehaviour
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

            normalCamera = new CameraProperty(MUtility.MainCamera);

            if (isSetCamera)
            {
                setCamera.SetCameraProperty(MUtility.MainCamera);
            }

            if (onAwakeEvent != null)
                onAwakeEvent.Invoke();
            IsExperimentHome = true;

            //behaviour.OnAwake_MBehaviour(() =>
            //{
            //    normalCamera = new CameraProperty(MUtility.MainCamera);

            //    if (isSetCamera)
            //    {
            //        setCamera.SetCameraProperty(MUtility.MainCamera);
            //    }

            //    if (onAwakeEvent != null)
            //        onAwakeEvent.Invoke();
            //    IsExperimentHome = true;
            //});

            //behaviour.OnEnable_MBehaviour(() =>
            //{
            //    if (onEnableEvent != null)
            //        onEnableEvent.Invoke();
            //});

            //behaviour.OnStart_MBehaviour(() =>
            //{
            //    if (IsSetLighting && lightingData != null)
            //        SystemParameters.SetLighting(lightingData);

            //    if (onStartEvent != null)
            //        onStartEvent.Invoke();
            //});

            //behaviour.OnDisable_MBehaviour(() =>
            //{
            //    if (onDisableEvent != null)
            //        onDisableEvent.Invoke();
            //});

            //behaviour.OnDestroy_MBehaviour(() =>
            //{
            //    if (onDestoryEvent != null)
            //        onDestoryEvent.Invoke();

            //    if (normalLighting != null)
            //    {
            //        SystemParameters.SetLighting(normalLighting);
            //    }
            //});
        }

        private void OnEnable()
        {
            //behaviour.IsEnable = true;
            if (onEnableEvent != null)
                onEnableEvent.Invoke();
        }

        private void Start()
        {
            if (IsSetLighting && lightingData != null)
                SystemParameters.SetLighting(lightingData);

            if (onStartEvent != null)
                onStartEvent.Invoke();
        }

        private void OnDisable()
        {
            //behaviour.IsEnable = false;
            if (onDisableEvent != null)
                onDisableEvent.Invoke();
        }

        private void OnDestroy()
        {
            //behaviour.OnExcuteDestroy();
            if (onDestoryEvent != null)
                onDestoryEvent.Invoke();

            if (normalLighting != null)
            {
                SystemParameters.SetLighting(normalLighting);
            }
        }
    }
}

