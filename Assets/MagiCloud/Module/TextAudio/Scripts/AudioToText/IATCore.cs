using MagiCloud.TextAudio;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

namespace MagiCloud.AudioToText
{
    /// <summary>
    /// 语音听写核心部分
    /// </summary>
    public class IATCore
    {
        /// <summary>
        /// 完成事件
        /// </summary>
        public event OnFinished onFinishEvent;
        /// <summary>
        /// 错误事件
        /// </summary>
        public event OnError onErrorEvent;
        /// <summary>
        /// 上传用户词表
        /// </summary>
        /// <returns></returns>
        private int UploadUserwords()
        {
            int ret = -1;
            return ret;
        }

        public void RunIat(string audioFile,string param)
        {
            string sessionID = null;
            EpStatus epStatus = EpStatus.MSP_EP_LOOKING_FOR_SPEECH;
            RsltStatus rsltStatus = RsltStatus.MSP_REC_STATUS_SUCCESS;
            long count = 0; //计次?
            uint totalLen = 0;
            //获取音频文件
            using (FileStream fs = new FileStream(audioFile,FileMode.Open,FileAccess.Read))
            {

                fs.Seek(0,SeekOrigin.End);
                var size = fs.Length;
                fs.Seek(0,SeekOrigin.Begin);
                var data = new byte[size];
                if (data==null)
                {
                    if (onErrorEvent!=null)
                        onErrorEvent.Invoke("内存不足.");
                    return;
                }
                var readSize = fs.Read(data,1,(int)size);
                if (readSize!=size)
                {
                    onErrorEvent.Invoke("读"+audioFile+"出错");
                    return;
                }

                int errcode = 0;
                //开始语音听写
                sessionID=Marshal.PtrToStringAuto(MSCDLL.QISRSessionBegin(null,param,ref errcode));
                if (errcode!=(int)ErrorCode.MSP_SUCCESS)
                {
                    if (onErrorEvent!=null)
                        onErrorEvent.Invoke("Iat开启出错"+errcode);
                    return;
                }
                while (true)
                {
                    uint len = 10*640;
                    int ret = 0;
                    if (size<2*len) len=(uint)size;
                    if (len<=0) break;
                    var audioStatus = AudioStatus.MSP_AUDIO_SAMPLE_CONTINUE;
                    if (count==0)
                        audioStatus=AudioStatus.MSP_AUDIO_SAMPLE_FIRST;
                    ret=MSCDLL.QISRAudioWrite(sessionID,data,len,audioStatus,ref epStatus,ref rsltStatus);
                    if (ret!=(int)ErrorCode.MSP_SUCCESS)
                    {
                        if (onErrorEvent!=null)
                            onErrorEvent.Invoke("写入本次识别的音频失败."+ret);
                        return;
                    }
                    count+=len;
                    size-=len;
                    //已经有部分听写结果
                    if (rsltStatus==RsltStatus.MSP_REC_STATUS_SUCCESS)
                    {
                        var result = Marshal.PtrToStringAuto(MSCDLL.QISRGetResult(sessionID,ref rsltStatus,0,ref ret));
                        if (ret!=0)
                        {
                            if (onErrorEvent!=null)
                                onErrorEvent.Invoke("获取识别结果失败."+ret);
                            return;
                        }
                        if (result!=null)
                        {
                            uint resultLen = (uint)result.Length;
                            totalLen+=resultLen;
                            if (totalLen>=4096)
                            {
                                if (onErrorEvent!=null)
                                    onErrorEvent.Invoke("对于临时资源没有足够的缓存空间。");
                                return;
                            }
                        }
                    }
                    if (epStatus==EpStatus.MSP_EP_AFTER_SPEECH)
                        break;
                    Thread.Sleep(200);
                }
                errcode=MSCDLL.QISRAudioWrite(sessionID,null,0,AudioStatus.MSP_AUDIO_SAMPLE_LAST,ref epStatus,ref rsltStatus);
                if (errcode!=0)
                {
                    if (onErrorEvent!=null)
                        onErrorEvent.Invoke("音频写入失败。"+errcode);
                    return;
                }

            }
        }

    }
}
