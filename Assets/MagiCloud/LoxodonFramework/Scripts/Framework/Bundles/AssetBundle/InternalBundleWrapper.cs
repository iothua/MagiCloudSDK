using UnityEngine;
using Loxodon.Framework.Execution;
using Loxodon.Framework.Asynchronous;

namespace Loxodon.Framework.Bundles
{
    /// <summary>
    /// Bundle wrapper
    /// </summary>
    internal class InternalBundleWrapper : IBundle
    {
        private DefaultBundle bundle;
        public InternalBundleWrapper(DefaultBundle bundle)
        {
            this.bundle = bundle;
            this.bundle.Retain();
        }
        #region IBundle Support
        public virtual string Name
        {
            get { return this.bundle.Name; }
        }

        public virtual Object LoadAsset(string name, System.Type type)
        {
            return this.bundle.LoadAsset(name, type);
        }

        public virtual T LoadAsset<T>(string name) where T : Object
        {
            return this.bundle.LoadAsset<T>(name);
        }

        public virtual IProgressResult<float, Object> LoadAssetAsync(string name, System.Type type)
        {
            return this.bundle.LoadAssetAsync(name, type);
        }

        public virtual IProgressResult<float, T> LoadAssetAsync<T>(string name) where T : Object
        {
            return this.bundle.LoadAssetAsync<T>(name);
        }

        public virtual IProgressResult<float, T[]> LoadAssetsAsync<T>(params string[] names) where T : Object
        {
            return this.bundle.LoadAssetsAsync<T>(names);
        }

        public virtual IProgressResult<float, Object[]> LoadAssetsAsync(System.Type type, params string[] names)
        {
            return this.bundle.LoadAssetsAsync(type, names);
        }

        public virtual Object[] LoadAllAssets(System.Type type)
        {
            return this.bundle.LoadAllAssets(type);
        }

        public virtual T[] LoadAllAssets<T>() where T : Object
        {
            return this.bundle.LoadAllAssets<T>();
        }

        public virtual IProgressResult<float, Object[]> LoadAllAssetsAsync(System.Type type)
        {
            return this.bundle.LoadAllAssetsAsync(type);
        }

        public virtual IProgressResult<float, T[]> LoadAllAssetsAsync<T>() where T : Object
        {
            return this.bundle.LoadAllAssetsAsync<T>();
        }

        public virtual UnityEngine.Object[] LoadAssetWithSubAssets(string name, System.Type type)
        {
            return this.bundle.LoadAssetWithSubAssets(name, type);
        }

        public virtual T[] LoadAssetWithSubAssets<T>(string name) where T : UnityEngine.Object
        {
            return this.bundle.LoadAssetWithSubAssets<T>(name);
        }

        public virtual IProgressResult<float, T[]> LoadAssetWithSubAssetsAsync<T>(string name) where T : Object
        {
            return this.bundle.LoadAssetWithSubAssetsAsync<T>(name);
        }

        public virtual IProgressResult<float, UnityEngine.Object[]> LoadAssetWithSubAssetsAsync(string name, System.Type type)
        {
            return this.bundle.LoadAssetWithSubAssetsAsync(name, type);
        }
        #endregion

        #region IDisposable Support
        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                try
                {
                    if (this.bundle != null)
                    {
                        /* Must be released in the main thread  */
                        Executors.RunOnMainThread(() =>
                        {
                            this.bundle.Release();
                            this.bundle = null;
                        });
                    }
                }
                catch (System.Exception)
                {
                }
                finally
                {
                    disposed = true;
                }
            }
        }

        ~InternalBundleWrapper()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            System.GC.SuppressFinalize(this);
        }
        #endregion
    }
}
