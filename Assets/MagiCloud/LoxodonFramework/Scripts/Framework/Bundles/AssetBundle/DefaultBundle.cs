using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Loxodon.Framework.Asynchronous;
using Loxodon.Framework.Execution;

namespace Loxodon.Framework.Bundles
{
    public class DefaultBundle : IBundle
    {
        private const string ASSETS = "Assets/";
        private const float TIME_PROGRESS_WEIGHT = 0.2f;

        private Dictionary<string, AssetBundleRequest> requestCache;
        private BundleInfo bundleInfo;
        private List<BundleLoader> loaders;

        private int refCount = 0;
        private object _lock = new object();
        private int priority = int.MinValue;
        private float startTime = 0f;
        private AssetBundle assetBundle;
        private IProgressResult<float, IBundle> result;
        private BundleManager manager;

        public DefaultBundle(BundleInfo bundleInfo, BundleManager manager)
        {
            if (bundleInfo == null)
                throw new System.ArgumentNullException("bundleInfo");

            this.bundleInfo = bundleInfo;
            this.manager = manager;
            this.manager.AddBundle(this);
            this.requestCache = new Dictionary<string, AssetBundleRequest>();
            this.loaders = new List<BundleLoader>();
        }

        public virtual string Name { get { return this.bundleInfo.Name; } }

        public virtual BundleInfo BundleInfo { get { return this.bundleInfo; } }

        public virtual bool IsReady { get { return this.IsDone && this.assetBundle != null; } }

        //public virtual IBundle Bundle { get { return this.IsDone && this.assetBundle != null ? new InternalBundleWrapper(this) : null; } }

        public virtual int Priority
        {
            get { return this.priority; }
            set
            {
                if (this.priority > value)
                    return;

                this.priority = value;
                for (int i = 0; i < loaders.Count; i++)
                    loaders[i].Priority = this.priority;
            }
        }

        protected virtual bool IsDone { get { return this.result != null ? this.result.IsDone : false; } }

        protected virtual BundleManager BundleManager { get { return this.manager; } }

        protected IEnumerator Wrap(IEnumerator task)
        {
            this.Retain();
            InterceptableEnumerator enumerator = new InterceptableEnumerator(task);
            enumerator.RegisterFinallyBlock(() =>
            {
                this.Release();
            });
            return enumerator;
        }

        public void Retain()
        {
            lock (_lock)
            {
                if (this.disposed)
                    throw new System.ObjectDisposedException(this.Name);

                this.refCount++;
            }
        }

        public void Release()
        {
            lock (_lock)
            {
                if (this.disposed)
                    return;

                this.refCount--;
                if (this.refCount <= 0)
                    this.Dispose();
            }
        }

        public virtual IProgressResult<float, IBundle> Load()
        {
            if (this.result == null || this.result.Exception != null)
                this.result = this.Execute<float, IBundle>(promise => Wrap(DoLoadBundleAndDependencies(promise)));

            return this.result;

            //ProgressResult<float, IBundle> resultCopy = new ProgressResult<float, IBundle>();
            //this.result.Callbackable().OnProgressCallback(p => resultCopy.UpdateProgress(p));
            //this.result.Callbackable().OnCallback((r) =>
            //{
            //    if (r.Exception != null)
            //        resultCopy.SetException(r.Exception);
            //    else
            //        resultCopy.SetResult(new InternalBundleWrapper(this));
            //});
            //return resultCopy;
        }

        protected virtual IEnumerator DoLoadBundleAndDependencies(IProgressPromise<float, IBundle> promise)
        {
            this.startTime = Time.realtimeSinceStartup;
            List<IProgressResult<float, AssetBundle>> results = new List<IProgressResult<float, AssetBundle>>();

            BundleLoader currLoader = manager.GetOrCreateBundleLoader(this.BundleInfo, this.Priority);
            currLoader.Retain();
            loaders.Add(currLoader);

            IProgressResult<float, AssetBundle> currResult = currLoader.LoadAssetBundle();

            if (!currResult.IsDone)
                results.Add(currResult);

            var dependencies = manager.GetOrCreateDependencies(this.BundleInfo, true, this.Priority);
            for (int i = 0; i < dependencies.Count; i++)
            {
                var dependency = dependencies[i];
                dependency.Retain();
                this.loaders.Add(dependency);

                var result = dependency.LoadAssetBundle();
                if (!result.IsDone)
                    results.Add(result);
            }

            bool finished = false;
            float progress = 0f;
            float timeProgress = 0f;
            int count = results.Count;
            while (!finished && count > 0)
            {
                yield return null;

                progress = 0f;
                finished = true;
                for (int i = 0; i < count; i++)
                {
                    var result = results[i];
                    if (!result.IsDone)
                        finished = false;

                    progress += result.Progress;
                }

                timeProgress = TIME_PROGRESS_WEIGHT * Mathf.Atan(Time.realtimeSinceStartup - this.startTime) * 2 / Mathf.PI;
                promise.UpdateProgress(timeProgress + (1.0f - TIME_PROGRESS_WEIGHT) * progress / count);
            }

            promise.UpdateProgress(1f);

            yield return null;

            if (currResult.Exception != null)
                promise.SetException(currResult.Exception);
            else
            {
                this.assetBundle = currResult.Result;
                promise.SetResult(this);
            }
        }

