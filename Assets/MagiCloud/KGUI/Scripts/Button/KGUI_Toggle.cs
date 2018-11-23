using System;
using System.Collections.Generic;
using UnityEngine;

namespace MagiCloud.KGUI
{
    /// <summary>
    /// KGUI开关
    /// </summary>
    [ExecuteInEditMode]
    public class KGUI_Toggle : KGUI_ButtonBase
    {
        [SerializeField]
        private bool isValue;

        public bool IsValue {
            get {
                return isValue;
            }
            set {

                //OnHandle("click");

                if (isValue == value)
                    return;

                isValue = value;

                OnHandle("click");

                if (OnValueChanged != null)
                {
                    OnValueChanged.Invoke(isValue);
                }
            }
        }

        //移入时精灵
        public Sprite onEnterSprite, offEnterSprite;
        //移出时精灵
        public Sprite onNormalSprite, offNormalSprite;

        //默认物体
        public GameObject onNormalObject, offNormalObject;
        //移入时物体
        public GameObject onEnterObject, offEnterObject;

        public ToggleEvent OnValueChanged;

        public AudioClip audioClip;//音频

        public AudioSource audioSource;
        public bool IsStartAudio = true;

        protected override void OnStart()
        {
            base.OnStart();

            OnHandle("click");
        }

        public override void OnClick(int handIndex)
        {
            if (onClick != null)
                onClick.Invoke(handIndex);

            IsValue = !IsValue;
        }

        public override void OnEnter(int handIndex)
        {

            if (IsStartAudio && audioSource != null)
            {
                audioSource.Play();
            }

            base.OnEnter(handIndex);

        }

        protected override void OnHandle(string cmd)
        {
            switch (buttonType)
            {
                case ButtonType.Image:
                    if (image == null)
                        return;

                    if (cmd.Equals("click")||cmd.Equals("normal"))
                    {
                        image.sprite = IsValue ? onNormalSprite : offNormalSprite;
                    }
                    else if (cmd.Equals("enter"))
                    {
                        image.sprite = IsValue ? onEnterSprite : offEnterSprite;
                    }
                    else
                    {
                        //什么都不处理
                    }

                    break;
                case ButtonType.Object:

                    if (cmd.Equals("click") || cmd.Equals("normal"))
                    {
                        onEnterObject.SetActive(false);
                        offEnterObject.SetActive(false);

                        onNormalObject.SetActive(IsValue);
                        offNormalObject.SetActive(!IsValue);
                    }
                    else if (cmd.Equals("enter"))
                    {
                        onNormalObject.SetActive(false);
                        offNormalObject.SetActive(false);

                        onEnterObject.SetActive(isValue);
                        offEnterObject.SetActive(!isValue);
                    }
                    else
                    {

                    }
                    break;
                case ButtonType.SpriteRenderer:

                    if (spriteRenderer == null) return;

                    if (cmd.Equals("click") || cmd.Equals("normal"))
                    {
                        spriteRenderer.sprite = IsValue ? onNormalSprite : offNormalSprite;
                    }
                    else if (cmd.Equals("enter"))
                    {
                        spriteRenderer.sprite = IsValue ? onEnterSprite : offEnterSprite;
                    }
                    else
                    {
                    }

                    break;
                default:
                    break;
            }
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
