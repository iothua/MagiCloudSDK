using Loxodon.Framework.Bundles;
using System;
using System.Collections;
using UnityEngine;

namespace MagiCloud.NetWorks
{
    public class ResourcesNetAsset : NetAsset
    {
        public ResourcesNetAsset(ClientManager clientManager) : base(clientManager) { }

        public override IEnumerator LoadAsset(ExperimentInfo experimentInfo, Action LoadComplete)
        {
            //yield return base.LoadAsset(experimentInfo, LoadComplete);
            this.OnComplete = LoadComplete;
            this.Experiment = experimentInfo;

            string[] results = experimentInfo.PrefabPath.Split(':');

            if (results.Length != 2)
            {
                //发送失败指定，关闭这个程序

                clientManager.OnExperimentError(experimentInfo.Name + "资源路径异常-" + experimentInfo.PrefabPath);
                yield break;
            }

            if (results[0] != "Resources")
            {
                clientManager.OnExperimentError(experimentInfo.Name + "资源路径解析异常，它的解析必须为Resources " + experimentInfo.PrefabPath);
                yield break;
            }

            LocalResources localResources = new LocalResources();

            var target = localResources.LoadAsset<GameObject>(results[1]);

            if (target == null)
            {
                clientManager.OnExperimentError(experimentInfo.Name + "所属资源不存在");
                yield break;

            }

            curExpPrefab = GameObject.Instantiate<GameObject>(target);
            curPrefabPath = Experiment != null ? Experiment.PrefabPath : string.Empty;

            yield return new WaitForSeconds(0.1f);

            if (OnComplete != null)
                OnComplete();
        }
    }
}