        #region IBundle Support
        protected virtual void Check()
        {
            if (this.disposed)
                throw new System.ObjectDisposedException(this.Name);

            if (!this.IsDone || this.assetBundle == null)
                throw new System.Exception(string.Format("The AssetBundle '{0}' isn't ready.", this.Name));
        }

        protected virtual string GetFullName(string name)
        {
            if (name.StartsWith(ASSETS, System.StringComparison.OrdinalIgnoreCase) || name.IndexOf("/") < 0)
                return name;
            return string.Format("{0}{1}", ASSETS, name);
        }

        protected IProgressResult<TProgress, TResult> Execute<TProgress, TResult>(System.Func<IProgressPromise<TProgress, TResult>, IEnumerator> func)
        {
            ProgressResult<TProgress, TResult> result = new ProgressResult<TProgress, TResult>();
            Executors.RunOnCoroutine(func(result), result);
            return result;
        }

        public virtual Object LoadAsset(string name, System.Type type)
        {
            this.Check();

            if (string.IsNullOrEmpty(name))
                throw new System.ArgumentNullException("name", "The name is null or empty!");

            if (type == null)
                throw new System.ArgumentNullException("type");

            var fullName = GetFullName(name);
            return this.assetBundle.LoadAsset<Object>(fullName);
        }

        public virtual T LoadAsset<T>(string name) where T : Object
        {
            this.Check();

            if (string.IsNullOrEmpty(name))
                throw new System.ArgumentNullException("name", "The name is null or empty!");

            var fullName = GetFullName(name);
            return this.assetBundle.LoadAsset<T>(fullName);
        }

        protected string Key(string name, System.Type type)
        {
            return string.Format("{0}-{1}", name, type);
        }

        protected string Key(string name, System.Type type, string flag)
        {
            return string.Format("{0}-{1}-{2}", name, type, flag);
        }

        public virtual IProgressResult<float, Object> LoadAssetAsync(string name, System.Type type)
        {
            try
            {
                this.Check();

                if (string.IsNullOrEmpty(name))
                    throw new System.ArgumentNullException("name", "The name is null or empty!");

                if (type == null)
                    throw new System.ArgumentNullException("type");

                return this.Execute<float, Object>(promise => Wrap(DoLoadAssetAsync(promise, name, type)));
            }
            catch (System.Exception e)
            {
                return new ImmutableProgressResult<float, Object>(e, 0f);
            }
        }

        protected virtual IEnumerator DoLoadAssetAsync(IProgressPromise<float, Object> promise, string name, System.Type type)
        {
            string key = Key(name, type);
            AssetBundleRequest request;
            if (!this.requestCache.TryGetValue(key, out request))
            {
                var fullName = GetFullName(name);
                request = this.assetBundle.LoadAssetAsync(fullName, type);
                this.requestCache.Add(key, request);
            }

            while (!request.isDone)
            {
                promise.UpdateProgress(request.progress);
                yield return null;
            }

            this.requestCache.Remove(key);

            Object asset = request.asset;
            if (asset == null)
            {
                promise.SetException(new System.Exception(string.Format("Not found the asset {0}", name)));
                yield break;
            }
            promise.UpdateProgress(1f);
            promise.SetResult(asset);
        }

        public virtual IProgressResult<float, T> LoadAssetAsync<T>(string name) where T : Object
        {
            try
            {
                this.Check();

                if (string.IsNullOrEmpty(name))
                    throw new System.ArgumentNullException("name", "The name is null or empty!");

                return this.Execute<float, T>(promise => Wrap(DoLoadAssetAsync<T>(promise, name)));
            }
            catch (System.Exception e)
            {
                return new ImmutableProgressResult<float, T>(e, 0f);
            }
        }

