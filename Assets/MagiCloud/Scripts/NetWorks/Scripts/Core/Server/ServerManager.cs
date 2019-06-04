using Google.Protobuf;
using System;

namespace MagiCloud.NetWorks
{
    public class ServerManager : ServerNetManager
    {

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
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="proto"></param>
        public virtual void OnExperimentReceipt(int sender, IMessage proto)
        {
            ExperimentInfo info = proto as ExperimentInfo;

            switch (info.ExperimentStatus)
            {
                //错误
                case -1:
                    windowsManager.ExitExe();
                    currentExperiment = null;

                    break;
                //加载成功
                case 1:
                    //关闭loading

                    //UnityEngine.Debug.Log("加载成功");

                    break;
                case 2:
                    //UnityEngine.Debug.Log("执行：返回");
                    windowsManager.SetTop();
                    break;
                //重置
                case 3:
                    //UnityEngine.Debug.Log("执行：重置");
                    //windowsManager.SetTop();
                    SelectExperiment(currentExperiment, windowsManager.processPath);
                    break;
                //关闭
                case 4:

                    //UnityEngine.Debug.Log("执行：关闭");

                    currentExperiment = null;

                    windowsManager.SetTop();
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
                SelectExperimentInfo(experiment, productExePath);

                windowsManager.SetTop();

                UnityEngine.Debug.Log("实验值：" + experiment.Name);

                OnSendData<ExperimentRequestEvent, ExperimentInfo>(currentExperiment);
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