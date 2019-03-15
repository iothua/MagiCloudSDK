using System;
using UnityEngine;
using UnityEngine.UI;

namespace MagiCloud.NetWorks.Client
{
    public class ExperimentEvent
    {
        ExperimentInfo experimentInfo;
     

        public Text txtExperimentPath;

        public ExperimentEvent(MessageDistribution messageDistribution)
        {
           
            experimentInfo=new ExperimentInfo()
            {
                Id=0,
                Own="",
                Name="",
                IsBack=false
            };
            messageDistribution.AddListener((int)EnumCmdID.ExpinfoReq,ExpinfoReqCallback);
        }
        /// <summary>
        /// 收到请求
        /// </summary>
        /// <param name="data"></param>
        private void ExpinfoReqCallback(ProtobufTool data)
        {
            data.DeSerialize(experimentInfo,data.bytes);

          

            txtExperimentPath.text = experimentInfo.ExperimentPath;

            //打开实验
            AssetBundleManager.LoadAsset<GameObject>(new string[1] { experimentInfo.ExperimentPath }, (games) =>
            {

                foreach (var item in games)
                {
                    GameObject.Instantiate(item);
                }
            });
        }

        /// <summary>
        /// 发送回执
        /// </summary>
        public void SendExpinfoRes()
        {

            //ptr = SystemDllHelper.GetForegroundWindow();
            //最小化自身窗口

            experimentInfo.IsBack=true;
            ProtobufTool tool = new ProtobufTool();
            tool.CreatData((int)EnumCmdID.ExpinfoRes,experimentInfo);
            ClientNetManager.connetion.BeginSendMessages(tool);
        }

    }
}

