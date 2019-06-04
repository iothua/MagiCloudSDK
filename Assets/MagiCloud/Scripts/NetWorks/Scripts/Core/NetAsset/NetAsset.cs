using System;
using System.Collections;
using UnityEngine;

namespace MagiCloud.NetWorks
{
    public class NetAsset
    {
        protected Action OnComplete;

        public ExperimentInfo Experiment;
        public bool IsComplete;

        public ClientManager clientManager;

        public GameObject curExpPrefab;
        public string curPrefabPath;

        public NetAsset(ClientManager clientManager)
        {
            this.clientManager = clientManager;
        }

        public virtual IEnumerator LoadAsset(ExperimentInfo experimentInfo,Action LoadComplete)
        {
            this.OnComplete = LoadComplete;
            this.Experiment = experimentInfo;

            yield return 0;
        }
    }
}
