using System;
using System.Net.Sockets;
using UnityEngine;

namespace MagiCloud.NetWorks
{
    public class MCConnetion
    {
        public MessageDistribution messageDistribution;
        /// <summary>
        /// 缓冲区最大长度
        /// </summary>
        public const int BUFFER_SIZE = 1024;
        public Socket socket;
        /// <summary>
        /// 缓存
        /// </summary>
        public byte[] readBuffer = new byte[BUFFER_SIZE];
        /// <summary>
        /// 当前缓存数量
        /// </summary>
        public int bufferCount = 0;
        /// <summary>
        /// 粘包分包
        /// </summary>
        public byte[] lenBytes = new byte[sizeof(UInt32)];
        /// <summary>
        /// 消息长度
        /// </summary>
        public Int32 msgLength = 0;
        /// <summary>
        /// 协议
        /// </summary>
        public ProtobufTool protocol = new ProtobufTool();
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

        public MCConnetion()
        {
            messageDistribution=new MessageDistribution();
        }
        /// <summary>
        /// 连接
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public bool Connect(string ip,int port)
        {
            try
            {
                socket=new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
                socket.Connect(ip,port);
                BeginReceiveMessage();
                //连接成功
                Debug.Log("连接成功:"); //连接失败
                status=ConnectStatus.Connected;
                return true;
            }
            catch (Exception e)
            {
                Debug.Log("连接失败:"+e.Message); //连接失败
                return false;
            }
        }

        private void BeginReceiveMessage()
        {
            socket.BeginReceive(readBuffer,bufferCount,BUFFER_SIZE-bufferCount,SocketFlags.None,ReceiveCallback,socket);
        }

        /// <summary>
        /// 异步Socket回调
        /// </summary>
        /// <param name="ar"></param>
        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                int count = socket.EndReceive(ar);
                bufferCount=bufferCount+count;
                ProcessData();
                BeginReceiveMessage();
            }
            catch (Exception e)
            {
                //接收失败
                Debug.Log("接收失败:"+e.Message);
                status=ConnectStatus.None;
            }
        }

        /// <summary>
        /// 数据处理
        /// </summary>
        private void ProcessData()
        { //如果小于长度字节
            if (bufferCount<sizeof(Int32)+sizeof(Int32))
                return;
            //////消息长度4个字节，消息类型4个字节
            Array.Copy(readBuffer,lenBytes,sizeof(Int32));
            //真实消息长度
            msgLength=BitConverter.ToInt32(lenBytes,0)-sizeof(Int32);
            if (bufferCount<msgLength+sizeof(Int32))
                return;
            //协议解码
            ProtobufTool proto = protocol.Read(readBuffer);
            Debug.Log("收到消息:"+(EnumCmdID)proto.type);
            lock (messageDistribution.msgList)
            {
                messageDistribution.msgList.Add(proto);
            }
            //清除已处理的消息
            int count = bufferCount-msgLength-sizeof(Int32)-sizeof(Int32);
            Array.Copy(readBuffer,sizeof(Int32)+msgLength,readBuffer,0,count);
            bufferCount=count;
            if (bufferCount>0)
            {
                ProcessData();
            }
        }

        public void BeginSendMessages(ProtobufTool protobuf)
        {
            try
            {
                Debug.Log("发送数据："+(EnumCmdID)protobuf.type);
                socket.BeginSend(protobuf.bytes,0,protobuf.byteLength,SocketFlags.None,SendCallback,socket);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                //Connect connect = ar.AsyncState as Connect;

                //connect.socket.EndSend(ar);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
       
        public void Update()
        {
            //消息
            messageDistribution.Update();
            //心跳
            if (status==ConnectStatus.Connected)
            {
                if (Time.time-lastTickTime>heartBeatTime)
                {
                    ProtobufTool proto = GetHeatBeatProtocol();
                    //Debug.Log("发送心跳包");
                    BeginSendMessages(proto);
                    lastTickTime=Time.time;
                }
            }
        }
        public ProtobufTool GetHeatBeatProtocol()
        {
            ProtobufTool protocol = new ProtobufTool();
            hearInfo=new HeartBeatInfo()
            {
                Curtime=0,
                Hostip="127.0.0.1",
            };
            protocol.CreatData((int)EnumCmdID.Heartbeat,hearInfo);
            return protocol;
        }

        /// <summary>
        /// 关闭连接
        /// </summary>
        public bool Close()
        {
            try
            {
                socket.Close();
                return true;
            }
            catch (Exception e)
            {
                Debug.Log("关闭失败:"+e.Message);
                //关闭失败
                return false;
            }
        }
    }


}
