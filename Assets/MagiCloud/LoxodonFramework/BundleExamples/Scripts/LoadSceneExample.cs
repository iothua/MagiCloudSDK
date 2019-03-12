using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

using Loxodon.Framework.Bundles;
using Loxodon.Framework.Contexts;

namespace Loxodon.Framework.Examples.Bundle
{
    public class LoadSceneExample : MonoBehaviour
    {
        private IResources resources;

        void Start()
        {
            ApplicationContext context = Context.GetApplicationContext();
            this.resources = context.GetService<IResources>();

            string sceneName = "LoxodonFramework/BundleExamples/Maps/Sample/SampleScene.unity";

            LoadSceneByCallback(sceneName);

            //this.StartCoroutine(LoadSceneByCoroutine(sceneName));
        }

        void LoadSceneByCallback(string sceneName)
        {
            ISceneLoadingResult<Scene> result = this.resources.LoadSceneAsync(sceneName);
            result.AllowSceneActivation = false;

            result.OnProgressCallback(p =>
            {
                //Debug.LogFormat("Loading {0}%", (p * 100));
            });

            result.OnStateChangedCallback(state =>
            {
                if (state == LoadState.Failed)
                    Debug.LogFormat("Loads scene '{0}' failure.Error:{1}", sceneName, result.Exception);
                else if (state == LoadState.Completed)
                    Debug.LogFormat("Loads scene '{0}' completed.", sceneName);
                else if (state == LoadState.AssetBundleLoaded)
                    Debug.LogFormat("The AssetBundle has been loaded.");
                else if (state == LoadState.SceneActivationReady)
                {
                    Debug.LogFormat("Ready to activate the scene.");
                    result.AllowSceneActivation = true;
                }
            });
        }

        IEnumerator LoadSceneByCoroutine(string sceneName)
        {
            ISceneLoadingResult<Scene> result = this.resources.LoadSceneAsync(sceneName);
            while (!result.IsDone)
            {
                //Debug.LogFormat("Loading {0}%", (result.Progress * 100));
                yield return null;
            }

            if (result.Exception != null)
            {
                Debug.LogFormat("Loads scene '{0}' failure.Error:{1}", sceneName, result.Exception);
            }
            else
            {
                Debug.LogFormat("Loads scene '{0}' completed.", sceneName);
            }
        }
    }

}