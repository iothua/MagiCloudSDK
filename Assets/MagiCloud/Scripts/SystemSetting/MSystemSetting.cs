using UnityEngine;
using MagiCloud.Core;
using MagiCloud.NetWorks;

namespace MagiCloud
{
    [System.Serializable]
    public class SystemData
    {
        ///// <summary>
        ///// 平台
        ///// </summary>
        //public int OperatePlatform;

        /// <summary>
        /// 是否全屏
        /// </summary>
        public bool IsFullDisplay = true;

        /// <summary>
        /// 分辨率-宽
        /// </summary>
        public int ScreenWidth = 1920;

        /// <summary>
        /// 分辨率-高
        /// </summary>
        public int ScreenHeight = 1080;

        /// <summary>
        /// 图像质量
        /// </summary>
        public int Quality = 2;
    }

    /// <summary>
    /// 系统设置
    /// </summary>
    public static class MSystemSetting
    {
        public static SystemData SystemDataValue = new SystemData();

        public static void OnInitialize(string settingPath)
        {
            //读取文件，初始化数据

            string jsonData = Json.JsonHelper.ReadJsonString(settingPath);
            if (string.IsNullOrEmpty(jsonData)) return;

            SystemDataValue = Json.JsonHelper.JsonToObject<SystemData>(jsonData);
            OnInitialize();
        }

        public static void OnInitialize()
        {

            SetQuality(SystemDataValue.Quality);
            SetFullScreen(SystemDataValue.IsFullDisplay);
            SetDPI(SystemDataValue.ScreenWidth,SystemDataValue.ScreenHeight);
        }

        /// <summary>
        /// 保存设置
        /// </summary>
        public static void SaveSetting(string savePath)
        {
            string jsonData = Json.JsonHelper.ObjectToJsonString(SystemDataValue);
            Json.JsonHelper.SaveJson(jsonData,savePath);
        }

        public static void SetSystemData(SystemSettingInfo systemSettingInfo)
        {
            SystemDataValue.IsFullDisplay = systemSettingInfo.IsFullDisplay;
            SystemDataValue.Quality = systemSettingInfo.Quality;
            SystemDataValue.ScreenHeight = systemSettingInfo.ScreenHeight;
            SystemDataValue.ScreenWidth = systemSettingInfo.ScreenWidth;
        }

        ///// <summary>
        ///// 设置操作平台
        ///// </summary>
        //public static void SetOperatePlatform(int operatePlatform)
        //{

        //}

        public static void SetQuality(int level)
        {
            switch (level)
            {
                case 0:
                    QualitySettings.SetQualityLevel(0,true);
                    break;
                case 1:
                    QualitySettings.SetQualityLevel(2,true);
                    break;
                case 2:
                    QualitySettings.SetQualityLevel(4,true);
                    break;
            }

            SystemDataValue.Quality = level;
        }

        public static void SetFullScreen(bool fullScreen)
        {
            SystemDataValue.IsFullDisplay = fullScreen;
            Screen.fullScreen = SystemDataValue.IsFullDisplay;
        }

        public static void SetDPI(Vector2Int dpi)
        {
            SetDPI(dpi.x,dpi.y);
        }

        public static void SetDPI(int width,int height)
        {
            Screen.SetResolution(width,height,SystemDataValue.IsFullDisplay);
            SystemDataValue.ScreenWidth = width;
            SystemDataValue.ScreenHeight = height;

        }
    }
}
