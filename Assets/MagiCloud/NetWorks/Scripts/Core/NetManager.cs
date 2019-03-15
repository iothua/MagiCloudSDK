
namespace MagiCloud.NetWorks.Server
{
    public class ServerNetManager
    {
        public static MCConnetion connetion = new MCConnetion();

        public static void Update()
        {
            connetion.Update();
        }
    }
}
namespace MagiCloud.NetWorks.Client
{
    public class ClientNetManager
    {
        public static MCConnetion connetion = new MCConnetion();

        public static void Update()
        {
            connetion.Update();
        }
    }
}