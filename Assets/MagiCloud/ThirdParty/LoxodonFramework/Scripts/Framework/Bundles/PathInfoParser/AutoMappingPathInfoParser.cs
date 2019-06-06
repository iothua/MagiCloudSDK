using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Loxodon.Framework.Bundles
{
    public class AutoMappingPathInfoParser : IPathInfoParser
    {
        private BundleManifest bundleManifest;
        private Dictionary<string, string> dict = new Dictionary<string, string>();

        public AutoMappingPathInfoParser(BundleManifest manifest)
        {
            this.BundleManifest = manifest;
        }

        public BundleManifest BundleManifest
        {
            get { return this.bundleManifest; }
            set
            {
                if (this.bundleManifest == value)
                    return;

                this.bundleManifest = value;
                this.Initialize();
            }
        }

        protected virtual void Initialize()
        {
            if (this.dict != null)
                this.dict.Clear();

            if (this.dict == null)
                this.dict = new Dictionary<string, string>();

            Regex regex = new Regex("^assets/", RegexOptions.IgnoreCase);
            BundleInfo[] bundleInfos = this.bundleManifest.GetAll();
            foreach (BundleInfo info in bundleInfos)
            {
                if (!info.Published)
                    continue;

                var assets = info.Assets;
                for (int i = 0; i < assets.Length; i++)
                {
                    var assetPath = assets[i].ToLower();
                    var key = regex.Replace(assetPath, "");
                    dict[key] = info.Name;
                }
            }
        }

        public virtual AssetPathInfo Parse(string path)
        {
            string bundleName;
            if (!this.dict.TryGetValue(path.ToLower(), out bundleName))
                return null;

            return new AssetPathInfo(bundleName, path);
        }
    }
}
