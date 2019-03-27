using MagiCloud.Json;
using MagiCloud.TextAudio;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;
namespace MagiCloud.TextToAudio
{
    /// <summary>
    /// 文本转语音控制
    /// </summary>
    public class TextToAudioController
    {
        private static object lockHelper = new object();

        private static TextToAudioController instance = default(TextToAudioController);
        public static TextToAudioController Instance
        {
            get
            {
                if (instance==null)
                    lock (lockHelper)
                        if (instance==null)
                            instance=new TextToAudioController();
                return instance;
            }
        }

        public event UnityAction<string> onError;
        private const string key = "";
        private Thread downLoadThread;
        private Queue<KeyValuePair<string,Params>> waitAudioQueue = new Queue<KeyValuePair<string,Params>>();
        private bool connectError;

        private static string audioPath;
        private static string AudioPath
        {
            get
            {
                if (audioPath==null)
                {
                    audioPath=Application.streamingAssetsPath+"/TextAudio/TestToAudio";
                    if (!Directory.Exists(audioPath))
                        Directory.CreateDirectory(audioPath);
                }
                return audioPath;
            }
        }

        private LocalAudio localAudio;

        private TTSCore tCore;
        public TTSCore TCore
        {
            get
            {
                if (tCore==null)
                {
                    Config config = new Config("5c6cf5a0");
                    string s = config.ToString();
                    tCore=new TTSCore(s);
                }
                return tCore;
            }
        }

        private Params defulParams;
        public Params DefulParams
        {
            get
            {
                if (defulParams==null)
                {
                    defulParams=new Params();
                }
                return defulParams;
            }
        }



        protected TextToAudioController()
        {
            if (PlayerPrefs.HasKey(key))
            {
                var value = PlayerPrefs.GetString(key);
                if (!string.IsNullOrEmpty(value))
                {
                    localAudio=JsonHelper.JsonToObject<LocalAudio>(value);
                }
            }
            if (localAudio==null)
                localAudio=new LocalAudio();
        }

        /// <summary>
        /// 获取音频
        /// </summary>
        /// <param name="text"></param>
        /// <param name="onGet"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public IEnumerator GetAudioClip(string text,UnityAction<AudioClip> onGet,Params param = null)
        {
            if (param==null)
                param=DefulParams;
            string name = AudioFileName(text,param);
            if (!localAudio.Contain(name)&&Application.platform!=RuntimePlatform.WebGLPlayer)
            {
                if (File.Exists(Path.Combine(AudioPath,name)))
                {
                    RecordToText(name);
                    yield return AudioHelper.LoadFromFile(AudioPath,name,onGet);
                }
                else
                {
                    yield return DownLoadFromWeb(text,param,onGet);
                }
            }
            else
            {
                yield return AudioHelper.LoadFromFile(AudioPath,name,onGet);
            }
        }

        /// <summary>
        /// 从网络下载
        /// </summary>
        /// <param name="text"></param>
        /// <param name="param"></param>
        /// <param name="onGet"></param>
        /// <returns></returns>
        private IEnumerator DownLoadFromWeb(string text,Params param,UnityAction<AudioClip> onGet)
        {
            var path = AudioPath;
            string name = AudioFileName(text,param);
            bool complete = false;
            string error = null;
            OnFinished onFinishedEvent = (result,data) =>
            {
                if (result==text)
                {
                    complete=true;
                }
            };
            OnError onErrorEvent = (err) =>
             {
                 complete=true;
                 error=err;
             };

            TCore.onFinishEvent+=onFinishedEvent;
            TCore.onErrorEvent+=onErrorEvent;
            waitAudioQueue.Enqueue(new KeyValuePair<string,Params>(text,param));

            if (downLoadThread==null||!downLoadThread.IsAlive)
            {
                downLoadThread=new Thread(ThreadDownLoad);
                downLoadThread.Start(AudioPath);
            }
            yield return new WaitUntil(() => complete||connectError);
            if (connectError)
                error="err:联网失败";
            if (error!=null)
            {
                if (onError!=null)
                {
                    onError.Invoke(error);
                }
                else
                {
                    Debug.LogError(error);
                }
            }
            else
            {
                RecordToText(name);
                yield return AudioHelper.LoadFromFile(AudioPath,name,onGet);
            }
            TCore.onFinishEvent-=onFinishedEvent;
            TCore.onErrorEvent-=onErrorEvent;
        }

        /// <summary>
        /// 开启线程加载
        /// </summary>
        /// <param name="audioPath"></param>
        private void ThreadDownLoad(object audioPath)
        {
            float waitTime = 5000;
            //Debug.Log("线程加载");
            while (!TCore.active)
            {
                Thread.Sleep(100);
                waitTime-=100;
                if (waitTime<0)
                {
                    connectError=true;
                    return;
                }
            }
            while (waitAudioQueue.Count>0)
            {

                var item = waitAudioQueue.Dequeue();
                //Debug.Log(item.Key+":"+item.Value);
                TCore.TextToSpeech(item.Key,item.Value.ToString(),Path.Combine(audioPath.ToString(),AudioFileName(item.Key,item.Value)));
            }

        }


        /// <summary>
        /// 记录数据
        /// </summary>
        /// <param name="name"></param>
        private void RecordToText(string name)
        {
            localAudio.Register(name);
            PlayerPrefs.SetString(key,localAudio.ToString());
        }

        private string AudioFileName(string text,Params param)
        {
            var source = text+param.ToString();

            return AudioHelper.Md5(source)+".wav";
        }

        public IEnumerator Downland(string[] text,UnityAction<float> onProgressChanged,Params paramss = null)
        {
            if (paramss == null)
            {
                paramss = DefulParams;
            }
            List<string> needDownLand = new List<string>();

            foreach (var item in text)
            {
                if (!localAudio.audioKey.Contains(AudioFileName(item,paramss)))
                {
                    needDownLand.Add(item);
                }
            }

            float totalCount = text.Length;
            float currentCount = totalCount - needDownLand.Count;

            if (currentCount > 0 && onProgressChanged != null) onProgressChanged(currentCount / totalCount);

            if (needDownLand.Count > 0)
            {
                OnFinished finishEvent = (result,data) =>
                {
                    currentCount++;
                };

                OnError errorEvent = (err) =>
                {
                    currentCount++;
                };

                TCore.onFinishEvent += finishEvent;
                TCore.onErrorEvent += errorEvent;


                foreach (var item in needDownLand.ToArray())
                {
                    waitAudioQueue.Enqueue(new KeyValuePair<string,Params>(item,paramss));
                }

                if (downLoadThread == null || !downLoadThread.IsAlive)
                {
                    downLoadThread = new Thread(ThreadDownLoad);
                    downLoadThread.Start(AudioPath);
                }

                var countTemp = currentCount;

                while (currentCount != totalCount && !connectError)
                {
                    if (countTemp != currentCount)
                    {
                        if (onProgressChanged != null) onProgressChanged(currentCount / totalCount);
                        countTemp = currentCount;
                    }
                    yield return null;
                }

                for (int i = 0; i < needDownLand.Count; i++)
                {
                    RecordToText(AudioFileName(needDownLand[i],paramss));
                }

                TCore.onFinishEvent -= finishEvent;
                TCore.onErrorEvent -= errorEvent;
            }

        }


        public void ClearCache()
        {
            for (int i = 0; i <localAudio.audioKey.Count; i++)
            {
                string path = Path.Combine(AudioPath,localAudio.audioKey[i]);
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
            localAudio.audioKey.Clear();
            PlayerPrefs.SetString(key,"");
        }
    }

}
