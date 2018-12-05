using DrawLine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DrawLine
{
    public class ScreenShotTest : MonoBehaviour
    {
        public GameObject mask;                                             //遮罩
        public RawImage screenShotImage;                                    //存放场景截图的RawImage
                                                                            // Use this for initialization
        void Start()
        {
            StartCoroutine("Test");
        }
        IEnumerator Test()
        {
            ScreenShotTool.StartScreenShot(new Rect(0, 0, Screen.width, Screen.height), 1);     //测试
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            mask.SetActive(true);
            screenShotImage.texture = ScreenShotTool.GetScreenShot;
            StopCoroutine("Test");
        }
    } 
}
