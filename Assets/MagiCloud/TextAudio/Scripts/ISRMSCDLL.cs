using System;
using System.Runtime.InteropServices;

namespace MagiCloud.TextAudio
{
    public partial class MSCDLL
    {
        /// <summary>
        /// 开启一次语音识别
        /// </summary>
        /// <param name="grammarList"></param>
        /// <param name="param"></param>
        /// <param name="errcode"></param>
        /// <returns></returns>
        [DllImport(mscdll,CharSet = CharSet.Unicode,CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr QISRSessionBegin(string grammarList,string param,ref int errcode);

        /// <summary>
        /// 写入本次识别的音频
        /// </summary>
        /// <param name="sessionID"></param>
        /// <param name="waveData"></param>
        /// <param name="waveLen"></param>
        /// <param name="audioStatus"></param>
        /// <param name="epStatus"></param>
        /// <param name="rsltStatus"></param>
        /// <returns></returns>
        [DllImport(mscdll,CharSet = CharSet.Unicode,CallingConvention = CallingConvention.StdCall)]
        public static extern int QISRAudioWrite(string sessionID,byte[] waveData,uint waveLen,AudioStatus audioStatus,ref EpStatus epStatus,ref RsltStatus rsltStatus);

        /// <summary>
        /// 获取识别结果
        /// </summary>
        /// <param name="sessionID"></param>
        /// <param name="rsltStatus"></param>
        /// <param name="waitTime"></param>
        /// <param name="errorCode"></param>
        /// <returns></returns>
        [DllImport(mscdll,CharSet = CharSet.Unicode,CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr QISRGetResult(string sessionID,ref RsltStatus rsltStatus,int waitTime,ref int errorCode);

        /// <summary>
        ///  结束本次语音识别
        /// </summary>
        /// <param name="sessionID"></param>
        /// <param name="hints"></param>
        /// <returns></returns>
        [DllImport(mscdll,CharSet = CharSet.Unicode,CallingConvention = CallingConvention.StdCall)]
        public static extern int QISRSessionEnd(string sessionID,string hints);

        /// <summary>
        /// 获取当次语音识别信息
        /// </summary>
        /// <param name="sessionID"></param>
        /// <param name="paramName"></param>
        /// <param name="paramValue"></param>
        /// <param name="valueLen"></param>
        /// <returns></returns>
        [DllImport(mscdll,CharSet = CharSet.Unicode,CallingConvention = CallingConvention.StdCall)]
        public static extern int QISRGetParam(string sessionID,string paramName,string paramValue,ref uint valueLen);

    }
}
