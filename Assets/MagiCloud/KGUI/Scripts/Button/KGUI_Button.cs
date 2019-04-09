using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.UI;

namespace MagiCloud.KGUI
{

    /// <summary>
    /// KGUI_Button
    /// </summary>
    public class KGUI_Button :KGUI_ButtonBase
    {

        public bool IsButtonGroup;
        public KGUI_ButtonGroup buttonGroup;

        public bool IsShowButton;

        public ButtonGroupReset onGroupReset;

        protected override void Start()
        {
            if (IsButtonGroup && IsShowButton)
                OnClick(0);
            else
                OnHandle("normal");
        }

        /// <summary>
        /// 按钮按下
        /// </summary>
        public override void OnClick(int handIndex)
        {
            if (IsButtonGroup)
            {
                if (buttonGroup == null) return;

                if (buttonGroup != null && buttonGroup.CurrentButton == this)
                {
                    return;
                }

                if (buttonGroup.CurrentButton != null)
                    buttonGroup.CurrentButton.OnReset();

                buttonGroup.CurrentButton = this;
                IsShowButton = true;
            }

            base.OnClick(handIndex);
        }

        public override void OnDown(int handIndex)
        {
            if (IsButtonGroup && buttonGroup != null && buttonGroup.CurrentButton == this)
                return;

            base.OnDown(handIndex);
        }

        /// <summary>
        /// 鼠标移入
        /// </summary>
        public override void OnEnter(int handIndex)
        {

            if (IsButtonGroup && buttonGroup != null && buttonGroup.CurrentButton == this)
                return;

            base.OnEnter(handIndex);
        }

        /// <summary>
        /// 鼠标移出
        /// </summary>
        public override void OnExit(int handIndex)
        {
            if (IsButtonGroup && buttonGroup != null && buttonGroup.CurrentButton == this)
                return;

            base.OnExit(handIndex);
        }
        public override void OnEnableUI()
        {
            base.OnEnableUI();
            IsEnable=true;
        }
        public override void OnDisableUI()
        {
            base.OnDisableUI();
            IsEnable=false;
        }
        public override void OnCovered()
        {
            base.OnCovered();
        }
        public override void OnReveal()
        {
            base.OnReveal();
        }
        /// <summary>
        /// 重置
        /// </summary>
        public void OnReset()
        {
            OnHandle("normal");
            IsEnter = false;

            IsShowButton = false;

            if (onGroupReset != null)
                onGroupReset.Invoke(this);

            buttonGroup.CurrentButton = null;
        }
    }
}

