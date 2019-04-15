using System;

namespace Loxodon.Framework.Bundles
{
    public interface ILoaderBuilder
    {
        //Uri BaseUri { get; set; }

        BundleLoader Create(BundleManager manager, BundleInfo bundleInfo, BundleLoader[] dependencies);
    }
}
