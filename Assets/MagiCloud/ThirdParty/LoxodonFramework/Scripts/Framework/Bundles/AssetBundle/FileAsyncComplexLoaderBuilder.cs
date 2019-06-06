using System;
using UnityEngine;

namespace Loxodon.Framework.Bundles
{
    public class FileAsyncComplexLoaderBuilder : AbstractLoaderBuilder
    {
        private IDecryptor decryptor;

        public FileAsyncComplexLoaderBuilder(Uri baseUri) : this(baseUri, null)
        {
        }

        public FileAsyncComplexLoaderBuilder(Uri baseUri, IDecryptor decryptor) : base(baseUri)
        {
            this.decryptor = decryptor;
        }

        protected override bool IsAllowedAbsoluteUri(Uri uri)
        {
            if (!uri.IsAbsoluteUri)
                return false;

            if (RuntimePlatform.Android.Equals(Application.platform) && uri.Scheme.Equals("jar", StringComparison.OrdinalIgnoreCase))
                return true;

            if ("file".Equals(uri.Scheme) && uri.OriginalString.IndexOf("jar:") < 0)
                return true;

            return false;
        }

        public override BundleLoader Create(BundleManager manager, BundleInfo bundleInfo)
        {
            Uri loadBaseUri = this.BaseUri;

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

#if UNITY_ANDROID && !UNITY_5_4_OR_NEWER
            if (loadBaseUri != null && loadBaseUri.Scheme.Equals("jar", StringComparison.OrdinalIgnoreCase))
                return new WWWBundleLoader(new Uri(loadBaseUri, bundleInfo.Filename), bundleInfo, manager, false);
#endif

            return new FileAsyncBundleLoader(new Uri(loadBaseUri, bundleInfo.Filename), bundleInfo, manager);
        }
    }
}
