#if UNITY_EDITOR
using System.Collections;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

namespace Loxodon.Framework.Bundles
{
    public class SimulationResources : AbstractResources
    {
        protected const string ASSETS = "Assets/";

        public SimulationResources() : this(new SimulationAutoMappingPathInfoParser(), new SimulationBundleManager())
        {
        }

        public SimulationResources(IPathInfoParser pathInfoParser) : base(pathInfoParser, new SimulationBundleManager(), false)
        {
        }

        public SimulationResources(IPathInfoParser pathInfoParser, IBundleManager manager) : base(pathInfoParser, manager, false)
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

            var name = string.Format("{0}{1}", ASSETS, pathInfo.AssetName);
#if UNITY_2018_3_OR_NEWER
            AsyncOperation operation = EditorSceneManager.LoadSceneAsyncInPlayMode(name, new LoadSceneParameters(mode));
#else
            AsyncOperation operation = LoadSceneMode.Additive.Equals(mode) ? EditorApplication.LoadLevelAdditiveAsyncInPlayMode(name) : EditorApplication.LoadLevelAsyncInPlayMode(name);
#endif
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
#endif