using UnityEngine;
using static MagiCloud.NetWorks.SystemSettingInfo.Types;

namespace MagiCloud.NetWorks.Server
{
    /// <summary>
    /// 系统参数设置事件
    ///     *设置数据读取、保存
    ///     *发送
    /// </summary>
    public class SettingEvent
    {
        public SettingReq setting;

        public SettingEvent(MessageDistribution messageDistribution)
        {
            InitSetting();
        }
        #region Setting

        public void InitSetting()
        {
            setting=new SettingReq
            {
                Info =new SystemSettingInfo()
            };
            if (PlayerPrefs.HasKey("type"))
                setting.Info.Type=(Performance)PlayerPrefs.GetInt("type");
            else
                setting.Info.Type = Performance.Middle;
            if (PlayerPrefs.HasKey("volume"))
                setting.Info.Volume=PlayerPrefs.GetInt("volume");
            else
                setting.Info.Volume = 10;
        }

        /// <summary>
        /// 画质切换，会自动向服务端发送
        /// </summary>
        /// <param name="type"></param>
        public void SwitchQulity(Performance type)
        {
            setting.Info.Type= type;
            //保存数据
            PlayerPrefs.SetInt("type",(int)setting.Info.Type);
            SendSetting();
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        public void SendSetting()
        {
            ServerNetManager.connetion.BeginSendMessages(GetSystemSetting());
        }

        /// <summary>
        /// 获取设置协议
        /// </summary>
        /// <returns></returns>
        public ProtobufTool GetSystemSetting()
        {
            ProtobufTool protocol = new ProtobufTool();
            protocol.CreatData((int)EnumCmdID.SettingReq,setting);
            return protocol;
        }

        /// <summary>
        /// 发送音量设置参数
        /// </summary>
        /// <param name="v"></param>
        public void SendVolumeSet(float v)
        {
            setting.Info.Volume= (int)(v*100);
            //保存数据
            PlayerPrefs.SetInt("volume",setting.Info.Volume);
            SendSetting();
        }

        #endregion
    }
}