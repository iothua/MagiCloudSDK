using System;
using UnityEngine;
using System.Collections;
using System.Net;
using System.IO;

namespace MagiCloud.Downloads
{
    public class HttpWebDownload : AbstractDownload
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

        public HttpWebDownload(string url, string path,MonoBehaviour behaviour) : base(url, path)
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
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.Method = "GET";

            FileStream fileStream;
            if (File.Exists(tempSaveFilePath))
            {
                //若之前已下载了一部分，继续下载
                fileStream = File.OpenWrite(tempSaveFilePath);
                currentLength = fileStream.Length;
                fileStream.Seek(currentLength, SeekOrigin.Current);

                request.AddRange(currentLength);
            }
            else
            {
                fileStream = new FileStream(tempSaveFilePath, FileMode.Create, FileAccess.Write);
                currentLength = 0;
            }

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream stream = response.GetResponseStream();

            //总文件大小= 当前需要下载的+已下载的
            fileLength = response.ContentLength + currentLength;

            isStartDownload = true;
            int lengthOnce;
            int bufferMaxLength = 1024 * 20;

            while (currentLength < fileLength)
            {
                byte[] buffer = new byte[bufferMaxLength];
                if (stream.CanRead)
                {
                    lengthOnce = stream.Read(buffer, 0, buffer.Length);
                    currentLength += lengthOnce;
                    fileStream.Write(buffer, 0, lengthOnce);
                }
                else
                {
                    break;
                }

                yield return null;
            }

            isStartDownload = false;
            response.Close();
            stream.Close();
            fileStream.Close();

            if (File.Exists(saveFilePath))
                File.Delete(saveFilePath);

            yield return null;

            File.Move(tempSaveFilePath, saveFilePath);

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
