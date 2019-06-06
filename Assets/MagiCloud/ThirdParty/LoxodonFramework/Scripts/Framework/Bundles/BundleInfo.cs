using UnityEngine;
using System.Collections.Generic;

namespace Loxodon.Framework.Bundles
{
    /// <summary>
    /// The information of the AssetBundle.
    /// </summary>
    [System.Serializable]
    public class BundleInfo : ISerializationCallbackReceiver
    {
        /// <summary>
        /// Convert from JSON string to BundleInfo object.
        /// </summary>
        /// <param name="json">The JSON string</param>
        /// <returns></returns>
        public static BundleInfo Parse(string json)
        {
            return JsonUtility.FromJson<BundleInfo>(json);
        }

        [SerializeField]
        private string name;
        [SerializeField]
        private string variant;
        [SerializeField]
        private string hash;
        [SerializeField]
        private uint crc;
        [SerializeField]
        private long fileSize;
        [SerializeField]
        private string filename;
        [SerializeField]
        private string encoding;
        [SerializeField]
        private bool published;
        [SerializeField]
        private string[] dependencies = null;
        [SerializeField]
        private string[] assets = null;

        /// <summary>
        /// BundleInfo
        /// </summary>
        /// <param name="name">The AssetBundle's name.</param>
        /// <param name="variant">The variant's name.</param>
        /// <param name="hash">Hash.</param>
        /// <param name="crc">CRC.</param>
        /// <param name="fileSize">The file's size of the AssetBundle.</param>
        /// <param name="filename">The filename of the AssetBundle.</param>
        public BundleInfo(string name, string variant, Hash128 hash, uint crc, long fileSize, string filename) : this(name, variant, hash, crc, fileSize, filename, false, null, null)
        {
        }

        /// <summary>
        /// BundleInfo
        /// </summary>
        /// <param name="name">The AssetBundle's name.</param>
        /// <param name="variant">The variant's name.</param>
        /// <param name="hash">Hash.</param>
        /// <param name="crc">CRC.</param>
        /// <param name="fileSize">The file's size of the AssetBundle.</param>
        /// <param name="filename">The filename of the AssetBundle.</param>
        /// <param name="assets">All of the assets in this AssetBundle.</param>
        public BundleInfo(string name, string variant, Hash128 hash, uint crc, long fileSize, string filename, string[] assets) : this(name, variant, hash, crc, fileSize, filename, false, assets, null)
        {
        }

        /// <summary>
        /// BundleInfo
        /// </summary>
        /// <param name="name">The AssetBundle's name.</param>
        /// <param name="variant">The variant's name.</param>
        /// <param name="hash">Hash.</param>
        /// <param name="crc">CRC.</param>
        /// <param name="fileSize">The file's size of the AssetBundle.</param>
        /// <param name="filename">The filename of the AssetBundle.</param>
        /// <param name="published">This is a publication flag.If true,this bundle is a bundle that's loaded directly,otherwise this's a bundle that is loaded indirectly.</param>
        /// <param name="assets">All of the assets in this AssetBundle.</param>
        /// <param name="dependencies">All of the dependencies of the AssetBundle.</param>
        public BundleInfo(string name, string variant, Hash128 hash, uint crc, long fileSize, string filename, bool published, string[] assets, string[] dependencies)
        {
            if (string.IsNullOrEmpty(name))
                throw new System.ArgumentNullException("name");

            if (string.IsNullOrEmpty(filename))
                throw new System.ArgumentNullException("filename");

            this.name = name;
            this.variant = string.IsNullOrEmpty(variant) ? string.Empty : variant;
            this.hash = hash.ToString();
            this.crc = crc;
            this.fileSize = fileSize;
            this.filename = filename;
            this.published = published;
            this.encoding = string.Empty;
            this.dependencies = new string[0];
            this.assets = new string[0];

            List<string> dependencyList = new List<string>();
            if (dependencies != null && dependencies.Length > 0)
            {
                for (int i = 0; i < dependencies.Length; i++)
                {
                    var bundleName = Path.GetFilePathWithoutExtension(dependencies[i]);
                    if (string.IsNullOrEmpty(bundleName) || dependencyList.Contains(bundleName))
                        continue;

                    if (bundleName.Equals(this.Name))
                        throw new System.Exception(string.Format("Cannot have dependencies between variants of the same name.Please check '{0}' and '{1}'.", this.FullName, dependencies[i]));

                    dependencyList.Add(bundleName);
                }

                this.dependencies = dependencyList.ToArray();
            }

            if (assets != null)
                this.assets = assets;
        }

        /// <summary>
        ///  Gets the name of the AssetBundle.
        /// </summary>
        public string Name { get { return this.name; } }

        /// <summary>
        ///  Gets the variant's name of the AssetBundle.
        /// </summary>
        public string Variant { get { return this.variant; } }

        /// <summary>
        ///  Gets the full name of the AssetBundle.
        /// </summary>
        public string FullName
        {
            get
            {
                if (string.IsNullOrEmpty(this.variant))
                    return this.name;

                return string.Format("{0}.{1}", this.name, this.variant);
            }
        }

        /// <summary>
        ///  Gets the Hash of the AssetBundle file.
        /// </summary>
        public Hash128 Hash { get { return Hash128.Parse(this.hash); } }

        /// <summary>
        /// Gets the CRC of the AssetBundle file.
        /// </summary>
        public uint CRC { get { return this.crc; } }

        /// <summary>
        ///  Gets or sets the size of the AssetBundle file.
        /// </summary>
        public long FileSize
        {
            get { return this.fileSize; }
            set { this.fileSize = value; }
        }

        /// <summary>
        /// Gets or sets the encoding of the AssetBundle file.
        /// </summary>
        public string Encoding
        {
            get { return this.encoding; }
            set { this.encoding = value; }
        }

        /// <summary>
        /// Whether the AssetBundle's data is encrypted.
        /// </summary>
        public bool IsEncrypted { get { return !string.IsNullOrEmpty(this.encoding); } }

        /// <summary>
        /// This is a publication flag.If true,this bundle is a bundle that's loaded directly,otherwise this is a bundle that's loaded indirectly.
        /// </summary>
        public bool Published
        {
            get { return this.published; }
            set { this.published = value; }
        }

        /// <summary>
        /// Gets or sets the filename of the AssetBundle.
        /// </summary>
        public string Filename
        {
            get { return this.filename; }
            set { this.filename = value; }
        }

        ///// <summary>
        ///// Return true if the AssetBundle is a streamed scene AssetBundle.
        ///// </summary>
        //public bool IsStreamedScene { get { return false; } }

        /// <summary>
        /// Gets all of the dependencies of the AssetBundle.
        /// </summary>
        public string[] Dependencies { get { return this.dependencies; } }

        /// <summary>
        /// Gets all of the assets in this AssetBundle.
        /// </summary>
        public string[] Assets { get { return this.assets; } }

        public virtual void OnBeforeSerialize()
        {
        }

        public virtual void OnAfterDeserialize()
        {
        }

        /// <summary>
        /// Convert from the BundleInfo to JSON string
        /// </summary>
        /// <returns></returns>
        public virtual string ToJson()
        {
            return JsonUtility.ToJson(this);
        }

        /// <summary>
        /// Convert from the BundleInfo to string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return ToJson();
        }
    }
}
