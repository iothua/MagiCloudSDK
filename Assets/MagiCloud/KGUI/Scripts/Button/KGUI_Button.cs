using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.UI;

namespace MagiCloud.KGUI
{
    public enum ButtonType
    {
        /// <summary>
        /// 无
        /// </summary>
        None,
        /// <summary>
        /// 精灵
        /// </summary>
        Image,
        /// <summary>
        /// 精灵渲染
        /// </summary>
        SpriteRenderer,
        /// <summary>
        /// 物体
        /// </summary>
        Object
    }

    /// <summary>
    /// KGUI_Button
    /// </summary>
    [RequireComponent(typeof(BoxCollider))]
    public class KGUI_Button : KGUI_ButtonBase
    {

        public bool IsButtonGroup;
        public KGUI_ButtonGroup buttonGroup;

        public bool IsShowButton;

        public AudioClip audioClip;//音频

        public AudioSource audioSource;
        public bool IsStartAudio = true;

        public ButtonGroupReset onGroupReset;

        protected override void OnStart()
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

        /// <summary>
        /// 鼠标移入
        /// </summary>
        public override void OnEnter(int handIndex)
        {

            if (IsButtonGroup && buttonGroup != null && buttonGroup.CurrentButton == this)
                return;

            if (IsStartAudio && audioSource != null)
            {
                audioSource.Play();
            }

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

        public void AddAudio()
        {
            if (audioClip == null)
            {
                audioClip = Resources.Load<AudioClip>("Audios\\手势划过-2");
            }

            if (audioSource == null)
            {
                if (audioSource == null)
                {
                    audioSource = gameObject.GetComponent<AudioSource>();
                }

                if (audioSource == null)
                {
                    audioSource = gameObject.AddComponent<AudioSource>();
                }
                if (audioSource.playOnAwake)
                    audioSource.playOnAwake = false;

                if (audioClip != audioSource.clip)
                    audioSource.clip = audioClip;
            }
        }

        public void DestroyAudio()
        {
            if (audioSource != null)
            {
                DestroyImmediate(audioSource);
            }
        }
    }
}

