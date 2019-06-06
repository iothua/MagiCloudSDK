using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

namespace Loxodon.Framework.Bundles
{
    public class LocalResources : AbstractResources
    {
        public LocalResources() : base(new LocalPathInfoParser(), new LocalBundleManager(),false)
        {
        }

        protected override IEnumerator DoLoadSceneAsync(ISceneLoadingPromise<Scene> promise, string path, LoadSceneMode mode = LoadSceneMode.Single)
        {
            AssetPathInfo pathInfo = pathInfoParser.Parse(path);
            if (pathInfo == null)
            {
                promise.Progress = 0f;
                promise.SetException(string.Format("Parses the path info '{0}' failure.", path));
                yield break;
            }

            AsyncOperation operation = SceneManager.LoadSceneAsync(Path.GetFileNameWithoutExtension(pathInfo.AssetName), mode);
            if (operation == null)
            {
                promise.SetException(string.Format("Not found the scene '{0}'.", path));
                yield break;
            }

            operation.allowSceneActivation = false;
            while (operation.progress < 0.9f)
            {
                if (operation.progress == 0f)
                    operation.priority = promise.Priority;

                promise.Progress = operation.progress;
                yield return waitForSeconds;
            }
            promise.Progress = operation.progress;
            promise.State = LoadState.SceneActivationReady;

            while (!operation.isDone)
            {
                if (promise.AllowSceneActivation && !operation.allowSceneActivation)
                    operation.allowSceneActivation = promise.AllowSceneActivation;

                promise.Progress = operation.progress;
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