        protected virtual IEnumerator DoLoadAssetAsync<T>(IProgressPromise<float, T> promise, string name) where T : Object
        {
            string key = Key(name, typeof(T));
            AssetBundleRequest request;
            if (!this.requestCache.TryGetValue(key, out request))
            {
                var fullName = GetFullName(name);
                request = this.assetBundle.LoadAssetAsync<T>(fullName);
                this.requestCache.Add(key, request);
            }

            while (!request.isDone)
            {
                promise.UpdateProgress(request.progress);
                yield return null;
            }

            this.requestCache.Remove(key);

            Object asset = request.asset;
            if (asset == null)
            {
                promise.SetException(new System.Exception(string.Format("Not found the asset '{0}'.", name)));
                yield break;
            }

            promise.UpdateProgress(1f);
            promise.SetResult(asset);
        }

        public virtual IProgressResult<float, Object[]> LoadAssetsAsync(System.Type type, params string[] names)
        {
            try
            {
                this.Check();

                if (names == null || names.Length <= 0)
                    new System.ArgumentNullException("names", "The names is null or empty!");

                if (type == null)
                    throw new System.ArgumentNullException("type");

                return this.Execute<float, Object[]>(promise => Wrap(DoLoadAssetsAsync(promise, type, names)));
            }
            catch (System.Exception e)
            {
                return new ImmutableProgressResult<float, Object[]>(e, 0f);
            }
        }

        protected virtual IEnumerator DoLoadAssetsAsync(IProgressPromise<float, Object[]> promise, System.Type type, params string[] names)
        {
            List<Object> results = new List<Object>();
            int count = names.Length;
            float progress = 0f;
            foreach (string name in names)
            {
                var fullName = GetFullName(name);
                AssetBundleRequest request = this.assetBundle.LoadAssetAsync(fullName, type);
                while (!request.isDone)
                {
                    promise.UpdateProgress(progress + request.progress / count);
                    yield return null;
                }
                progress += 1f / count;
                Object asset = request.asset;
                if (asset != null)
                    results.Add(asset);
            }

            promise.UpdateProgress(1f);
            promise.SetResult(results.ToArray());
        }

        public virtual IProgressResult<float, T[]> LoadAssetsAsync<T>(params string[] names) where T : Object
        {
            try
            {
                this.Check();

                if (names == null || names.Length <= 0)
                    new System.ArgumentNullException("names", "The names is null or empty!");

                return this.Execute<float, T[]>(promise => Wrap(DoLoadAssetsAsync<T>(promise, names)));
            }
            catch (System.Exception e)
            {
                return new ImmutableProgressResult<float, T[]>(e, 0f);
            }
        }

        protected virtual IEnumerator DoLoadAssetsAsync<T>(IProgressPromise<float, T[]> promise, params string[] names) where T : Object
        {
            List<T> results = new List<T>();
            int count = names.Length;
            float progress = 0f;
            foreach (string name in names)
            {
                var fullName = GetFullName(name);
                AssetBundleRequest request = this.assetBundle.LoadAssetAsync<T>(fullName);
                while (!request.isDone)
                {
                    promise.UpdateProgress(progress + request.progress / count);
                    yield return null;
                }
                progress += 1f / count;
                T asset = (T)request.asset;
                if (asset != null)
                    results.Add(asset);
            }

            promise.UpdateProgress(1f);
            promise.SetResult(results.ToArray());
        }

        public virtual Object[] LoadAllAssets(System.Type type)
        {
            this.Check();

            if (type == null)
                throw new System.ArgumentNullException("type");

            return this.assetBundle.LoadAllAssets(type);
        }

        public virtual T[] LoadAllAssets<T>() where T : Object
        {
            this.Check();
            return this.assetBundle.LoadAllAssets<T>();
        }

        public virtual IProgressResult<float, Object[]> LoadAllAssetsAsync(System.Type type)
        {
            try
            {
                this.Check();

                if (type == null)
                    throw new System.ArgumentNullException("type");

                return this.Execute<float, Object[]>(promise => Wrap(DoLoadAllAssetsAsync(promise, type)));
            }
            catch (System.Exception e)
            {
                return new ImmutableProgressResult<float, Object[]>(e, 0f);
            }
        }

        protected virtual IEnumerator DoLoadAllAssetsAsync(IProgressPromise<float, Object[]> promise, System.Type type)
        {
            string key = Key("_ALL", type);
            AssetBundleRequest request;
            if (!this.requestCache.TryGetValue(key, out request))
            {
                request = this.assetBundle.LoadAllAssetsAsync(type);
                this.requestCache.Add(key, request);
            }

            while (!request.isDone)
            {
                promise.UpdateProgress(request.progress);
                yield return null;
            }

            this.requestCache.Remove(key);
            promise.UpdateProgress(1f);
            promise.SetResult(request.allAssets);
        }

        public virtual IProgressResult<float, T[]> LoadAllAssetsAsync<T>() where T : Object
        {
            try
            {
                this.Check();
                return this.Execute<float, T[]>(promise => Wrap(DoLoadAllAssetsAsync<T>(promise)));
            }
            catch (System.Exception e)
            {
                return new ImmutableProgressResult<float, T[]>(e, 0f);
            }
        }

