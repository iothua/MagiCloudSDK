using System.IO;
using UnityEngine;

namespace MagiCloud.NetWorks.Client
{
    public class SettingEvent
    {
        public SettingReq req;
        public SettingEvent()
        {
            SettingInit();
        }
        public void SettingInit()
        {
            req = new SettingReq();
            MessageDistribution.AddListener((int)EnumCmdID.SettingReq,SettingReaCallback);
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

