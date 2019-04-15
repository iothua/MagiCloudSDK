namespace Loxodon.Framework.Bundles
{
    public class LocalPathInfoParser : IPathInfoParser
    {
        public virtual AssetPathInfo Parse(string path)
        {
            return new AssetPathInfo("", path);
        }
    }
}
