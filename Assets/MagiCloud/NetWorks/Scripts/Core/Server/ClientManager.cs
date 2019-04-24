using System;
using Google.Protobuf;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace MagiCloud.NetWorks
{
    public class ClientManager :MonoBehaviour
    {
        private ServerConnection clientConnection;
        private EventPool clientEventPool;  //事件池
        private GameObject curExpPrefab;    //当前实验物体
        private string curPrefabPath;       //当前预制文件路径
        ExperimentInfo curInfo;             //当前实验信息

        private IntPtr curWindowIntPtr;                     //自身窗口
        private IntPtr curID;
        private IntPtr foreID;

        public AssetBundleManager BundleManager;

        [Header("Bundle的本地/网络路径,也可不填写")]
        public string bundleUri;
        //Request资源名称，用于优先下载指定的
        public List<string> requestDownloadBundleNames;

        public Text txt;

        private void Awake()
        {
            curWindowIntPtr = SystemDllHelper.GetActiveWindow();
            curID = SystemDllHelper.GetCurrentThreadId();
            foreID = SystemDllHelper.GetWindowThreadProcessId(curWindowIntPtr, default);
        }

        private IEnumerator Start()
        {
            if (BundleManager == null)
                BundleManager = new AssetBundleManager(bundleUri);

            if (!string.IsNullOrEmpty(bundleUri))
                yield return BundleManager.Download(requestDownloadBundleNames);

            clientConnection = new ServerConnection();
            clientEventPool=new EventPool(clientConnection.messageDistribution);
            clientEventPool.GetEvent<ExperimentRequestEvent>().AddReceiveEvent(OnExpReq);
            clientConnection.Connect("127.0.0.1",8888);
        }

        /// <summary>
        /// 接收到实验请求
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="proto">ExperimentInfo</param>
        private void OnExpReq(int sender,IMessage proto)
        {
            ExperimentInfo info = proto as ExperimentInfo;

            if (curPrefabPath != info.PrefabPath)
            {
                if (curExpPrefab != null)
                {
                    Destroy(curExpPrefab);
                    curExpPrefab = null;
                    curInfo = null;
                }

                txt.text += "实验信息：" + info.Name + "\r\n";

                BundleManager.LoadAsset<GameObject>(new string[1] { info.PrefabPath }, (targets) =>
                {
                    //加载资源
                    curExpPrefab = (GameObject)Instantiate(targets[0]);

                    if (curExpPrefab == null)
                    {
                        txt.text += "实例化的预制物体为Null";
                    }
                    else
                    {
                        txt.text += "实例化的物体不为Null:" + curExpPrefab.name;
                    }

                    curPrefabPath = info.PrefabPath;
                    curInfo = info;

                    if (!IsInvoking("LoadComplete"))
                    {
                        Invoke("LoadComplete", 0.5f);
                    }
                });
            }
            else
            {
                LoadComplete();
            }
        }
        /// <summary>
        /// 资源加载完成
        /// </summary>
        public void LoadComplete()
        {
            SystemDllHelper.AttachThreadInput(curID, foreID, 1);
            SystemDllHelper.ShowWindow(curWindowIntPtr, 3);
            SystemDllHelper.SetWindowPos(curWindowIntPtr, -1, 0, 0, 0, 0, 1 | 2);
            SystemDllHelper.SetWindowPos(curWindowIntPtr, -2, 0, 0, 0, 0, 1 | 2);
            bool max = SystemDllHelper.SetForegroundWindow(curWindowIntPtr);
            SystemDllHelper.AttachThreadInput(curID, foreID, 0);
        }

        /// <summary>
        /// 返回
        /// </summary>
        public void OnBack()
        {
            curInfo.IsBack = true;
            clientEventPool.GetEvent<ExperimentReceiptEvent>().Send(clientConnection,curInfo);
        }

        private void Update()
        {
            clientConnection.Update();
        }

        private void OnDestroy()
        {
            clientConnection.Close();
        }
    }
}
