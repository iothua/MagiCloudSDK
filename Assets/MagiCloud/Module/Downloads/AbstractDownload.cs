using System;
using System.IO;
using System.Collections;

using Loxodon.Framework.Asynchronous;

namespace MagiCloudPlatform.Downloads
{

    public abstract class AbstractDownload
    {
        /// <summary>
        /// 网络资源Url路径
        /// </summary>
        protected string Uri;

        /// <summary>
        /// 资源下载存放路径，不包含文件名
        /// </summary>
        protected string savePath;

        /// <summary>
        /// 文件名，不包含后缀
        /// </summary>
        protected string fileNameWithoutExt;

        /// <summary>
        /// 文件后缀
        /// </summary>
        protected string fileExt;

        /// <summary>
        /// 下载文件全路径，路径+文件名+后缀
        /// </summary>
        protected string saveFilePath;
        /// <summary>
        /// 原文件大小
        /// </summary>
        protected long fileLength;

        /// <summary>
        /// 当前下载好了的大小
        /// </summary>
        protected long currentLength;

        /// <summary>
        /// 临时文件后缀名
        /// </summary>
        protected string tempFileExt = ".temp";
        /// <summary>
        /// 临时文件路径
        /// </summary>
        protected string tempSaveFilePath;

        /// <summary>
        /// 是否开始下载
        /// </summary>
        protected bool isStartDownload;

        /// <summary>
        /// 是否开始下载
        /// </summary>
        public bool IsStartDownload
        {
            get
            {
                return isStartDownload;
            }
        }

        /// <summary>
        /// 开始下载
        /// </summary>
        /// <returns>The download.</returns>
        /// <param name="url">下载路径，路径已经包含文件信息</param>
        /// <param name="path">保存文件夹路径</param>
        /// <param name="promise">下载完成回调.</param>
        public virtual IEnumerator StartDownload(string url, string savePath, IProgressPromise<float,string> promise)
        {

            this.Uri = url;
            this.savePath = savePath;
            isStartDownload = false;
            fileNameWithoutExt = Path.GetFileNameWithoutExtension(this.Uri);
            fileExt = Path.GetExtension(this.Uri);

            saveFilePath = string.Format("{0}/{1}{2}", savePath, fileNameWithoutExt, fileExt);

            if (string.IsNullOrEmpty(this.Uri) || string.IsNullOrEmpty(savePath))
                yield break;

            CreateDirectory(saveFilePath);
            tempSaveFilePath = string.Format("{0}/{1}{2}", savePath, fileNameWithoutExt, tempFileExt);
        }

        /// <summary>
        /// 开始下载
        /// </summary>
        /// <returns>The download.</returns>
        /// <param name="url">下载链接，不包含下载文件信息</param>
        /// <param name="savePath">保存本地路径</param>
        /// <param name="fileName">文件名称</param>
        /// <param name="promise">下载完成回调</param>
        public virtual IEnumerator StartDownload(string url, string savePath, string fileName, IProgressPromise<float, string> promise)
        {

            this.Uri = url;
            this.savePath = savePath;
            isStartDownload = false;

            fileNameWithoutExt = Path.GetFileNameWithoutExtension(fileName);
            fileExt = Path.GetExtension(fileName);

            saveFilePath = string.Format("{0}/{1}{2}", savePath, fileNameWithoutExt, fileExt);


            if (string.IsNullOrEmpty(this.Uri) || string.IsNullOrEmpty(savePath))
                yield break;

            CreateDirectory(saveFilePath);

            tempSaveFilePath = string.Format("{0}/{1}{2}", savePath, fileNameWithoutExt, tempFileExt);

        }

        /// <summary>
        /// 获取下载进度
        /// </summary>
        /// <returns></returns>
        public abstract float GetProcess();

        /// <summary>
        /// 获取当前下载的文件大小
        /// </summary>
        /// <returns></returns>
        public abstract long GetCurrentLength();

        /// <summary>
        /// 获取到下载的文件大小
        /// </summary>
        /// <returns></returns>
        public abstract long GetLength();

        public virtual void Destroy(Loxodon.Framework.Asynchronous.IAsyncResult result)
        {
            result.Cancel();
        }

        /// <summary>
        /// 创建目录
        /// </summary>
        /// <param name="filePath">需要创建的目录路径</param>
        public static void CreateDirectory(string filePath)
        {
            if (!string.IsNullOrEmpty(filePath))
            {
                string dirName = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(dirName))
                {
                    Directory.CreateDirectory(dirName);
                }
            }
        }

        /// <summary>
        /// 创建文件
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="bytes">文件内容</param>
        public static void CreatFile(string filePath, byte[] bytes)
        {
            FileInfo file = new FileInfo(filePath);
            Stream stream = file.Create();

            stream.Write(bytes, 0, bytes.Length);

            stream.Close();
            stream.Dispose();
        }
    }
}