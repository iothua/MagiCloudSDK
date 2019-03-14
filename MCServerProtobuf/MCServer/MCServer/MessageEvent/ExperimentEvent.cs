using System;

namespace MCServer
{
    public class ExperimentEvent
    {
        readonly ExperimentInfo experimentInfo;
        public ExperimentEvent()
        {
            experimentInfo=new ExperimentInfo()
            {
                Id=0,
                Own="",
                Name="",
                IsBack=false
            };
            MessageDistribution.AddListener((int)EnumCmdID.ExpinfoReq,ExpinfoReqCallback);
            MessageDistribution.AddListener((int)EnumCmdID.ExpinfoRes,ExpinfoResCallback);
        }

        /// <summary>
        /// 收到回执
        /// </summary>
        /// <param name="data"></param>
        private void ExpinfoResCallback(int connectID, ProtobufTool data)
        {
            data.DeSerialize(experimentInfo,data.bytes);
            ProtobufTool tool = GetProtobuf(data);
            //转发出去
            Server.Instance.Broadcast(tool);
        }

        /// <summary>
        /// 收到请求
        /// </summary>
        /// <param name="data"></param>
        private void ExpinfoReqCallback(int connectID,ProtobufTool data)
        {
            data.DeSerialize(experimentInfo,data.bytes);
            ProtobufTool tool = GetProtobuf(data);
            //转发出去
            Server.Instance.Broadcast(tool);
        }

        private ProtobufTool GetProtobuf(ProtobufTool data)
        {
            ProtobufTool tool = new ProtobufTool();
            tool.CreatData(data.type,experimentInfo);
            return tool;
        }
    }
}
