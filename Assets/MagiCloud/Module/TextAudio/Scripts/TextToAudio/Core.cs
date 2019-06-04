using MagiCloud.TextAudio;

namespace MagiCloud.TextToAudio
{
    public class Core
    {  /// <summary>
       /// 完成事件
       /// </summary>
        public event OnFinished onFinishEvent;
        /// <summary>
        /// 错误事件
        /// </summary>
        public event OnError onErrorEvent;
        public bool active;
        public Core(string config)
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
            }
        }
        ~Core()
        {
            var ret = MSCDLL.MSPLogout();
            if (ret!=(int)ErrorCode.MSP_SUCCESS)
                if (onErrorEvent!=null)
                    onErrorEvent.Invoke("退出出错."+ret);
        }
    }
}
