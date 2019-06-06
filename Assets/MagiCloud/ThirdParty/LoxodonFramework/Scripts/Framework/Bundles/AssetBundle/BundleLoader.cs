using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Loxodon.Framework.Asynchronous;
using Loxodon.Framework.Execution;
//using Loxodon.Log;

namespace Loxodon.Framework.Bundles
{
    public abstract class BundleLoader
    {
        private BundleInfo bundleInfo;

        private System.Uri uri;
        private int refCount = 0;
        private object _lock = new object();
        private int priority = int.MinValue;
        private AssetBundle assetBundle;
        private ProgressResult<float, AssetBundle> loadResult;
        private ITaskExecutor executor;
        private BundleManager manager;

        public BundleLoader(System.Uri uri, BundleInfo bundleInfo, BundleManager manager)
        {
            if (bundleInfo == null)
                throw new System.ArgumentNullException("bundleInfo");

            this.uri = uri;
            this.bundleInfo = bundleInfo;
            this.manager = manager;
            this.manager.AddBundleLoader(this);
            this.executor = this.manager.Executor;
        }

        public virtual string Name { get { return this.bundleInfo.Name; } }

        public virtual BundleInfo BundleInfo { get { return this.bundleInfo; } }

        public virtual int Priority
        {
            get { return this.priority; }
            set
            {
                if (this.priority > value)
                    return;

                this.priority = value;
            }
        }

        protected virtual System.Uri Uri { get { return this.uri; } }

        protected virtual BundleManager BundleManager { get { return this.manager; } }

        protected virtual string GetAbsoluteUri()
        {
            string path = this.Uri.AbsoluteUri;
            if (this.Uri.Scheme.Equals("jar") && !path.StartsWith("jar:file://"))
                path = path.Replace("jar:file:", "jar:file://");
            return path;
        }

        protected virtual string GetAbsolutePath()
        {
            string path = System.Uri.UnescapeDataString(this.Uri.AbsolutePath);
            if (this.Uri.Scheme.Equals("jar"))
                path = path.Replace("file://", "jar:file://");
            return path;
        }

        protected virtual bool IsRemoteUri()
        {
            if ("http".Equals(uri.Scheme) || "https".Equals(uri.Scheme) || "ftp".Equals(uri.Scheme))
                return true;
            return false;
        }

        protected IEnumerator Wrap(IEnumerator task, IPromise promise)
        {
            this.Retain();
            InterceptableEnumerator enumerator = new InterceptableEnumerator(task);
            enumerator.RegisterCatchBlock(e =>
            {
                promise.SetException(e);
            });
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

        public virtual IProgressResult<float, AssetBundle> LoadAssetBundle()
        {
            if (this.loadResult == null || this.loadResult.Exception != null)
            {
                this.loadResult = new ProgressResult<float, AssetBundle>();
                LoadingTask<float, AssetBundle> task = new LoadingTask<float, AssetBundle>(loadResult, Wrap(DoLoadAssetBundle(loadResult), loadResult), this);
                this.executor.Execute(task);
                this.loadResult.Callbackable().OnCallback(r =>
                {
                    if (r.Exception == null)
                        this.assetBundle = r.Result;
                });
            }

            return this.loadResult;
        }

        protected abstract IEnumerator DoLoadAssetBundle(IProgressPromise<float, AssetBundle> promise);

        #region IDisposable Support
        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                try
                {
                    this.manager.RemoveBundleLoader(this);
                    if (this.assetBundle != null)
                        this.assetBundle.Unload(false);
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

    #region LoadingTask Support
    class LoadingTask<TProgress, TResult> : ITask
    {
        private long startTime = 0L;
        private IEnumerator routine;
        private BundleLoader loader;
        private IProgressPromise<TProgress, TResult> result;

        public LoadingTask(IProgressPromise<TProgress, TResult> result, IEnumerator routine, BundleLoader loader)
        {
            this.result = result;
            this.routine = routine;
            this.loader = loader;
            this.startTime = System.DateTime.Now.Ticks / 10000;
        }

        public bool IsDone { get { return this.result.IsDone; } }

        public int Priority { get { return this.loader.Priority; } }

        public long StartTime { get { return this.startTime; } }

        public virtual IEnumerator GetRoutin()
        {
            return this.routine;
        }

        #region IDisposable Support
        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                this.loader = null;
                this.result = null;
                this.routine = null;
                this.disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
    #endregion
}
