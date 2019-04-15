using System;

namespace Loxodon.Framework.Bundles
{
    /// <summary>
    /// <para>
    /// Simple path information parser.
    /// </para>
    /// <para>
    /// The path's format:{bundleName}{separator}{assetName}
    /// </para>
    /// <para>
    /// eg:
    /// bundle/textures@Characters/Textures/MonkeyKing.png
    /// bundle/characters@Characters/MonkeyKing.prefab
    /// </para>
    /// </summary>
    public class SimplePathInfoParser : IPathInfoParser
    {
        private string[] separators;

        public SimplePathInfoParser(string separator) : this(new string[] { separator })
        {
        }

        public SimplePathInfoParser(string[] separators)
        {
            this.separators = separators;
        }

        protected virtual string BundleNameNormalize(string bundleName)
        {
            int index = bundleName.IndexOf(".");
            if (index < 0)
                return bundleName.ToLower();
            return bundleName.Substring(0, index).ToLower();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path">The path.format:{bundleName}{separator}{assetName} eg:bundle/textures@Characters/Textures/MonkeyKing.png</param>
        /// <returns></returns>
        public virtual AssetPathInfo Parse(string path)
        {
            string[] texts = path.Split(this.separators, StringSplitOptions.RemoveEmptyEntries);
            if (texts.Length != 2)
                return null;

            var bundleName = BundleNameNormalize(texts[0]);
            var assetName = texts[1];
            return new AssetPathInfo(bundleName, assetName);
        }
    }
}
