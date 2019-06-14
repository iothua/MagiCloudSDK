using System;
using System.Net.Sockets;

namespace MagiCloud.NetWorks
{
    /// <summary>
    /// 连接信息
    /// </summary>
    public class ConnectControl :IConnect
    {
        public int id;
        public bool isUse = false;

        /// <summary>
        /// 复制服务器向客户端发送或接收消息
        /// </summary>
        public Socket socket;

        public long lastTickTime = long.MinValue; //计时
        public const int Buffer_Size = 1024;//最大数据缓存长度

        public byte[] buffer; //缓存
        public int bufferCount = 0; //当前有效缓存长度
        public byte[] lenBytes = new byte[sizeof(uint)]; //缓存中的长度字节数组
        public byte[] typeBytes = new byte[sizeof(uint)]; //缓存中的消息类型字节数组

        public int msgLength = 0;

        public ConnectControl()
        {
            buffer = new byte[Buffer_Size];
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="id"></param>
        public void Initialize(Socket socket,int id)
        {
            this.id = id;
            this.socket = socket;

            isUse = true;
            bufferCount = 0;
            MessageDistributionControl.Instance.AddListener((int)CommandID.HeartbeatPacketRequest,HeartBeatCallback);
            lastTickTime = TimeHelper.GetTimeStamp();
        }

        private void HeartBeatCallback(int connectID,ProtobufTool protobuf)
        {
            lastTickTime = TimeHelper.GetTimeStamp();
        }

        public int BUffRemain()
        {
            return Buffer_Size - bufferCount;
        }


        public string GetAddress()
        {
            if (!isUse) return "无法获取地址";

            return socket.RemoteEndPoint.ToString();
        }

        public void Close()
        {
            if (!isUse) return;
            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
            isUse = false;
        }

        public void Send(ProtobufTool protobuf)
        {
            if (!socket.Connected)
            {
                isUse=false;
                return;
            }
            try
            {
                socket.BeginSend(protobuf.bytes,0,protobuf.byteLength,SocketFlags.None,SendCallback
                ,protobuf);
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError(e);
                //throw e;
            }
        }

        private void SendCallback(IAsyncResult ar)
        {

            try
            {
                ProtobufTool connect = ar.AsyncState as ProtobufTool;
                socket.EndSend(ar);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
