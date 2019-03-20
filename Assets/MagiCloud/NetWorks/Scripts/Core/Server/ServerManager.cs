using System;
using Google.Protobuf;
using UnityEngine;
using UnityEngine.UI;

namespace MagiCloud.NetWorks
{
    public class ServerManager :MonoBehaviour
    {
        private ServerConnection connection;

        // private ExperimentEventServer eventServer;
        private EventPool eventPool;
        public Text txtSocket;

        private void Start()
        {
            connection = new ServerConnection();
            eventPool=new EventPool(connection,connection.messageDistribution);

            eventPool.GetEvent<ExperimentReceiptEvent>().AddReceiveEvent(OnHandler);

            txtSocket.text = "Server：";

            // eventServer = new ExperimentEventServer(connection,(log) =>
            //{
            //    if (txtSocket != null)
            //        txtSocket.text += log + " ";
            //});

            connection.Connect("127.0.0.1",8888);
        }

        private void OnHandler(IMessage proto)
        {

        }

        private void Update()
        {
            connection.Update();
            if (Input.GetKeyDown(KeyCode.Q))
            {
                eventPool.GetEvent<ExperimentReceiptEvent>().Send(new ExperimentInfo()
                {
                    Id = 0,
                    Name = "高锰酸钾制取氧气",
                    PrefabPath = "ClientDemo/Prefabs/TestDemo.prefab",
                    OwnProject = "科学",
                    IsBack = false
                });
            }
        }

        private void OnDestroy()
        {
            connection.Close();
        }
    }
}
