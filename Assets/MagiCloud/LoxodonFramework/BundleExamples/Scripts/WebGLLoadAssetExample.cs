using System;
using System.Collections;
using UnityEngine;

using Loxodon.Framework.Bundles;
using Loxodon.Framework.Asynchronous;
using Loxodon.Framework.Contexts;

namespace Loxodon.Framework.Examples.Bundle
{

    public class WebGLLoadAssetExample : MonoBehaviour
    {
#if UNITY_WEBGL
        private IResources resources;

        IEnumerator Start()
        {
            ApplicationContext context = Context.GetApplicationContext();

            while (this.resources == null)
            {
                this.resources = context.GetService<IResources>();
                yield return null;
            }

            this.Load(new string[] { "LoxodonFramework/BundleExamples/Models/Red/Red.prefab", "LoxodonFramework/BundleExamples/Models/Green/Green.prefab" });
            this.StartCoroutine(Load2("LoxodonFramework/BundleExamples/Models/Plane/Plane.prefab"));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        void Load(string[] names)
        {
            IProgressResult<float, GameObject[]> result = resources.LoadAssetsAsync<GameObject>(names);
            result.Callbackable().OnProgressCallback(p =>
            {
                Debug.LogFormat("Progress:{0}%", p * 100);
            });
            result.Callbackable().OnCallback((r) =>
            {
                try
                {
                    if (r.Exception != null)
                        throw r.Exception;

                    foreach (GameObject template in r.Result)
                    {
                        GameObject.Instantiate(template);
                    }

                }
                catch (Exception e)
                {
                    Debug.LogErrorFormat("Load failure.Error:{0}", e);
                }
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        IEnumerator Load2(string name)
        {
            IProgressResult<float, GameObject> result = resources.LoadAssetAsync<GameObject>(name);

            while (!result.IsDone)
            {
                Debug.LogFormat("Progress:{0}%", result.Progress * 100);
                yield return null;
            }

            try
            {
                if (result.Exception != null)
                    throw result.Exception;

                GameObject.Instantiate(result.Result);
            }
            catch (Exception e)
            {
                Debug.LogErrorFormat("Load failure.Error:{0}", e);
            }
        }
#endif
    }
}