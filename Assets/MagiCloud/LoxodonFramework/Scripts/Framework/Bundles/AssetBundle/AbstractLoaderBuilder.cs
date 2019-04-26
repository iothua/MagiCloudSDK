using System;
using UnityEngine;

namespace Loxodon.Framework.Bundles
{
    public abstract class AbstractLoaderBuilder : ILoaderBuilder
    {
        private Uri baseUri;

        public AbstractLoaderBuilder(Uri baseUri)
        {
            this.BaseUri = baseUri;
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

        protected virtual bool IsRemoteUri(Uri uri)
        {
            if ("http".Equals(uri.Scheme) || "https".Equals(uri.Scheme) || "ftp".Equals(uri.Scheme))
                return true;
            return false;
        }

        protected virtual bool IsJarUri(Uri uri)
        {
            if ("jar".Equals(uri.Scheme))
                return true;
            return false;
        }

        public abstract BundleLoader Create(BundleManager manager, BundleInfo bundleInfo);
    }
}
