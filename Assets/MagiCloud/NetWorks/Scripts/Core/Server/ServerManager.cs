using UnityEngine;
using UnityEngine.UI;

namespace MagiCloud.NetWorks
{
    public class ServerManager : MonoBehaviour
    {
        private ServerConnection connection;

        private ExperimentEventServer eventServer;

        public Text txtSocket;

        private void Start()
        {
            connection = new ServerConnection();

            txtSocket.text = "Server：";

            eventServer = new ExperimentEventServer(connection, (log) =>
            {
                if (txtSocket != null)
                    txtSocket.text += log + " ";
            });

            connection.Connect("127.0.0.1", 8888);
        }

        private void Update()
        {
            connection.Update();
            if (Input.GetKeyDown(KeyCode.Q))
            {
                eventServer.SendRequest(new ExperimentInfo()
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
