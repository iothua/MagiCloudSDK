using System;

using Loxodon.Framework.Bundles;

namespace Loxodon.Framework.Examples.Bundle
{
    public class CustomBundleLoaderBuilder : AbstractLoaderBuilder
    {
        private bool useCache;
        private IDecryptor decryptor;

        public CustomBundleLoaderBuilder(Uri baseUri, bool useCache) : this(baseUri, useCache, null)
        {
        }

        public CustomBundleLoaderBuilder(Uri baseUri, bool useCache, IDecryptor decryptor) : base(baseUri)
        {
            this.useCache = useCache;
            this.decryptor = decryptor;
        }

        public override BundleLoader Create(BundleManager manager, BundleInfo bundleInfo)
        {
            //Customize the rules for finding assets.

            Uri loadBaseUri = this.BaseUri; //eg: http://your ip/bundles

            if (this.useCache && BundleUtil.ExistsInCache(bundleInfo))
            {
                //Load assets from the cache of Unity3d.
                loadBaseUri = this.BaseUri;
#if UNITY_5_4_OR_NEWER
                return new UnityWebRequestBundleLoader(new Uri(loadBaseUri, bundleInfo.Filename), bundleInfo, manager, this.useCache);
#else
                return new WWWBundleLoader(new Uri(loadBaseUri, bundleInfo.Filename), bundleInfo, manager, this.useCache);
#endif
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
#if UNITY_5_4_OR_NEWER
            if (this.IsRemoteUri(loadBaseUri))
                return new UnityWebRequestBundleLoader(new Uri(loadBaseUri, bundleInfo.Filename), bundleInfo, manager, this.useCache);
            else
                return new FileAsyncBundleLoader(new Uri(loadBaseUri, bundleInfo.Filename), bundleInfo, manager);
#else
            return new WWWBundleLoader(new Uri(loadBaseUri, bundleInfo.Filename), bundleInfo, manager, this.useCache);
#endif
        }
    }
}
