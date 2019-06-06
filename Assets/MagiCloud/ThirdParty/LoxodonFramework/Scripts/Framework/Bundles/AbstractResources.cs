using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using Loxodon.Log;
using Loxodon.Framework.Asynchronous;
using Loxodon.Framework.Execution;
using Loxodon.Framework.Utilities;

namespace Loxodon.Framework.Bundles
{
#pragma warning disable 0414, 0219
    public abstract class AbstractResources : IResources
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(AbstractResources));

        protected const float DEFAULT_WEIGHT = 0.8f;
        protected readonly static WaitForSeconds waitForSeconds = new WaitForSeconds(0.1f);

        protected WeakValueDictionary<string, Object> assetCache = new WeakValueDictionary<string, Object>();
        protected IPathInfoParser pathInfoParser;
        protected IBundleManager bundleManager;
        protected bool useWeakCache;

        public AbstractResources(IPathInfoParser pathInfoParser, IBundleManager manager, bool useWeakCache)
        {
            this.pathInfoParser = pathInfoParser;
            this.bundleManager = manager;
            this.useWeakCache = useWeakCache;
        }

        protected virtual void AddCache<T>(string key, T obj) where T : Object
        {
            if (!useWeakCache)
                return;

            this.assetCache[key] = obj;
        }

        protected virtual T GetCache<T>(string key) where T : Object
        {
            try
            {
                if (!useWeakCache)
                    return null;

                Object value;
                if (this.assetCache.TryGetValue(key, out value) && value != null && value is T)
                {
                    //Check if the object is valid because it may have been destroyed.
                    //Unmanaged objects,the weak caches do not accurately track the validity of objects.
                    var name = value.name;
                    return (T)value;
                }
                return null;
            }
            catch (System.Exception)
            {
                //if (log.IsWarnEnabled)
                //    log.WarnFormat("The cache is invalid and the object[{0}] has been destroyed.", key);
                return null;
            }
        }

        public virtual IBundleManager BundleManager { get { return this.bundleManager; } }

        public virtual IPathInfoParser PathInfoParser { get { return this.pathInfoParser; } }

        #region IBundleManager Support
        public virtual IBundle GetBundle(string bundleName)
        {
            return this.bundleManager.GetBundle(bundleName);
        }

        public virtual IProgressResult<float, IBundle> LoadBundle(string bundleName)
        {
            return this.bundleManager.LoadBundle(bundleName);
        }

        public virtual IProgressResult<float, IBundle> LoadBundle(string bundleName, int priority)
        {
            return this.bundleManager.LoadBundle(bundleName, priority);
        }

        public virtual IProgressResult<float, IBundle[]> LoadBundle(params string[] bundleNames)
        {
            return this.bundleManager.LoadBundle(bundleNames);
        }

        public virtual IProgressResult<float, IBundle[]> LoadBundle(string[] bundleNames, int priority)
        {
            return this.bundleManager.LoadBundle(bundleNames, priority);
        }

        #endregion

        #region IResource Support
        public virtual byte[] LoadData(string path)
        {
            TextAsset text = this.LoadAsset<TextAsset>(path);
            if (text != null)
                return text.bytes;
            return null;
        }

        public virtual string LoadText(string path)
        {
            TextAsset text = this.LoadAsset<TextAsset>(path);
            if (text != null)
                return text.text;
            return null;
        }

        public virtual Object LoadAsset(string path)
        {
            return this.LoadAsset<Object>(path);
        }

        public virtual Object LoadAsset(string path, System.Type type)
        {
            if (string.IsNullOrEmpty(path))
                throw new System.ArgumentNullException("path", "The path is null or empty!");

            if (type == null)
                throw new System.ArgumentNullException("type");

            AssetPathInfo pathInfo = this.pathInfoParser.Parse(path);
            if (pathInfo == null)
                throw new System.Exception(string.Format("Not found the AssetBundle or parses the path info '{0}' failure.", path));

            Object asset = this.GetCache<Object>(path);
            if (asset != null)
                return asset;

            using (IBundle bundle = this.GetBundle(pathInfo.BundleName))
            {
                if (bundle == null)
                {
                    if (log.IsWarnEnabled)
                        log.WarnFormat("The AssetBundle of the current asset(path:{0}) is not loaded, please load the AssetBundle first.", path);
                    return null;
                }

                asset = bundle.LoadAsset(pathInfo.AssetName, type);
                if (asset != null)
                    this.AddCache(path, asset);

                return asset;
            }
        }

        public virtual T LoadAsset<T>(string path) where T : Object
        {
            if (string.IsNullOrEmpty(path))
                throw new System.ArgumentNullException("path", "The path is null or empty!");

            AssetPathInfo pathInfo = this.pathInfoParser.Parse(path);
            if (pathInfo == null)
                throw new System.Exception(string.Format("Not found the AssetBundle or parses the path info '{0}' failure.", path));

            T asset = this.GetCache<T>(path);
            if (asset != null)
                return asset;

            using (IBundle bundle = this.GetBundle(pathInfo.BundleName))
            {
                if (bundle == null)
                {
                    if (log.IsWarnEnabled)
                        log.WarnFormat("The AssetBundle of the current asset(path:{0}) is not loaded, please load the AssetBundle first.", path);
                    return null;
                }

                asset = bundle.LoadAsset<T>(pathInfo.AssetName);
                if (asset != null)
                    this.AddCache<T>(path, asset);

                return asset;
            }
        }

        public virtual IProgressResult<float, Object> LoadAssetAsync(string path)
        {
            return this.LoadAssetAsync<Object>(path);
        }

        public virtual IProgressResult<float, Object> LoadAssetAsync(string path, System.Type type)
        {
            try
            {
                if (string.IsNullOrEmpty(path))
                    throw new System.ArgumentNullException("path", "The path is null or empty!");

                if (type == null)
                    throw new System.ArgumentNullException("type");

                ProgressResult<float, Object> result = new ProgressResult<float, Object>();
                AssetPathInfo pathInfo = this.pathInfoParser.Parse(path);
                if (pathInfo == null)
                    throw new System.Exception(string.Format("Not found the AssetBundle or parses the path info '{0}' failure.", path));

                Object asset = this.GetCache<Object>(path);
                if (asset != null)
                {
                    result.UpdateProgress(1f);
                    result.SetResult(asset);
                    return result;
                }

                IProgressResult<float, IBundle> bundleResult = this.LoadBundle(pathInfo.BundleName);
                float weight = bundleResult.IsDone ? 0f : DEFAULT_WEIGHT;
                bundleResult.Callbackable().OnProgressCallback(p => result.UpdateProgress(p * weight));
                bundleResult.Callbackable().OnCallback((r) =>
                {
                    if (r.Exception != null)
                    {
                        result.SetException(r.Exception);
                        return;
                    }

                    using (IBundle bundle = r.Result)
                    {
                        IProgressResult<float, Object> assetResult = bundle.LoadAssetAsync(pathInfo.AssetName, type);
                        assetResult.Callbackable().OnProgressCallback(p => result.UpdateProgress(weight + (1f - weight) * p));
                        assetResult.Callbackable().OnCallback((ar) =>
                        {
                            if (ar.Exception != null)
                                result.SetException(ar.Exception);
                            else
                            {
                                result.SetResult(ar.Result);
                                this.AddCache<Object>(path, ar.Result);
                            }
                        });
                    }
                });
                return result;
            }
            catch (System.Exception e)
            {
                return new ImmutableProgressResult<float, Object>(e, 0f);
            }
        }

        public virtual IProgressResult<float, T> LoadAssetAsync<T>(string path) where T : Object
        {
            try
            {
                if (string.IsNullOrEmpty(path))
                    throw new System.ArgumentNullException("path", "The path is null or empty!");

                ProgressResult<float, T> result = new ProgressResult<float, T>();
                AssetPathInfo pathInfo = this.pathInfoParser.Parse(path);
                if (pathInfo == null)
                    throw new System.Exception(string.Format("Not found the AssetBundle or parses the path info '{0}' failure.", path));

                T asset = this.GetCache<T>(path);
                if (asset != null)
                {
                    result.UpdateProgress(1f);
                    result.SetResult(asset);
                    return result;
                }

                IProgressResult<float, IBundle> bundleResult = this.LoadBundle(pathInfo.BundleName);
                float weight = bundleResult.IsDone ? 0f : DEFAULT_WEIGHT;
                bundleResult.Callbackable().OnProgressCallback(p => result.UpdateProgress(p * weight));
                bundleResult.Callbackable().OnCallback((r) =>
                {
                    if (r.Exception != null)
                    {
                        result.SetException(r.Exception);
                        return;
                    }

                    using (IBundle bundle = r.Result)
                    {
                        IProgressResult<float, T> assetResult = bundle.LoadAssetAsync<T>(pathInfo.AssetName);
                        assetResult.Callbackable().OnProgressCallback(p => result.UpdateProgress(weight + (1f - weight) * p));
                        assetResult.Callbackable().OnCallback((ar) =>
                        {
                            if (ar.Exception != null)
                                result.SetException(ar.Exception);
                            else
                            {
                                result.SetResult(ar.Result);
                                this.AddCache<T>(path, ar.Result);
                            }
                        });
                    }
                });
                return result;
            }
            catch (System.Exception e)
            {
                return new ImmutableProgressResult<float, T>(e, 0f);
            }
        }

        public virtual Object[] LoadAssets(params string[] paths)
        {
            return this.LoadAssets<Object>(paths);
        }

        public virtual Object[] LoadAssets(System.Type type, params string[] paths)
        {
            if (paths == null || paths.Length <= 0)
                throw new System.ArgumentNullException("paths", "The paths is null or empty!");

            if (type == null)
                throw new System.ArgumentNullException("type");

            List<Object> list = new List<Object>();
            foreach (string path in paths)
            {
                Object r = this.LoadAsset(path, type);
                if (r != null)
                    list.Add(r);
            }
            return list.ToArray();
        }

        public virtual T[] LoadAssets<T>(params string[] paths) where T : Object
        {
            if (paths == null || paths.Length <= 0)
                throw new System.ArgumentNullException("paths", "The paths is null or empty!");

            List<T> list = new List<T>();
            foreach (string path in paths)
            {
                T r = this.LoadAsset<T>(path);
                if (r != null)
                    list.Add(r);
            }
            return list.ToArray();
        }

        public virtual IProgressResult<float, Object[]> LoadAssetsAsync(params string[] paths)
        {
            return this.LoadAssetsAsync<Object>(paths);
        }

        public virtual IProgressResult<float, Object[]> LoadAssetsAsync(System.Type type, params string[] paths)
        {
            try
            {
                if (paths == null || paths.Length <= 0)
                    throw new System.ArgumentNullException("paths", "The paths is null or empty!");

                if (type == null)
                    throw new System.ArgumentNullException("type");

                return Executors.RunOnCoroutine<float, Object[]>((promise) => DoLoadAssetsAsync(promise, type, paths));
            }
            catch (System.Exception e)
            {
                return new ImmutableProgressResult<float, Object[]>(e, 0f);
            }
        }

        protected virtual IEnumerator DoLoadAssetsAsync(IProgressPromise<float, Object[]> promise, System.Type type, params string[] paths)
        {
            List<Object> results = new List<Object>();
            Dictionary<string, List<string>> groups = new Dictionary<string, List<string>>();
            List<string> bundleNames = new List<string>();
            foreach (string path in paths)
            {
                AssetPathInfo pathInfo = this.pathInfoParser.Parse(path);
                if (pathInfo == null || pathInfo.BundleName == null)
                {
                    if (log.IsWarnEnabled)
                        log.WarnFormat("Not found the AssetBundle or parses the path info '{0}' failure.", path);
                    continue;
                }

                var asset = this.GetCache<Object>(path);
                if (asset != null)
                {
                    results.Add(asset);
                    continue;
                }

                List<string> list = null;
                if (!groups.TryGetValue(pathInfo.BundleName, out list))
                {
                    list = new List<string>();
                    groups.Add(pathInfo.BundleName, list);
                    bundleNames.Add(pathInfo.BundleName);
                }

                if (!list.Contains(pathInfo.AssetName))
                    list.Add(pathInfo.AssetName);
            }

            if (bundleNames.Count <= 0)
            {
                promise.UpdateProgress(1f);
                promise.SetResult(results.ToArray());
                yield break;
            }

            IProgressResult<float, IBundle[]> bundleResult = this.LoadBundle(bundleNames.ToArray(), 0);
            float weight = bundleResult.IsDone ? 0f : DEFAULT_WEIGHT;
            bundleResult.Callbackable().OnProgressCallback(p => promise.UpdateProgress(weight * p));

            yield return bundleResult.WaitForDone();

            if (bundleResult.Exception != null)
            {
                promise.SetException(bundleResult.Exception);
                yield break;
            }

            Dictionary<string, IProgressResult<float, Object[]>> assetResults = new Dictionary<string, IProgressResult<float, Object[]>>();
            IBundle[] bundles = bundleResult.Result;
            for (int i = 0; i < bundles.Length; i++)
            {
                using (IBundle bundle = bundles[i])
                {
                    if (!groups.ContainsKey(bundle.Name))
                        continue;

                    List<string> assetNames = groups[bundle.Name];
                    if (assetNames == null || assetNames.Count < 0)
                        continue;

                    IProgressResult<float, Object[]> assetResult = bundle.LoadAssetsAsync(type, assetNames.ToArray());
                    assetResult.Callbackable().OnCallback(ar =>
                    {
                        if (ar.Exception != null)
                            return;

                        results.AddRange(ar.Result);
                    });
                    assetResults.Add(bundle.Name, assetResult);
                }
            }

            if (assetResults.Count < 0)
            {
                promise.UpdateProgress(1f);
                promise.SetResult(results.ToArray());
                yield break;
            }

            bool finished = false;
            float progress = 0f;
            int assetCount = assetResults.Count;
            do
            {
                yield return waitForSeconds;

                finished = true;
                progress = 0f;

                var assetEnumerator = assetResults.GetEnumerator();
                while (assetEnumerator.MoveNext())
                {
                    var kv = assetEnumerator.Current;
                    var assetResult = kv.Value;
                    if (!assetResult.IsDone)
                        finished = false;

                    progress += (1f - weight) * assetResult.Progress / assetCount;
                }

                promise.UpdateProgress(weight + progress);
            } while (!finished);

            promise.UpdateProgress(1f);
            promise.SetResult(results.ToArray());
        }

        public virtual IProgressResult<float, T[]> LoadAssetsAsync<T>(params string[] paths) where T : Object
        {
            try
            {
                if (paths == null || paths.Length <= 0)
                    throw new System.ArgumentNullException("paths", "The paths is null or empty!");

                return Executors.RunOnCoroutine<float, T[]>((promise) => DoLoadAssetsAsync(promise, paths));
            }
            catch (System.Exception e)
            {
                return new ImmutableProgressResult<float, T[]>(e, 0f);
            }
        }

        protected virtual IEnumerator DoLoadAssetsAsync<T>(IProgressPromise<float, T[]> promise, params string[] paths) where T : Object
        {
            List<T> results = new List<T>();
            Dictionary<string, List<string>> groups = new Dictionary<string, List<string>>();
            List<string> bundleNames = new List<string>();
            foreach (string path in paths)
            {
                AssetPathInfo pathInfo = this.pathInfoParser.Parse(path);
                if (pathInfo == null || pathInfo.BundleName == null)
                {
                    if (log.IsWarnEnabled)
                        log.WarnFormat("Not found the AssetBundle or parses the path info '{0}' failure.", path);
                    continue;
                }

                var asset = this.GetCache<T>(path);
                if (asset != null)
                {
                    results.Add(asset);
                    continue;
                }

                List<string> list = null;
                if (!groups.TryGetValue(pathInfo.BundleName, out list))
                {
                    list = new List<string>();
                    groups.Add(pathInfo.BundleName, list);
                    bundleNames.Add(pathInfo.BundleName);
                }

                if (!list.Contains(pathInfo.AssetName))
                    list.Add(pathInfo.AssetName);
            }

            if (bundleNames.Count <= 0)
            {
                promise.UpdateProgress(1f);
                promise.SetResult(results.ToArray());
                yield break;
            }

            IProgressResult<float, IBundle[]> bundleResult = this.LoadBundle(bundleNames.ToArray(), 0);
            float weight = bundleResult.IsDone ? 0f : DEFAULT_WEIGHT;
            bundleResult.Callbackable().OnProgressCallback(p => promise.UpdateProgress(weight * p));

            yield return bundleResult.WaitForDone();

            if (bundleResult.Exception != null)
            {
                promise.SetException(bundleResult.Exception);
                yield break;
            }

            Dictionary<string, IProgressResult<float, T[]>> assetResults = new Dictionary<string, IProgressResult<float, T[]>>();
            IBundle[] bundles = bundleResult.Result;
            for (int i = 0; i < bundles.Length; i++)
            {
                using (IBundle bundle = bundles[i])
                {
                    if (!groups.ContainsKey(bundle.Name))
                        continue;

                    List<string> assetNames = groups[bundle.Name];
                    if (assetNames == null || assetNames.Count < 0)
                        continue;

                    IProgressResult<float, T[]> assetResult = bundle.LoadAssetsAsync<T>(assetNames.ToArray());
                    assetResult.Callbackable().OnCallback(ar =>
                    {
                        if (ar.Exception != null)
                            return;

                        results.AddRange(ar.Result);
                    });
                    assetResults.Add(bundle.Name, assetResult);
                }
            }

            if (assetResults.Count < 0)
            {
                promise.UpdateProgress(1f);
                promise.SetResult(results.ToArray());
                yield break;
            }

            bool finished = false;
            float progress = 0f;
            int assetCount = assetResults.Count;
            do
            {
                yield return waitForSeconds;

                finished = true;
                progress = 0f;

                var assetEnumerator = assetResults.GetEnumerator();
                while (assetEnumerator.MoveNext())
                {
                    var kv = assetEnumerator.Current;
                    var assetResult = kv.Value;
                    if (!assetResult.IsDone)
                        finished = false;

                    progress += (1f - weight) * assetResult.Progress / assetCount;
                }

                promise.UpdateProgress(weight + progress);
            } while (!finished);

            promise.UpdateProgress(1f);
            promise.SetResult(results.ToArray());
        }

