using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MagiCloud.NetWorks;
using System;
using UnityEngine.Networking;
using MagiCloudPlatform.Data;

namespace MagiCloudPlatform
{

    /// <summary>
    /// 平台管理
    /// 思路：
    /// 1、判断是否联网
    /// 2、加载本地资源(如果没联网，加载本地资源)
    /// 3、如果联网，则下载资源到本地
    /// </summary>
    public class PlatformManager : MonoBehaviour
    {
        public ServerManager serverManager;

        public string url;

        /// <summary>
        /// 是否连接网络
        /// </summary>
        public bool IsNetworking { get; private set; }

        private void Start()
        {
            OnHttpPost(url, null, (jsonData,isError)=> {
                if (isError) return;

                var datas = MagiCloud.Json.JsonHelper.JsonToObject<ProductData>(jsonData);
            });
        }

        public void OnHttpGet(string url, Action<string, bool> callback)
        {
            StartCoroutine(UntiyWebGet(url, callback));
        }

        public void OnHttpPost(string url, WWWForm form,Action<string, bool> callback)
        {
            StartCoroutine(UnityWebPost(url, form,callback));
        }

        IEnumerator UntiyWebGet(string url, Action<string,bool> callback)
        {
            var webRequest = UnityWebRequest.Get(url);

            yield return webRequest.SendWebRequest();

            bool isError = false;
            if (webRequest.isHttpError || webRequest.isNetworkError)
            {
                isError = true;
            }

            if (callback != null)
                callback(webRequest.downloadHandler.text, isError);
        }

        IEnumerator UnityWebPost(string url,WWWForm fieldForm = null, Action<string, bool> callback = null)
        {
            string postData = string.Empty;

            UnityWebRequest webRequest;

            if (fieldForm == null)
            {
                webRequest = UnityWebRequest.Post(url, postData);
            }
            else
            {
                webRequest = UnityWebRequest.Post(url, fieldForm);
            }

            var testRequest = UnityWebRequest.Head(url);

            yield return testRequest.SendWebRequest();

            Debug.Log(testRequest.timeout);

            yield return webRequest.SendWebRequest();

            bool isError = false;
            if (webRequest.isHttpError || webRequest.isNetworkError)
            {
                isError = true;
            }

            if (callback != null)
                callback(webRequest.downloadHandler.text, isError);
        }
    }
}
