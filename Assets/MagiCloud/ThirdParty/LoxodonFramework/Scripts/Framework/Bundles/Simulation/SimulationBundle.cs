#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Loxodon.Framework.Asynchronous;
using Loxodon.Framework.Execution;

namespace Loxodon.Framework.Bundles
{
    public class SimulationBundle : IBundle
    {
        private const string ASSETS = "Assets/";
        private string bundleName;
        private string extension;
        private string[] assets;
        private SimulationBundleManager manager;

        public SimulationBundle(string bundleName, string extension, SimulationBundleManager manager)
        {
            this.bundleName = bundleName;
            this.extension = extension;
            this.manager = manager;
            this.manager.AddBundle(this);
        }

        public virtual string Name
        {
            get { return this.bundleName; }
        }

        protected virtual string[] FindFullNames(string name)
        {
            if (name.StartsWith(ASSETS, System.StringComparison.OrdinalIgnoreCase))
                return new string[] { name };

            if (name.IndexOf("/") >= 0)
                return new string[] { string.Format("{0}{1}", ASSETS, name) };

            /* search assets */
            List<string> names = new List<string>();
            string[] assets = this.Assets;
            string text = name.IndexOf(".") < 0 ? name + "." : name;
            for (int i = 0; i < assets.Length; i++)
            {
                if (assets[i].IndexOf(text, System.StringComparison.OrdinalIgnoreCase) > 0)
                    names.Add(assets[i]);
            }
            if (names.Count > 0)
                return names.ToArray();
            return new string[] { name };
        }

        protected virtual string GetBundleNameWhitExtension()
        {
            if (string.IsNullOrEmpty(this.extension))
                return this.bundleName;
            return string.Format("{0}.{1}", this.bundleName, this.extension);
        }

        protected virtual string[] Assets
        {
            get
            {
                if (this.assets == null)
                    this.assets = AssetDatabase.GetAssetPathsFromAssetBundle(this.GetBundleNameWhitExtension());
                return this.assets;
            }
        }

        protected IProgressResult<TProgress, TResult> Execute<TProgress, TResult>(System.Func<IProgressPromise<TProgress, TResult>, IEnumerator> func)
        {
            ProgressResult<TProgress, TResult> result = new ProgressResult<TProgress, TResult>();
            Executors.RunOnCoroutine(func(result), result);
            return result;
        }

        public virtual Object LoadAsset(string name, System.Type type)
        {
            if (string.IsNullOrEmpty(name))
                throw new System.ArgumentNullException("name", "The name is null or empty!");

            if (type == null)
                throw new System.ArgumentNullException("type");

            foreach (string fullName in this.FindFullNames(name))
            {
                var asset = AssetDatabase.LoadAssetAtPath(fullName, type);
                if (asset == null)
                    continue;
                if ((asset is SceneAsset) || (asset is DefaultAsset))
                    continue;
                return asset;
            }
            return null;
        }

        public virtual T LoadAsset<T>(string name) where T : Object
        {
            if (string.IsNullOrEmpty(name))
                throw new System.ArgumentNullException("name", "The name is null or empty!");

            foreach (string fullName in this.FindFullNames(name))
            {
                var asset = AssetDatabase.LoadAssetAtPath<T>(fullName);
                if (asset == null)
                    continue;
                if ((asset is SceneAsset) || (asset is DefaultAsset))
                    continue;
                return asset;
            }
            return null;
        }

        public virtual IProgressResult<float, Object> LoadAssetAsync(string name, System.Type type)
        {
            try
            {
                if (string.IsNullOrEmpty(name))
                    throw new System.ArgumentNullException("name", "The name is null or empty!");

                if (type == null)
                    throw new System.ArgumentNullException("type");

                return new ImmutableProgressResult<float, Object>(LoadAsset(name, type), 1f);
            }
            catch (System.Exception e)
            {
                return new ImmutableProgressResult<float, Object>(e, 0f);
            }
        }

        public virtual IProgressResult<float, T> LoadAssetAsync<T>(string name) where T : Object
        {
            try
            {
                if (string.IsNullOrEmpty(name))
                    throw new System.ArgumentNullException("name", "The name is null or empty!");

                return new ImmutableProgressResult<float, T>(LoadAsset<T>(name), 1f);
            }
            catch (System.Exception e)
            {
                return new ImmutableProgressResult<float, T>(e, 0f);
            }
        }

        public virtual IProgressResult<float, Object[]> LoadAssetsAsync(System.Type type, params string[] names)
        {
            try
            {
                if (names == null || names.Length <= 0)
                    new System.ArgumentNullException("names", "The names is null or empty!");

                if (type == null)
                    throw new System.ArgumentNullException("type");

                return this.Execute<float, Object[]>(promise => DoLoadAssetsAsync(promise, type, names));
            }
            catch (System.Exception e)
            {
                return new ImmutableProgressResult<float, Object[]>(e, 0f);
            }
        }

        protected virtual IEnumerator DoLoadAssetsAsync(IProgressPromise<float, Object[]> promise, System.Type type, params string[] names)
        {
            List<Object> assets = new List<Object>();
            for (int i = 0; i < names.Length; i++)
            {
                var asset = this.LoadAsset(names[i], type);
                if (asset != null)
                    assets.Add(asset);

                promise.UpdateProgress((float)i / names.Length);
                yield return null;
            }

            promise.UpdateProgress(1f);
            promise.SetResult(assets.ToArray());
        }

        public virtual IProgressResult<float, T[]> LoadAssetsAsync<T>(params string[] names) where T : Object
        {
            try
            {
                if (names == null || names.Length <= 0)
                    new System.ArgumentNullException("names", "The names is null or empty!");

                return this.Execute<float, T[]>(promise => DoLoadAssetsAsync<T>(promise, names));
            }
            catch (System.Exception e)
            {
                return new ImmutableProgressResult<float, T[]>(e, 0f);
            }
        }

