using Loxodon.Log;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Loxodon.Framework.Bundles
{
    /// <summary>
    /// The manifest about the AssetBundle.
    /// </summary>
    [Serializable]
    public class BundleManifest : ISerializationCallbackReceiver
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(BundleManifest));

        /// <summary>
        /// Convert from JSON string to BundleManifest object.
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static BundleManifest Parse(string json)
        {
            return JsonUtility.FromJson<BundleManifest>(json);
        }

        [SerializeField]
        private BundleInfo[] bundleInfos = null;
        [SerializeField]
        private string defaultVariant = "";
        [SerializeField]
        private string version;

        [NonSerialized]
        private string[] activeVariants;
        [NonSerialized]
        private Dictionary<string, BundleInfo> bundles = new Dictionary<string, BundleInfo>();

        /// <summary>
        /// BundleManifest
        /// </summary>
        public BundleManifest() : this(null, "1.0.0", null, null)
        {
        }

        /// <summary>
        /// BundleManifest
        /// </summary>
        /// <param name="bundleInfos">All of the BundleInfos.</param>
        /// <param name="version">The version of the AssetBundle data.</param>
        public BundleManifest(List<BundleInfo> bundleInfos, string version) : this(bundleInfos, version, null, null)
        {
        }

        /// <summary>
        /// BundleManifest
        /// </summary>
        /// <param name="bundleInfos">All of the BundleInfos.</param>
        /// <param name="version">The version of the AssetBundle data.</param>
        /// <param name="defaultVariant">The default variant's name.</param>
        public BundleManifest(List<BundleInfo> bundleInfos, string version, string defaultVariant) : this(bundleInfos, version, defaultVariant, null)
        {
        }

        /// <summary>
        /// BundleManifest
        /// </summary>
        /// <param name="bundleInfos">All of the BundleInfos.</param>
        /// <param name="version">The version of the AssetBundle data.</param>
        /// <param name="defaultVariant">The default variant's name.</param>
        /// <param name="variants">All of the variants has been activated.According to the priority ascending.</param>
        public BundleManifest(List<BundleInfo> bundleInfos, string version, string defaultVariant, string[] variants)
        {
            if (bundleInfos != null)
                this.bundleInfos = bundleInfos.ToArray();
            else
                this.bundleInfos = new BundleInfo[0];

            if (defaultVariant != null)
                this.defaultVariant = defaultVariant;
            else
                this.defaultVariant = this.AnalyzeDefaultVariant(bundleInfos);

            this.version = version;
            this.ActiveVariants = variants != null ? variants : new string[] { this.defaultVariant };
        }

        public virtual void OnBeforeSerialize()
        {
        }

        public virtual void OnAfterDeserialize()
        {
            if (this.defaultVariant != null)
                this.ActiveVariants = new string[] { this.defaultVariant };
            else
                this.ActiveVariants = new string[] { "" };
        }

        /// <summary>
        /// Analysis of the default variants.
        /// </summary>
        /// <param name="bundleInfos"></param>
        /// <returns></returns>
        protected string AnalyzeDefaultVariant(List<BundleInfo> bundleInfos)
        {
            if (bundleInfos == null || bundleInfos.Count <= 0)
                return "";

            Dictionary<string, int> dict = new Dictionary<string, int>();
            for (int i = 0; i < bundleInfos.Count; i++)
            {
                BundleInfo info = bundleInfos[i];
                if (string.IsNullOrEmpty(info.Variant))
                    return "";

                if (!dict.ContainsKey(info.Variant))
                    dict[info.Variant] = 1;
                else
                    dict[info.Variant] = dict[info.Variant] + 1;
            }

            List<KeyValuePair<string, int>> list = new List<KeyValuePair<string, int>>();
            var it = dict.GetEnumerator();
            while (it.MoveNext())
                list.Add(it.Current);

            list.Sort((x, y) => y.Value.CompareTo(x.Value));

            return list[0].Key;
        }

        protected virtual BundleInfo Compare(BundleInfo info1, BundleInfo info2)
        {
            if (this.activeVariants != null && this.activeVariants.Length > 0)
            {
                int index1 = Array.IndexOf(this.activeVariants, info1.Variant);
                int index2 = Array.IndexOf(this.activeVariants, info2.Variant);
                return index1 > index2 ? info1 : info2;
            }
            return info2;
        }

        protected virtual void Initialize()
        {
            bundles.Clear();

            if (this.bundleInfos == null || this.bundleInfos.Length <= 0)
                return;

            for (int i = 0; i < this.bundleInfos.Length; i++)
            {
                BundleInfo info = this.bundleInfos[i];
                BundleInfo old = null;
                if (bundles.TryGetValue(info.Name, out old) && old != null)
                    bundles[info.Name] = Compare(old, info);
                else
                    bundles[info.Name] = info;
            }
        }

        /// <summary>
        ///  Gets the version of the AssetBundle data.
        /// </summary>
        public virtual string Version { get { return this.version; } }

        /// <summary>
        ///  Gets or sets all of the variants,they have been activated.
        /// </summary>
        public virtual string[] ActiveVariants
        {
            get { return this.activeVariants; }
            set
            {
                List<string> variants = new List<string>() { "" };
                if (!variants.Contains(this.defaultVariant))
                    variants.Add(this.defaultVariant);

                if (value != null && value.Length > 0)
                {
                    foreach (string variant in value)
                    {
                        if (string.IsNullOrEmpty(variant))
                            continue;

                        if (variants.Contains(variant))
                            continue;

                        variants.Add(variant);
                    }
                }

                this.activeVariants = variants.ToArray();
                Initialize();
            }
        }

        public virtual bool Contains(string bundleName)
        {
            if (this.bundleInfos == null || this.bundleInfos.Length <= 0)
                return false;

            var name = Path.GetFilePathWithoutExtension(bundleName);
            var variant = Path.GetExtension(bundleName);
            return Array.Exists(this.bundleInfos, b => b.Name == name && b.Variant == variant);
        }

        /// <summary>
        /// Gets the BundleInfo for the given name.
        /// </summary>
        /// <param name="bundleName"></param>
        /// <returns></returns>
        public virtual BundleInfo GetBundleInfo(string bundleName)
        {
            BundleInfo info;
            if (this.bundles.TryGetValue(Path.GetFilePathWithoutExtension(bundleName), out info))
                return info;
            return null;
        }

        /// <summary>
        /// Gets the BundleInfos for the given name.
        /// </summary>
        /// <param name="bundleNames"></param>
        /// <returns></returns>
        public virtual BundleInfo[] GetBundleInfos(params string[] bundleNames)
        {
            if (bundleNames == null || bundleNames.Length <= 0)
                return new BundleInfo[0];

            List<BundleInfo> list = new List<BundleInfo>();
            for (int i = 0; i < bundleNames.Length; i++)
            {
                var name = Path.GetFilePathWithoutExtension(bundleNames[i]);
                BundleInfo info;
                if (bundles.TryGetValue(name, out info))
                {
                    if (info != null && !list.Contains(info))
                        list.Add(info);
                }
            }
            return list.ToArray();
        }

        /// <summary>
        /// Gets all of the dependencies for the given name.
        /// </summary>
        /// <param name="bundleName"></param>
        /// <param name="recursive"></param>
        /// <returns></returns>
        public virtual BundleInfo[] GetDependencies(string bundleName, bool recursive)
        {
            BundleInfo info = this.GetBundleInfo(bundleName);
            if (info == null)
                return new BundleInfo[0];

            List<BundleInfo> list = new List<BundleInfo>();
            this.GetDependencies(info, info, recursive, list);
            return list.ToArray();
        }

        protected virtual void GetDependencies(BundleInfo root, BundleInfo info, bool recursive, List<BundleInfo> list)
        {
            string[] dependencyNames = info.Dependencies;
            if (dependencyNames == null || dependencyNames.Length <= 0)
                return;

            BundleInfo[] dependencies = this.GetBundleInfos(dependencyNames);
            for (int i = 0; i < dependencies.Length; i++)
            {
                var dependency = dependencies[i];
                if (dependency.Equals(root))
                {
                    if (log.IsWarnEnabled)
                        log.WarnFormat("It has an loop reference between '{0}' and '{1}',it is recommended to redistribute their assts.", root.Name, info.Name);

                    continue;
                    //throw new LoopingReferenceException(string.Format("There is a error occurred.It has an unresolvable loop reference between '{0}' and '{1}'.", root.Name, info.Name));
                }

                if (list.Contains(dependency))
                    continue;

                list.Add(dependency);

                if (recursive)
                    this.GetDependencies(root, dependency, recursive, list);
            }
        }

        /// <summary>
        /// Gets all of the BundleInfos.
        /// </summary>
        /// <returns></returns>
        public virtual BundleInfo[] GetAll()
        {
            return this.bundleInfos;
        }

        /// <summary>
        /// Gets all of the BundleInfos has been activated.
        /// </summary>
        /// <returns></returns>
        public virtual BundleInfo[] GetAllActivated()
        {
            BundleInfo[] bundleInfos = new BundleInfo[this.bundles.Count];
            var it = this.bundles.GetEnumerator();
            int i = 0;
            while (it.MoveNext())
                bundleInfos[i++] = it.Current.Value;
            return bundleInfos;
        }

        /// <summary>
        /// Convert from the BundleManifest to JSON string
        /// </summary>
        /// <returns></returns>
        public virtual string ToJson()
        {
            return JsonUtility.ToJson(this);
        }

        /// <summary>
        /// Convert from the BundleManifest to string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return ToJson();
        }

    }
}
