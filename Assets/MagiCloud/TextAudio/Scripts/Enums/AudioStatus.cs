namespace MagiCloud.TextAudio
{
    /// <summary>
    /// 用来告知MSC音频发送是否完成
    /// </summary>
    public enum AudioStatus
    {
        /// <summary>
        /// 第一块音频
        /// </summary>
        MSP_AUDIO_SAMPLE_FIRST = 1,
        /// <summary>
        /// 还有后继音频
        /// </summary>
        MSP_AUDIO_SAMPLE_CONTINUE = 2,
        /// <summary>
        /// 最后一块音频
        /// </summary>
        MSP_AUDIO_SAMPLE_LAST = 4
    }

}