using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

using Loxodon.Framework.Bundles;
using Loxodon.Framework.Contexts;

namespace Loxodon.Framework.Examples.Bundle
{
    public class LoadVariantExample : MonoBehaviour
    {
        private IResources resources;
        private bool loading = false;
        string sceneName = "LoxodonFramework/BundleExamples/Maps/Variants/VariantScene.unity";
        void Start()
        {
            ApplicationContext context = Context.GetApplicationContext();
            this.resources = context.GetService<IResources>();
        }

        void OnGUI()
        {
            if (!loading)
            {
#if UNITY_EDITOR
                GUILayout.Space(20);
                if (SimulationSetting.IsSimulationMode)
                {
                    GUILayout.Label("The variant is not supported in the simulation mode");
                }
#endif
                GUILayout.Space(20);
                GUILayout.BeginHorizontal();
                GUILayout.Space(20);
                GUILayout.BeginVertical();
                if (GUILayout.Button("Load SD", GUILayout.Width(250f), GUILayout.MinHeight(60f)))
                {
                    StartCoroutine(Load(sceneName, "sd"));
                }
                GUILayout.Space(5);
                if (GUILayout.Button("Load HD", GUILayout.Width(250f), GUILayout.MinHeight(60f)))
                {
                    StartCoroutine(Load(sceneName, "hd"));
                }
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
            }
        }

        IEnumerator Load(string sceneName, string resolution)
        {
            this.loading = true;
            if (this.resources is BundleResources)
            {
                BundleManager manager = (BundleManager)(this.resources as BundleResources).BundleManager;
                BundleManifest manifest = manager.BundleManifest;
                manifest.ActiveVariants = new string[] { "", resolution };
            }

            ISceneLoadingResult<Scene> result = this.resources.LoadSceneAsync(sceneName);
            while (!result.IsDone)
            {
                Debug.LogFormat("Loading {0}%", (result.Progress * 100));
                yield return null;
            }
            this.loading = false;
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