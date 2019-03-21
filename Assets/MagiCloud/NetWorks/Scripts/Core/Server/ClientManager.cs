using System;
using Google.Protobuf;
using UnityEngine;
using UnityEngine.UI;

namespace MagiCloud.NetWorks
{
    public class ClientManager :MonoBehaviour
    {
        private ServerConnection clientConnection;
        private EventPool clientEventPool;  //事件池
        private GameObject curExpPrefab;    //当前实验物体
        private string curPrefabPath;       //当前预制文件路径
        ExperimentInfo curInfo;             //当前实验信息
        IntPtr ptr;
        private void Start()
        {
            clientConnection = new ServerConnection();
            clientEventPool=new EventPool(clientConnection.messageDistribution);
            clientEventPool.GetEvent<ExperimentRequestEvent>().AddReceiveEvent(OnExpReq);
            clientConnection.Connect("127.0.0.1",8888);
            ptr=SystemDllHelper.GetForegroundWindow();
        }

        /// <summary>
        /// 接收到实验请求
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="proto">ExperimentInfo</param>
        private void OnExpReq(int sender,IMessage proto)
        {
            ExperimentInfo info = proto as ExperimentInfo;

            if (curPrefabPath!=info.PrefabPath)
            {
                if (curExpPrefab!=null)
                {
                    Destroy(curExpPrefab);
                    curExpPrefab=null;
                }
                //加载资源
                curExpPrefab=(GameObject)Instantiate(Resources.Load(info.PrefabPath));
                curPrefabPath=info.PrefabPath;
                curInfo=info;
            }
            LoadComplete();
        }
        /// <summary>
        /// 资源加载完成
        /// </summary>
        public void LoadComplete()
        {
            SystemDllHelper.SetForegroundWindow(ptr);
        }

        /// <summary>
        /// 返回
        /// </summary>
        public void OnBack()
        {
            curInfo.IsBack=true;
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
