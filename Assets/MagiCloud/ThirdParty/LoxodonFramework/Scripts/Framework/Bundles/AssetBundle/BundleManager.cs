using System;
using System.Collections;
using System.Collections.Generic;
using Loxodon.Log;
using Loxodon.Framework.Asynchronous;
using Loxodon.Framework.Execution;

namespace Loxodon.Framework.Bundles
{
    public class BundleManager : IBundleManager, IDisposable
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(BundleManager));
        protected const int DEFAULT_PRIORITY = 0;

        protected Dictionary<string, BundleLoader> loaders;
        protected Dictionary<string, DefaultBundle> bundles;
        protected BundleManifest bundleManifest = null;
        protected ILoaderBuilder loaderBuilder;
        protected ITaskExecutor executor;

        public BundleManager(BundleManifest manifest, ILoaderBuilder builder) : this(manifest, builder, null)
        {
        }

        public BundleManager(BundleManifest manifest, ILoaderBuilder builder, ITaskExecutor executor)
        {
            this.bundleManifest = manifest;
            this.loaderBuilder = builder;
            this.executor = executor != null ? executor : new PriorityTaskExecutor();
            this.loaders = new Dictionary<string, BundleLoader>();
            this.bundles = new Dictionary<string, DefaultBundle>();
        }

        public virtual BundleManifest BundleManifest
        {
            get { return this.bundleManifest; }
            set { this.bundleManifest = value; }
        }

        public virtual ILoaderBuilder LoaderBuilder
        {
            get { return this.loaderBuilder; }
            set { this.loaderBuilder = value; }
        }

        public virtual ITaskExecutor Executor
        {
            get { return this.executor; }
        }

        public virtual void AddBundleLoader(BundleLoader loader)
        {
            if (this.loaders == null)
                return;

            this.loaders.Add(loader.BundleInfo.Name, loader);
        }

        public virtual void RemoveBundleLoader(BundleLoader loader)
        {
            if (this.loaders == null)
                return;

            this.loaders.Remove(loader.BundleInfo.Name);
        }

        public virtual BundleLoader GetBundleLoader(string bundleName)
        {
            if (this.loaders == null)
                return null;

            BundleLoader loader;
            if (this.loaders.TryGetValue(bundleName, out loader))
                return loader;
            return null;
        }

        public virtual BundleLoader GetOrCreateBundleLoader(BundleInfo bundleInfo, int priority)
        {
            BundleLoader loader;
            if (this.loaders.TryGetValue(bundleInfo.Name, out loader))
            {
                loader.Priority = priority;
                return loader;
            }

            loader = this.loaderBuilder.Create(this, bundleInfo);
            loader.Priority = priority;
            return loader;
        }

        public virtual List<BundleLoader> GetOrCreateDependencies(BundleInfo bundleInfo, bool recursive, int priority)
        {
            List<BundleLoader> dependencies = new List<BundleLoader>();

            BundleInfo[] bundleInfos = this.BundleManifest.GetDependencies(bundleInfo.Name, recursive);

            foreach (BundleInfo info in bundleInfos)
            {
                BundleLoader loader = this.GetOrCreateBundleLoader(info, priority);
                dependencies.Add(loader);
            }

            return dependencies;
        }

        public virtual void AddBundle(DefaultBundle bundle)
        {
            if (this.bundles == null)
                return;

            this.bundles.Add(bundle.Name, bundle);
        }

        public virtual void RemoveBundle(DefaultBundle bundle)
        {
            if (this.bundles == null)
                return;

            this.bundles.Remove(bundle.Name);
        }

        public virtual DefaultBundle GetOrCreateBundle(BundleInfo bundleInfo, int priority)
        {
            DefaultBundle bundle;
            if (this.bundles.TryGetValue(bundleInfo.Name, out bundle))
                return bundle;

            bundle = new DefaultBundle(bundleInfo, this);
            bundle.Priority = priority;
            return bundle;
        }

        #region IBundleManager Support

        protected virtual string BundleNameNormalize(string bundleName)
        {
            return Path.GetFilePathWithoutExtension(bundleName).ToLower();
        }

        public virtual IBundle GetBundle(string bundleName)
        {
            bundleName = BundleNameNormalize(bundleName);
            BundleInfo info = this.bundleManifest.GetBundleInfo(bundleName);
            if (info == null)
                return null;

            DefaultBundle bundle;
            if (!this.bundles.TryGetValue(info.Name, out bundle))
                return null;

            return bundle.IsReady ? new InternalBundleWrapper(bundle) : null;
        }

        public virtual IProgressResult<float, IBundle> LoadBundle(string bundleName)
        {
            return this.LoadBundle(bundleName, DEFAULT_PRIORITY);
        }

        public virtual IProgressResult<float, IBundle> LoadBundle(string bundleName, int priority)
        {
            try
            {
                if (bundleManifest == null)
                    throw new Exception("The bundleManifest is null!");

                if (string.IsNullOrEmpty(bundleName))
                    throw new ArgumentNullException("The bundleName is empty!");

                bundleName = BundleNameNormalize(bundleName);
                BundleInfo bundle = bundleManifest.GetBundleInfo(bundleName);
                if (bundle == null)
                    throw new Exception(string.Format("The Bundle '{0}' doesn't exist! ", bundleName));

                return this.LoadBundle(bundle, priority);
            }
            catch (Exception e)
            {
                return new ImmutableProgressResult<float, IBundle>(e, 0f);
            }
        }

        public virtual IProgressResult<float, IBundle> LoadBundle(BundleInfo bundleInfo, int priority)
        {
            try
            {
                if (bundleInfo == null)
                    throw new ArgumentNullException("The bundleInfo is null!");

                DefaultBundle bundle = this.GetOrCreateBundle(bundleInfo, priority);
                var result = bundle.Load();

                ProgressResult<float, IBundle> resultCopy = new ProgressResult<float, IBundle>();
                result.Callbackable().OnProgressCallback(p => resultCopy.UpdateProgress(p));
                result.Callbackable().OnCallback((r) =>
                {
                    if (r.Exception != null)
                        resultCopy.SetException(r.Exception);
                    else
                        resultCopy.SetResult(new InternalBundleWrapper(bundle));
                });
                return resultCopy;
            }
            catch (Exception e)
            {
                return new ImmutableProgressResult<float, IBundle>(e, 0f);
            }
        }

        public virtual IProgressResult<float, IBundle[]> LoadBundle(params string[] bundleNames)
        {
            return this.LoadBundle(bundleNames, DEFAULT_PRIORITY);
        }

        public virtual IProgressResult<float, IBundle[]> LoadBundle(string[] bundleNames, int priority)
        {
            try
            {
                if (bundleManifest == null)
                    throw new Exception("The bundleManifest is null!");

                if (bundleNames == null || bundleNames.Length <= 0)
                    throw new ArgumentNullException("bundleNames", "The bundleNames is null or empty!");

                List<BundleInfo> bundleInfos = new List<BundleInfo>();
                for (int i = 0; i < bundleNames.Length; i++)
                {
                    var bundleName = BundleNameNormalize(bundleNames[i]);
                    BundleInfo info = bundleManifest.GetBundleInfo(bundleName);
                    if (info == null)
                    {
                        if (log.IsWarnEnabled)
                            log.WarnFormat("The Bundle '{0}' doesn't exist! ", bundleName);

                        continue;
                    }

                    if (!bundleInfos.Contains(info))
                        bundleInfos.Add(info);
                }

                return this.LoadBundle(bundleInfos.ToArray(), priority);
            }
            catch (Exception e)
            {
                return new ImmutableProgressResult<float, IBundle[]>(e, 0f);
            }
        }

        public virtual IProgressResult<float, IBundle[]> LoadBundle(BundleInfo[] bundleInfos, int priority)
        {
            try
            {
                if (bundleInfos == null || bundleInfos.Length <= 0)
                    throw new ArgumentNullException("bundleInfos", "The bundleInfos is null or empty!");

                return Executors.RunOnCoroutine<float, IBundle[]>(promise => DoLoadBundle(promise, bundleInfos, priority));
            }
            catch (Exception e)
            {
                return new ImmutableProgressResult<float, IBundle[]>(e, 0f);
            }
        }

        protected virtual IEnumerator DoLoadBundle(IProgressPromise<float, IBundle[]> promise, BundleInfo[] bundleInfos, int priority)
        {
            List<IBundle> bundles = new List<IBundle>();
            Exception exception = new Exception("unkown");
            List<IProgressResult<float, IBundle>> bundleResults = new List<IProgressResult<float, IBundle>>();
            for (int i = 0; i < bundleInfos.Length; i++)
            {
                try
                {
                    DefaultBundle bundle = this.GetOrCreateBundle(bundleInfos[i], priority);
                    IProgressResult<float, IBundle> bundleResult = bundle.Load();
                    bundleResult.Callbackable().OnCallback(r =>
                    {
                        if (r.Exception != null)
                        {
                            exception = r.Exception;
                            if (log.IsWarnEnabled)
                                log.WarnFormat("Loads Bundle failure! Error:{0}", r.Exception);
                        }
                        else
                        {
                            bundles.Add(new InternalBundleWrapper((DefaultBundle)r.Result));
                        }
                    });

                    if (!bundleResult.IsDone)
                        bundleResults.Add(bundleResult);
                }
                catch (Exception e)
                {
                    exception = e;
                    if (log.IsWarnEnabled)
                        log.WarnFormat("Loads Bundle '{0}' failure! Error:{1}", bundleInfos[i], e);
                }
            }

            bool finished = false;
            float progress = 0f;
            int count = bundleResults.Count;
            while (!finished && count > 0)
            {
                yield return null;

                progress = 0f;
                finished = true;
                for (int i = 0; i < count; i++)
                {
                    var result = bundleResults[i];
                    if (!result.IsDone)
                        finished = false;

                    progress += result.Progress;
                }
                promise.UpdateProgress(progress / count);
            }

            promise.UpdateProgress(1f);
            if (bundles.Count > 0)
                promise.SetResult(bundles.ToArray());
            else
                promise.SetException(exception);
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
                    if (disposing)
                    {
                        if (this.loaders != null)
                        {
                            foreach (var kv in this.loaders)
                            {
                                var loader = kv.Value;
                                loader.Dispose();
                            }
                            this.loaders = null;
                        }
                    }
                }
                catch (Exception)
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
