using System.Collections.Generic;

namespace MagiCloud.NetWorks
{
    /// <summary>
    /// 服务端/客户端 消息分发
    /// </summary>
    public class MessageDistributionServer :IMessageDistribution
    {
        private int num = 20;
        public List<ReceiveMessageStruct> msgList = new List<ReceiveMessageStruct>(); //当前消息列表

        private Dictionary<int,MessageDelegate> msgEvents = new Dictionary<int,MessageDelegate>(); //消息事件集合
        private Dictionary<int,MessageDelegate> onceMsgEvents = new Dictionary<int,MessageDelegate>(); //单次消息事件集合

        #region AddOrRemove

        /// <summary>
        /// 注册消息事件
        /// </summary>
        /// <param name="type"></param>
        /// <param name="msg"></param>
        public void AddListener(int type,MessageDelegate msg)
        {
            if (msgEvents.ContainsKey(type))
                msgEvents[type] += msg;
            else
                msgEvents[type] = msg;
        }
        /// <summary>
        /// 注册消息事件
        /// </summary>
        /// <param name="type"></param>
        /// <param name="msg"></param>
        public void AddOnceListener(int type,MessageDelegate msg)
        {
            if (onceMsgEvents.ContainsKey(type))
                onceMsgEvents[type] += msg;
            else
                onceMsgEvents[type] = msg;
        }
        /// <summary>
        /// 注销消息事件
        /// </summary>
        /// <param name="type"></param>
        /// <param name="msg"></param>
        public void RemoveListener(int type,MessageDelegate msg)
        {
            if (msgEvents.ContainsKey(type))
            {
                msgEvents[type] -= msg;
                if (msgEvents[type] == null)
                    msgEvents.Remove(type);
            }
        }
        /// <summary>
        /// 注销消息事件
        /// </summary>
        /// <param name="type"></param>
        /// <param name="msg"></param>
        public void RemoveOnceListener(int type,MessageDelegate msg)
        {
            if (msgEvents.ContainsKey(type))
            {
                onceMsgEvents[type] -= msg;
                if (onceMsgEvents[type] == null)
                    onceMsgEvents.Remove(type);
            }
        }

        #endregion

        /// <summary>
        /// 每帧处理消息队列
        /// </summary>
        public void Update()
        {
            //通过当前消息与已注册消息事件的类型匹配来进行分发事件
            for (int i = 0; i < num; i++)        //由于msgList是动态的，这里每帧最多处理固定的消息数量
            {
                if (msgList.Count > 0)
                {
                    lock (msgList)
                    {
                        OnMessageEvent(msgList[0]);
                        msgList.RemoveAt(0);
                    }
                }
            }
        }

        private  void OnMessageEvent(ReceiveMessageStruct sendData)
        {
            OnMessageEvent(sendData.connectID,sendData.protobuf);
        }

        /// <summary>
        /// 执行消息事件
        /// </summary>
        /// <param name="key"></param>
        public void OnMessageEvent(int connectID,ProtobufTool protobuf)
        {
            int key = protobuf.type;
            if (msgEvents.ContainsKey(key))
                msgEvents[key](connectID,protobuf);
            if (onceMsgEvents.ContainsKey(key))
            {
                onceMsgEvents[key](connectID,protobuf);
                onceMsgEvents[key] = null;
                onceMsgEvents.Remove(key);
            }
        }


    }
}
