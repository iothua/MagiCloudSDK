using UnityEngine;
using System.Collections;

namespace MagiCloud.Downloads
{
    /// <summary>
    /// 下载功能
    /// </summary>
    public class MDownloader : MonoBehaviour
    {
        AbstractDownload abstractDownload;
        string testUrl = "http://download.microsoft.com/download/F/5/B/F5B06C7A-2B61-4CC0-91CC-48939EE7C7AF/Azure_Developer_Guide_eBook_zh-CN.pdf";

        private void Start()
        {
            Debug.Log(Application.persistentDataPath);
            abstractDownload = new UnityWebDownload(testUrl, Application.persistentDataPath,this);

            abstractDownload.StartDownload(() =>
            {
                Debug.Log("下载完成");
            });
        }

        private void Update()
        {
            if (Time.frameCount % 20 == 0)
            {
                if (abstractDownload != null && abstractDownload.IsStartDownload)
                {
                    Debug.Log("下载进度-------------" + abstractDownload.GetProcess() + "--------已下载大小----" + abstractDownload.GetCurrentLength());
                }
            }
        }

        private void OnDestroy()
        {
            abstractDownload.Destroy();
        }
    }
}

