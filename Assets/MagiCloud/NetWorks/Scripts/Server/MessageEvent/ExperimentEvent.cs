using System;
using UnityEngine;

namespace MagiCloud.NetWorks.Server
{
    /// <summary>
    /// 实验事件
    /// </summary>
    public class ExperimentEvent
    {
        ExperimentInfo experimentInfo;
        MessageEvent message;
        public ExperimentEvent(MessageEvent message,MessageDistribution messageDistribution)
        {
            this.message=message;
            experimentInfo =new ExperimentInfo()
            {
                Id=0,
                Own="",
                Name="",
                IsBack=false
            };
            messageDistribution.AddListener((int)EnumCmdID.ExpinfoRes,ExpinfoResCallback);
        }

        /// <summary>
        /// 实验退出后的回调
        /// </summary>
        /// <param name="data"></param>
        private void ExpinfoResCallback(ProtobufTool data)
        {
            data.DeSerialize(experimentInfo,data.bytes);

            //最大化
            message.wakeupEvent.SetMax();
        }

        /// <summary>
        /// 发送打开实验请求
        /// </summary>
        public void SendReq(Action action,ExperimentInfo experimentInfo)
        {
            if (action!=null)
                action.Invoke();

            //message.wakeupEvent.SetMin();

            ProtobufTool tool = new ProtobufTool();
            tool.CreatData((int)EnumCmdID.ExpinfoReq,experimentInfo);
            ServerNetManager.connetion.BeginSendMessages(tool);
        }
    }
}