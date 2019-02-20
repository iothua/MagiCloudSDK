using MagiCloud.KGUI;
using UnityEngine;
using UnityEngine.UI;

namespace MagiCloud.Kinect
{
    public class KinectAR : MonoBehaviour
    {
        private KinectManager kinectManager;                //Kinect管理对象

        public RawImage kinectImg;

        public KGUI_Toggle kGUI_Button;

        private bool isTrue;

        public static GameObject initialScene;

        private void Start()
        {
            kGUI_Button = GetComponentInChildren<KGUI_Toggle>();
            kGUI_Button.onClick.AddListener(OnClick);
        }

        private void OnClick(int arg0)
        {
            if (isTrue)
            {
                SetUp();
            }
            else
            {
                ShutDown();
            }
            isTrue = !isTrue;
        }

        public void SetUp()
        {
            if (MUtility.CurrentPlatform == Core.OperatePlatform.Kinect)
            {
                Camera.main.clearFlags = CameraClearFlags.Depth;

                kinectImg = GetComponentInChildren<RawImage>();

                kinectManager = KinectManager.Instance;

                if (kinectManager == null) return;
                if (!kinectManager.IsInitialized()) return;

                if (kinectImg.texture == null)
                {
                    Texture2D kinectPic = kinectManager.GetUsersClrTex();  //从设备获取彩色数据  
                    kinectImg.texture = kinectPic;  //把彩色数据给控件显示  

                }

                if (initialScene != null)
                {
                    initialScene.SetActive(false);
                }
            }
        }

        public void ShutDown()
        {
            if (MUtility.CurrentPlatform == Core.OperatePlatform.Kinect)
            {
                if (Camera.main != null)
                {
                    Camera.main.clearFlags = CameraClearFlags.Skybox;

                }
                if (initialScene != null)
                {
                    initialScene.SetActive(true);
                }

                if (kinectImg.texture != null)
                {
                    kinectImg.texture = null;  //把彩色数据给控件显示  
                }
            }
        }
    }
}