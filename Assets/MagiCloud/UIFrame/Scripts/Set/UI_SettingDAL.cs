using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace MagiCloud.UIFrame
{
    /// <summary>
    /// 系统设置参数
    /// </summary>
    public static class UI_SettingDAL
    {
        private static int _intQualityLevel;                    //图像质量等级
        private static float _fltSound;                         //音量大小
                                                                //private static int _intWantedResX;                      //屏幕宽
                                                                //private static int _intWantedResY;                      //屏幕高
        private static int _intWantedKey;                       //屏幕 键
        private static bool _bolFullScreen;                     //是否全屏
        private static int _intLanguage;                        //语言

        //private static string savePathParent;
        private static string savePath;
        private static int lineCounter;
        private static string lineToRead;
        private static string[] outPut = new string[5], splitLine = new string[2], inPut = new string[5];

        private static Dictionary<int, int[]> _dicDPIData = new Dictionary<int, int[]>();

        public static Dictionary<int, int[]> DicDPIData {
            get { return _dicDPIData; }
        }

        public static void OnInitialize()
        {
            //初始化信息存储
            //savePathParent = Application.dataPath;
            //DirectoryInfo initInfo = Directory.CreateDirectory(savePathParent);
            //initInfo.Attributes = FileAttributes.Hidden;
            //savePath = savePathParent + "\\ChemistryAR";
            InitDPIData();
            LoadPlayerprefs();
        }

        /// <summary>
        ///设置分辨率
        /// </summary>
        public static void SetDPI(int index)
        {
            _intWantedKey = index;
        }

        /// <summary>
        /// 设置画质
        /// </summary>
        /// <param name="level">低画质0； 中画质1； 高画质2</param>
        public static void SetQuality(int level)
        {
            switch (level)
            {
                case 0:
                    _intQualityLevel = 0;
                    break;
                case 1:
                    _intQualityLevel = 1;
                    break;
                case 2:
                    _intQualityLevel = 2;
                    break;
            }
        }

        ///// <summary>
        ///// 设置语言
        ///// </summary>
        ///// <param name="index">简体中文0； 繁体中文1； 英语2</param>
        //public static void SetLanguage(MCLanguage language)
        //{
        //    _intLanguage = (int)language;
        //}

        /// <summary>
        /// 设置音量
        /// </summary>
        /// <param name="sound">声音大小</param>
        public static void SetVolume(float sound)
        {
            _fltSound = sound;
        }

        /// <summary>
        /// 设置窗口模式（全屏，窗口化）
        /// </summary>
        /// <param name="fullscreen">true 全屏，false 窗口化</param>
        public static void SetScreen(bool fullscreen)
        {
            _bolFullScreen = fullscreen;
        }

        /// <summary> 设置屏幕（显示在哪个屏幕上） </summary>
        public static void SetDisplay(int index)
        {
            //TODO...
        }

        public static void InitDPIData()
        {
            _dicDPIData.Clear();
            _dicDPIData.Add(0, new[] { 1280, 720 });
            _dicDPIData.Add(1, new[] { 1366, 768 });
            _dicDPIData.Add(2, new[] { 1600, 900 });
            _dicDPIData.Add(3, new[] { 1920, 1080 });
            _dicDPIData.Add(4, new[] { 3840, 2160 });

        }

        public static int GetLanguage()
        {
            return _intLanguage;
        }

        public static float GetVolume()
        {
            return _fltSound;
        }

        public static int GetQuality()
        {
            return _intQualityLevel;
        }

        public static bool GetIsFullScreen()
        {
            return _bolFullScreen;
        }

        public static int GetDPI()
        {
            return _intWantedKey;
        }

        /// <summary>
        /// 默认设置
        /// </summary>
        public static void DefaultSetting()
        {
            //第一次运行程序
            SetDPI(3);
            SetDisplay(0);
           // SetLanguage(0);
            SetQuality(2);
            SetVolume(1f);
            SetScreen(true);
        }

        /// <summary>
        /// 加载窗口属性
        /// </summary>
        public static void LoadPlayerprefs()
        {
            DirectoryInfo initInfo = Directory.CreateDirectory(savePath);
            initInfo.Attributes = FileAttributes.Hidden;
            if (System.IO.File.Exists(savePath + "/QualitySettings2.ini"))
            {
                //读取数据
                StreamReader sr = new StreamReader(savePath + "/QualitySettings2.ini");
                lineCounter = 0;
                while ((lineToRead = sr.ReadLine()) != null)
                {
                    splitLine = lineToRead.Split('=');
                    inPut[lineCounter] = splitLine[1];
                    lineCounter++;
                }
                sr.Close();

                _intQualityLevel = int.Parse(inPut[0]);
                _fltSound = float.Parse(inPut[1]);
                _intWantedKey = int.Parse(inPut[2]);
                _bolFullScreen = bool.Parse(inPut[3]);
                _intLanguage = int.Parse(inPut[4]);
            }
            else
            {
                DefaultSetting();
            }
        }

        /// <summary>
        /// 保存窗口属性
        /// </summary>
        public static void SavePlayerprefs()
        {
            StreamWriter wr = new StreamWriter(savePath + "/QualitySettings2.ini");

            string str_QualityLevel = _intQualityLevel.ToString();
            string str_SldSound = _fltSound.ToString();
            string str_IntWanteKey = _intWantedKey.ToString();
            string str_BolDisplay = _bolFullScreen.ToString();
            string str_IntLanguage = _intLanguage.ToString();

            outPut[0] = string.Format("QualittyLevel={0}", str_QualityLevel);
            outPut[1] = string.Format("VolumeLevel={0}", str_SldSound);
            outPut[2] = string.Format("ScreenKey={0}", str_IntWanteKey);
            outPut[3] = string.Format("Display={0}", str_BolDisplay);
            outPut[4] = string.Format("Language={0}", str_IntLanguage);

            for (int i = 0; i < outPut.Length; i++)
            {
                wr.WriteLine(outPut[i]);
            }

            wr.Close();
        }

    }
}

