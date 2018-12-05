using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DrawLine
{
    public class ScreenShotTool : MonoBehaviour
    {
        private static ScreenShotTool _ScreenShotTool;

        public Camera kinectImageCamera;                                    //Kinect AR背景相机
        public RawImage kinectImage;                                        //存放AR背景图片的RawImage
        public Camera screenShotCamera;                                     //截屏相机

        private Rect _Rect;
        private LayerMask _LayerMask;
        private string _Path;
        private Texture2D _Texture2D;
        public static Texture2D GetScreenShot { get { return _ScreenShotTool._Texture2D; } }
        private void Awake()
        {
            if (_ScreenShotTool == null)
                _ScreenShotTool = this;
        }

        /// <summary>
        /// 开始截图(带AR背景)，等待两帧后使用ScreenShotTool.GetScreenShot获取结果
        /// </summary>
        /// <param name="rect">截图区域</param>
        /// <param name="path">所截图片保存路劲，无需保存时给null</param>
        public static void StartScreenShotAR(Rect rect, LayerMask layerMask, string path = null)
        {
            _ScreenShotTool._Rect = rect;
            _ScreenShotTool._LayerMask = layerMask;
            _ScreenShotTool._Path = path;
            _ScreenShotTool.StartCoroutine("ScreenShotAR");
        }

        /// <summary>
        /// 开始截图，等待两帧后使用ScreenShotTool.GetScreenShot获取结果
        /// </summary>
        /// <param name="rect">截图区域</param>
        /// <param name="path">所截图片保存路劲，无需保存时给null</param>
        public static void StartScreenShot(Rect rect, LayerMask layerMask, string path = null)
        {
            _ScreenShotTool._Rect = rect;
            _ScreenShotTool._LayerMask = layerMask;
            _ScreenShotTool._Path = path;
            _ScreenShotTool.StartCoroutine("ScreenShot");
        }

        /// <summary>
        /// 带AR背景的截图函数
        /// </summary>
        /// <returns></returns>
        IEnumerator ScreenShotAR()
        {
            RenderTexture RT1 = new RenderTexture((int)_Rect.width, (int)_Rect.height, 0);
            kinectImageCamera.targetTexture = RT1;
            yield return new WaitForEndOfFrame();                               //等待Kinect相机渲染到RT1


            RenderTexture currentRT = RenderTexture.active;                     //RenderTexture.active
            RenderTexture.active = RT1;
            Texture2D screenShot1 = new Texture2D((int)_Rect.width, (int)_Rect.height, TextureFormat.RGB24, false);
            screenShot1.ReadPixels(_Rect, 0, 0);                               //获得AR背景
            screenShot1.Apply();
            kinectImage.gameObject.SetActive(true);
            kinectImage.texture = screenShot1;

            RenderTexture RT2 = new RenderTexture((int)_Rect.width, (int)_Rect.height, 0);
            screenShotCamera.targetTexture = RT2;
            screenShotCamera.cullingMask = _LayerMask;
            screenShotCamera.Render();                                          //主动调用相机渲染
            RenderTexture.active = RT2;
            Texture2D screenShot2 = new Texture2D((int)_Rect.width, (int)_Rect.height, TextureFormat.RGB24, false);
            screenShot2.ReadPixels(_Rect, 0, 0);                               //获得指定相机的渲染结果
            screenShot2.Apply();

            RenderTexture.active = currentRT;                                   //重置相关参数
            kinectImageCamera.targetTexture = null;
            screenShotCamera.targetTexture = null;
            GameObject.Destroy(RT1);
            GameObject.Destroy(RT2);
            kinectImage.gameObject.SetActive(false);
            kinectImage.texture = null;
            if (_Path != null)
            {
                SaveScreenShot(screenShot2);
            }

            _Texture2D = screenShot2;
            StopCoroutine("ScreenShotAR");
        }

        /// <summary>
        /// 截图函数
        /// </summary>
        /// <returns></returns>
        IEnumerator ScreenShot()
        {
            yield return new WaitForEndOfFrame();
            RenderTexture RT2 = new RenderTexture((int)_Rect.width, (int)_Rect.height, 0);
            screenShotCamera.targetTexture = RT2;
            RenderTexture currentRT = RenderTexture.active;
            screenShotCamera.cullingMask = _LayerMask;
            screenShotCamera.Render();                                          //主动调用相机渲染
            RenderTexture.active = RT2;
            Texture2D screenShot2 = new Texture2D((int)_Rect.width, (int)_Rect.height, TextureFormat.RGB24, false);
            screenShot2.ReadPixels(_Rect, 0, 0);                               //获得指定相机的渲染结果
            screenShot2.Apply();

            RenderTexture.active = currentRT;                                   //重置相关参数
            screenShotCamera.targetTexture = null;
            GameObject.Destroy(RT2);
            if (_Path != null)
            {
                SaveScreenShot(screenShot2);
            }

            _Texture2D = screenShot2;
            StopCoroutine("ScreenShot");
        }
        /// <summary>
        /// 保存到磁盘
        /// </summary>
        /// <param name="texture2D"></param>
        private void SaveScreenShot(Texture2D texture2D)
        {
            System.DateTime now = new System.DateTime();
            now = System.DateTime.Now;
            string picName = string.Format("{0}-{1}-{2}-{3}-{4}.png", now.Month, now.Day, now.Hour, now.Minute, now.Second);
            //string _PisPath = Application.persistentDataPath + "/" + picName;
            string _PisPath = Application.dataPath + "/" + picName;
            byte[] byts = texture2D.EncodeToPNG();                          //Texture2D转PNG
            System.IO.File.WriteAllBytes(_PisPath, byts);                   //写入磁盘
        }
    } 
}
