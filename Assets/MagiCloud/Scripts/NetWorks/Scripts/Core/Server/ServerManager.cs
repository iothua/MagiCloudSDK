using Google.Protobuf;
using System;
using Loxodon.Framework.Messaging;
using UnityEngine;

namespace MagiCloud.NetWorks
{
    public class ServerManager : ServerNetManager
    {

        public const string StartLoading = "start_loading";
        public const string LoadingComplete = "loading_complete";
        public const string LoadingError = "loading_error";
        public const string StartExperiment = "start_experiment";
        
        public event Action<ExperimentInfo> EventExperimentRecipt;

        public ServerManager() : base()
        {

            eventPool.GetEvent<ConnectEvent>().AddReceiveEvent(OnConnectEvent);
            eventPool.GetEvent<ExperimentReceiptEvent>().AddReceiveEvent(OnExperimentReceipt);


            connection.Connect("127.0.0.1", 8888);
        }

        private void OnConnectEvent(int sender, IMessage proto)
        {
            IsConnect = true;

            //发送数据
            OnSendData<ExperimentRequestEvent, ExperimentInfo>(currentExperiment);
        }

        /// <summary>
        /// 实验接收
        ///    0xFF:错误
        ///    0x00:默认加载
        ///    0x01:成功
        ///    0x02:返回
        ///    0x03:重置
        ///    0xAA:应答实验
        ///    0xCC:关闭
        ///  
        ///
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="proto"></param>
        public virtual void OnExperimentReceipt(int sender, IMessage proto)
        {
            ExperimentInfo info = proto as ExperimentInfo;

            switch (info.ExperimentStatus)
            {
                //错误
                case 255:
                    
                    windowsManager.ExitExe();
                    currentExperiment = null;

                    Messenger.Default.Publish(LoadingError, info);
                    
                    break;
                //加载成功
                case 1:

                    //通知另一个端口，表示加载成功
                    info.ExperimentStatus = 170;

                    OnSendData<ExperimentRequestEvent, ExperimentInfo>(info);
                    //Messenger.Default.Publish(LoadingComplete, info.ExperimentStatus);

                    Debug.LogError("命令：加载成功 " + info.Name);

                    break;
                case 2:
                    //返回
                    windowsManager.SetTop();
                    Debug.LogError("命令：返回 " + info.Name);
                    break;
                //重置
                case 3:
                    SelectExperiment(currentExperiment, windowsManager.processPath);
                    Debug.Log("命令：重置 " + info.Name);
                    break;
                //关闭
                case 204:

                    currentExperiment = null;

                    windowsManager.SetTop();

                    Debug.Log("命令：关闭程序");
                    break;
                default:
                    break;
            }

            if (EventExperimentRecipt != null)
                EventExperimentRecipt(info);
        }

        /// <summary>
        /// 选择实验
        /// </summary>
        /// <param name="experiment">Experiment.</param>
        /// <param name="productExePath">Product exe path.</param>
        public void SelectExperiment(ExperimentInfo experiment, string productExePath)
        {
            try
            {
                /*
                 * 
                 
                if (currentExperiment!=null&&currentExperiment.Id == experiment.Id)
                {
                    Debug.Log("值相同，直接跳过");
                }
                else
                {
                    Messenger.Default.Publish(StartLoading, experiment);
                
                    SelectExperimentInfo(experiment, productExePath);

                    windowsManager.SetTop();
                    
                }
                
                */
                
                //Messenger.Default.Publish(StartLoading, experiment);
                
                SelectExperimentInfo(experiment, productExePath);

                OnSendData<ExperimentRequestEvent, ExperimentInfo>(currentExperiment);
                windowsManager.SetTop();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void SelectExperimentInfo(ExperimentInfo experiment, string productExePath)
        {
            if (experiment == null)
                throw new Exception("实验数据对象为Null");

            if (!System.IO.File.Exists(productExePath))
                throw new Exception("请先进行下载");

            if (currentExperiment == null || experiment.OwnProject != currentExperiment.OwnProject)
            {
                windowsManager.ExitExe();

                windowsManager.processPath = productExePath;
                windowsManager.OpenExe();
            }

            currentExperiment = experiment;
        }

        public override void OnDestroy()
        {
            windowsManager.ExitExe(() =>
            {

                if (!IsConnect) return;

                eventPool.GetEvent<BreakConnectEvent>().Send(connection, new ConnectInfo() { Id = 0 });

            });
            base.OnDestroy();
        }
    }
}