using System;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Security.Cryptography;

namespace MagiCloud.TextToAudio
{
    public class AudioHelper
    {
        /// <summary>
        /// 加载
        /// </summary>
        /// <param name="audioPath"></param>
        /// <param name="name"></param>
        /// <param name="onGet"></param>
        /// <returns></returns>
        public static IEnumerator LoadFromFile(string audioPath,string name,UnityAction<AudioClip> onGet)
        {
            string url = audioPath+"/"+name;
            if (Application.platform==RuntimePlatform.WindowsEditor||Application.platform==RuntimePlatform.WindowsPlayer)
                url="file:///"+url;
            WWW www = new WWW(url);
            yield return www;
            if (www.error!=null)
            {
               // Debug.Log("??");
                onGet(null);
            }
            else
            {
                onGet(www.GetAudioClip(false,false,AudioType.WAV));
            }
        }
        public static AudioClip CreateWavAudio(string audioName,byte[] bytes)
        {
            WAV wav = new WAV(bytes);
            AudioClip audioClip = AudioClip.Create(audioName,wav.SampleCount,1,wav.Frequency,false);
            audioClip.SetData(wav.LeftChannel,0);
            return audioClip;
        }

        /// <summary>
        /// 计算字串的MD5
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string Md5(byte[] source)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] md5Data = md5.ComputeHash(source,0,source.Length);
            md5.Clear();
            string temp = "";
            for (int i = 0; i < md5Data.Length; i++)
            {
                temp+=Convert.ToString(md5Data[i],16).PadLeft(2,'0');
            }
            temp=temp.PadLeft(32,'0');
            return temp;
        }

        /// <summary>
        /// 计算字符串的MD5值
        /// </summary>
        public static string Md5(string source)
        {
            byte[] data = System.Text.Encoding.UTF8.GetBytes(source);

            return Md5(data);
        }
    }
}