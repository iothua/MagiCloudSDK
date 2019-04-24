#if !UNITY_2018_3_OR_NEWER
using System;

namespace Loxodon.Framework.Bundles
{
    public class WWWComplexLoaderBuilder : AbstractLoaderBuilder
    {
        private bool useCache;
        private IDecryptor decryptor;

        public WWWComplexLoaderBuilder(Uri baseUri, bool useCache) : this(baseUri, useCache, null)
        {
        }

        public WWWComplexLoaderBuilder(Uri baseUri, bool useCache, IDecryptor decryptor) : base(baseUri)
        {
            this.useCache = useCache;
            this.decryptor = decryptor;
        }

        public override BundleLoader Create(BundleManager manager, BundleInfo bundleInfo)
        {
            Uri loadBaseUri = this.BaseUri;
            if (this.useCache && BundleUtil.ExistsInCache(bundleInfo))
            {
                loadBaseUri = this.BaseUri;
                return new WWWBundleLoader(new Uri(loadBaseUri, bundleInfo.Filename), bundleInfo, manager, this.useCache);
            }

            if (BundleUtil.ExistsInStorableDirectory(bundleInfo))
            {
                /* Path: Application.persistentDataPath + "/bundles/" + bundleInfo.Filename  */
                loadBaseUri = new Uri(BundleUtil.GetStorableDirectory());
            }
#if !UNITY_WEBGL || UNITY_EDITOR
            else if (BundleUtil.ExistsInReadOnlyDirectory(bundleInfo))
            {
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

            return new WWWBundleLoader(new Uri(loadBaseUri, bundleInfo.Filename), bundleInfo, manager, this.useCache);
        }
    }
}
#endif
