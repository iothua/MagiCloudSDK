using System.Collections;
using UnityEngine;
using System;



/// <summary>
/// 摄像机资源
/// </summary>
public class CameraInfo :MonoBehaviour {

    public Camera Camera;

    public string ClearFlags {
        get {
            return Camera.clearFlags.ToString();
        }
    }

    public string Depth { get { return Camera.depth.ToString(); } }
    public string Depict; //描述
}
