using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

using Loxodon.Framework.Asynchronous;

namespace Loxodon.Framework.Bundles
{
    public class BundleResources : AbstractResources
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pathInfoParser">The parser for the asset path.</param>
        /// <param name="manager">The manager of Assetbundles.</param>
        public BundleResources(IPathInfoParser pathInfoParser, IBundleManager manager) : this(pathInfoParser, manager, true)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pathInfoParser">The parser for the asset path.</param>
        /// <param name="manager">The manager of Assetbundles.</param>
        /// <param name="useWeakCache">Whether to use weak cache, if it is true, use weak cache, otherwise close weak cache.
        /// Objects loaded from AssetBundles are unmanaged objects,the weak caches do not accurately track the validity of objects.
        /// If there are some problems after the Resource.UnloadUnusedAssets() is called, please turn off the weak cache.
        /// </param>
        public BundleResources(IPathInfoParser pathInfoParser, IBundleManager manager, bool useWeakCache) : base(pathInfoParser, manager, useWeakCache)
        {
        }

        protected override IEnumerator DoLoadSceneAsync(ISceneLoadingPromise<Scene> promise, string path, LoadSceneMode mode = LoadSceneMode.Single)
        {
            AssetPathInfo pathInfo = pathInfoParser.Parse(path);
            if (pathInfo == null)
            {
                promise.Progress = 1f;
                promise.SetException(string.Format("Parses the path info '{0}' failure.", path));
                yield break;
            }

            yield return null;//Wait for a frame.

            IProgressResult<float, IBundle> bundleResult = this.LoadBundle(pathInfo.BundleName, promise.Priority);
            float weight = bundleResult.IsDone ? 0f : DEFAULT_WEIGHT;
            bundleResult.Callbackable().OnProgressCallback(p => promise.Progress = p * weight);

            while (!bundleResult.IsDone)
                yield return null;

            if (bundleResult.Exception != null)
            {
                promise.SetException(bundleResult.Exception);
                yield break;
            }

            promise.State = LoadState.AssetBundleLoaded;

            using (IBundle bundle = bundleResult.Result)
            {
                AsyncOperation operation = SceneManager.LoadSceneAsync(Path.GetFileNameWithoutExtension(pathInfo.AssetName), mode);
                if (operation == null)
                {
                    promise.SetException(string.Format("Not found the scene '{0}'.", path));
                    yield break;
                }
                operation.priority = promise.Priority;
                operation.allowSceneActivation = false;
                while (operation.progress < 0.9f)
                {
                    promise.Progress = weight + (1f - weight) * operation.progress;
                    yield return waitForSeconds;
                }
                promise.Progress = weight + (1f - weight) * operation.progress;
                promise.State = LoadState.SceneActivationReady;
                while (!operation.isDone)
                {
                    if (promise.AllowSceneActivation && !operation.allowSceneActivation)
                        operation.allowSceneActivation = promise.AllowSceneActivation;

                    promise.Progress = weight + (1f - weight) * operation.progress;
                    yield return waitForSeconds;
                }

                Scene scene = SceneManager.GetSceneByName(Path.GetFileNameWithoutExtension(pathInfo.AssetName));
                if (!scene.IsValid())
                {
                    promise.SetException(string.Format("Not found the scene '{0}'.", path));
                    yield break;
                }

                promise.Progress = 1f;
                promise.SetResult(scene);
            }
        }
    }
}
