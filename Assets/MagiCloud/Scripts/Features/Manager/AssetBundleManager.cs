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

        /* 注释
         

        //public static void LoadAsset<T>(AssetBundleInfo[] bundleInfos,Action<T[]> completed = null)
        //    where T : UnityEngine.Object
        //{
        //    string[] bundleNames = new string[bundleInfos.Length];
        //    string[] assetNames = new string[bundleInfos.Length];

        //    T[] assets = new T[bundleInfos.Length];

        //    for (int i = 0; i < bundleInfos.Length; i++)
        //    {
        //        bundleNames[i] = bundleInfos[i].bundleName;
        //        assetNames[i] = bundleInfos[i].assetName;
        //    }

        //    //方式一：
        //    //加载AssetBundle资源
        //    Instance.StartCoroutine(Instance.OnLoad(bundleNames, ()=> {

        //        for (int i = 0; i < bundleInfos.Length; i++)
        //        {
        //            var bundle = Instance.bundles[bundleInfos[i].bundleName];
        //            if (bundle == null) continue;

        //            T t = bundle.LoadAsset<T>(bundleInfos[i].assetName);

        //            assets[i] = t;
        //        }

        //        if (completed != null)
        //            completed(assets);
        //    }));
        //}

        ///// <summary>
        ///// 加载AssetBundle下的资源
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="bundleName"></param>
        ///// <param name="assetName"></param>
        ///// <returns></returns>
        //public static void LoadAsset<T>(string bundleName, string assetName,Action<T> completed)
        //    where T : UnityEngine.Object
        //{
        //    LoadAsset<T>(new AssetBundleInfo[1] { new AssetBundleInfo(bundleName, assetName) }, (t)=>{
        //        if (completed != null)
        //            completed(t.Length > 0 ? t[0] : default);
        //    });
        //}

        //public static void LoadAsset<T>(string bundleName, string[] assetName,Action<T[]> completed = null)
        //    where T : UnityEngine.Object
        //{
        //    T[] assets = new T[assetName.Length];
        //    //加载AssetBundle资源
        //    Instance.StartCoroutine(Instance.OnLoad(new string[1] { bundleName }, ()=> {


        //        for (int i = 0; i < assetName.Length; i++)
        //        {
        //            var bundle = Instance.bundles[assetName[i]];
        //            if (bundle == null) continue;

        //            T t = bundle.LoadAsset<T>(assetName[i]);

        //            assets[i] = t;
        //        }

        //        completed(assets);
        //    }));

        //}

        //IEnumerator OnLoad(string[] bundleNames,Action completed)
        //{
        //    IProgressResult<float, IBundle[]> result = this.resources.LoadBundle(bundleNames);

        //    yield return result.WaitForDone();

        //    if (result.Exception != null)
        //    {
        //        Debug.LogWarningFormat("加载出错：{0}", result.Exception);
        //        yield break;
        //    }

        //    foreach (var bundle in result.Result)
        //    {
        //        if (bundles.ContainsKey(bundle.Name))
        //        {
        //            //bundles[bundle.Name] = bundle;
        //            continue;
        //        }
        //        else
        //        {
        //            bundles.Add(bundle.Name, bundle);
        //        }
        //    }

        //    if (result.IsDone)
        //    {
        //        if (completed != null)
        //            completed();
        //    }
        //}

        ///// <summary>
        ///// 本地Resources资源加载
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="path"></param>
        ///// <returns></returns>
        //public static T ResourcesLoad<T>(string path)
        //    where T : UnityEngine.Object
        //{
        //    return Instance.localResources.LoadAsset<T>(path);
        //}

         */
    }

}
