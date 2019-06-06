using System;
using System.Text;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Loxodon.Framework.Asynchronous;
using Loxodon.Framework.Execution;
using Loxodon.Framework.Bundles;
#if UNITY_2017_1_OR_NEWER
using UnityEngine.Networking;
#endif

namespace Loxodon.Framework.Examples.Bundle
{
    public abstract class AbstractDownloader : IDownloader
    {
        private Uri baseUri;
        private int maxTaskCount;

        public AbstractDownloader() : this(null, SystemInfo.processorCount * 2)
        {
        }

        public AbstractDownloader(Uri baseUri, int maxTaskCount)
        {
            this.BaseUri = baseUri;
            this.MaxTaskCount = maxTaskCount;
        }

        public virtual Uri BaseUri
        {
            get { return this.baseUri; }
            set
            {
                if (value == null || !this.IsAllowedAbsoluteUri(value))
                    throw new NotSupportedException(string.Format("Invalid uri:{0}", value == null ? "null" : value.OriginalString));

                this.baseUri = value;
            }
        }

        public virtual int MaxTaskCount
        {
            get { return this.maxTaskCount; }
            set { this.maxTaskCount = Mathf.Max(value > 0 ? value : SystemInfo.processorCount * 2, 1); }
        }

        protected virtual bool IsAllowedAbsoluteUri(Uri uri)
        {
            if (!uri.IsAbsoluteUri)
                return false;

            if ("http".Equals(uri.Scheme) || "https".Equals(uri.Scheme) || "ftp".Equals(uri.Scheme))
                return true;

            if (RuntimePlatform.Android.Equals(Application.platform) && uri.Scheme.Equals("jar", StringComparison.OrdinalIgnoreCase))
                return true;

            if ("file".Equals(uri.Scheme) && uri.OriginalString.IndexOf("jar:") < 0)
                return true;

            return false;
        }

        protected virtual string GetAbsoluteUri(string relativePath)
        {
            string path = this.BaseUri.AbsoluteUri;
            if (this.BaseUri.Scheme.Equals("jar") && !path.StartsWith("jar:file://"))
                path = path.Replace("jar:file:", "jar:file://");

            if (path.EndsWith("/"))
                return path + relativePath;
            return path + "/" + relativePath;
        }

        protected virtual string GetAbsolutePath(string relativePath)
        {
            string path = this.BaseUri.AbsolutePath;
            if (this.BaseUri.Scheme.Equals("jar"))
                path = path.Replace("file://", "jar:file://");

            if (path.EndsWith("/"))
                return path + relativePath;
            return path + "/" + relativePath;
        }

        public IProgressResult<Progress, BundleManifest> DownloadManifest(string relativePath)
        {
            ProgressResult<Progress, BundleManifest> result = new ProgressResult<Progress, BundleManifest>();
            Executors.RunOnCoroutine(DoDownloadManifest(relativePath, result), result);
            return result;
        }

        protected virtual IEnumerator DoDownloadManifest(string relativePath, IProgressPromise<Progress, BundleManifest> promise)
        {
            Progress progress = new Progress();
            promise.UpdateProgress(progress);
            byte[] data;
            string path = this.GetAbsoluteUri(relativePath);

#if UNITY_2017_1_OR_NEWER
            using (UnityWebRequest www = new UnityWebRequest(path))
            {
                www.downloadHandler = new DownloadHandlerBuffer();
#if UNITY_2018_1_OR_NEWER
                www.SendWebRequest();
#else
                www.Send();
#endif
                while (!www.isDone)
                {
                    if (www.downloadProgress >= 0)
                    {
                        if (progress.TotalSize <= 0)
                            progress.TotalSize = (long)(www.downloadedBytes / www.downloadProgress);
                        progress.CompletedSize = (long)www.downloadedBytes;
                        promise.UpdateProgress(progress);
                    }
                    yield return null;
                }

                if (!string.IsNullOrEmpty(www.error))
                {
                    promise.SetException(new Exception(www.error));
                    yield break;
                }

                data = www.downloadHandler.data;
            }
#else
            using (WWW www = new WWW(path))
            {
                while (!www.isDone)
                {
                    if (www.bytesDownloaded > 0f)
                    {
                        if (progress.TotalSize <= 0)
                            progress.TotalSize = (long)(www.bytesDownloaded / www.progress);
                        progress.CompletedSize = www.bytesDownloaded;
                        promise.UpdateProgress(progress);
                    }
                    yield return null;
                }

                progress.CompletedSize = www.bytesDownloaded;
                promise.UpdateProgress(progress);

                if (!string.IsNullOrEmpty(www.error))
                {
                    promise.SetException(new Exception(www.error));
                    yield break;
                }

                data = www.bytes;
            }
#endif

            try
            {
                BundleManifest manifest = BundleManifest.Parse(Encoding.UTF8.GetString(data));

                FileInfo file = new FileInfo(BundleUtil.GetStorableDirectory() + relativePath);
                if (file.Exists)
                {
                    FileInfo bakFile = new FileInfo(BundleUtil.GetStorableDirectory() + relativePath + ".bak");
                    if (bakFile.Exists)
                        bakFile.Delete();

                    file.CopyTo(bakFile.FullName);
                }

                if (!file.Directory.Exists)
                    file.Directory.Create();

                File.WriteAllBytes(file.FullName, data);
                promise.SetResult(manifest);
            }
            catch (IOException e)
            {
                promise.SetException(e);
            }
        }

        public IProgressResult<float, List<BundleInfo>> GetDownloadList(BundleManifest manifest)
        {
            ProgressResult<float, List<BundleInfo>> result = new ProgressResult<float, List<BundleInfo>>();
            Executors.RunOnCoroutine(DoAnalyzeDownloadList(result, manifest), result);
            return result;
        }

        protected virtual IEnumerator DoAnalyzeDownloadList(IProgressPromise<float, List<BundleInfo>> promise, BundleManifest manifest)
        {
            List<BundleInfo> downloads = new List<BundleInfo>();
            BundleInfo[] bundleInfos = manifest.GetAll();
            float last = Time.realtimeSinceStartup;
            int length = bundleInfos.Length;

            for (int i = 0; i < bundleInfos.Length; i++)
            {
                BundleInfo info = bundleInfos[i];
                if (Time.realtimeSinceStartup - last > 0.15f)
                {
                    yield return null;
                    last = Time.realtimeSinceStartup;
                }
                promise.UpdateProgress(i + 1 / (float)length);
                if (BundleUtil.Exists(info))
                    continue;

                downloads.Add(info);
            }
            promise.SetResult(downloads);
        }

        public IProgressResult<Progress, bool> DownloadBundles(List<BundleInfo> bundles)
        {
            ProgressResult<Progress, bool> result = new ProgressResult<Progress, bool>();
            Executors.RunOnCoroutine(DoDownloadBundles(result, bundles), result);
            return result;
        }

        protected abstract IEnumerator DoDownloadBundles(IProgressPromise<Progress, bool> promise, List<BundleInfo> bundles);
    }
}
