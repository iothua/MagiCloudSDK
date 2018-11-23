using System;
using System.Collections.Generic;
using UnityEngine;
using MagiCloud.Core.MInput;
using UnityEngine.UI;

namespace MagiCloud.Operate
{
    /// <summary>
    /// 手UI
    /// </summary>
    public class MHandUI : MonoBehaviour, IHandUI
    {
        public Sprite NormalIcon { get; set; }
        public Vector2 Size { get; set; }

        public Image handIcon;

        private bool isEnable;

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
        /// <param name="normalIcon"></param>
        /// <param name="size"></param>
        public void OnInitialized(Sprite normalIcon, Vector2? size)
        {
            this.NormalIcon = normalIcon;

            handIcon = gameObject.GetComponent<Image>() ?? gameObject.AddComponent<Image>();

            this.Size = size != null ? size.Value : handIcon.rectTransform.sizeDelta;

            if (normalIcon == null)
                handIcon.enabled = false;
            else
                handIcon.sprite = normalIcon;

            handIcon.rectTransform.sizeDelta = Size;
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
