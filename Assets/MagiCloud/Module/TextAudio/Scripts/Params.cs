using System;
namespace MagiCloud.TextToAudio
{
    [Serializable]
    public class Params
    {
        /// <summary>
        /// 引擎类型
        /// </summary>
        public string engine_type = "cloud";
        /// <summary>
        ///发音人
        /// </summary>
        public string voice_name = "xiaoyan";
        /// <summary>
        /// 语速
        /// </summary>
        public byte speed = 50;
        /// <summary>
        /// 音量
        /// </summary>
        public byte volume = 50;
        /// <summary>
        /// 语调
        /// </summary>
        public byte pitch = 50;
        ///// <summary>
        ///// 合成资源路径
        ///// </summary>
        //public string tts_res_path;
        /// <summary>
        /// 数字发音
        /// </summary>
        public byte rdn = 0;
        /// <summary>
        /// 1的中文发音
        /// </summary>
        public byte rcn = 1;
        /// <summary>
        /// 文本编码格式
        /// </summary>
        public string text_encoding = "Unicode";
        /// <summary>
        /// 合成音频采样率
        /// </summary>
        public short sample_rate = 16000;
        /// <summary>
        /// 背景音
        /// </summary>
        public byte background_sound = 0;
        /// <summary>
        /// 音频编码格式和压缩等级
        /// </summary>
        public string aue = "speex-wb;7";
        /// <summary>
        /// 文本类型
        /// </summary>
        public string ttp = "text";
        ///// <summary>
        ///// 语速增强
        ///// </summary>
        public byte speed_increase = 1;
        ///// <summary>
        ///// 合成音效
        ///// </summary>
        //public byte effect = 0;
        public override string ToString()
        {

            var fields = typeof(Params).GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.GetField | System.Reflection.BindingFlags.Instance);
            var param = new string[fields.Length];
            for (int i = 0; i < fields.Length; i++)
            {
                param[i] = fields[i].Name + "=" + fields[i].GetValue(this);
            }
            return string.Join(",",param);
        }
    }
}
