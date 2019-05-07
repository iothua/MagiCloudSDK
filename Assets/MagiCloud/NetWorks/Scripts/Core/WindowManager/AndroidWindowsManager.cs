#if UNITY_ANDROID
using UnityEngine;

namespace MagiCloud.NetWorks
{
    /// <summary>
    /// 在安卓上的实验窗口管理
    /// </summary>
    public class AndroidWindowsManager :WindowsManager
    {

        UnityCallAndroid callAndroid;
        public AndroidWindowsManager() : base()
        {

            callAndroid=new UnityCallAndroid();
        }

        protected override string ReadJson()
        {
            WWW www = new WWW(Url);
            while (!www.isDone)
            {

            }
            string json = www.text;
            return json;
        }


        protected override void OpenOther(ExperimentInfo info)
        {
            callAndroid.ActiveOtherApp(projectPaths[info.OwnProject]);
        }
        public override void SetTop()
        {
            callAndroid.ActiveOtherApp(Application.installerName);
        }
    }
}
#endif