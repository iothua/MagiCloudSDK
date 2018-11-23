using System;
using UnityEngine;
using UnityEngine.UI;
using MagiCloud.Core.UI;
using MagiCloud.Core;

namespace MagiCloud.KGUI
{

    /// <summary>
    /// KGUI基类
    /// </summary>
    public class KGUI_ButtonBase : KGUI_Base,IButton
    {
        public ButtonType buttonType;

        public SpriteRenderer spriteRenderer;
        public Image image;

        public Sprite normalSprite, enterSprite, pressedSprite, disableSprite;
        public GameObject normalObject, enterObject, pressedObject, disableObject;

        protected bool IsEnter;

        private bool _IsEnable = true;
        protected BoxCollider boxCollider;

        protected MBehaviour behaviour;

        /// <summary>
        /// 是否激活
        /// </summary>
        public virtual bool IsEnable {
            get {
                return _IsEnable;
            }
            set {

                ////如果相等，则不进行任何处理
                //if (_IsEnable == value) return;

                _IsEnable = value;

                if (Collider != null)
                    Collider.enabled = value;

                IsEnter = false;

                if (!_IsEnable)
                {
                    OnHandle("disable");
                }
                else
                {
                    OnHandle("normal");
                }
            }
        }

        public BoxCollider Collider {
            get {

                try
                {
                    if (boxCollider == null)
                        boxCollider = GetComponent<BoxCollider>();

                    return boxCollider;
                }
                catch
                {
                    return null;
                }
            }
        }

        #region 事件

        public ButtonEvent onClick;  //鼠标点击

        public ButtonEvent onEnter;  //鼠标移入
        public ButtonEvent onExit;   //鼠标移出
        public ButtonEvent onDown;   //鼠标按下
        public ButtonEvent onUp;     //鼠标抬起

        public ButtonEvent onDownStay; //按下持续

        public PanelEvent onUpRange;

        #endregion

        protected virtual void Awake()
        {
            if (onClick == null)
                onClick = new ButtonEvent();

            if (onEnter == null)
                onEnter = new ButtonEvent();

            if (onExit == null)
                onExit = new ButtonEvent();

            if (onDown == null)
                onDown = new ButtonEvent();

            if (onUp == null)
                onUp = new ButtonEvent();

            if (onDownStay == null)
                onDownStay = new ButtonEvent();

            if (onUpRange == null)
                onUpRange = new PanelEvent();

            gameObject.tag = "button";

            behaviour = new MBehaviour(ExecutionPriority.High, -799, enabled);
            behaviour.OnStart(OnStart);

            MBehaviourController.AddBehaviour(behaviour);
        }

        protected virtual void OnStart()
        {
            Debug.Log("执行OnStart");
        }

        /// <summary>
        /// 按钮按下
        /// </summary>
        public virtual void OnClick(int handIndex)
        {
            OnHandle("click");

            if (onClick != null)
                onClick.Invoke(handIndex);
        }

        /// <summary>
        /// 鼠标移入
        /// </summary>
        public virtual void OnEnter(int handIndex)
        {

            if (IsEnter) return;

            OnHandle("enter");

            IsEnter = true;

            if (onEnter != null)
                onEnter.Invoke(handIndex);
        }

        /// <summary>
        /// 鼠标移出
        /// </summary>
        public virtual void OnExit(int handIndex)
        {
            if (!IsEnter) return;

            OnHandle("normal");

            if (onExit != null)
                onExit.Invoke(handIndex);

            IsEnter = false;
        }


        /// <summary>
        /// 鼠标抬起
        /// </summary>
        public override void OnDown(int handIndex)
        {
            base.OnDown(handIndex);

            if (onDown != null)
                onDown.Invoke(handIndex);
        }

        /// <summary>
        /// 鼠标释放
        /// </summary>
        public override void OnUp(int handIndex)
        {
            base.OnUp(handIndex);

            if (onUp != null)
                onUp.Invoke(handIndex);

        }

        /// <summary>
        /// 鼠标释放时，是否在范围内
        /// </summary>
        /// <param name="handIndex"></param>
        /// <param name="isRange"></param>
        public virtual void OnUpRange(int handIndex, bool isRange)
        {
            if (onUpRange != null)
                onUpRange.Invoke(handIndex, isRange);
        }

        /// <summary>
        /// 持续执行按下
        /// </summary>
        /// <param name="handIndex"></param>
        public virtual void OnDownStay(int handIndex)
        {
            if (onDownStay != null)
                onDownStay.Invoke(handIndex);
        }

        protected virtual void OnHandle(string cmd)
        {

            //Debug.Log("执行：" + cmd);

            switch (buttonType)
            {
                case ButtonType.Image:
                    if (image == null)
                        return;

                    if (cmd.Equals("click"))
                    {
                        if (pressedSprite != null)
                            image.sprite = pressedSprite;
                    }
                    else if (cmd.Equals("enter"))
                    {
                        if (enterSprite != null)
                            image.sprite = enterSprite;
                    }
                    else if (cmd.Equals("normal"))
                    {
                        if (normalSprite != null)
                            image.sprite = normalSprite;
                    }
                    else if (cmd.Equals("disable"))
                    {
                        if (disableSprite != null)
                            image.sprite = disableSprite;
                    }
                    else
                    {
                        break;
                    }

                    break;
                case ButtonType.Object:

                    if (cmd.Equals("click"))
                    {
                        if (disableObject != null)
                            disableObject.SetActive(false);

                        if (normalObject != null)
                            normalObject.SetActive(false);

                        if (enterObject != null)
                            enterObject.SetActive(false);

                        if (pressedObject != null)
                            pressedObject.SetActive(true);
                    }
                    else if (cmd.Equals("enter"))
                    {
                        if (disableObject != null)
                            disableObject.SetActive(false);

                        if (normalObject != null)
                            normalObject.SetActive(false);

                        if (pressedObject != null)
                            pressedObject.SetActive(false);

                        if (enterObject != null)
                            enterObject.SetActive(true);

                    }
                    else if (cmd.Equals("normal"))
                    {
                        if (disableObject != null)
                            disableObject.SetActive(false);

                        if (enterObject != null)
                            enterObject.SetActive(false);

                        if (pressedObject != null)
                            pressedObject.SetActive(false);

                        if (normalObject != null)
                            normalObject.SetActive(true);
                    }
                    else if (cmd.Equals("disable"))
                    {
                        if (enterObject != null)
                            enterObject.SetActive(false);
                        if (pressedObject != null)
                            pressedObject.SetActive(false);
                        if (normalObject != null)
                            normalObject.SetActive(false);

                        if (disableObject != null)
                            disableObject.SetActive(true);
                    }
                    else
                    {
                        break;
                    }

                    break;
                case ButtonType.SpriteRenderer:

                    if (spriteRenderer == null) return;

                    if (cmd.Equals("click"))
                    {
                        spriteRenderer.sprite = pressedSprite;
                    }
                    else if (cmd.Equals("enter"))
                    {
                        spriteRenderer.sprite = enterSprite;
                    }
                    else if (cmd.Equals("normal"))
                    {
                        spriteRenderer.sprite = normalSprite;
                    }
                    else if (cmd.Equals("disable"))
                    {
                        spriteRenderer.sprite = disableSprite;
                    }
                    else
                    {
                        break;
                    }

                    break;
                default:
                    break;
            }
        }

        protected virtual void OnEnable()
        {
            
        }

        protected virtual void OnDisable()
        {
            
        }

        protected virtual void OnDestroy()
        {
            if (behaviour != null)
                behaviour.OnExcuteDestroy();
        }
    }
}
