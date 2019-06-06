using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Loxodon.Framework.Bundles;
using Loxodon.Framework.Asynchronous;
using Loxodon.Framework.Contexts;

namespace Loxodon.Framework.Examples.Bundle
{
    public class PreloadAssetBundleExample : MonoBehaviour
    {
        private IResources resources;
        private Dictionary<string, IBundle> bundles = new Dictionary<string, IBundle>();

        IEnumerator Start()
        {
            ApplicationContext context = Context.GetApplicationContext();
            this.resources = context.GetService<IResources>();

            /* Preload AssetBundle */
            yield return Preload(new string[] { "models/red", "models/green", "models/plane" }, 1);

            /* Use IBundle,loads plane */
            IBundle bundle = this.bundles["models/plane"];          
            //GameObject goTemplate = bundle.LoadAsset<GameObject>("Plane.prefab"); //OK
            GameObject goTemplate = bundle.LoadAsset<GameObject>("LoxodonFramework/BundleExamples/Models/Plane/Plane.prefab"); //OK
            GameObject.Instantiate(goTemplate);
            
            /* Green and Red */
            GameObject[] goTemplates = this.resources.LoadAssets<GameObject>("LoxodonFramework/BundleExamples/Models/Green/Green.prefab", "LoxodonFramework/BundleExamples/Models/Red/Red.prefab");
            foreach(GameObject template in goTemplates)
            {
                GameObject.Instantiate(template);
            }
        }

        /// <summary>
        /// Preloads AssetBundle.
        /// </summary>
        /// <param name="bundleNames"></param>
        /// <param name="priority"></param>
        /// <returns></returns>
        IEnumerator Preload(string[] bundleNames, int priority)
        {
            IProgressResult<float, IBundle[]> result = this.resources.LoadBundle(bundleNames, priority);
            yield return result.WaitForDone();

            if (result.Exception != null)
            {
                Debug.LogWarningFormat("Loads failure.Error:{0}", result.Exception);
                yield break;
            }

            foreach (IBundle bundle in result.Result)
            {
                bundles.Add(bundle.Name, bundle);
            }
        }

        void OnDestroy()
        {
            if (bundles == null)
                return;

            foreach (IBundle bundle in bundles.Values)
                bundle.Dispose();

            this.bundles = null;
        }

    }
}
