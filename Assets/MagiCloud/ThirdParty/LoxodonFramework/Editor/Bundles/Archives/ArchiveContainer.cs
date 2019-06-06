#if UNITY_EDITOR
using System;
using System.Collections.Generic;

namespace Loxodon.Framework.Bundles.Archives
{
    public class ArchiveContainer : IDisposable
    {
        private object _lock = new object();

        private List<AssetBundleArchive> bundles = new List<AssetBundleArchive>();

        private List<BuiltinArchive> builtinArchives = new List<BuiltinArchive>();

        public IList<AssetBundleArchive> Bundles { get { return this.bundles.AsReadOnly(); } }

        public ArchiveContainer()
        {
            builtinArchives.Add(new BuiltinArchive("resources/unity_builtin_extra"));
            builtinArchives.Add(new BuiltinArchive("library/unity default resources"));
        }

        public void AddBundleArchive(AssetBundleArchive bundle)
        {
            lock (_lock)
            {
                if (bundle == null)
                    throw new ArgumentNullException("bundle");

                if (this.bundles.Contains(bundle))
                    return;

                bundle.ArchiveContainer = this;
                this.bundles.Add(bundle);
            }
        }

        public void RemoveBundleArchive(AssetBundleArchive bundle)
        {
            lock (_lock)
            {
                bundle.ArchiveContainer = null;
                this.bundles.Remove(bundle);
            }
        }

        public virtual AssetBundleArchive GetBundleArchive(string name)
        {
            lock (_lock)
            {
                foreach (var bundle in bundles)
                {
                    if (bundle.Name.Equals(name))
                        return bundle;
                }
                return null;
            }
        }

        public virtual void Clear()
        {
            lock (_lock)
            {
                foreach (var bundle in this.bundles)
                {
                    bundle.Dispose();
                }
                this.bundles.Clear();
            }
        }

        public virtual AssetArchive GetAssetArchive(string name)
        {
            lock (_lock)
            {
                if (string.IsNullOrEmpty(name))
                    return null;

                AssetArchive asset = null;
                foreach (var bundle in bundles)
                {
                    asset = bundle.GetAssetArchive(name);
                    if (asset != null)
                        return asset;
                }
                return asset;
            }
        }

        public virtual IObjectInfo GetObjectInfo(string assetName, long pathId)
        {
            lock (_lock)
            {
                foreach (var builtinAsset in builtinArchives)
                {
                    if (!builtinAsset.Name.Equals(assetName))
                        continue;

                    return builtinAsset.GetObjectInfo(pathId);
                }

                ObjectArchive asset = GetAssetArchive(assetName) as ObjectArchive;
                if (asset != null)
                    return asset.GetObjectInfo(pathId);

                UnityEngine.Debug.LogWarningFormat("Object not found,AssetName:{0} ID:{1}", assetName,pathId);
                return new MissingObjectInfo(null, pathId, TypeID.UnknownType);
            }
        }

        #region IDisposable Support
        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            lock (_lock)
            {
                if (!disposed)
                {
                    if (disposing)
                    {
                        this.Clear();
                    }
                    disposed = true;
                }
            }
        }

        ~ArchiveContainer()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
#endif