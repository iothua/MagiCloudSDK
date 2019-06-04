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
        protected string tempFileExt = ".temp";
        /// <summary>
        /// 临时文件路径
        /// </summary>
        protected string tempSaveFilePath;

        protected MonoBehaviour behaviour;

        public UnityWebDownload(string url, string path, MonoBehaviour behaviour) : base(url, path)
        {
            tempSaveFilePath = string.Format("{0}/{1}{2}", savePath, fileNameWithoutExt, tempFileExt);
            this.behaviour = behaviour;
        }

        public UnityWebDownload(string url, string savePath, string fileName, MonoBehaviour behaviour)
        {
            this.uri = url;
            this.savePath = savePath;
            isStartDownload = false;

            fileNameWithoutExt = Path.GetFileNameWithoutExtension(fileName);
            fileExt = Path.GetExtension(fileName);

            string last = savePath.Substring(savePath.Length - 1, 1);
            if (last == "/")
            {
                savePath = savePath.Remove(savePath.Length - 1, 1);
            }

            saveFilePath = string.Format("{0}/{1}{2}", savePath, fileNameWithoutExt, fileExt);

            tempSaveFilePath = string.Format("{0}/{1}{2}", savePath, fileNameWithoutExt, tempFileExt);
            this.behaviour = behaviour;
        }

        public override void StartDownload(Action<bool> callback = null)
        {
            base.StartDownload(callback);
            behaviour.StartCoroutine(Download(callback));
        }

        IEnumerator Download(Action<bool> callback = null)
        {
            var webRequest = UnityWebRequest.Head(uri);
            
            isStartDownload = true;
            webRequest.timeout = 30;

            yield return webRequest.SendWebRequest();

            using (FileStream fileStream = new FileStream(tempSaveFilePath, FileMode.OpenOrCreate, FileAccess.Write))
            {
                currentLength = fileStream.Length;
                var totalLength = long.Parse(webRequest.GetResponseHeader("Content-Length"));

                if (currentLength < totalLength)
                {
                    fileStream.Seek(currentLength, SeekOrigin.Begin);

                    var request = UnityWebRequest.Get(uri);

                    if (request.isHttpError || request.isNetworkError)
                    {
                        if (callback != null)
                            callback(false);

                        yield break;
                    }

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
                callback(true);
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
