using System;
using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

namespace MagiCloud.Downloads
{
    public class ResetfulController
    {

        private MonoBehaviour behaviour;

        public ResetfulController(MonoBehaviour behaviour)
        {
            this.behaviour = behaviour;
        }

        public void OnHttpGet(string url, Action<string, bool> callback)
        {
            behaviour.StartCoroutine(UnityWebGet(url, callback));
        }

        public void OnHttpPost(string url, WWWForm form, Action<string, bool> callback)
        {
            behaviour.StartCoroutine(UnityWebPost(url, form, callback));
        }

        public IEnumerator UnityWebGet(string url, Action<string, bool> callback)
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

        public IEnumerator UnityWebPost(string url, WWWForm fieldForm = null, Action<string, bool> callback = null)
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

            //var testRequest = UnityWebRequest.Head(url);

            //yield return testRequest.SendWebRequest();

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
