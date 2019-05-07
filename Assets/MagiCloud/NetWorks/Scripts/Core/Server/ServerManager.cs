using System;
using Google.Protobuf;
using UnityEngine;
using UnityEngine.UI;

namespace MagiCloud.NetWorks
{

    public class ServerManager :MonoBehaviour
    {
        private ServerConnection connection;
        private EventPool controllerEventPool;              //事件池

        private WindowsManager windowsManager;        //实验项目窗口管理

        bool clientConnect = false;
        private void Start()
        {
#if UNITY_ANDROID
            windowsManager = new AndroidWindowsManager();
#elif UNITY_IOS
            windowsManager=new IosWindowsManager();
#else
              windowsManager = new ExperimentWindowsManager();
#endif
            connection = new ServerConnection();

            controllerEventPool=new EventPool(connection.messageDistribution);
            ProcessHelper process = new ProcessHelper();

            controllerEventPool.GetEvent<ConnectEvent>().AddReceiveEvent(OnConnectEvent);
            controllerEventPool.GetEvent<ExperimentReceiptEvent>().AddReceiveEvent(OnExpRec);

            connection.Connect("127.0.0.1",8888);
        }


        /// <summary>
        /// 有客户端连接
        /// </summary>
        /// <param name="proto"></param>
        private void OnConnectEvent(int sender,IMessage proto)
        {
            //print("客户端已连接");
            clientConnect=true;
            //发送数据
            SendExpInfo();
        }

        /// <summary>
        /// 实验结束后
        /// </summary>
        /// <param name="proto"></param>
        private void OnExpRec(int sender,IMessage proto)
        {
            //该窗口置顶
            windowsManager.SetTop();
        }

        private void Update()
        {
            connection.Update();
            //if (Input.GetKeyDown(KeyCode.Q))
            //{
            //    //开启实验0
            //    SelectExpInfo(0);
            //}
            //if (Input.GetKeyDown(KeyCode.W))
            //{
            //    //开启实验1
            //    SelectExpInfo(1);
            //}
        }

        /// <summary>
        /// 选择实验启动,如果实验已连接,直接发送实验信息,否则在连接回调事件中等待发送
        /// </summary>
        public void SelectExpInfo(int i = 0)
        {
            windowsManager.Select(i,SendExitReq);
            windowsManager.SetTop();
            SendExpInfo();
        }

        /// <summary>
        /// 发送实验信息数据
        /// </summary>
        private void SendExpInfo()
        {
            if (clientConnect)
                controllerEventPool.GetEvent<ExperimentRequestEvent>().Send(connection,windowsManager.CurExpInfo);
        }
        /// <summary>
        /// 发送关闭请求
        /// </summary>
        private void SendExitReq()
        {
            if (clientConnect)
                controllerEventPool.GetEvent<BreakConnectEvent>().Send(connection,new ConnectInfo() { Id=0 });
        }
        private void OnDestroy()
        {
            windowsManager.ExitOther(SendExitReq);
            connection.Close();
        }
    }
}
