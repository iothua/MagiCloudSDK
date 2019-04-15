using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace MagiCloud.Downloads
{
    /// <summary>
    /// 下载信息
    /// </summary>
    public class DownloadData
    {
        public string url;
        public string path;

        public Action callback; //回调
    }

    public class DownloadResetfulData
    {
        public string url;
        public string savePath;
        public string fileName;

        public Action callback; //回调
    }

    public class DownloadHandler
    {
        public AbstractDownload webDownload;
        public Action callback;
    }

    public class UnityWebDownloader
    {
        public Queue<DownloadHandler> downloaders = new Queue<DownloadHandler>();

        private MonoBehaviour behaviour;

        private DownloadHandler currentHandler;

        public DownloadHandler CurrentHandler {
            get {
                return currentHandler;
            }
        }

        /// <summary>
        /// 下载器是否运行
        /// </summary>
        public bool IsPlaying { get; private set; }

        public bool IsNetError { get; private set; }

        public UnityWebDownloader(MonoBehaviour behaviour)
        {
            this.behaviour = behaviour;
        }

        public void StartDownload()
        {
            if (IsNetError) return;

            if (IsPlaying) return;
            if (downloaders.Count == 0) return;

            var handler = downloaders.Dequeue();
            if (handler == null) return;

            currentHandler = handler;

            IsPlaying = true;

            handler.webDownload.StartDownload(OnDownloadComplete);
        }

        void OnDownloadComplete(bool result)
        {
            if (currentHandler != null && currentHandler.callback != null)
            {
                currentHandler.callback();
            }

            IsPlaying = false;

            IsNetError = !result;

            StartDownload();
            //获取下一个
        }


        public void AddDownload(DownloadData download)
        {
            DownloadHandler handler = new DownloadHandler()
            {
                webDownload = new HttpWebDownload(download.url, download.path, this.behaviour),
                callback = download.callback
            };

            downloaders.Enqueue(handler);

            StartDownload();
        }

        public void AddDownloader(DownloadResetfulData resetfulData)
        {
            DownloadHandler handler = new DownloadHandler()
            {
                webDownload = new HttpWebDownload(resetfulData.url, resetfulData.savePath,resetfulData.fileName, this.behaviour),
                callback = resetfulData.callback
            };

            downloaders.Enqueue(handler);

            IsNetError = false;

            StartDownload();
        }
    }
}