#if SUPPORT_LOADALL
        public virtual Object[] LoadAllAssets(string bundleName)
        {
            return this.LoadAllAssets<Object>(bundleName);
        }

        public virtual Object[] LoadAllAssets(string bundleName, System.Type type)
        {
            if (bundleName == null)
                throw new System.ArgumentNullException("bundleName");

            if (type == null)
                throw new System.ArgumentNullException("type");

            using (IBundle bundle = this.GetBundle(bundleName))
            {
                if (bundle == null)
                    return null;
                return bundle.LoadAllAssets(type);
            }
        }

        public virtual T[] LoadAllAssets<T>(string bundleName) where T : Object
        {
            if (bundleName == null)
                throw new System.ArgumentNullException("bundleName");

            using (IBundle bundle = this.GetBundle(bundleName))
            {
                if (bundle == null)
                    return null;
                return bundle.LoadAllAssets<T>();
            }
        }

        public virtual IProgressResult<float, Object[]> LoadAllAssetsAsync(string bundleName)
        {
            return this.LoadAllAssetsAsync<Object>(bundleName);
        }

        public virtual IProgressResult<float, Object[]> LoadAllAssetsAsync(string bundleName, System.Type type)
        {
            try
            {
                if (bundleName == null)
                    throw new System.ArgumentNullException("bundleName");

                if (type == null)
                    throw new System.ArgumentNullException("type");

                ProgressResult<float, Object[]> result = new ProgressResult<float, Object[]>();
                IProgressResult<float, IBundle> bundleResult = this.LoadBundle(bundleName);
                float weight = bundleResult.IsDone ? 0f : DEFAULT_WEIGHT;
                bundleResult.Callbackable().OnProgressCallback(p => result.UpdateProgress(p * weight));
                bundleResult.Callbackable().OnCallback((r) =>
                {
                    if (r.Exception != null)
                    {
                        result.SetException(r.Exception);
                        return;
                    }

                    using (IBundle bundle = r.Result)
                    {
                        IProgressResult<float, Object[]> assetResult = bundle.LoadAllAssetsAsync(type);
                        assetResult.Callbackable().OnProgressCallback(p => result.UpdateProgress(weight + (1f - weight) * p));
                        assetResult.Callbackable().OnCallback((ar) =>
                        {
                            if (ar.Exception != null)
                                result.SetException(ar.Exception);
                            else {
                                result.SetResult(ar.Result);
                            }
                        });
                    }
                });
                return result;
            }
            catch (System.Exception e)
            {
                return new ImmutableProgressResult<float, Object[]>(e, 0f);
            }
        }

        public virtual IProgressResult<float, T[]> LoadAllAssetsAsync<T>(string bundleName) where T : Object
        {
            try
            {
                if (bundleName == null)
                    throw new System.ArgumentNullException("bundleName");

                ProgressResult<float, T[]> result = new ProgressResult<float, T[]>();
                IProgressResult<float, IBundle> bundleResult = this.LoadBundle(bundleName);
                float weight = bundleResult.IsDone ? 0f : DEFAULT_WEIGHT;
                bundleResult.Callbackable().OnProgressCallback(p => result.UpdateProgress(p * weight));
                bundleResult.Callbackable().OnCallback(r =>
                {
                    if (r.Exception != null)
                    {
                        result.SetException(r.Exception);
                        return;
                    }

                    using (IBundle bundle = r.Result)
                    {
                        IProgressResult<float, T[]> assetResult = bundle.LoadAllAssetsAsync<T>();
                        assetResult.Callbackable().OnProgressCallback(p => result.UpdateProgress(weight + (1f - weight) * p));
                        assetResult.Callbackable().OnCallback((ar) =>
                        {
                            if (ar.Exception != null)
                                result.SetException(ar.Exception);
                            else {
                                result.SetResult(ar.Result);
                            }
                        });
                    }
                });
                return result;
            }
            catch (System.Exception e)
            {
                return new ImmutableProgressResult<float, T[]>(e, 0f);
            }
        }
#endif
        public virtual ISceneLoadingResult<Scene> LoadSceneAsync(string path, LoadSceneMode mode = LoadSceneMode.Single)
        {
            SceneLoadingResult<Scene> result = new SceneLoadingResult<Scene>();
            try
            {
                if (string.IsNullOrEmpty(path))
                    throw new System.ArgumentNullException("path", "The path is null or empty!");

                InterceptableEnumerator enumerator = new InterceptableEnumerator(DoLoadSceneAsync(result, path, mode));
                enumerator.RegisterCatchBlock(e =>
                {
                    result.SetException(e);
                    if (log.IsErrorEnabled)
                        log.Error(e);
                });
                enumerator.RegisterFinallyBlock(() =>
                {
                    if (!result.IsDone)
                        result.SetException(new System.Exception("No value given the Result"));
                });
                Executors.RunOnCoroutineNoReturn(enumerator);
            }
            catch (System.Exception e)
            {
                result.Progress = 0f;
                result.SetException(e);
            }
            return result;
        }

        protected abstract IEnumerator DoLoadSceneAsync(ISceneLoadingPromise<Scene> promise, string path, LoadSceneMode mode = LoadSceneMode.Single);

        #endregion
    }
}
