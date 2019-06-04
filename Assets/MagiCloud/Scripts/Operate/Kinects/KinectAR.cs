using MagiCloud.Core.UI;
using UnityEngine;
using UnityEngine.UI;

namespace MagiCloud.Kinect
{
    public class KinectAR :MonoBehaviour
    {
        private KinectManager kinectManager;                //Kinect管理对象

        public RawImage kinectImg;

        public IToggle toggle;

        private bool isTrue;

        public static GameObject initialScene;

        private GameObject[] ARIgnoreObjects;

        private void Start()
        {
            toggle = GetComponentInChildren<IToggle>();
            toggle.Click.AddListener(OnClick);
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

                ARIgnoreObjects = GameObject.FindGameObjectsWithTag("ARIgnore");
                if (ARIgnoreObjects != null)
                {
                    foreach (var item in ARIgnoreObjects)
                    {
                        item.SetActive(false);
                        //Debug.Log("false");
                    }
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

                if (ARIgnoreObjects != null)
                {
                    foreach (var item in ARIgnoreObjects)
                    {
                        item.SetActive(true);
                    }
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

        private void OnDestroy()
        {
            ARIgnoreObjects = null;
        }
    }
}