using System;
using System.Net.Sockets;
using Google.Protobuf;

namespace MCServer
{
    /// <summary>
    /// 连接器复制服务端和客户端的数据连接
    /// </summary>
    public class Connect
    {
        public bool isUse = false;
        public Socket socket;                       //复制服务端向客户端发送或接收数据
        public long lastTickTime = long.MinValue;   //计时
        public const int BUFFER_SIZE = 1024;        //最大数据缓存长度
        public byte[] buffer;                       //缓存
        public int bufferCount = 0;                 //当前有效缓存长度
        public byte[] lenBytes = new byte[sizeof(UInt32)];  //缓存中的长度字节数组
        public byte[] typeBytes = new byte[sizeof(UInt32)]; //缓存中的消息类型字节数组
        public Int32 msgLength = 0;

        public Connect()
        {
            buffer=new byte[BUFFER_SIZE];
        }

        public void Init(Socket socket)
        {
            this.socket=socket;
            isUse=true;
            bufferCount=0;
            MessageDistribution.AddListener((int)EnumCmdID.Heartbeat,HeartBeatCallback);
            lastTickTime=TimeHelper.GetTimeStamp();
        }

        private void HeartBeatCallback(ProtobufTool protobuf)
        {
            //Console.WriteLine("收到心跳包");
            lastTickTime=TimeHelper.GetTimeStamp();
        }

        /// <summary>
        /// 剩余的Buff
        /// </summary>
        /// <returns></returns>
        public int BuffRemain()
        {
            return BUFFER_SIZE-bufferCount;
        }
        public string GetAddress()
        {
            if (!isUse) return "无法获取地址";
            return socket.RemoteEndPoint.ToString();
        }



        public void Close()
        {
            if (!isUse) return;
            Console.WriteLine(GetAddress()+"断开连接");
            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
            isUse=false;
        }
    }
}
