using System.Collections.Generic;
using Loxodon.Framework.Asynchronous;

namespace Loxodon.Framework.Bundles
{
    public class LocalBundleManager : IBundleManager
    {
        protected Dictionary<string, IBundle> bundles = new Dictionary<string, IBundle>();

        public LocalBundleManager()
        {
        }

        public virtual IBundle GetBundle(string bundleName)
        {
            bundleName = Path.GetFilePathWithoutExtension(bundleName).ToLower();
            IBundle bundle;
            if (this.bundles.TryGetValue(bundleName, out bundle))
                return bundle;

            bundle = new LocalBundle(bundleName);
            this.bundles.Add(bundleName, bundle);
            return bundle;
        }
        public virtual IProgressResult<float, IBundle> LoadBundle(string bundleName)
        {
            return this.LoadBundle(bundleName, 0);
        }

        public virtual IProgressResult<float, IBundle> LoadBundle(string bundleName, int priority)
        {
            IBundle bundle = GetBundle(bundleName);
            return new ImmutableProgressResult<float, IBundle>(bundle, 1f);
        }

        public virtual IProgressResult<float, IBundle[]> LoadBundle(params string[] bundleNames)
        {
            return this.LoadBundle(bundleNames, 0);
        }

        public virtual IProgressResult<float, IBundle[]> LoadBundle(string[] bundleNames, int priority)
        {
            IBundle[] bundles = new IBundle[bundleNames.Length];
            for (int i = 0; i < bundleNames.Length; i++)
            {
                bundles[i] = this.GetBundle(bundleNames[i]);
            }
            return new ImmutableProgressResult<float, IBundle[]>(bundles, 1f);
        }
    }
}
