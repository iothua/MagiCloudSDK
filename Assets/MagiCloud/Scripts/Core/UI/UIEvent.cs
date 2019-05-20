using System;
using UnityEngine;
using UnityEngine.Events;

namespace MagiCloud.Core.UI
{

    [Serializable]
    public class ButtonEvent :UnityEvent<int> { }
    [Serializable]
    public class EventObject :UnityEvent<GameObject> { }
    [Serializable]
    public class EventVector3 :UnityEvent<int,Vector3> { }

    [Serializable]
    public class EventFloat :UnityEvent<float> { }

    /// <summary>
    /// Button是否在区域内
    /// </summary>
    [Serializable]
    public class PanelEvent :UnityEvent<int,bool> { }

    [Serializable]
    public class ToggleEvent :UnityEvent<bool> { }

}
