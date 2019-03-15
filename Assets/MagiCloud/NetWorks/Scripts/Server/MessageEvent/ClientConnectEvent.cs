using System;
using System.Collections.Generic;

namespace MagiCloud.NetWorks.Server
{
    /// <summary>
    /// 客户端连接事件
    /// </summary>
    public class ClientConnectEvent
    {
        ConnectInfo info;
        private Dictionary<int,Action> actions;
        public ClientConnectEvent(MessageDistribution messageDistribution)
        {
            actions=new Dictionary<int,Action>();
            info =new ConnectInfo()
            {
                Id=0
            };
            messageDistribution.AddListener((int)EnumCmdID.Connect,OnConnect);
        }

        public void Add(int id,Action a)
        {
            if (a==null) return;
            if (actions.ContainsKey(id))
            {
                actions[id]=null;
                actions[id]+=a;
            }
            else
            {
                actions.Add(id,a);
            }
        }

        private void OnConnect(ProtobufTool data)
        {
            data.DeSerialize(info,data.bytes);
            if (actions.ContainsKey(info.Id))
            {
                actions[info.Id].Invoke();
            }
        }

        public void Remove(int id)
        {
            if (actions.ContainsKey(id))
            {
                actions[id]=null;
                actions.Remove(id);
            }
        }

    }
}
