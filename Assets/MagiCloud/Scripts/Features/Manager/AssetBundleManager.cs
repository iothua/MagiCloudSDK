using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using Loxodon.Framework.Bundles;
using Loxodon.Framework.Asynchronous;
using Loxodon.Framework.Examples.Bundle;
using System.Text;

namespace MagiCloud
{
    //[Serializable]
    //public class AssetBundleInfo
    //{
    //    public string bundleName;
    //    public string assetName;

    //    public AssetBundleInfo(string bundleName, string assetName)
    //    {
    //        this.bundleName = bundleName;
    //        this.assetName = assetName;
    //    }
    //}

    /// <summary>
    /// AssetBundle管理端
    /// </summary>
    public class AssetBundleManager : MonoBehaviour
    {
        private static AssetBundleManager Instance;

        public LocalResources localResources = new LocalResources();

        public IResources resources;

        private Dictionary<string, IBundle> bundles = new Dictionary<string, IBundle>();

        private string iv = "I9Ldk05g2ezWEXE9";
        private string key = "uz0NlpJaMnG7dHrR";

        private void Awake()
        {
            Instance = this;

            IBundleManifestLoader manifestLoader = new BundleManifestLoader();
            BundleManifest manifest = manifestLoader.Load(BundleUtil.GetReadOnlyDirectory() + BundleSetting.ManifestFilename);

            IPathInfoParser pathInfoParser = new AutoMappingPathInfoParser(manifest);

            ILoaderBuilder builder = new CustomBundleLoaderBuilder(new Uri(BundleUtil.GetReadOnlyDirectory()), false, new RijndaelCryptograph(128, Encoding.ASCII.GetBytes(key), Encoding.ASCII.GetBytes(iv)));

            IBundleManager manager = new BundleManager(manifest, builder);

            resources = new BundleResources(pathInfoParser, manager);
        }

        public static void LoadAsset<T>(string[] assetNames, Action<T[]> completed, Action<float> progress = null)
             where T : UnityEngine.Object
        {
            IProgressResult<float, T[]> result = Instance.resources.LoadAssetsAsync<T>(assetNames);

            result.Callbackable().OnProgressCallback(p =>
            {
                if (progress != null)
                    progress(p * 100);
            });

            result.Callbackable().OnCallback((r) =>
            {
                try
                {
                    if (r.Exception != null)
                        throw r.Exception;

                    if (completed != null)
                        completed(r.Result);
                }
                catch (Exception e)
                {
                    Debug.LogErrorFormat("加载错误:{0}", e);
                }
            });
        }
    }

}
