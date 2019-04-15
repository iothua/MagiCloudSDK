using System;
using System.IO;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Loxodon.Framework.Bundles;
using Loxodon.Framework.Asynchronous;

namespace Loxodon.Framework.Examples.Bundle
{
    public class DownloadExample : MonoBehaviour
    {
        private IResources resources;
        private IDownloader downloader;
        private bool downloading = false;

        void Start()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            Uri baseUri = new Uri(BundleUtil.GetReadOnlyDirectory());
#else
            DirectoryInfo dir = new DirectoryInfo("./AssetBundles/StandaloneWindows/1.0.0/");

            if (!dir.Exists)
            {
                Debug.LogFormat("Directory '{0}' does not exist.", dir.FullName);
                return;
            }

            Uri baseUri = new Uri(dir.FullName);
#endif
            this.downloader = new WWWDownloader(baseUri, false);
        }

        void OnGUI()
        {
            if (!downloading)
            {
                GUILayout.Space(20);
                GUILayout.BeginHorizontal();
                GUILayout.Space(20);
                GUILayout.BeginVertical();
                if (GUILayout.Button("Clear persistentDataPath"))
                {
#if UNITY_2017_1_OR_NEWER
                    Caching.ClearCache();
#else
                    Caching.CleanCache();
#endif
                    BundleUtil.ClearStorableDirectory();
                }
#if UNITY_EDITOR
                if (GUILayout.Button("Remove StreamingAssets"))
                {
                    Directory.Delete(BundleUtil.GetReadOnlyDirectory(), true);
                    UnityEditor.AssetDatabase.Refresh();
                }
#endif
                GUILayout.Space(5);
                if (GUILayout.Button("Download AssetBundle"))
                {
                    StartCoroutine(Download());
                }

                if (GUILayout.Button("Load an asset"))
                {
                    if (!File.Exists(BundleUtil.GetStorableDirectory() + BundleSetting.ManifestFilename))
                    {
                        Debug.LogFormat("Please download assetbundles first,try again.");
                    }
                    else
                    {
                        this.LoadAsset("LoxodonFramework/BundleExamples/Models/Red/Red.prefab");
                    }
                }
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
            }
        }

        IEnumerator Download()
        {
            this.downloading = true;
            try
            {
                IProgressResult<Progress, BundleManifest> manifestResult = this.downloader.DownloadManifest(BundleSetting.ManifestFilename);

                yield return manifestResult.WaitForDone();

                if (manifestResult.Exception != null)
                {
                    Debug.LogFormat("Downloads BundleManifest failure.Error:{0}", manifestResult.Exception);
                    yield break;
                }

                BundleManifest manifest = manifestResult.Result;

                IProgressResult<float, List<BundleInfo>> bundlesResult = this.downloader.GetDownloadList(manifest);
                yield return bundlesResult.WaitForDone();

                List<BundleInfo> bundles = bundlesResult.Result;

                if (bundles == null || bundles.Count <= 0)
                {
                    Debug.LogFormat("Please clear cache and remove StreamingAssets,try again.");
                    yield break;
                }

                IProgressResult<Progress, bool> downloadResult = this.downloader.DownloadBundles(bundles);
                downloadResult.Callbackable().OnProgressCallback(p =>
                {
                    Debug.LogFormat("Downloading {0:F2}KB/{1:F2}KB {2:F3}KB/S", p.GetCompletedSize(UNIT.KB), p.GetTotalSize(UNIT.KB), p.GetSpeed(UNIT.KB));
                });

                yield return downloadResult.WaitForDone();

                if (downloadResult.Exception != null)
                {
                    Debug.LogFormat("Downloads AssetBundle failure.Error:{0}", downloadResult.Exception);
                    yield break;
                }

                Debug.Log("OK");

                if (this.resources != null)
                {
                    //update BundleManager's manifest
                    BundleManager manager = (this.resources as BundleResources).BundleManager as BundleManager;
                    manager.BundleManifest = manifest;
                }

#if UNITY_EDITOR
                UnityEditor.EditorUtility.OpenWithDefaultApp(BundleUtil.GetStorableDirectory());
#endif

            }
            finally
            {
                this.downloading = false;
            }
        }

        IResources GetResources()
        {
            if (this.resources != null)
                return this.resources;

            /* Create a BundleManifestLoader. */
            IBundleManifestLoader manifestLoader = new BundleManifestLoader();

            /* Loads BundleManifest. */
            BundleManifest manifest = manifestLoader.Load(BundleUtil.GetStorableDirectory() + BundleSetting.ManifestFilename);

            //manifest.ActiveVariants = new string[] { "", "sd" };
            //manifest.ActiveVariants = new string[] { "", "hd" };

            /* Create a PathInfoParser. */
            IPathInfoParser pathInfoParser = new AutoMappingPathInfoParser(manifest);

            /* Use a custom BundleLoaderBuilder */
            ILoaderBuilder builder = new CustomBundleLoaderBuilder(new Uri(BundleUtil.GetReadOnlyDirectory()), false);

            /* Create a BundleManager */
            IBundleManager manager = new BundleManager(manifest, builder);

            /* Create a BundleResources */
            this.resources = new BundleResources(pathInfoParser, manager);
            return this.resources;
        }

        void LoadAsset(string name)
        {
            var resources = this.GetResources();
            IProgressResult<float, GameObject> result = resources.LoadAssetAsync<GameObject>(name);
            result.Callbackable().OnCallback((r) =>
            {
                try
                {
                    if (r.Exception != null)
                        throw r.Exception;

                    GameObject.Instantiate(r.Result);

                }
                catch (Exception e)
                {
                    Debug.LogErrorFormat("Load failure.Error:{0}", e);
                }
            });
        }
    }
}
