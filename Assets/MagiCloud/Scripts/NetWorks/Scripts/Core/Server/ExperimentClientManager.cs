using System;
using Google.Protobuf;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Loxodon.Framework.Bundles;

namespace MagiCloud.NetWorks
{
    public class ExperimentClientManager : ClientManager
    {
        public NetAsset netAsset;

        private MonoBehaviour monoBehaviour;

        string currentExperimentPrefabPath = "";

        public ExperimentClientManager(MonoBehaviour behaviour) : base()
        {
            this.monoBehaviour = behaviour;
        }

        protected override void OnExperimentRequest(int sender, IMessage proto)
        {
            //base.OnExperimentRequest(sender, proto);

            ExperimentInfo experimentInfo = proto as ExperimentInfo;

            if (currentExperiment == null || experimentInfo.PrefabPath != currentExperiment.PrefabPath)//(string.IsNullOrEmpty(currentExperimentPrefabPath) || currentExperimentPrefabPath != experimentInfo.PrefabPath)//
            {
                if (currentExperiment != null)
                {
                    currentExperiment = null;

                    SendExperimentStatus(2);
                }

                //if (!string.IsNullOrEmpty(currentExperimentPrefabPath))
                //{
                //    currentExperimentPrefabPath = "";
                //    SendExperimentStatus(2);
                //}

                monoBehaviour.StartCoroutine(LoadAsset(experimentInfo));
            }
            else
            {
                //Debug.LogError("加载现有资源完成:" + currentExperiment.Name);

                OnLoadComplete();
            }
        }

        IEnumerator LoadAsset(ExperimentInfo experimentInfo)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(1);

            yield return new WaitForSeconds(0.1f);

            yield return netAsset.LoadAsset(experimentInfo, () =>
            {

                //Debug.LogError("加载新资源完成：" + experimentInfo.Name);
                //  currentExperimentPrefabPath = experimentInfo.PrefabPath;
                currentExperiment = new ExperimentInfo()
                {
                    Id = experimentInfo.Id,
                    ExperimentStatus = experimentInfo.ExperimentStatus,
                    Name = experimentInfo.Name,
                    OwnProject = experimentInfo.OwnProject,
                    PrefabPath = experimentInfo.PrefabPath
                };

                //发送加载完成
                OnLoadComplete();//加载完成
            });
        }
    }
}
