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

            ExperimentInfo experimentInfo = proto as ExperimentInfo;

            if (currentExperiment != null && experimentInfo.ExperimentStatus == 170 && currentExperiment.Id == experimentInfo.Id)
            {
                Debug.LogError("接收0xAA,表示加载成功");
                windowsManager.SetTop();
                return;
            }

            if (currentExperiment == null || experimentInfo.PrefabPath != currentExperiment.PrefabPath)
            {
                if (currentExperiment != null)
                {
                    currentExperiment = null;

                    SendExperimentStatus(2);
                }

                monoBehaviour.StartCoroutine(LoadAsset(experimentInfo));

                Debug.LogError("加载资源：" + experimentInfo.Name);
            }
            else
            {
                Debug.LogError("相同资源，直接跳过");
                //Debug.LogError("加载现有资源完成:" + currentExperiment.Name);
                windowsManager.SetTop();
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
