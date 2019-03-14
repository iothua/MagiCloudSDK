namespace MCServer
{
    /// <summary>
    /// 接收消息结构
    /// </summary>
    public struct ReceiveMessageStruct
    {
        /// <summary>
        /// 连接器id
        /// </summary>
        public int connectID;

        /// <summary>
        /// 协议
        /// </summary>
        public ProtobufTool protobuf;

        public ReceiveMessageStruct(int connectID,ProtobufTool protobuf)
        {
            this.connectID=connectID;
            this.protobuf=protobuf;
        }
    }
}
