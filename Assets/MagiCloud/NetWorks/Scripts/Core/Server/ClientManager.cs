using System;
using UnityEngine;
using UnityEngine.UI;

namespace MagiCloud.NetWorks
{
    public class ClientManager : MonoBehaviour
    {
        private ServerConnection serverConnection;
        private ExperimentEventServer eventServer;

        public Text txtSocket;

        private void Start()
        {
            txtSocket.text = "Client：";
            serverConnection = new ServerConnection();

          //  distributionServer = new MessageDistributionServer();
            eventServer = new ExperimentEventServer( serverConnection,(log)=> {

                if (txtSocket != null)
                    txtSocket.text += log + " ";

            });

            serverConnection.Connect("127.0.0.1", 8888);

        }

        private void Update()
        {
            serverConnection.Update();
        }

        private void OnDestroy()
        {
            serverConnection.Close();
        }
    }
}
