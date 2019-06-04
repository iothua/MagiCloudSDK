using System.Collections.Generic;
using UnityEngine;

public class UnityCallAndroid
{
    AndroidJavaObject curActivity;
    private string curPackageName = "";

    public string CurPackageName { get { return curPackageName; } }

    public UnityCallAndroid()
    {
        Init();
    }

    public void Init()
    {
        curActivity =new AndroidJavaObject("com.magicloud.backstagemanager.MainActivity");

    }

    public void ActiveOtherApp(string packageName)
    {
        if (curPackageName!=packageName)
        {
            curPackageName=packageName;
        }

        if (curActivity!=null)
            curActivity.Call("ActiveOtherApp",packageName);
    }

    public void ExitApp(string packageName)
    {
        //if (curActivity!=null)
        //    curActivity.Call("Exit",packageName);
    }

    public void StopApp()
    {
        if (curActivity!=null)
            curActivity.Call("Stop");

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
