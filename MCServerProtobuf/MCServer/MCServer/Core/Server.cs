using Google.Protobuf;
using System;
using System.Net;
using System.Net.Sockets;

namespace MCServer
{

    public class Server
    {
        private static object lockHelper = new object();
        private Socket serverSocket;  //服务端口
        private int maxConnect = 2;
        public Connect[] connects;
        public ProtobufTool proto = new ProtobufTool();
        //单例
        private static Server instance;
        public static Server Instance
        {
            get
            {
                if (instance==null)
                    lock (lockHelper)
                        if (instance==null)
                            instance=new Server();
                return instance;
            }
        }

        public void Start(string ip,int port)
        {
            CreatConnects();
            serverSocket = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
            IPEndPoint point = new IPEndPoint(IPAddress.Parse(ip),port);
            serverSocket.Bind(point);
            serverSocket.Listen(maxConnect);
            serverSocket.BeginAccept(AcceptCallback,serverSocket);
            Console.WriteLine("启动服务器");
            HeartBeatController heartBeat = new HeartBeatController(this);
        }

        private void AcceptCallback(IAsyncResult ar)
        {
            try
            {
                //向客户端收发消息的Socket
                Socket toClientSocket = serverSocket.EndAccept(ar);
                int i = NewIndex();
                if (i<0)
                {
                    toClientSocket.Close();
                    Console.WriteLine("连接已满");
                }
                else
                {
                    Connect connect = connects[i];
                    connect.Init(toClientSocket);
                    string ip = connect.GetAddress();
                    Console.WriteLine("{0},已连接",ip);
                    BeginReceiveMessages(connect);
                    //connect.socket.BeginReceive(connect.buffer,connect.bufferCount,connect.BuffRemain(),SocketFlags.None,ReceiveCallback,connect);
                }
                serverSocket.BeginAccept(AcceptCallback,null);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #region Send
        public void BeginSendMessages(Connect connect,int type,IMessage message)
        {
            ProtobufTool protobuf = new ProtobufTool();
            protobuf.CreatData(type,message);
            BeginSendMessage(connect,protobuf);
        }

        public void BeginSendMessage(Connect connect,ProtobufTool protobuf)
        {
            try
            {
                connect.socket.BeginSend(protobuf.bytes,0,protobuf.byteLength,SocketFlags.None,SendCallback,connect);
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
                Connect connect = ar.AsyncState as Connect;

                connect.socket.EndSend(ar);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }



        #endregion
        #region Receive

        public void BeginReceiveMessages(Connect connect)
        {
            connect.socket.BeginReceive(connect.buffer,connect.bufferCount,connect.BuffRemain(),SocketFlags.None,ReceiveCallback,connect);
        }

        /// <summary>
        /// 接收数据
        /// </summary>
        /// <param name="ar"></param>
        private void ReceiveCallback(IAsyncResult ar)
        {
            Connect connect = null;
            connect=ar.AsyncState as Connect;
            lock (connect)
            {
                try
                {
                    int count = connect.socket.EndReceive(ar);
                    if (count==0)
                    {
                        connect.Close();
                        return;
                    }
                    //  Console.WriteLine("从客户端：{0}接收到数据，解析中",connect.socket.RemoteEndPoint);
                    connect.bufferCount+=count;
                    ProcessData(connect);
                    BeginReceiveMessages(connect);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    if (connect!=null)
                        connect.Close();
                }
                finally { }
            }
        }

        /// <summary>
        /// 数据处理
        /// </summary>
        /// <param name="connect"></param>
        private void ProcessData(Connect connect)
        {
            //如果小于长度字节
            if (connect.bufferCount<sizeof(Int32)+sizeof(Int32))
                return;
            //////消息长度4个字节，消息类型4个字节
            Array.Copy(connect.buffer,connect.lenBytes,sizeof(Int32));
            //真实消息长度
            connect.msgLength=BitConverter.ToInt32(connect.lenBytes,0)-sizeof(Int32);
            if (connect.bufferCount<connect.msgLength+sizeof(Int32))
                return;
            ProtobufTool protobuf = proto.Read(connect.buffer);
            lock (MessageDistribution.msgList)
            {
                // Console.WriteLine("处理{0}类型的消息。",(EnumCmdID)protobuf.type);
                MessageDistribution.msgList.Add(protobuf);
            }
            int count = connect.bufferCount-connect.msgLength-sizeof(Int32)-sizeof(Int32);
            Array.Copy(connect.buffer,sizeof(Int32)+connect.msgLength,connect.buffer,0,count);
            connect.bufferCount=count;
            if (connect.bufferCount>0)
            {
                ProcessData(connect);
            }
        }

        #endregion


        #region Other
        /// <summary>
        /// 创建连接器
        /// </summary>
        private void CreatConnects()
        {
            connects=new Connect[maxConnect];
            for (int i = 0; i < maxConnect; i++)
            {
                connects[i]=new Connect();
            }
        }

        /// <summary>
        /// 获取连接池索引,返回负数时表示获取失败
        /// </summary>
        /// <returns></returns>
        public int NewIndex()
        {
            if (connects==null) return -1;
            for (int i = 0; i < connects.Length; i++)
            {
                if (connects[i]==null)
                {
                    connects[i]=new Connect();
                    return i;
                }
                else if (connects[i].isUse==false)
                {
                    return i;
                }
            }
            return -1;
        }

        public void Close()
        {
            for (int i = 0; i < connects.Length; i++)
            {
                Connect connect = connects[i];
                if (connect==null) continue;
                if (!connect.isUse) continue;
                lock (connect)
                {
                    connect.Close();
                }
            }
        }

        /// <summary>
        /// 消息广播
        /// </summary>
        /// <param name="protocol"></param>
        public void Broadcast(ProtobufTool protobuf)
        {
            for (int i = 0; i < connects.Length; i++)
            {
                if (!connects[i].isUse) continue;
                if (connects[i].socket==null) continue;
                BeginSendMessage(connects[i],protobuf);
            }
        }
        #endregion
    }


}
