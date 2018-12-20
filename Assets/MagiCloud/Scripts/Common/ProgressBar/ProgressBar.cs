using MagiCloud.KGUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace MagiCloud.Common
{
    /// <summary>
    /// 进度条
    /// </summary>
    public class ProgressBar :MonoBehaviour
    {
        private BarBase barBase;

        [Range(0f,1f)]
        public float value;
        public bool isHorizontal = false;           //横向还是纵向
        public bool isReverse = false;              //是否反向
        public Sprite bgSprite, frontSprite;        //背景图和前景图
        public ButtonType type;
        public Vector2 bgSize = new Vector2(400,100);
      
        [Header("--在Image类型下有效--")]
        public Vector2 frontSize = new Vector2(50,100);
        public bool isFilled = false;               //是否使用填充模式，在Image下才能使用
        public Image.FillMethod fillMethod;
        
        public float Value
        {
            get
            {
                return barBase!=null ? barBase.GetValue() : value;
            }
            set
            {
                SetValue(value);
            }
        }

        private float lastValue = -1;
        private void Start()
        {
            Init();
        }

        private void Update()
        {
            Value=value;
        }

        private void SetValue(float value)
        {
            if (lastValue!=value&&barBase!=null)
            {
                barBase.SetValue(value);
                lastValue=value;
            }
            this.value=value;
        }

        #region Init
        public void Init()
        {
            switch (type)
            {
                case ButtonType.None:
                    break;
                case ButtonType.Image:
                    if (barBase!=null)
                    {
                        if (barBase.BarType!=type)
                        {
                            barBase.Remove();
                            barBase=null;
                        }
                    }
                    else
                        barBase = new ImageBar(isHorizontal,isReverse,transform,isFilled,fillMethod);
                    barBase.Init(bgSprite,frontSprite,bgSize,frontSize);
                    break;
                case ButtonType.SpriteRenderer:
                    if (barBase!=null)
                    {
                        if (barBase.BarType!=type)
                        {
                            barBase.Remove();
                            barBase=null;
                        }
                    }
                    else
                        barBase=new SpriteImageBar(isHorizontal,isReverse,transform);
                    barBase.Init(bgSprite,frontSprite,bgSize,frontSize);
                    break;
                case ButtonType.Object:
                    break;
                default:
                    break;
            }
        }


        #endregion

    }


}