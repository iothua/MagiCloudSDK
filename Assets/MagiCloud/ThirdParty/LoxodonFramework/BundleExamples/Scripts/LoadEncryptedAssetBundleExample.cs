using System;
using System.Text;
using UnityEngine;

using Loxodon.Framework.Bundles;
using Loxodon.Framework.Asynchronous;

namespace Loxodon.Framework.Examples.Bundle
{
    public class LoadEncryptedAssetBundleExample : MonoBehaviour
    {

        private IResources resources;

        private string iv = "5Hh2390dQlVh0AqC";
        private string key = "E4YZgiGQ0aqe5LEJ";      

        void Awake()
        {
            /* Create a BundleManifestLoader. */
            IBundleManifestLoader manifestLoader = new BundleManifestLoader();

            /* Loads BundleManifest. */
            BundleManifest manifest = manifestLoader.Load(BundleUtil.GetReadOnlyDirectory() + BundleSetting.ManifestFilename);

            /* Create a PathInfoParser. */
            IPathInfoParser pathInfoParser = new AutoMappingPathInfoParser(manifest);

            /* Use a BundleLoaderBuilder */
            ILoaderBuilder builder = new CustomBundleLoaderBuilder(new Uri(BundleUtil.GetReadOnlyDirectory()), false, new RijndaelCryptograph(128, Encoding.ASCII.GetBytes(key), Encoding.ASCII.GetBytes(iv)));

            /* Create a BundleManager */
            IBundleManager manager = new BundleManager(manifest, builder);

            /* Create a BundleResources */
            resources = new BundleResources(pathInfoParser, manager);
        }

        void Start()
        {
            this.Load(new string[] { "LoxodonFramework/BundleExamples/Encrypted/Tanks/Prefabs/Terrain.prefab", "LoxodonFramework/BundleExamples/Encrypted/Tanks/Prefabs/Tank.prefab" });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        void Load(string[] names)
        {
            IProgressResult<float, GameObject[]> result = resources.LoadAssetsAsync<GameObject>(names);
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
                
    }
}