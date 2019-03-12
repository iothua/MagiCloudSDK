#if !UNITY_5_4_OR_NEWER
using UnityEditor;

namespace Loxodon.Framework.Bundles.Editors
{
    public static class AssetImporterExtensions
    {
        public static void SetAssetBundleNameAndVariant(this AssetImporter importer, string assetBundleName, string assetBundleVariant)
        {
            if (!importer.assetBundleName.Equals(assetBundleName))
                importer.assetBundleName = assetBundleName;

            if (!importer.assetBundleVariant.Equals(assetBundleVariant))
                importer.assetBundleVariant = assetBundleVariant;

            importer.SaveAndReimport();
        }
    }
}
#endif