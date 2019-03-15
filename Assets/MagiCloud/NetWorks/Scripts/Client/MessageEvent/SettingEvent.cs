using System.IO;
using UnityEngine;

namespace MagiCloud.NetWorks.Client
{
    public class SettingEvent
    {
        public SettingReq req;
        public SettingEvent(MessageDistribution messageDistribution)
        {
            SettingInit();
            messageDistribution.AddListener((int)EnumCmdID.SettingReq,SettingReaCallback);
        }
        public void SettingInit()
        {
            req = new SettingReq();
          
        }

        private void SettingReaCallback(ProtobufTool protobuf)
        {
            using (MemoryStream stream = new MemoryStream(protobuf.bytes))
            {
                protobuf.DeSerialize(req,protobuf.bytes);
                QualitySettings.SetQualityLevel((int)req.Info.Type,true);
            }
        }
    }

}

