using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagiCloud.NetWorks
{
    /// <summary>
    /// 服务端/客户端 管理端
    /// </summary>
    public class ServerNetManager
    {
        public ServerConnection serverConnection = new ServerConnection();

        public void Update()
        {
            serverConnection.Update();
        }
    }
}
