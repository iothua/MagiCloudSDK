using System;
using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.IO;

namespace MagiCloud.Downloads
{
    public class UnityWebDownload : AbstractDownload
    {
        /// <summary>
        /// 临时文件后缀名
        /// </summary>
        private string tempFileExt = ".temp";
        /// <summary>
        /// 临时文件路径
        /// </summary>
        private string tempSaveFilePath;

        private MonoBehaviour behaviour;

        public UnityWebDownload(string url, string path, MonoBehaviour behaviour) : base(url, path)
        {
            tempSaveFilePath = string.Format("{0}/{1}{2}", savePath, fileNameWithoutExt, tempFileExt);
            this.behaviour = behaviour;
        }

        public override void StartDownload(Action callback = null)
        {
            base.StartDownload(callback);
            behaviour.StartCoroutine(Download(callback));
        }

        IEnumerator Download(Action callback = null)
        {
            var webRequest = UnityWebRequest.Head(uri);
            
            //webRequest.SetRequestHeader("Range", "bytes = " + currentLength+"-"+);//断点续传设置文件的数据流开始索引
            isStartDownload = true;
            webRequest.timeout = 30;

            yield return webRequest.SendWebRequest();

            //FileStream fileStream;
            //if (File.Exists(tempSaveFilePath))
            //{
            //    //若之前已下载了一部分，继续下载
            //    fileStream = File.OpenWrite(tempSaveFilePath);
            //    currentLength = fileStream.Length;
            //    fileStream.Seek(currentLength, SeekOrigin.Current);
            //}
            //else
            //{
            //    fileStream = new FileStream(tempSaveFilePath, FileMode.Create, FileAccess.Write);
            //    currentLength = 0;
            //}

            using (FileStream fileStream = new FileStream(tempSaveFilePath, FileMode.OpenOrCreate, FileAccess.Write))
            {
                currentLength = fileStream.Length;
                var totalLength = long.Parse(webRequest.GetResponseHeader("Content-Length"));

                if (currentLength < totalLength)
                {
                    fileStream.Seek(currentLength, SeekOrigin.Begin);

                    var request = UnityWebRequest.Get(uri);

                    request.SetRequestHeader("Range", "bytes=" + currentLength + "-" + totalLength);
                    request.SendWebRequest();

                    var index = 0;
                    while (!request.isDone)
                    {
                        yield return null;

                        var buff = request.downloadHandler.data;
                        if (buff != null)
                        {
                            var length = buff.Length - index;
                            fileStream.Write(buff, index, length);
                            index += length;
                            currentLength += length;
                        }
                    }

                }

                fileStream.Close();
                fileStream.Dispose();
            }

            if (File.Exists(saveFilePath))
                File.Delete(saveFilePath);

            yield return null;

            File.Move(tempSaveFilePath, saveFilePath);

            isStartDownload = false;

            if (callback != null)
                callback();
        }

        public override void Destroy()
        {
            behaviour.StopCoroutine(Download());
        }

        public override long GetCurrentLength()
        {
            return currentLength;
        }

        public override long GetLength()
        {
            return fileLength;
        }

        public override float GetProcess()
        {
            if (fileLength > 0)
                return Mathf.Clamp((float)currentLength / fileLength, 0, 1);

            return 0;
        }
    }
}
