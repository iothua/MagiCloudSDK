#if !UNITY_2018_3_OR_NEWER
using System;
using System.Collections;
using UnityEngine;

using Loxodon.Framework.Asynchronous;
using System.IO;
using Loxodon.Log;

namespace Loxodon.Framework.Bundles
{
    public class WWWBundleLoaderBuilder : AbstractLoaderBuilder
    {
        private bool useCache;
        public WWWBundleLoaderBuilder(Uri baseUri, bool useCache) : base(baseUri)
        {
            this.useCache = useCache;
        }

        public override BundleLoader Create(BundleManager manager, BundleInfo bundleInfo)
        {
            return new WWWBundleLoader(new Uri(this.BaseUri, bundleInfo.Filename), bundleInfo, manager, this.useCache);
        }
    }

    public class WWWBundleLoader : BundleLoader
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(WWWBundleLoader));

        private bool useCache = false;

        public WWWBundleLoader(Uri uri, BundleInfo bundleInfo, BundleManager manager, bool useCache) : base(uri, bundleInfo, manager)
        {
            this.useCache = useCache;
        }

        protected override IEnumerator DoLoadAssetBundle(IProgressPromise<float, AssetBundle> promise)
        {
            if (this.BundleInfo.IsEncrypted)
            {
                promise.UpdateProgress(0f);
                promise.SetException(new NotSupportedException(string.Format("The data of the AssetBundle named '{0}' is encrypted,use the CryptographBundleLoader to load,please.", this.BundleInfo.Name)));
                yield break;
            }

            string path = this.GetAbsoluteUri();

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

                var assetBundle = www.assetBundle;
                if (assetBundle == null)
                {
                    promise.SetException(new Exception(string.Format("Failed to load the AssetBundle '{0}' at the address '{1}'.", this.BundleInfo.Name, path)));
                    yield break;
                }

                if (!useCache && this.IsRemoteUri())
                {
                    string fullname = BundleUtil.GetStorableDirectory() + this.BundleInfo.Filename;
                    try
                    {
                        FileInfo info = new FileInfo(fullname);
                        if (info.Exists)
                            info.Delete();

                        if (!info.Directory.Exists)
                            info.Directory.Create();

                        File.WriteAllBytes(info.FullName, www.bytes);
                    }
                    catch (Exception e)
                    {
                        if (log.IsWarnEnabled)
                            log.WarnFormat("Save AssetBundle '{0}' to the directory '{1}' failed.Reason:{2}", this.BundleInfo.FullName, fullname, e);
                    }
                }
                promise.UpdateProgress(1f);
                promise.SetResult(assetBundle);
            }
        }
    }
}
#endif