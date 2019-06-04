using System;
using System.Net;
using System.Net.Sockets;
using DG.Tweening;
using Google.Protobuf;
namespace MagiCloud.NetWorks
{
    public class ControlTerminal
    {
        private static object lockHelper = new object();
        private Socket controlSocket;
        private int maxConnect = 2;

        public ConnectControl[] connects;
        public ProtobufTool proto = new ProtobufTool();

        public void Start(string ip,int port)
        {

            CreateConnects();
            controlSocket = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);

            IPEndPoint point = new IPEndPoint(IPAddress.Parse(ip),port);
            controlSocket.Bind(point);
            controlSocket.Listen(maxConnect);
            controlSocket.BeginAccept(AcceptCallback,controlSocket);

            UnityEngine.Debug.Log("启动服务器");

            HeartBeatController heartBeat = new HeartBeatController(this);
        }

        public void Update()
        {
            MessageDistributionControl.Instance.Update();
        }

        public void Stop()
        {
            Close();
        }

        private void AcceptCallback(IAsyncResult asyncResult)
        {
            try
            {
                Socket toClientSocket = controlSocket.EndAccept(asyncResult);

                int i = NewIndex();

                if (i < 0)
                {
                    toClientSocket.Close();
                }
                else
                {
                    ConnectControl connect = connects[i];
                    connect.Initialize(toClientSocket,i);

                    string ip = connect.GetAddress();

                    UnityEngine.Debug.Log(ip + "已连接");
                    SendConnectMessage(i);
                    BeginReceiveMessage(connect);
                }

                controlSocket.BeginAccept(AcceptCallback,null);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// 发送连接消息
        /// </summary>
        /// <param name="i"></param>
        private void SendConnectMessage(int i)
        {
            ConnectInfo info = new ConnectInfo()
            {
                Id = i
            };

            ProtobufTool protobuf = new ProtobufTool();
            protobuf.CreatData((int)CommandID.Connect,info);
            Broadcast(protobuf,i);
        }

        public void BeginSendMessage(ConnectControl connect,int type,IMessage message)
        {
            ProtobufTool protobuf = new ProtobufTool();
            protobuf.CreatData(type,message);

            BeginSendMessage(connect,protobuf);
        }


        public void BeginSendMessage(ConnectControl connect,ProtobufTool protobuf)
        {
            connect.Send(protobuf);
        }

        public void BeginReceiveMessage(ConnectControl connect)
        {
            connect.socket.BeginReceive(connect.buffer,connect.bufferCount,connect.BUffRemain(),SocketFlags.None,(asyncResult) =>
        {
            ConnectControl connectControl = null;

            connectControl = asyncResult.AsyncState as ConnectControl;
            lock (connectControl)
            {
                try
                {
                    int count = connectControl.socket.EndReceive(asyncResult);
                    if (count == 0)
                    {
                        connectControl.Close();
                        return;
                    }

                    connectControl.bufferCount += count;
                    ProcessData(connectControl);

                    BeginReceiveMessage(connectControl);
                }
                catch (Exception e)
                {
                    if (connectControl != null)
                        connectControl.Close();

                    //throw e;
                }
            }

        },connect);
        }

        private void ProcessData(ConnectControl connect)
        {
            if (connect.bufferCount < sizeof(int) + sizeof(int))
                return;

            //消息长度4个字节，消息类型4个字节
            Array.Copy(connect.buffer,connect.lenBytes,sizeof(int));
            //真实消息长度
            connect.msgLength = BitConverter.ToInt32(connect.lenBytes,0) - sizeof(int);

            if (connect.bufferCount < connect.msgLength + sizeof(int))
                return;
            try
            {


                ProtobufTool protobuf = proto.Read(connect.buffer);

                //添加到消息集合等待处理
                lock (MessageDistributionControl.Instance.msgList)
                {
                    MessageDistributionControl.Instance.msgList.Add(new ReceiveMessageStruct(connect.id,protobuf));
                }

                //清除已处理的数据
                int count = connect.bufferCount - connect.msgLength - sizeof(int) - sizeof(int);
                Array.Copy(connect.buffer,sizeof(int) + connect.msgLength,connect.buffer,0,count);
                connect.bufferCount = count;

                if (connect.bufferCount > 0)
                    ProcessData(connect);
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError(e);
            }

        }

        private void CreateConnects()
        {
            connects = new ConnectControl[maxConnect];
            for (int i = 0; i < maxConnect; i++)
            {
                connects[i] = new ConnectControl();
            }
        }

        public int NewIndex()
        {
            if (connects == null) return -1;

            for (int i = 0; i < connects.Length; i++)
            {
                if (connects[i] == null)
                {
                    connects[i] = new ConnectControl();
                    return i;
                }

                if (!connects[i].isUse)
                    return i;
            }

            return -1;
        }

        public void Close()
        {
            for (int i = 0; i < connects.Length; i++)
            {
                ConnectControl connect = connects[i];

                if (connect == null) continue;
                if (!connect.isUse) continue;

                lock (connect)
                {
                    connect.Close();
                }
            }
        }

        public void Broadcast(ProtobufTool protobuf,int? sender = null)
        {
            int senderId = sender.HasValue&&sender.Value>=0 ? sender.Value : -1;
            for (int i = 0; i < connects.Length; i++)
            {
                if (i == senderId) continue;
                if (!connects[i].isUse) continue;

                if (connects[i].socket == null) continue;
                BeginSendMessage(connects[i],protobuf);
            }
        }

    }
}
