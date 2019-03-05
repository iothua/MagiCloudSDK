using UnityEditor;
using UnityEditor.Callbacks;
using System;

public static class KinectFacePostBuildCopyPluginData
{
#if UNITY_EDITOR
    [PostProcessBuild]
    public static void OnPostProcessBuild(BuildTarget target, string path)
    {
        KinectCopyPluginDataHelper.CopyPluginData(target, path, "NuiDatabase");
    }
#endif
}
