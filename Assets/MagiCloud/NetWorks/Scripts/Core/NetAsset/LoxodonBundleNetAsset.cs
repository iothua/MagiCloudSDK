using Loxodon.Framework.Bundles;
using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

namespace MagiCloud.NetWorks
{
    public class LoxodonBundleNetAsset : NetAsset
    {
        private string bundleUri;
        private List<string> requestDownloadBundleNames;

        public AssetBundleManager BundleManager;

        public LoxodonBundleNetAsset(ClientManager clientManager, string bundleUri, List<string> requestDownloadBundleNames) : base(clientManager)
        {

            this.bundleUri = bundleUri;

            if (requestDownloadBundleNames == null || requestDownloadBundleNames.Count == 0)
                requestDownloadBundleNames = new List<string>();

            this.requestDownloadBundleNames = requestDownloadBundleNames;
        }

        public IEnumerator OnLoadAsset(ExperimentInfo experimentInfo, Action LoadComplete)
        {
            base.LoadAsset(experimentInfo, LoadComplete);

            if (BundleManager == null)
                BundleManager = new AssetBundleManager(bundleUri);

            if (!string.IsNullOrEmpty(bundleUri))
                yield return BundleManager.Download(requestDownloadBundleNames);

            string[] results = experimentInfo.PrefabPath.Split(':');

            if (results.Length != 2)
            {
                //发送失败指定，关闭这个程序

                clientManager.OnExperimentError(experimentInfo.Name + "资源路径异常-" + experimentInfo.PrefabPath);
                yield break;
            }

            if (results[0] != "AssetBundle")
            {
                clientManager.OnExperimentError(experimentInfo.Name + "资源路径解析异常，它的解析必须为AssetBundle " + experimentInfo.PrefabPath);
                yield break;
            }

            yield return BundleManager.OnLoadAsset<GameObject>(new string[1] { results[1] }, (targets) =>
            {
                curExpPrefab = GameObject.Instantiate(targets[0]);

                curPrefabPath = experimentInfo.PrefabPath;
            });

            yield return new WaitForSeconds(0.1f);

            if (OnComplete != null)
                OnComplete();
        }
    }
}
