using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MagiCloud.UITool
{
    /// <summary>
    /// UI工具按钮
    /// </summary>
    public class UIToolButton : MonoBehaviour
    {

        public OperateModeType modeType;

        [Header("屏蔽的UI层，默认是从小到大屏蔽")]
        public int order;

        [HideInInspector]
        public KGUI.KGUI_Button Button;

        private void Awake()
        {
            if (Button == null)
                Button = gameObject.GetComponent<KGUI.KGUI_Button>();
        }

        public void OnSetOperateMode()
        {
            MSwitchManager.CurrentMode = modeType;
            KGUI.UIShieldController.ShieldDownward(order);

            
        }
    }
}

