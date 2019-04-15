using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using RenderHeads.Media.AVProMovieCapture;

namespace MagiCloud
{
    /// <summary>
    /// 视频录制控制端
    /// </summary>
    [RequireComponent(typeof(CaptureFromCamera))]
    public class CaptureFromComtroller : MonoBehaviour
    {
        private CaptureFromCamera fromCamera;

        private string[] _audioDeviceNames = new string[0];

        /// <summary>
        /// 是否暂停
        /// </summary>
        public bool IsPaused {
            get {
                return fromCamera.IsPaused();
            }
        }

        /// <summary>
        /// 开始录制
        /// </summary>
        public void StartCapture(string moveName)
        {
            //设置麦克风
            fromCamera._audioDeviceIndex = _audioDeviceNames.Length;
            fromCamera._audioDeviceName = _audioDeviceNames[0];

            //设置名称
            fromCamera._forceFilename = "视频1";

            //设置相机
            Camera[] cameras = FindObjectsOfType<Camera>();
            cameras = cameras.Where(obj => !obj.Equals(MUtility.MainCamera)).ToArray();

            fromCamera.UseContributingCameras = true;
            fromCamera.SetCamera(MUtility.MainCamera, cameras);



            fromCamera.StartCapture();
        }

        /// <summary>
        /// 停止录制
        /// </summary>
        public void StopCapture()
        {
            fromCamera.StopCapture();
        }

        /// <summary>
        /// 暂停录制
        /// </summary>
        public void PausedCapture()
        {
            fromCamera.PauseCapture();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                StartCapture("视频1");
            }
        }

        private void Start()
        {
            fromCamera = GetComponent<CaptureFromCamera>();

            
            fromCamera._outputFolderPath = "";

            int numAudioDevices = NativePlugin.GetNumAVIAudioInputDevices();
            if (numAudioDevices > 0)
            {
                _audioDeviceNames = new string[numAudioDevices];
                for (int i = 0; i < numAudioDevices; i++)
                {
                    _audioDeviceNames[i] = NativePlugin.GetAVIAudioInputDeviceName(i);
                }
            }
        }
    }
}

