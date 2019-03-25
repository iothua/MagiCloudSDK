using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MagiCloud.KGUI;
using DG.Tweening;

namespace MagiCloud.UITool
{
    public class UIToolManager : MonoBehaviour
    {
        //打开时的坐标值
        [SerializeField,Header("关闭坐标")]
        private Vector3 closePosition;

        //关闭时的坐标值
        [SerializeField,Header("打开坐标")]
        private Vector3 openPosition;

        [SerializeField,Header("移动的对象")]
        private Transform moveTransform;

        public bool IsOpen { get; set; }
        public bool IsOpening { get; set; }

        [SerializeField,Header("工具按钮")]
        private KGUI_Button toolButton;

        [SerializeField,Header("容器对象")]
        private KGUI_Panel panel;

        [SerializeField,Header("持续时间")]
        private float continuedTime;

        private WaitForSeconds forSeconds;

        private int handIndex = -1;

        private UIToolButton[] ToolButtons;

        private KGUI_ButtonGroup buttonGroup;

        public void OnInitialize()
        {
            buttonGroup = gameObject.GetComponentInChildren<KGUI_ButtonGroup>();

            ToolButtons = gameObject.GetComponentsInChildren<UIToolButton>();
            for (int i = 0; i < ToolButtons.Length; i++)
            {
                //先都禁用掉
                ToolButtons[i].Button.IsEnable = false;
            }

            //if (MSwitchManager.ActiveMode == OperateModeType.Operate)
            //{
            //    SetButton(OperateModeType.Operate);
            //}

            foreach (var item in System.Enum.GetValues(typeof(OperateModeType)))
            {
                if ((MSwitchManager.ActiveMode & ((OperateModeType)item)) != 0)
                {
                    SetButton((OperateModeType)item);
                }
            }

            //switch (MSwitchManager.ActiveMode)
            //{
            //    case OperateModeType.Operate:
            //        break;

            //    case OperateModeType.Rotate:
            //        SetButton(OperateModeType.Rotate);
            //        break;
            //    case OperateModeType.Tool:
            //        SetButton(OperateModeType.Tool);

            //        break;
            //    case OperateModeType.Zoom:
            //        SetButton(OperateModeType.Zoom);
            //        break;
            //    default:
            //        break;
            //}

            forSeconds = new WaitForSeconds(continuedTime);

            toolButton.onEnter.AddListener(OnEnter);
            panel.onExit.AddListener(OnExit);
        }

        void SetButton(OperateModeType modeType)
        {
            var button = GetToolButton(modeType);
            if (button != null)
                button.Button.IsEnable = true;

            if (MSwitchManager.CurrentMode == modeType)
            {
                buttonGroup.SetButton(button.Button);
            }
        }

        UIToolButton GetToolButton(OperateModeType operate)
        {
            for (int i = 0; i < ToolButtons.Length; i++)
            {
                if (ToolButtons[i].modeType == operate)
                    return ToolButtons[i];
            }

            return null;
        }

        private void OnDestroy()
        {
            toolButton.onEnter.RemoveListener(OnEnter);
            panel.onExit.RemoveListener(OnExit);
        }

        /// <summary>
        /// 移入
        /// </summary>
        public void OnEnter(int handIndex)
        {
            if (IsOpen) return;
            this.handIndex = handIndex;
            StartCoroutine(RunOpen(true));
        }

        /// <summary>
        /// 退出
        /// </summary>
        public void OnExit(int handIndex)
        {
            if (!IsOpen) return;

            if (this.handIndex != handIndex) return;

            handIndex = -1;

            StartCoroutine(RunOpen(false));
        }

        IEnumerator RunOpen(bool isOpen)
        {
            IsOpening = true;

            toolButton.IsEnable = false;

            moveTransform.DOLocalMove(isOpen ? openPosition : closePosition, continuedTime);

            yield return forSeconds;

            IsOpening = false;

            toolButton.IsEnable = true;

            IsOpen = isOpen;

        }
    }
}

