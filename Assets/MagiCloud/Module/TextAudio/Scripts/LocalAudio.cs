using MagiCloud.Json;
using System;
using System.Collections.Generic;

namespace MagiCloud.TextToAudio
{
    /// <summary>
    /// 本地音频文件
    /// </summary>
    [Serializable]
    public class LocalAudio
    {
        public List<string> audioKey = new List<string>();
        public void Register(string text)
        {
            if (audioKey.Contains(text))
            {
                audioKey.Add(text);
            }
        }
        public void Remove(string text)
        {
            if (audioKey.Contains(text))
            {
                audioKey.Remove(text);
            }
        }
        public bool Contain(string text)
        {
            return audioKey.Contains(text);
        }
        public override string ToString()
        {
            return JsonHelper.ObjectToJsonString(this);
        }
    }
}
