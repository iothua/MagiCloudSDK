using Google.Protobuf;

namespace MagiCloud.NetWorks
{
    /// <summary>
    /// 接收事件委托
    /// </summary>
    /// <param name="proto"></param>
    public delegate void ReceiveDelegate(IMessage proto);

    /// <summary>
    /// 事件基类
    /// </summary>
    public class EventBase
    {
        /// <summary>
        /// 使用protobuf生成的类
        /// </summary>
        protected IMessage Proto { get; set; }
        /// <summary>
        /// 连接器
        /// </summary>
        protected IConnect Connection { get; private set; }
         /// <summary>
        /// 连接器
        /// </summary>
        protected IMessageDistribution MessageDistribution { get; private set; }
        /// <summary>
        /// 协议类型
        /// </summary>
        public CommandID Command { get; protected set; }
        /// <summary>
        /// 消息类型
        /// </summary>
        public int MessageType { get { return (int)(Command); } }

        protected ReceiveDelegate handler;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="connection"></param>
        public virtual void Init(IConnect connection,IMessageDistribution messageDistribution)
        {
            this.Connection=connection;
            MessageDistribution=messageDistribution;
            MessageDistribution.AddListener(MessageType,Receive);
        }
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="info"></param>
        /// <param name="command"></param>
        public virtual void Send<T>(T info) where T : IMessage
        {
            Connection.Send(GetProtobuf(info));
        }
        /// <summary>
        /// 收到消息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        protected virtual void Receive(int senderID,ProtobufTool data)
        {
            data.DeSerialize(Proto,data.bytes);
            if (handler!=null)
            {
                handler.Invoke(Proto);
            }
        }

        /// <summary>
        /// 添加消息接收事件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="handler"></param>
        public virtual void AddReceiveEvent(ReceiveDelegate handler)
        {
            handler+=handler;
        }

        /// <summary>
        /// 添加消息接收事件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="handler"></param>
        public virtual void RemoveReceiveEvent(ReceiveDelegate handler)
        {
            handler-=handler;
        }

        /// <summary>
        /// 从消息分发中移除监听
        /// </summary>
        public virtual void Remove()
        {
            MessageDistribution.RemoveListener(MessageType,Receive);
        }

        /// <summary>
        /// 获取协议
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="info"></param>
        /// <returns></returns>
        protected virtual ProtobufTool GetProtobuf<T>(T info) where T : IMessage
        {
            ProtobufTool tool = new ProtobufTool();
            byte[] data = tool.CreatData(MessageType,info);
            return tool;
        }
    }


}
