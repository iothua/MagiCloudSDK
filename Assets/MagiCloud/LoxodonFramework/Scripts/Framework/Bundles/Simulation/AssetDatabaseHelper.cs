#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;

namespace Loxodon.Framework.Bundles
{
    public static class AssetDatabaseHelper
    {
        static Dictionary<string, string> bundleNames = new Dictionary<string, string>();

        public static List<string> GetUsedAssetBundleNames()
        {
            List<string> list = new List<string>(AssetDatabase.GetAllAssetBundleNames());
            foreach (string name in AssetDatabase.GetUnusedAssetBundleNames())
            {
                list.Remove(name);
            }
            return list;
        }

        public static string GetImplicitAssetBundleName(string path)
        {
            var fullName = GetImplicitAssetBundleFullName(path);
            if (string.IsNullOrEmpty(fullName))
                return fullName;
            return Path.GetFilePathWithoutExtension(fullName);
        }

        public static string GetImplicitAssetBundleVariantName(string path)
        {
            var fullName = GetImplicitAssetBundleFullName(path);
            if (string.IsNullOrEmpty(fullName))
                return fullName;
            return Path.GetExtension(fullName);
        }

        static string GetImplicitAssetBundleFullName(string path)
        {
            string bundleFullName;
            if (bundleNames.TryGetValue(path, out bundleFullName))
                return bundleFullName;

            AssetImporter importer = AssetImporter.GetAtPath(path);
            if (importer == null)
                return string.Empty;

            var bundleName = importer.assetBundleName;
            var variant = importer.assetBundleVariant;
            if (!string.IsNullOrEmpty(bundleName))
            {
                bundleFullName = string.IsNullOrEmpty(variant) ? bundleName : string.Format("{0}.{1}", bundleName, variant);
                bundleNames.Add(path, bundleFullName);
                return bundleFullName;
            }

            path = System.IO.Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(path))
                return GetImplicitAssetBundleFullName(path);

            return string.Empty;
        }
    }
}
#endif
