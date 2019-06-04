using System;

namespace MagiCloud.NetWorks
{
    public class ExperimentEventServer
    {
        private ExperimentInfo experimentInfo;
        MessageDistributionServer distributionServer;
        private ServerConnection serverConnection;

        public ExperimentEventServer(ServerConnection serverConnection,Action<string> actionLog = null)
        {
            this.distributionServer = serverConnection.messageDistribution;
            this.serverConnection = serverConnection;

            distributionServer.AddListener((int)CommandID.ExperimentInfoReceipt,(senderID,data) =>
           {
               data.DeSerialize(experimentInfo,data.bytes);

               if (actionLog != null)
                   actionLog("请求：ExperimentInfoReceipt");
           });

            distributionServer.AddListener((int)CommandID.ExperimentInfoRequest,(senderID,data) =>
           {
               data.DeSerialize(experimentInfo,data.bytes);

               if (actionLog!=null)
                   actionLog("请求：ExperimentInfoRequest");
           });
        }

        public void SendReceipt(ExperimentInfo experimentInfo)
        {
            ProtobufTool tool = new ProtobufTool();
            tool.CreatData((int)CommandID.ExperimentInfoReceipt,experimentInfo);
            serverConnection.BeginSendMessage(tool);
        }

        public void SendRequest(ExperimentInfo experimentInfo)
        {
            ProtobufTool tool = new ProtobufTool();
            tool.CreatData((int)CommandID.ExperimentInfoRequest,experimentInfo);
            serverConnection.BeginSendMessage(tool);
        }
    }
}
