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
        private IntPtr curWindowIntPtr;                     //自身窗口

        private ExperimentWindowsManager experiment;        //实验项目窗口管理

        bool clientConnect = false;
        private void Start()
        {
            experiment = new ExperimentWindowsManager();
            curWindowIntPtr =SystemDllHelper.GetForegroundWindow();
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
            SystemDllHelper.SetForegroundWindow(curWindowIntPtr);
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
            experiment.Select(i);
            SystemDllHelper.SetForegroundWindow(curWindowIntPtr);
            SendExpInfo();
        }

        /// <summary>
        /// 发送实验信息数据
        /// </summary>
        private void SendExpInfo()
        {
            if (clientConnect)
                controllerEventPool.GetEvent<ExperimentRequestEvent>().Send(connection,experiment.CurExpInfo);
        }

        private void OnDestroy()
        {
            experiment.Exit();
            connection.Close();
        }
    }
}
