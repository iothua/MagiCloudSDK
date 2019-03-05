using System;
using System.Runtime.InteropServices;

namespace MagiCloud.TextAudio
{
    public delegate void OnFinished(string text,byte[] bytes);
    public delegate void OnError(string error);

    public partial class MSCDLL
    {
        #region TTS dll import
#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_ANDROID
        public const string mscdll = "msc";
#elif UNITY_IOS
        public const string mscdll = "__Internal";
#elif UNITY_WEBGL
        public const string mscdll = "__Internal";
#endif
        [DllImport(mscdll,CallingConvention = CallingConvention.StdCall)]
        public static extern int MSPLogin(string usr,string pwd,string parameters);
        [DllImport(mscdll,CallingConvention = CallingConvention.StdCall)]
        public static extern int MSPLogout();

        [DllImport(mscdll,CharSet = CharSet.Ansi,CallingConvention = CallingConvention.Winapi)]
        public static extern IntPtr QTTSSessionBegin(string _params,ref int errorCode);

        [DllImport(mscdll,CharSet = CharSet.Unicode,CallingConvention = CallingConvention.Winapi)]
        public static extern int QTTSTextPut(string sessionID,string textString,uint textLen,string _params);

        [DllImport(mscdll,CharSet = CharSet.Unicode,CallingConvention = CallingConvention.Winapi)]
        public static extern IntPtr QTTSAudioGet(string sessionID,ref uint audioLen,ref SynthStatus synthStatus,ref int errorCode);

        [DllImport(mscdll,CharSet = CharSet.Unicode,CallingConvention = CallingConvention.Winapi)]
        public static extern IntPtr QTTSAudioInfo(string sessionID);

        [DllImport(mscdll,CharSet = CharSet.Unicode,CallingConvention = CallingConvention.Winapi)]
        public static extern int QTTSSessionEnd(string sessionID,string hints);

        [DllImport(mscdll,CharSet = CharSet.Unicode,CallingConvention = CallingConvention.StdCall)]
        public static extern int QTTSGetParam(string sessionID,string paramName,string paramValue,ref uint valueLen);


        #endregion
    }
}