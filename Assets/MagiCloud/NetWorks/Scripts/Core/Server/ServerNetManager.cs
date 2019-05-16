using Google.Protobuf;
using MagiCloud.Core;

namespace MagiCloud.NetWorks
{
    /// <summary>
    /// 服务端/客户端 管理端
    /// </summary>
    public class ServerNetManager
    {
        protected ServerConnection connection;
        protected EventPool eventPool;  //事件池

        protected ExperimentInfo currentExperiment; //当前发送的实验
        protected WindowsManager windowsManager;        //实验项目窗口管理

        protected MBehaviour behaviour;

        protected bool IsConnect = false; //是否连接成功

        public ServerNetManager()
        {
#if UNITY_ANDROID
            windowsManager = new AndroidWindowsManager();
#elif UNITY_IOS
            windowsManager=new IosWindowsManager();
#else
            windowsManager = new ExperimentWindowsManager();
#endif

            connection = new ServerConnection();
            eventPool = new EventPool(connection.messageDistribution);


            behaviour = new MBehaviour();
            behaviour.OnUpdate_MBehaviour(() =>
            {
                if (connection != null)
                    connection.Update();

                if (UnityEngine.Time.frameCount % 30==0)
                {
                    if (windowsManager.IsTopping)
                    {
                        MOperateManager.ActiveHandController(true, 0);
                    }
                    else
                    {
                        MOperateManager.ActiveHandController(false, 0);
                    }
                }
            });
        }

        /// <summary>
        /// 往客户端发送数据
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="data"></param>
        public virtual void OnSendData<T1, T2>(T2 data)
            where T1 : EventBase where T2 : IMessage
        {
            if (IsConnect)
                eventPool.GetEvent<T1>().Send(connection, data);
        }

        public virtual void OnDestroy()
        {
            behaviour.OnExcuteDestroy();

            if (connection != null)
                connection.Close();
        }
    }
}
