using System;
using System.Collections;
using System.IO;
using System.Net;
using Loxodon.Framework.Asynchronous;
using UnityEngine;

namespace MagiCloudPlatform.Downloads {

    public class HttpWebDownload : AbstractDownload
    {

        public override IEnumerator StartDownload(string url, string savePath, IProgressPromise<float, string> promise)
        {
            yield return base.StartDownload(url, savePath, promise);

            yield return Download(promise);

        }

        public override IEnumerator StartDownload(string url, string savePath, string fileName, IProgressPromise<float, string> promise)
        {
            yield return base.StartDownload(url, savePath, fileName, promise);

            yield return Download(promise);
        }

        IEnumerator Download(IProgressPromise<float, string> promise)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Uri);
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

            HttpWebResponse response;
            try
            {
                response = (HttpWebResponse)request.GetResponse();

            }
            catch (Exception e)
            {
                //Debug.LogError("抛出异常处理：" + e.Message);
                promise.SetException("下载异常：" + e.Message);

                yield break;
            }

            Stream stream = response.GetResponseStream();

            //总文件大小= 当前需要下载的+已下载的
            fileLength = response.ContentLength + currentLength;

            isStartDownload = true;
            int lengthOnce;
            int bufferMaxLength = 1024 * 20;

            while (currentLength < fileLength)
            {
                if(promise.IsCancellationRequested)
                {
                    promise.SetCancelled();
                    yield break;
                }

                byte[] buffer = new byte[bufferMaxLength];
                if (stream.CanRead)
                {
                    lengthOnce = stream.Read(buffer, 0, buffer.Length);
                    currentLength += lengthOnce;
                    fileStream.Write(buffer, 0, lengthOnce);
                    
                    //更新进度信息
                    promise.UpdateProgress((float)currentLength / fileLength);

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

            promise.UpdateProgress(1f);

            promise.SetResult(fileExt);

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

