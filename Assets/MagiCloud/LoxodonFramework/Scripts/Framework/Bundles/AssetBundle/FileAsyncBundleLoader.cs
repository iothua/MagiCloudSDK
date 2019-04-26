using System;
using System.Collections;
using UnityEngine;

using Loxodon.Framework.Asynchronous;

namespace Loxodon.Framework.Bundles
{
    public class FileAsyncBundleLoaderBuilder : AbstractLoaderBuilder
    {
        public FileAsyncBundleLoaderBuilder(Uri baseUri) : base(baseUri)
        {
        }

        public override BundleLoader Create(BundleManager manager, BundleInfo bundleInfo)
        {
            return new FileAsyncBundleLoader(new Uri(this.BaseUri, bundleInfo.Filename), bundleInfo, manager);
        }
    }

    public class FileAsyncBundleLoader : BundleLoader
    {
        public FileAsyncBundleLoader(Uri uri, BundleInfo bundleInfo, BundleManager manager) : base(uri, bundleInfo, manager)
        {
        }

        protected override IEnumerator DoLoadAssetBundle(IProgressPromise<float, AssetBundle> promise)
        {
            if (this.BundleInfo.IsEncrypted)
            {
                promise.UpdateProgress(0f);
                promise.SetException(new NotSupportedException(string.Format("The data of the AssetBundle named '{0}' is encrypted,use the CryptographBundleLoader to load,please.", this.BundleInfo.Name)));
                yield break;
            }

            string path = this.GetAbsolutePath();
#if UNITY_ANDROID && !UNITY_5_4_OR_NEWER
            if (this.Uri.Scheme.Equals("jar", StringComparison.OrdinalIgnoreCase))
            {
                promise.UpdateProgress(0f);
                promise.SetException(new NotSupportedException(string.Format("Failed to load the AssetBundle '{0}' at the address '{1}'.It is not supported before the Unity3d 5.4.0 version.", this.BundleInfo.Name, path)));
                yield break;
            }
#endif
            AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(path);
            while (!request.isDone)
            {
                promise.UpdateProgress(request.progress);
                yield return null;
            }

            var assetBundle = request.assetBundle;
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