        protected virtual IEnumerator DoLoadAssetsAsync<T>(IProgressPromise<float, T[]> promise, params string[] names) where T : Object
        {
            List<T> assets = new List<T>();
            for (int i = 0; i < names.Length; i++)
            {
                var asset = this.LoadAsset<T>(names[i]);
                if (asset != null)
                    assets.Add(asset);

                promise.UpdateProgress((float)i / names.Length);
                yield return null;
            }

            promise.UpdateProgress(1f);
            promise.SetResult(assets.ToArray());
        }

        public virtual Object[] LoadAllAssets(System.Type type)
        {
            if (type == null)
                throw new System.ArgumentNullException("type");

            List<Object> assets = new List<Object>();
            string[] fullNames = AssetDatabase.GetAssetPathsFromAssetBundle(this.GetBundleNameWhitExtension());
            foreach (string fullName in fullNames)
            {
                if (fullName.EndsWith(".unity", System.StringComparison.OrdinalIgnoreCase))
                    continue;

                Object[] array = AssetDatabase.LoadAllAssetRepresentationsAtPath(fullName);
                foreach (Object asset in array)
                {
                    if (asset != null && type.IsAssignableFrom(asset.GetType()) && !(asset is SceneAsset) && !(asset is DefaultAsset))
                        assets.Add(asset);
                }
            }
            return assets.ToArray();
        }

        public virtual T[] LoadAllAssets<T>() where T : Object
        {
            List<T> assets = new List<T>();
            string[] fullNames = AssetDatabase.GetAssetPathsFromAssetBundle(this.GetBundleNameWhitExtension());
            foreach (string fullName in fullNames)
            {
                if (fullName.EndsWith(".unity", System.StringComparison.OrdinalIgnoreCase))
                    continue;

                Object[] array = AssetDatabase.LoadAllAssetRepresentationsAtPath(fullName);
                foreach (Object asset in array)
                {
                    if (asset != null && asset is T && !(asset is SceneAsset) && !(asset is DefaultAsset))
                        assets.Add((T)asset);
                }
            }
            return assets.ToArray();
        }

        public virtual IProgressResult<float, Object[]> LoadAllAssetsAsync(System.Type type)
        {
            try
            {
                if (type == null)
                    throw new System.ArgumentNullException("type");

                return this.Execute<float, Object[]>(promise => DoLoadAllAssetsAsync(promise, type));
            }
            catch (System.Exception e)
            {
                return new ImmutableProgressResult<float, Object[]>(e, 0f);
            }
        }

        protected virtual IEnumerator DoLoadAllAssetsAsync(IProgressPromise<float, Object[]> promise, System.Type type)
        {
            List<Object> assets = new List<Object>();
            string[] fullNames = AssetDatabase.GetAssetPathsFromAssetBundle(this.GetBundleNameWhitExtension());
            for (int i = 0; i < fullNames.Length; i++)
            {
                var fullName = fullNames[i];
                promise.UpdateProgress((float)i / fullNames.Length);
                if (fullName.EndsWith(".unity", System.StringComparison.OrdinalIgnoreCase))
                    continue;

                Object[] array = AssetDatabase.LoadAllAssetRepresentationsAtPath(fullName);
                foreach (Object asset in array)
                {
                    if (asset != null && type.IsAssignableFrom(asset.GetType()) && !(asset is SceneAsset) && !(asset is DefaultAsset))
                        assets.Add(asset);
                }

                yield return null;
            }

            promise.UpdateProgress(1f);
            promise.SetResult(assets.ToArray());
        }

        public virtual IProgressResult<float, T[]> LoadAllAssetsAsync<T>() where T : Object
        {
            return this.Execute<float, T[]>(promise => DoLoadAllAssetsAsync(promise));
        }

        protected virtual IEnumerator DoLoadAllAssetsAsync<T>(IProgressPromise<float, T[]> promise) where T : Object
        {
            List<T> assets = new List<T>();
            string[] fullNames = AssetDatabase.GetAssetPathsFromAssetBundle(this.GetBundleNameWhitExtension());
            for (int i = 0; i < fullNames.Length; i++)
            {
                var fullName = fullNames[i];
                promise.UpdateProgress((float)i / fullNames.Length);
                if (fullName.EndsWith(".unity", System.StringComparison.OrdinalIgnoreCase))
                    continue;

                Object[] array = AssetDatabase.LoadAllAssetRepresentationsAtPath(fullName);
                foreach (Object asset in array)
                {
                    if (asset != null && asset is T && !(asset is SceneAsset) && !(asset is DefaultAsset))
                        assets.Add((T)asset);
                }

                yield return null;
            }

            promise.UpdateProgress(1f);
            promise.SetResult(assets.ToArray());
        }
        public virtual UnityEngine.Object[] LoadAssetWithSubAssets(string name, System.Type type)
        {
            throw new System.NotSupportedException();
        }

        public virtual T[] LoadAssetWithSubAssets<T>(string name) where T : UnityEngine.Object
        {
            throw new System.NotSupportedException();
        }

        public virtual IProgressResult<float, T[]> LoadAssetWithSubAssetsAsync<T>(string name) where T : Object
        {
            throw new System.NotSupportedException();
        }

        public virtual IProgressResult<float, UnityEngine.Object[]> LoadAssetWithSubAssetsAsync(string name, System.Type type)
        {
            throw new System.NotSupportedException();
        }

        #region IDisposable Support
        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (this.manager != null)
                {
                    this.manager.RemoveBundle(this);
                    this.manager = null;
                }
                disposed = true;
            }
        }

        ~SimulationBundle()
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
#endif