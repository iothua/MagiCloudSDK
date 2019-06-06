using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using Loxodon.Framework.Bundles;
using Loxodon.Framework.Asynchronous;
using MagiCloud.Bundle.Downloader;
using System.Text;

namespace MagiCloud
{
    /// <summary>
    /// AssetBundle管理端
    /// </summary>
    public class AssetBundleManager
    {
        public LocalResources localResources = new LocalResources();

        public IResources resources;

        private Dictionary<string, IBundle> bundles = new Dictionary<string, IBundle>();

        private string iv = "I9Ldk05g2ezWEXE9";
        private string key = "uz0NlpJaMnG7dHrR";

        private string uriString;

        private IDownloader downloader;
        private bool downloading = false;

        /// <summary>
        /// 是否在下载
        /// </summary>
        public bool IsDownloading {
            get {
                return downloading;
            }
        }

        public enum AssetPath
        {
            streamingAssetsPath,
            persistentDataPath,
            temporaryCachePath
        }

        public AssetBundleManager(string bundleDownladerUri,AssetPath assetPath = AssetPath.streamingAssetsPath)
        {
            switch (assetPath)
            {
                case AssetPath.streamingAssetsPath:
                    uriString = BundleUtil.GetReadOnlyDirectory();
                    break;
                case AssetPath.persistentDataPath:
                    uriString = BundleUtil.GetStorableDirectory();
                    break;
                case AssetPath.temporaryCachePath:
                    uriString = BundleUtil.GetTemporaryCacheDirectory();
                    break;
            }

            this.resources = null;
            if (string.IsNullOrEmpty(bundleDownladerUri)) return;

            Uri baseUri = new Uri(bundleDownladerUri);
            this.downloader = new WWWDownloader(baseUri, false);

        }



        IResources GetResources()
        {
            if (resources != null)
                return resources;

            IBundleManifestLoader manifestLoader = new BundleManifestLoader();
            BundleManifest manifest = manifestLoader.Load(uriString + BundleSetting.ManifestFilename);

            IPathInfoParser pathInfoParser = new AutoMappingPathInfoParser(manifest);

            ILoaderBuilder builder = new Loxodon.Framework.Examples.Bundle.CustomBundleLoaderBuilder(new Uri(uriString), false, new RijndaelCryptograph(128, Encoding.ASCII.GetBytes(key), Encoding.ASCII.GetBytes(iv)));

            IBundleManager manager = new BundleManager(manifest, builder);

            resources = new BundleResources(pathInfoParser, manager);

            return resources;
        }

        public AssetBundleManager(string bundleDownladerUri, string customBundlePath)
        {
            this.uriString = customBundlePath;
            resources = null;

            Uri baseUri = new Uri(bundleDownladerUri);
            this.downloader = new WWWDownloader(baseUri, false);
        }


        public IEnumerator Download(List<string> bundleNames)
        {
            this.downloading = true;

            try
            {
                IProgressResult<Progress, BundleManifest> manifestResult = this.downloader.DownloadManifest(BundleSetting.ManifestFilename);

                yield return manifestResult.WaitForDone();

                if (manifestResult.Exception != null)
                {
                    //Debug.LogFormat("Downloads BundleManifest failure.Error:{0}", manifestResult.Exception);
                    yield break;
                }

                BundleManifest manifest = manifestResult.Result;

                IProgressResult<float, List<BundleInfo>> bundlesResult = this.downloader.GetDownloadList(manifest);

                yield return bundlesResult.WaitForDone();

                List<BundleInfo> bundles = bundlesResult.Result.FindAll(obj => bundleNames.Contains(obj.FullName));

                if (bundles == null || bundles.Count <= 0)
                {
                    Debug.LogFormat("Please clear cache and remove StreamingAssets,try again.");
                    yield break;
                }

                IProgressResult<Progress, bool> downloadResult = this.downloader.DownloadBundles(bundles);
                downloadResult.Callbackable().OnProgressCallback(p =>
                {
                    Debug.LogFormat("Downloading {0:F2}KB/{1:F2}KB {2:F3}KB/S", p.GetCompletedSize(UNIT.KB), p.GetTotalSize(UNIT.KB), p.GetSpeed(UNIT.KB));
                });

                yield return downloadResult.WaitForDone();

                if (downloadResult.Exception != null)
                {
                    Debug.LogFormat("Downloads AssetBundle failure.Error:{0}", downloadResult.Exception);
                    yield break;
                }

                if (this.resources != null)
                {
                    //update BundleManager's manifest
                    BundleManager manager = (this.resources as BundleResources).BundleManager as BundleManager;
                    manager.BundleManifest = manifest;
                }

#if UNITY_EDITOR
                UnityEditor.EditorUtility.OpenWithDefaultApp(BundleUtil.GetReadOnlyDirectory());
#endif

            }
            finally
            {
                this.downloading = false;
            }
        }


        public void LoadAsset<T>(string[] assetNames, Action<T[]> completed, Action<float> progress = null)
             where T : UnityEngine.Object
        {
            var resources = GetResources();

            IProgressResult<float, T[]> result = resources.LoadAssetsAsync<T>(assetNames);

            result.Callbackable().OnProgressCallback(p =>
            {
                if (progress != null)
                    progress(p * 100);
            });

            result.Callbackable().OnCallback((r) =>
            {
                try
                {
                    if (r.Exception != null)
                        throw r.Exception;

                    if (completed != null)
                        completed(r.Result);
                }
                catch (Exception e)
                {
                    Debug.LogErrorFormat("加载错误:{0}", e);
                }
            });
        }

        public IEnumerator OnLoadAsset<T>(string[] assetNames, Action<T[]> completed, Action<float> progress = null)
            where T : UnityEngine.Object
        {
            var resources = GetResources();

            IProgressResult<float, T[]> result = resources.LoadAssetsAsync<T>(assetNames);

            while (!result.IsDone)
            {
                if (progress != null)
                    progress(result.Progress * 100);

                yield return null;
            }

            result.Callbackable().OnCallback((r) =>
            {
                try
                {
                    if (r.Exception != null)
                        throw r.Exception;

                    if (completed != null)
                        completed(r.Result);
                }
                catch (Exception e)
                {
                    Debug.LogErrorFormat("加载错误:{0}", e);
                }
            });

            yield return null;
        }
    }

}
