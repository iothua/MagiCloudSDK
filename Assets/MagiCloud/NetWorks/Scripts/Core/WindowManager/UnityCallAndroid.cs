#if UNITY_ANDROID
using System.Collections.Generic;
using UnityEngine;

public class UnityCallAndroid
{
    AndroidJavaObject curActivity;
    List<string> caches;
    private string curPackageName = "";
    private int maxActiveApp = 5;       //允许最多后台数量，当超出这个数时，从后台关闭
    public int Count => caches.Count;
    public string CurPackageName => curPackageName;

    public UnityCallAndroid()
    {
        Init();
    }

    public void Init()
    {
        caches=new List<string>();
        using (AndroidJavaClass pluginClass = new AndroidJavaClass("com.magicloud.backstagemanager"))
        {
            //AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            //curActivity=androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
            if (pluginClass!=null)
            {
                curActivity= pluginClass.CallStatic<AndroidJavaObject>("getInstance");
            }
        }
        //  curActivity =new AndroidJavaObject("com.magicloud.backstagemanager.MainActivity");
    }

    public void ActiveOtherApp(string packageName)
    {
        Debug.Log(packageName);
        if (curPackageName!=packageName)
        {
            curPackageName=packageName;
        }
        AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        curActivity=androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
        if (curActivity!=null)
            curActivity.Call("ActiveOtherApp",packageName);
        AddToCache(packageName);
    }

    private void AddToCache(string packageName)
    {
        if (caches.Contains(packageName))
        {
            caches.Remove(packageName);
        }
        caches.Add(packageName);
        if (Count>maxActiveApp)
        {
            StopApp(caches[0]);
            caches.Remove(packageName);
        }
    }
    public void ExitApp(string packageName)
    {
        Debug.Log(packageName);
        AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        curActivity=androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
        if (curActivity!=null)
            curActivity.Call("Exit",packageName);
    }

    public void StopApp(string packageName)
    {
        AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        curActivity=androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
        if (curActivity!=null)
            curActivity.Call("StopApp",packageName);

    }

    /// <summary>
    /// 关闭所有
    /// </summary>
    public void ExitAll()
    {
        for (int i = 0; i < Count; i++)
        {
            ExitApp(caches[i]);
        }
        caches.Clear();
    }


    /// <summary>
    /// 判断是否存在APP
    /// </summary>
    /// <param name="packageName"></param>
    /// <returns></returns>
    private bool CheckAppExist(string packageName)
    {
        return curActivity.Call<bool>("CheckAppExist",packageName);
    }
}
#endif