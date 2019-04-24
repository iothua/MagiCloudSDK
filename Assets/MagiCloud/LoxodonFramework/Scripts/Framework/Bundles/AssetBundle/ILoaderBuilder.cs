using System;

namespace Loxodon.Framework.Bundles
{
    public interface ILoaderBuilder
    {
        BundleLoader Create(BundleManager manager, BundleInfo bundleInfo);
    }
}
