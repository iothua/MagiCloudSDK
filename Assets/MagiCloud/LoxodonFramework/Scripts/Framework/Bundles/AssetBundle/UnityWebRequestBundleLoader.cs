#if UNITY_5_4_OR_NEWER
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

using Loxodon.Framework.Asynchronous;
//using Loxodon.Log;

namespace Loxodon.Framework.Bundles
{
    public class UnityWebRequestBundleLoaderBuilder : AbstractLoaderBuilder
    {
        private bool useCache;
        public UnityWebRequestBundleLoaderBuilder(System.Uri baseUri, bool useCache = true) : base(baseUri)
        {
            this.useCache = useCache;
        }

        public override BundleLoader Create(BundleManager manager, BundleInfo bundleInfo)
        {
            return new UnityWebRequestBundleLoader(new System.Uri(this.BaseUri, bundleInfo.Filename), bundleInfo, manager, this.useCache);
        }
    }

    public class UnityWebRequestBundleLoader : BundleLoader
    {
        //private static readonly ILog log = LogManager.GetLogger(typeof(UnityWebRequestBundleLoader));

        private bool useCache;
        public UnityWebRequestBundleLoader(System.Uri uri, BundleInfo bundleInfo, BundleManager manager, bool useCache = true) : base(uri, bundleInfo, manager)
        {
            this.useCache = useCache | this.IsRemoteUri();
        }

        protected override IEnumerator DoLoadAssetBundle(IProgressPromise<float, AssetBundle> promise)
        {
            if (this.BundleInfo.IsEncrypted)
            {
                promise.UpdateProgress(0f);
                promise.SetException(new NotSupportedException(string.Format("The data of the AssetBundle named '{0}' is encrypted,use the CryptographBundleLoader to load,please.", this.BundleInfo.Name)));
                yield break;
            }

            AssetBundle assetBundle;
            string path = this.GetAbsoluteUri();

#if UNITY_ANDROID && !UNITY_2017_1_OR_NEWER
            if (this.Uri.Scheme.Equals("jar", StringComparison.OrdinalIgnoreCase))
            {
                using (WWW www = useCache ? WWW.LoadFromCacheOrDownload(path, this.BundleInfo.Hash) : new WWW(path))
                {
                    while (!www.isDone)
                    {
                        if (www.progress >= 0)
                            promise.UpdateProgress(www.progress);
                        yield return null;
                    }

                    if (!string.IsNullOrEmpty(www.error))
                    {
                        promise.SetException(new Exception(string.Format("Failed to load the AssetBundle '{0}' at the address '{1}'.Error:{2}", this.BundleInfo.Name, path, www.error)));
                        yield break;
                    }

                    assetBundle = www.assetBundle;
                }
            }
            else
#endif
            {
#if UNITY_2018_1_OR_NEWER
                using (UnityWebRequest www = useCache ? UnityWebRequestAssetBundle.GetAssetBundle(path, this.BundleInfo.Hash, 0) : UnityWebRequestAssetBundle.GetAssetBundle(path))
                {
                    www.SendWebRequest();
#else
                using (UnityWebRequest www = useCache ? UnityWebRequest.GetAssetBundle(path, this.BundleInfo.Hash, 0) : UnityWebRequest.GetAssetBundle(path))
                {
                    www.Send();
#endif
                    while (!www.isDone)
                    {
                        if (www.downloadProgress >= 0)
                            promise.UpdateProgress(www.downloadProgress);
                        yield return null;
                    }

                    if (!string.IsNullOrEmpty(www.error))
                    {
                        promise.SetException(new Exception(string.Format("Failed to load the AssetBundle '{0}' at the address '{1}'.Error:{2}", this.BundleInfo.Name, path, www.error)));
                        yield break;
                    }

                    DownloadHandlerAssetBundle handler = (DownloadHandlerAssetBundle)www.downloadHandler;
                    assetBundle = handler.assetBundle;
                }
            }

            if (assetBundle == null)
            {
                promise.SetException(new Exception(string.Format("Failed to load the AssetBundle '{0}' at the address '{1}'.", this.BundleInfo.Name, path)));
                yield break;
            }

            promise.UpdateProgress(1f);
            promise.SetResult(assetBundle);
        }
    }
}
#endif