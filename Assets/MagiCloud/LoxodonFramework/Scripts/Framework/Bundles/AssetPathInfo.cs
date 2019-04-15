namespace Loxodon.Framework.Bundles
{
    /// <summary>
    /// Asset path info
    /// </summary>
    public class AssetPathInfo
    {
        private string bundleName;
        private string assetName;
        public AssetPathInfo(string bundleName, string assetName)
        {
            this.bundleName = bundleName;
            this.assetName = assetName;
        }

        public string BundleName
        {
            get { return this.bundleName; }
        }

        public string AssetName
        {
            get { return this.assetName; }
        }

        public override int GetHashCode()
        {
            return assetName.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            AssetPathInfo info = (AssetPathInfo)obj;
            return info.assetName == this.assetName && info.bundleName == this.bundleName;
        }

        public override string ToString()
        {
            return string.Format("{0}@{1}",this.bundleName,this.assetName);
        }
    }
}
