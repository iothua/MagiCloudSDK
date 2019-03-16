using System;
using System.IO;

namespace MagiCloud.Downloads
{
    public abstract class AbstractDownload
    {
        /// <summary>
        /// 网络资源Url路径
        /// </summary>
        protected string uri;

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
        /// 是否开始下载
        /// </summary>
        protected bool isStartDownload;

        /// <summary>
        /// 是否开始下载
        /// </summary>
        public bool IsStartDownload {
            get {
                return isStartDownload;
            }
        }

        public AbstractDownload(string url, string path)
        {
            this.uri = url;
            savePath = path;
            isStartDownload = false;
            fileNameWithoutExt = Path.GetFileNameWithoutExtension(this.uri);
            fileExt = Path.GetExtension(this.uri);

            saveFilePath = string.Format("{0}/{1}{2}", savePath, fileNameWithoutExt, fileExt);
        }

        public virtual void StartDownload(Action callback = null)
        {
            if (string.IsNullOrEmpty(uri) || string.IsNullOrEmpty(savePath))
                return;

            CreateDirectory(saveFilePath);
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

        public abstract void Destroy();

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
