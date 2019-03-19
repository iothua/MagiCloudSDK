using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

namespace MagiCloud.Restfuls
{
    /// <summary>
    /// Restful接口对接 
    /// 参考网址    https://www.jianshu.com/p/e5b4236166d7
    /// </summary>
    public class RestfulController : MonoBehaviour
    {

        IEnumerator UnityWebGet(string url)
        {
            var webRequest = UnityWebRequest.Get(url);

            yield return webRequest.SendWebRequest();

            Debug.Log("值：" + webRequest.downloadHandler.text);
        }

        IEnumerator UnityWebPost(string url)
        {
            Dictionary<string, string> JsonDic = new Dictionary<string, string>();
            JsonDic.Add("Content-Type", "application/json");

            //请求Json数据
            Dictionary<string, string> UserDic = new Dictionary<string, string>();
            UserDic["height"] = "170";
            UserDic["weight"] = "62";
            string data = Newtonsoft.Json.JsonConvert.SerializeObject(UserDic);

            //转化为字节
            byte[] post_data;
            post_data = System.Text.UTF8Encoding.UTF8.GetBytes(data);

            var webRequest = UnityWebRequest.Post(url, data);

            yield return webRequest.SendWebRequest();

            
        }
    }
}
