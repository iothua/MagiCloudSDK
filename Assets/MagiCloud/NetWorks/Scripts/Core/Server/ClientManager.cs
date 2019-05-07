using System;
using Google.Protobuf;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Loxodon.Framework.Bundles;

namespace MagiCloud.NetWorks
{
    public class ClientManager :MonoBehaviour
    {
        protected ServerConnection clientConnection;
        protected EventPool clientEventPool;  //事件池
        protected GameObject curExpPrefab;    //当前实验物体
        protected string curPrefabPath;       //当前预制文件路径
        protected ExperimentInfo curInfo;             //当前实验信息

        WindowsManager windowsManager;

        public AssetBundleManager BundleManager;

        [Header("Bundle的本地/网络路径,也可不填写")]
        public string bundleUri;
        //Request资源名称，用于优先下载指定的
        public List<string> requestDownloadBundleNames;

        public static ClientManager Manager;

        public event Action<int> EventExperimentStatus;

        private void Awake()
        {
            Manager = this;
#if UNITY_ANDROID
            windowsManager = new AndroidWindowsManager();
#elif UNITY_IOS
            experiment=new IosWindowsManager();
#else
              experiment = new ExperimentWindowsManager();
#endif

        }

        protected virtual IEnumerator Start()
        {
            if (BundleManager == null)
                BundleManager = new AssetBundleManager(bundleUri);

            if (!string.IsNullOrEmpty(bundleUri))
                yield return BundleManager.Download(requestDownloadBundleNames);

            clientConnection = new ServerConnection();
            clientEventPool=new EventPool(clientConnection.messageDistribution);
            clientEventPool.GetEvent<ExperimentRequestEvent>().AddReceiveEvent(OnExpReq);
            clientEventPool.GetEvent<BreakConnectEvent>().AddReceiveEvent(OnExitReq);
            clientConnection.Connect("127.0.0.1",8888);

            DontDestroyOnLoad(gameObject);

            //InvokeRepeating("DetectionWindows", 1.0f, 5.0f);
        }

        void DetectionWindows()
        {
            if (windowsManager.IsTopping())
            {
                MOperateManager.ActiveHandController(true);
            }
            else
            {
                MOperateManager.ActiveHandController(false);
            }
        }
        private void OnExitReq(int sender,IMessage proto)
        {
            Application.Quit();
        }
        /// <summary>
        /// 接收到实验请求
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="proto">ExperimentInfo</param>
        protected void OnExpReq(int sender,IMessage proto)
        {
            ExperimentInfo info = proto as ExperimentInfo;

            if (curPrefabPath != info.PrefabPath)
            {
                if (curExpPrefab != null)
                {
                    //Destroy(curExpPrefab);

                    curExpPrefab = null;
                    curInfo = null;

                    if (EventExperimentStatus != null)
                        EventExperimentStatus(2);
                }

                UnityEngine.SceneManagement.SceneManager.LoadScene(1);

                //切换到实验场景中
                StartCoroutine(OnLoadAsset(info));
            }
            else
            {
                LoadComplete();
            }
        }

        IEnumerator OnLoadAsset(ExperimentInfo info)
        {
            yield return 0;

            LoadAsset(info);
        }

        protected virtual void LoadAsset(ExperimentInfo info)
        {

            BundleManager.LoadAsset<GameObject>(new string[1] { info.PrefabPath },(targets) =>
           {
               //加载资源
               curExpPrefab = (GameObject)Instantiate(targets[0]);

               curPrefabPath = info.PrefabPath;
               curInfo = info;

               if (!IsInvoking("LoadComplete"))
               {
                   Invoke("LoadComplete",0.5f);
               }
           });
        }

        public void AssetBundleLoad(string prefabPath,ExperimentInfo info)
        {
            Debug.LogError("AssetBundleLoad:" + info);

            BundleManager.LoadAsset<GameObject>(new string[1] { prefabPath },(targets) =>
           {
               //加载资源
               curExpPrefab = (GameObject)Instantiate(targets[0]);

               curPrefabPath = info.PrefabPath;
               curInfo = info;

               if (!IsInvoking("LoadComplete"))
               {
                   Invoke("LoadComplete",0.5f);
               }
           });
        }

        public void ResourcesLoad(string prefabPath,ExperimentInfo info)
        {
            LocalResources localResources = new LocalResources();

            var target = localResources.LoadAsset<GameObject>(prefabPath);

            if (target == null)
                Debug.LogError("路径物体不存在");

            //加载资源
            curExpPrefab = (GameObject)Instantiate(target);


            curPrefabPath = info != null ? info.PrefabPath : string.Empty;
            curInfo = info;

            if (!IsInvoking("LoadComplete"))
            {
                Invoke("LoadComplete",0.5f);
            }
        }

        /// <summary>
        /// 资源加载完成
        /// </summary>
        public void LoadComplete()
        {
            windowsManager.SetTop();
        }

        /// <summary>
        /// 返回
        /// </summary>
        public void OnBack()
        {
            curInfo.ExperimentStatus = 1;
            clientEventPool.GetEvent<ExperimentReceiptEvent>().Send(clientConnection,curInfo);

            MOperateManager.ActiveHandController(false);
        }

        public void OnExit()
        {
            curInfo.ExperimentStatus = 3;
            clientEventPool.GetEvent<ExperimentReceiptEvent>().Send(clientConnection,curInfo);
        }

        /// <summary>
        /// 返回
        /// -1：失败
        /// 0：成功
        /// 1：返回
        /// 2：重置
        /// </summary>
        public void OnExperimentBack()
        {
            OnBack();
        }

        public void OnExperimentReset()
        {
            curInfo.ExperimentStatus = 2;
            clientEventPool.GetEvent<ExperimentReceiptEvent>().Send(clientConnection,curInfo);

            if (EventExperimentStatus != null)
                EventExperimentStatus(2);

            //curInfo = null;
            curPrefabPath = string.Empty;
        }

        protected virtual void Update()
        {
            if (clientConnection != null)
                clientConnection.Update();
        }

        private void OnDestroy()
        {
            OnExit();

            if (clientConnection != null)
                clientConnection.Close();
        }
    }
}
