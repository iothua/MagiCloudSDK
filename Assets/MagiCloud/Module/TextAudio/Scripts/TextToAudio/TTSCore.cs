using MagiCloud.TextAudio;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace MagiCloud.TextToAudio
{
  

    /// <summary>
    /// 语音合成核心，负责与DLL对接
    /// </summary>
    public class TTSCore
    {
        //private const string HENRY = "henry";
        private string sessionID;
        /// <summary>
        /// 完成事件
        /// </summary>
        public event OnFinished onFinishEvent;
        /// <summary>
        /// 错误事件
        /// </summary>
        public event OnError onErrorEvent;

        public bool active;

        public TTSCore(string config)
        {

            int ret = MSCDLL.MSPLogin(null,null,config);
            if (ret!=(int)ErrorCode.MSP_SUCCESS)
            {
                if (onErrorEvent!=null)
                    onErrorEvent.Invoke("登录出错."+ret);
            }
            else
            {
                active=true;
                //Debug.Print("登录成功");
            }
        }
        ~TTSCore()
        {
            var ret = MSCDLL.MSPLogout();
            if (ret!=(int)ErrorCode.MSP_SUCCESS)
                if (onErrorEvent!=null)
                    onErrorEvent.Invoke("退出出错."+ret);
        }

        /// <summary>
        /// 音频头部格式
        /// </summary>
        private struct WaveHeader
        {
            public int riffID;
            public int fileSize;
            public int riffType;
            public int fmtID;
            public int fmtSize;
            public short fmtTag;
            /// <summary>
            /// 声道数目
            /// </summary>
            public ushort fmtChannel;
            /// <summary>
            /// 采样频率
            /// </summary>
            public int fmtSamplesPerSec;
            /// <summary>
            /// 每秒所需字节数
            /// </summary>
            public int avgBytesPerSec;
            /// <summary>
            /// 数据块对齐单位
            /// </summary>
            public ushort blockAlign;
            /// <summary>
            /// 每个采样需要的bit数
            /// </summary>
            public ushort BitsPerSameple;

            public int dataID;
            public int dataSize;
        }

        WaveHeader GetWaveHeader(int dataLen)
        {
            WaveHeader header = new WaveHeader()
            {
                riffID=0x46464952,
                fileSize=dataLen+36,
                riffType=0x45564157,
                fmtID=0x20746D66,
                fmtSize=16,
                fmtTag=0x0001,
                fmtChannel=1,
                fmtSamplesPerSec=16000,
                avgBytesPerSec=32000,
                blockAlign=2,
                BitsPerSameple=16,
                dataID=0x61746164,
                dataSize=dataLen
            };
            return header;
        }

        /// <summary>
        /// 结构体转化位字节序列
        /// </summary>
        /// <param name="structure"></param>
        /// <returns></returns>
        Byte[] StructToBytes(Object structure)
        {
            Int32 size = Marshal.SizeOf(structure);
            IntPtr buffer = Marshal.AllocHGlobal(size);
            try
            {
                Marshal.StructureToPtr(structure,buffer,false);
                Byte[] bytes = new Byte[size];
                Marshal.Copy(buffer,bytes,0,size);
                return bytes;
            }
            finally
            {
                Marshal.FreeHGlobal(buffer);
            }
        }

        /// <summary>
        /// 文本合成
        /// </summary>
        /// <param name="text"></param>
        /// <param name="param"></param>
        /// <param name="waveFile"></param>
        public void TextToSpeech(string text,string param,string waveFile)
        {
            byte[] bytes = null;
            int ret = 0;
            try
            {
                sessionID=Marshal.PtrToStringAuto(MSCDLL.QTTSSessionBegin(param,ref ret));
                //UnityEngine.Debug.Log(sessionID);
                if (ret!=(int)ErrorCode.MSP_SUCCESS)
                {
                    if (onErrorEvent!=null)
                        onErrorEvent.Invoke("初始化出错."+ret);
                    return;
                }
                //  UnityEngine.Debug.Log("初始化成功");
                ret=MSCDLL.QTTSTextPut(sessionID,text,(uint)Encoding.Unicode.GetByteCount(text),string.Empty);
                if (ret!=(int)ErrorCode.MSP_SUCCESS)
                {
                    if (onErrorEvent!=null)
                        onErrorEvent.Invoke("发生数据出错."+ret);
                    return;
                }
                //  UnityEngine.Debug.Log("发生数据成功");
                IntPtr audioData;
                uint audioLen = 0;
                SynthStatus synthStatus = SynthStatus.MSP_TTS_FLAG_STILL_HAVE_DATA;
                using (MemoryStream ms = new MemoryStream())
                {
                    ms.Write(new byte[44],0,44);
                    while (synthStatus==SynthStatus.MSP_TTS_FLAG_STILL_HAVE_DATA)
                    {
                        audioData = MSCDLL.QTTSAudioGet(sessionID,ref audioLen,ref synthStatus,ref ret);
                        if (audioData!=IntPtr.Zero)
                        {
                            byte[] data = new byte[audioLen];
                            Marshal.Copy(audioData,data,0,data.Length);
                            ms.Write(data,0,data.Length);
                            if (synthStatus==SynthStatus.MSP_TTS_FLAG_DATA_END||ret!=(int)ErrorCode.MSP_SUCCESS)
                            {
                                if (ret!=(int)ErrorCode.MSP_SUCCESS)
                                {
                                    if (onErrorEvent!=null)
                                        onErrorEvent.Invoke("下载文件错误."+ret);
                                    return;
                                }
                                break;
                            }
                        }
                        Thread.Sleep(150);
                    }
                    //UnityEngine.Debug.Log("wav header");
                    WaveHeader header = GetWaveHeader((int)ms.Length-44);
                    byte[] headerByte = StructToBytes(header);
                    ms.Position=0;
                    ms.Write(headerByte,0,headerByte.Length);
                    bytes=ms.ToArray();
                    ms.Close();
                }
                if (waveFile!=null)
                {
                    if (File.Exists(waveFile))
                    {
                        File.Delete(waveFile);
                    }
                    File.WriteAllBytes(waveFile,bytes);
                }

            }
            catch (Exception ex)
            {
                if (onErrorEvent!=null) onErrorEvent.Invoke("Error:"+ex.Message);
                return;
            }
            finally
            {
                ret =MSCDLL.QTTSSessionEnd(sessionID,"");
                if (ret !=(int)ErrorCode.MSP_SUCCESS)
                {
                    if (onErrorEvent!=null)
                        onErrorEvent.Invoke("结束时出错."+ret);
                }
                else
                {
                    if (onFinishEvent!=null)
                        onFinishEvent.Invoke(text,bytes);
                }
            }
        }

    }
}


