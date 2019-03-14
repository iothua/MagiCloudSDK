using System;
using UnityEngine;
using UnityEngine.UI;

namespace MagiCloud.NetWorks.Client
{
    public class ExperimentEvent
    {
        ExperimentInfo experimentInfo;
        IntPtr ptr;

        public Text txtExperimentPath;

        public ExperimentEvent()
        {
            ptr = SystemDllHelper.GetForegroundWindow();
            experimentInfo=new ExperimentInfo()
            {
                Id=0,
                Own="",
                Name="",
                IsBack=false
            };
            MessageDistribution.AddListener((int)EnumCmdID.ExpinfoReq,ExpinfoReqCallback);
        }
        /// <summary>
        /// 收到请求
        /// </summary>
        /// <param name="data"></param>
        private void ExpinfoReqCallback(ProtobufTool data)
        {
            data.DeSerialize(experimentInfo,data.bytes);

            SystemDllHelper.ShowWindow(ptr,3);
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

            ptr = SystemDllHelper.GetForegroundWindow();
            //最小化自身窗口
            SystemDllHelper.ShowWindow(ptr,2);

            experimentInfo.IsBack=true;
            ProtobufTool tool = new ProtobufTool();
            tool.CreatData((int)EnumCmdID.ExpinfoRes,experimentInfo);
            NetManager.connetion.BeginSendMessages(tool);
        }

    }
}

