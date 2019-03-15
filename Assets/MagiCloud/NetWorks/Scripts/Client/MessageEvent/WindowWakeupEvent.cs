using UnityEngine;
namespace MagiCloud.NetWorks.Client
{
    public class WindowWakeupEvent
    {
        public WindowWakeupEvent(MessageDistribution messageDistribution )
        {
            messageDistribution.AddListener((int)EnumCmdID.WindowwakeupReq,WindowWakeupCallback);
        }
        private void WindowWakeupCallback(ProtobufTool data)
        {
            //Debug.Log("收到唤醒信息，正在启动");
        }
    }
}

