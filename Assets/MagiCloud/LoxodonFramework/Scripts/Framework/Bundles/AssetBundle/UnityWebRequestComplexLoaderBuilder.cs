#if UNITY_5_4_OR_NEWER
using System;

namespace Loxodon.Framework.Bundles
{
    public class UnityWebRequestComplexLoaderBuilder : AbstractLoaderBuilder
    {
        private bool useCache;
        private IDecryptor decryptor;

        public UnityWebRequestComplexLoaderBuilder(Uri baseUri, bool useCache) : this(baseUri, useCache, null)
        {
        }

        public UnityWebRequestComplexLoaderBuilder(Uri baseUri, bool useCache, IDecryptor decryptor) : base(baseUri)
        {
            this.useCache = useCache;
            this.decryptor = decryptor;
        }

        public override BundleLoader Create(BundleManager manager, BundleInfo bundleInfo)
        {
            Uri loadBaseUri = this.BaseUri;

            if (this.useCache && BundleUtil.ExistsInCache(bundleInfo))
            {
                //Load assets from the cache of Unity3d.
                loadBaseUri = this.BaseUri;
                return new UnityWebRequestBundleLoader(new Uri(loadBaseUri, bundleInfo.Filename), bundleInfo, manager, this.useCache);
            }

            if (BundleUtil.ExistsInStorableDirectory(bundleInfo))
            {
                //Load assets from the "Application.persistentDataPath/bundles" folder.
                /* Path: Application.persistentDataPath + "/bundles/" + bundleInfo.Filename  */
                loadBaseUri = new Uri(BundleUtil.GetStorableDirectory());
            }

#if !UNITY_WEBGL || UNITY_EDITOR
            else if (BundleUtil.ExistsInReadOnlyDirectory(bundleInfo))
            {
                //Load assets from the "Application.streamingAssetsPath/bundles" folder.
                /* Path: Application.streamingAssetsPath + "/bundles/" + bundleInfo.Filename */
                loadBaseUri = new Uri(BundleUtil.GetReadOnlyDirectory());
            }
#endif

            if (bundleInfo.IsEncrypted)
            {
                if (this.decryptor != null && bundleInfo.Encoding.Equals(decryptor.AlgorithmName))
                    return new CryptographBundleLoader(new Uri(loadBaseUri, bundleInfo.Filename), bundleInfo, manager, decryptor);

                throw new NotSupportedException(string.Format("Not support the encryption algorithm '{0}'.", bundleInfo.Encoding));
            }

            //Loads assets from an Internet address if it does not exist in the local directory.
            if (this.IsRemoteUri(loadBaseUri))
                return new UnityWebRequestBundleLoader(new Uri(loadBaseUri, bundleInfo.Filename), bundleInfo, manager, this.useCache);
            else
                return new FileAsyncBundleLoader(new Uri(loadBaseUri, bundleInfo.Filename), bundleInfo, manager);
        }
    }
}
#endif
