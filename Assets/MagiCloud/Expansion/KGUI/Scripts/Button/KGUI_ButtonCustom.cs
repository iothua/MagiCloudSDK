using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MagiCloud.KGUI
{
    /// <summary>
    /// KGUI按钮自定义式，移入时自动触发，但是离开需要手动触发
    /// </summary>
    public class KGUI_ButtonCustom : KGUI_ButtonBase {

        public override void OnEnter(int handIndex)
        {
            base.OnEnter(handIndex);
        }

        public override void OnExit(int handIndex)
        {

        }

        public override void OnClick(int handIndex)
        {
            if (onClick != null)
                onClick.Invoke(handIndex);
        }

        /// <summary>
        /// 获取当前状态
        /// </summary>
        /// <returns></returns>
        public bool GetEnter()
        {
            return IsEnter;
        }

        /// <summary>
        /// 自定义离开触发
        /// </summary>
        /// <param name="handIndex"></param>
        public void OnCustomExit(int handIndex)
        {
            if (!IsEnter) return;

            OnHandle("normal");

            if (onExit != null)
                onExit.Invoke(handIndex);

            IsEnter = false;
        }

        /// <summary>
        /// 设置状态
        /// </summary>
        /// <param name="isEnter"></param>
        public void SetEnter(bool isEnter)
        {
            IsEnter = isEnter;
        }
    }
}