        protected virtual IEnumerator DoLoadAllAssetsAsync<T>(IProgressPromise<float, T[]> promise) where T : Object
        {
            string key = Key("_ALL", typeof(T));
            AssetBundleRequest request;
            if (!this.requestCache.TryGetValue(key, out request))
            {
                request = this.assetBundle.LoadAllAssetsAsync<T>();
                this.requestCache.Add(key, request);
            }

            while (!request.isDone)
            {
                promise.UpdateProgress(request.progress);
                yield return null;
            }

            this.requestCache.Remove(key);

            var all = request.allAssets;
            T[] assets = new T[all.Length];
            for (int i = 0; i < all.Length; i++)
            {
                assets[i] = ((T)all[i]);
            }

            promise.UpdateProgress(1f);
            promise.SetResult(assets);
        }

        public virtual UnityEngine.Object[] LoadAssetWithSubAssets(string name, System.Type type)
        {
            this.Check();

            if (string.IsNullOrEmpty(name))
                throw new System.ArgumentNullException("name", "The name is null or empty!");

            if (type == null)
                throw new System.ArgumentNullException("type");

            var fullName = GetFullName(name);
            return this.assetBundle.LoadAssetWithSubAssets(fullName, type);
        }

        public virtual T[] LoadAssetWithSubAssets<T>(string name) where T : UnityEngine.Object
        {
            this.Check();

            if (string.IsNullOrEmpty(name))
                throw new System.ArgumentNullException("name", "The name is null or empty!");

            var fullName = GetFullName(name);
            return this.assetBundle.LoadAssetWithSubAssets<T>(fullName);
        }

        public virtual IProgressResult<float, T[]> LoadAssetWithSubAssetsAsync<T>(string name) where T : UnityEngine.Object
        {
            try
            {
                this.Check();

                if (string.IsNullOrEmpty(name))
                    throw new System.ArgumentNullException("name", "The name is null or empty!");

                return this.Execute<float, T[]>(promise => Wrap(DoLoadAssetWithSubAssetsAsync(promise, name)));
            }
            catch (System.Exception e)
            {
                return new ImmutableProgressResult<float, T[]>(e, 0f);
            }
        }

        protected virtual IEnumerator DoLoadAssetWithSubAssetsAsync<T>(IProgressPromise<float, T[]> promise, string name) where T : Object
        {
            string key = Key(name, typeof(T), "SubAssets");
            AssetBundleRequest request;
            if (!this.requestCache.TryGetValue(key, out request))
            {
                var fullName = GetFullName(name);
                request = this.assetBundle.LoadAssetWithSubAssetsAsync(fullName, typeof(T));
                this.requestCache.Add(key, request);
            }

            while (!request.isDone)
            {
                promise.UpdateProgress(request.progress);
                yield return null;
            }

            this.requestCache.Remove(key);

            var all = request.allAssets;
            T[] assets = new T[all.Length];
            for (int i = 0; i < all.Length; i++)
            {
                assets[i] = ((T)all[i]);
            }

            promise.UpdateProgress(1f);
            promise.SetResult(assets);
        }

        public virtual IProgressResult<float, UnityEngine.Object[]> LoadAssetWithSubAssetsAsync(string name, System.Type type)
        {
            try
            {
                this.Check();

                if (string.IsNullOrEmpty(name))
                    throw new System.ArgumentNullException("name", "The name is null or empty!");

                if (type == null)
                    throw new System.ArgumentNullException("type");

                return this.Execute<float, Object[]>(promise => Wrap(DoLoadAssetWithSubAssetsAsync(promise, name, type)));
            }
            catch (System.Exception e)
            {
                return new ImmutableProgressResult<float, Object[]>(e, 0f);
            }
        }

        protected virtual IEnumerator DoLoadAssetWithSubAssetsAsync(IProgressPromise<float, Object[]> promise, string name, System.Type type)
        {
            string key = Key(name, type, "SubAssets");
            AssetBundleRequest request;
            if (!this.requestCache.TryGetValue(key, out request))
            {
                var fullName = GetFullName(name);
                request = this.assetBundle.LoadAssetWithSubAssetsAsync(fullName, type);
                this.requestCache.Add(key, request);
            }

            while (!request.isDone)
            {
                promise.UpdateProgress(request.progress);
                yield return null;
            }

            this.requestCache.Remove(key);

            promise.UpdateProgress(1f);
            promise.SetResult(request.allAssets);
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
                    this.manager.RemoveBundle(this);
                    for (int i = 0; i < this.loaders.Count; i++)
                    {
                        this.loaders[i].Release();
                    }
                    this.loaders.Clear();
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

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
