using System;
using System.Net.Sockets;
using UnityEngine;

namespace MagiCloud.NetWorks
{
    /// <summary>
    /// 用于客户端向服务器连接的连接器，具有发送协议和解析协议并分发的功能
    /// </summary>
    public class ServerConnection :IConnect
    {
        public MessageDistributionServer messageDistribution;

        /// <summary>
        /// 缓冲区最大长度
        /// </summary>
        public const int Buffer_Size = 1024;

        public Socket socket;

        /// <summary>
        /// 缓存
        /// </summary>
        public byte[] readBuffer = new byte[Buffer_Size];

        /// <summary>
        /// 当前缓存数量
        /// </summary>
        public int bufferCount = 0;
        /// <summary>
        /// 粘包分包
        /// </summary>
        public byte[] lenBytes = new byte[sizeof(int)];

        /// <summary>
        /// 消息长度
        /// </summary>
        public int msgLength = 0;

        /// <summary>
        /// 协议
        /// </summary>
        public ProtobufTool protobuf = new ProtobufTool();

        /// <summary>
        /// 心跳时间
        /// </summary>
        public float lastTickTime = 0;

        public float heartBeatTime = 2;
        public HeartBeatInfo hearInfo;

        public enum ConnectStatus
        {
            None,
            Connected
        }

        public ConnectStatus status = ConnectStatus.None;

        public ServerConnection()
        {
            messageDistribution = new MessageDistributionServer();
        }

        public bool Connect(string ip,int port)
        {
            try
            {
                socket = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
                socket.Connect(ip,port);
                BeginReceiveMessage();

                status = ConnectStatus.Connected;

                return true;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private void BeginReceiveMessage()
        {
            socket.BeginReceive(readBuffer,bufferCount,Buffer_Size - bufferCount,SocketFlags.None,(asyncResult) =>
        {
            try
            {
                int count = socket.EndReceive(asyncResult);
                bufferCount = bufferCount + count;
                ProcessData();

                BeginReceiveMessage();

            }
            catch (Exception e)
            {
                status = ConnectStatus.None;
                Debug.Log("接收信息失败：" + e.Message);
            }
        },socket);
        }

        /// <summary>
        /// 数据处理
        /// </summary>
        private void ProcessData()
        {
            //如果小于长度字节
            if (bufferCount < sizeof(int) + sizeof(int))
                return;

            //消息长度4个字节，消息类型4个字节
            Array.Copy(readBuffer,lenBytes,sizeof(int));

            msgLength = BitConverter.ToInt32(lenBytes,0) - sizeof(int);

            if (bufferCount < msgLength + sizeof(int))
                return;

            ProtobufTool proto = protobuf.Read(readBuffer);

            Debug.Log("收到消息：" + (CommandID)proto.type);

            lock (messageDistribution.msgList)
            {
                messageDistribution.msgList.Add(new ReceiveMessageStruct(0,proto));
            }

            //清除已处理的消息
            int count = bufferCount - msgLength - sizeof(int) - sizeof(int);
            Array.Copy(readBuffer,sizeof(int) + msgLength,readBuffer,0,count);
            bufferCount = count;

            if (bufferCount > 0)
            {
                ProcessData();
            }
        }

        public void BeginSendMessage(ProtobufTool protobuf)
        {
            try
            {
                socket.BeginSend(protobuf.bytes,0,protobuf.byteLength,SocketFlags.None,(asyncResult) =>
            {

            },socket);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void Update()
        {
            //消息
            messageDistribution.Update();

            //心跳
            if (status == ConnectStatus.Connected)
            {
                if (Time.time - lastTickTime > heartBeatTime)
                {
                    ProtobufTool protobuf = GetHeatBeatProtocol();

                    BeginSendMessage(protobuf);
                    lastTickTime = Time.time;
                }
            }
        }

        public ProtobufTool GetHeatBeatProtocol()
        {
            ProtobufTool protobuf = new ProtobufTool();

            hearInfo = new HeartBeatInfo()
            {
                CurrentTime = 0,
                Hostip = "127.0.0.1"
            };

            protobuf.CreatData((int)CommandID.HeartbeatPacketRequest,hearInfo);

            return protobuf;
        }

        public bool Close()
        {
            try
            {
                socket.Close();
                return true;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void Send(ProtobufTool protobuf)
        {
            BeginSendMessage(protobuf);
        }
    }
}

