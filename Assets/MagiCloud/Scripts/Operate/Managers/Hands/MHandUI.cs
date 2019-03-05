using System;
using System.Collections.Generic;
using UnityEngine;
using MagiCloud.Core.MInput;
using UnityEngine.UI;
using MagiCloud.Core;

namespace MagiCloud.Operate
{
    /// <summary>
    /// 手图标
    /// </summary>
    [Serializable]
    public struct HandIcon
    {
        public Sprite IdelIcon;
        public Sprite GripIcon;
       //public Sprite GrabingIcon;
        public Sprite InvalidIcon;
        public Sprite ErrorIcon;
    }

    /// <summary>
    /// 手UI
    /// </summary>
    public class MHandUI : MonoBehaviour, IHandUI
    {
        public Sprite NormalIcon { get; set; }
        public Vector2 Size { get; set; }

        public Image handIcon;

        private bool isEnable;

        public HandIcon handSprite;//手精灵对象

        /// <summary>
        /// 是否激活
        /// </summary>
        public bool IsEnable {
            get {
                return isEnable;
            }
            set {

                isEnable = value;

                gameObject.SetActive(isEnable);
            }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="handSprite">手精灵对象</param>
        /// <param name="size"></param>
        public void OnInitialized(HandIcon handSprite, Vector2? size)
        {
            this.NormalIcon = handSprite.IdelIcon;

            this.handSprite = handSprite;

            handIcon = gameObject.GetComponent<Image>() ?? gameObject.AddComponent<Image>();

            this.Size = size != null ? size.Value : handIcon.rectTransform.sizeDelta;

            if (NormalIcon == null)
                handIcon.enabled = false;
            else
                handIcon.sprite = NormalIcon;

            handIcon.rectTransform.sizeDelta = Size;
            //默认设置为false
            IsEnable = false;
        }

        /// <summary>
        /// 移动UI图标
        /// </summary>
        /// <param name="screenPoint"></param>
        public void MoveHand(Vector3 screenPoint)
        {
            if (handIcon == null) return;

            handIcon.transform.position = screenPoint;
        }

        /// <summary>
        /// 设置手势图标
        /// </summary>
        /// <param name="sprite"></param>
        /// <param name="size"></param>
        public void SetHandIcon(Sprite sprite, Vector2? size = null)
        {
            if (handIcon == null) return;

            if (sprite != null)
            {
                if (!handIcon.enabled) handIcon.enabled = true;
            } 
            else
            {
                if (handIcon.enabled) handIcon.enabled = false;
                return;
            }

            handIcon.sprite = sprite;

            if (size == null) return;
            handIcon.rectTransform.sizeDelta = size.Value;

        }

        public void SetHandIcon(MInputHandStatus status)
        {
            switch(status)
            {
                case MInputHandStatus.Idle:
                    SetHandIcon(handSprite.IdelIcon);
                    break;
                case MInputHandStatus.Grip:
                case MInputHandStatus.Grab:
                case MInputHandStatus.Pressed:
                case MInputHandStatus.Grabing:

                    SetHandIcon(handSprite.GripIcon);
                    break;
                case MInputHandStatus.Invalid:
                    SetHandIcon(handSprite.InvalidIcon);
                    break;
                case MInputHandStatus.Error:
                    SetHandIcon(handSprite.ErrorIcon);
                    break;
            }
        }

        /// <summary>
        /// 设置默认图标
        /// </summary>
        public void SetNormalIcon()
        {
            if (handIcon == null) return;

            if (NormalIcon == null)
                handIcon.enabled = false;

            handIcon.sprite = NormalIcon;
            handIcon.rectTransform.sizeDelta = Size;
        }
    }
}
