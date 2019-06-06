using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Loxodon.Framework.Asynchronous;
using Loxodon.Framework.Execution;

namespace Loxodon.Framework.Bundles
{
    public class LocalBundle : IBundle
    {
        private string root;

        public LocalBundle() : this(null)
        {
        }

        public LocalBundle(string root)
        {
            this.Root = root;
        }

        protected virtual string Root
        {
            get { return this.root; }
            set { this.root = value != null ? value.Trim() : ""; }
        }

        public virtual string Name
        {
            get { return string.Format("Resources/{0}", this.Root); }
        }

        protected virtual string GetFilePathWithoutExtension(string name)
        {
            return Path.GetFilePathWithoutExtension(name);
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

            return Resources.Load(this.GetFilePathWithoutExtension(name), type);
        }

        public virtual T LoadAsset<T>(string name) where T : Object
        {
            if (string.IsNullOrEmpty(name))
                throw new System.ArgumentNullException("name", "The name is null or empty!");

            return Resources.Load<T>(this.GetFilePathWithoutExtension(name));
        }

        public virtual IProgressResult<float, Object> LoadAssetAsync(string name, System.Type type)
        {
            try
            {
                if (string.IsNullOrEmpty(name))
                    throw new System.ArgumentNullException("name", "The name is null or empty!");

                if (type == null)
                    throw new System.ArgumentNullException("type");

                return this.Execute<float, Object>(promise => DoLoadAssetAsync(promise, name, type));
            }
            catch (System.Exception e)
            {
                return new ImmutableProgressResult<float, Object>(e, 0f);
            }
        }

        protected virtual IEnumerator DoLoadAssetAsync(IProgressPromise<float, Object> promise, string name, System.Type type)
        {
            var fullName = this.GetFilePathWithoutExtension(name);
            ResourceRequest request = Resources.LoadAsync(fullName, type);
            while (!request.isDone)
            {
                promise.UpdateProgress(request.progress);
                yield return null;
            }

            promise.UpdateProgress(1f);
            Object asset = request.asset;
            if (asset != null)
                promise.SetResult(request.asset);
            else
                promise.SetException(new System.Exception(string.Format("Not found the asset[{0}].", fullName)));
        }

        public virtual IProgressResult<float, T> LoadAssetAsync<T>(string name) where T : Object
        {
            try
            {
                if (string.IsNullOrEmpty(name))
                    throw new System.ArgumentNullException("name", "The name is null or empty!");

                return this.Execute<float, T>(promise => DoLoadAssetAsync(promise, name));
            }
            catch (System.Exception e)
            {
                return new ImmutableProgressResult<float, T>(e, 0f);
            }
        }

        protected virtual IEnumerator DoLoadAssetAsync<T>(IProgressPromise<float, T> promise, string name) where T : Object
        {
            var fullName = this.GetFilePathWithoutExtension(name);
            ResourceRequest request = Resources.LoadAsync<T>(fullName);
            while (!request.isDone)
            {
                promise.UpdateProgress(request.progress);
                yield return null;
            }

            promise.UpdateProgress(1f);
            T asset = (T)request.asset;
            if (asset != null)
                promise.SetResult(asset);
            else
                promise.SetException(new System.Exception(string.Format("Not found the asset '{0}'.", fullName)));
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
            if (names == null || names.Length <= 0)
            {
                promise.SetResult(new Object[0]);
                yield break;
            }

            Dictionary<string, ResourceRequest> requests = new Dictionary<string, ResourceRequest>();
            foreach (string name in names)
            {
                var fullName = this.GetFilePathWithoutExtension(name);
                if (requests.ContainsKey(fullName))
                    continue;

                var request = Resources.LoadAsync<T>(fullName);
                requests.Add(fullName, request);
            }

            int count = requests.Count;
            float progress = 0f;
            bool finished = false;
            do
            {

                yield return null;

                finished = true;
                progress = 0f;
                foreach (ResourceRequest request in requests.Values)
                {
                    if (!request.isDone)
                        finished = false;

                    progress += request.progress;
                }
                promise.UpdateProgress(progress / count);

            } while (!finished);

            List<T> assets = new List<T>();
            foreach (ResourceRequest request in requests.Values)
            {
                T asset = (T)request.asset;
                if (asset != null)
                    assets.Add(asset);
            }
            promise.UpdateProgress(1f);
            promise.SetResult(assets.ToArray());
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
            if (names == null || names.Length <= 0)
            {
                promise.SetResult(new Object[0]);
                yield break;
            }

            Dictionary<string, ResourceRequest> requests = new Dictionary<string, ResourceRequest>();
            foreach (string name in names)
            {
                var fullName = this.GetFilePathWithoutExtension(name);
                if (requests.ContainsKey(fullName))
                    continue;

                var request = Resources.LoadAsync(fullName, type);
                requests.Add(fullName, request);
            }

            int count = requests.Count;
            float progress = 0f;
            bool finished = false;

            do
            {

                yield return null;

                finished = true;
                progress = 0f;
                foreach (ResourceRequest request in requests.Values)
                {
                    if (!request.isDone)
                        finished = false;

                    progress += request.progress;
                }

                promise.UpdateProgress(progress / count);
            } while (!finished);

            List<Object> assets = new List<Object>();
            foreach (ResourceRequest request in requests.Values)
            {
                Object asset = request.asset;
                if (asset != null)
                    assets.Add(asset);
            }
            promise.UpdateProgress(1f);
            promise.SetResult(assets.ToArray());
        }

        public virtual Object[] LoadAllAssets(System.Type type)
        {
            if (type == null)
                throw new System.ArgumentNullException("type");

            return Resources.LoadAll(this.Root, type);
        }

        public virtual T[] LoadAllAssets<T>() where T : Object
        {
            return Resources.LoadAll<T>(this.Root);
        }

        public virtual IProgressResult<float, Object[]> LoadAllAssetsAsync(System.Type type)
        {
            try
            {
                if (type == null)
                    throw new System.ArgumentNullException("type");

                return this.Execute<float, Object[]>(promise => DoLoadAllAssetsAsync(promise));
            }
            catch (System.Exception e)
            {
                return new ImmutableProgressResult<float, Object[]>(e, 0f);
            }
        }

        protected virtual IEnumerator DoLoadAllAssetsAsync(IProgressPromise<float, Object[]> promise, System.Type type)
        {
            try
            {
                Object[] assets = Resources.LoadAll(this.Root);
                promise.UpdateProgress(1f);
                promise.SetResult(assets);
            }
            catch (System.Exception e)
            {
                promise.UpdateProgress(0f);
                promise.SetException(e);
            }
            yield break;
        }

        public virtual IProgressResult<float, T[]> LoadAllAssetsAsync<T>() where T : Object
        {
            return this.Execute<float, T[]>(promise => DoLoadAllAssetsAsync(promise));
        }

        protected virtual IEnumerator DoLoadAllAssetsAsync<T>(IProgressPromise<float, T[]> promise) where T : Object
        {
            try
            {
                T[] assets = Resources.LoadAll<T>(this.Root);
                promise.UpdateProgress(1f);
                promise.SetResult(assets);
            }
            catch (System.Exception e)
            {
                promise.UpdateProgress(0f);
                promise.SetException(e);
            }
            yield break;
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

        public virtual void Dispose()
        {
        }
    }
}
