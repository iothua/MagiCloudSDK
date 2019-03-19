using Google.Protobuf;
using System;

namespace MagiCloud.NetWorks
{
    public class ExperimentEventControl
    {
        private ControlTerminal controlTerminal;
        private readonly ExperimentInfo experimentInfo;


        private Action<string> actionLog;

        public ExperimentEventControl(ControlTerminal controlTerminal,Action<string> actionlog = null)
        {
            this.controlTerminal = controlTerminal;

            experimentInfo = new ExperimentInfo()
            {
                Id = 0,
                OwnProject = string.Empty,
                Name = string.Empty,
                IsBack = false
            };

            this.actionLog = actionlog;

            //收到请求
            MessageDistributionControl.AddListener((int)CommandID.ExperimentInfoRequest, (int connectID, ProtobufTool data) =>
            {
                data.DeSerialize(experimentInfo, data.bytes);

                ProtobufTool tool = GetProtobuf(data, experimentInfo);

                controlTerminal.Broadcast(tool);

                if (actionLog != null)
                    actionLog("ExperimentInfoRequest");

            });

            MessageDistributionControl.AddListener((int)CommandID.ExperimentInfoReceipt, (int connectID, ProtobufTool data) =>
            {
                data.DeSerialize(experimentInfo, data.bytes);

                ProtobufTool tool = GetProtobuf(data, experimentInfo);

                controlTerminal.Broadcast(tool);

                if (actionLog != null)
                    actionLog("发送请求，转发出去：ExperimentInfoReceipt");
            });

        }

        private ProtobufTool GetProtobuf(ProtobufTool data, ExperimentInfo experimentInfo)
        {
            ProtobufTool tool = new ProtobufTool();
            tool.CreatData(data.type, experimentInfo);
            return tool;
        }
    }
}
