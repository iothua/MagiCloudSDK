using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MagiCloud.Core;

namespace MagiCloud.KGUI
{
    /// <summary>
    /// KGUI_Canvas
    /// </summary>
    [ExecuteInEditMode]
    [DefaultExecutionOrder(-800)]
    public class KGUI_Canvas : MonoBehaviour
    {
        private Canvas canvas;

        private MBehaviour behaviour;

        private void Awake()
        {
            //behaviour = new MBehaviour(ExecutionPriority.High, -800, enabled);
            //behaviour.OnAwake_MBehaviour(() =>
            //{
            //    canvas = GetComponent<Canvas>();
            //    canvas.renderMode = RenderMode.ScreenSpaceCamera;
            //    canvas.worldCamera = MUtility.UICamera;
            //});

            canvas = GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = MUtility.UICamera;
        }

        //private void OnEnable()
        //{
        //    if (behaviour != null)
        //        behaviour.IsEnable = true;
        //}

        //private void OnDisable()
        //{
        //    if (behaviour != null)
        //        behaviour.IsEnable = false;
        //}

        //private void OnDestroy()
        //{
        //    if (behaviour != null)
        //        behaviour.OnExcuteDestroy();
        //}
    }
}

