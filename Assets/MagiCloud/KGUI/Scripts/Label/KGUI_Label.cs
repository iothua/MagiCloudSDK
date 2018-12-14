using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using MagiCloud.Features;
using System;

namespace MagiCloud.KGUI
{
    /// <summary>
    /// KGUI标签
    /// </summary>
    public class KGUI_Label :KGUI_Base
    {
        public int id;

        private bool isShow = true;                         //是否能够显示标签，标签的主要显示控制
        private bool showing = false;                       //是否正在显示
        private bool allowShow = false;                     //用于touch显示/关闭标签，次级显示控制
        private LabelType curType = LabelType.不显示;      //当前类型

        private LabelData _Data;
        public LabelData Data { get { return _Data; } set { _Data=value; } }

        private RectTransform rectTransform;
        public RectTransform Rect
        {
            get
            {
                return rectTransform??(GetComponent<RectTransform>()??gameObject.AddComponent<RectTransform>());
            }
        }
        private Text text;
        public Text Text
        {
            get
            {
                if (text==null)
                {
                    if (GetComponentInChildren<Text>()==null)
                    {
                        text= new GameObject("text",typeof(RectTransform)).AddComponent<Text>();
                        text.transform.SetParent(transform);
                        text.transform.localPosition=Vector3.zero;
                        text.transform.localRotation=Quaternion.identity;
                        text.transform.localScale=Vector3.one;
                    }
                    else
                    {
                        text=GetComponentInChildren<Text>();
                    }
                }
                return text;
                //text??(GetComponent<Text>()??gameObject.AddComponent<Text>());
            }
        }

        private Image image;
        public Image Image
        {
            get
            {
                if (Data==null||Data.labelBackground==null) return null;

                if (image==null)
                    image=transform.GetComponentInChildren<Image>();
                if (image==null)
                {
                    GameObject obj = new GameObject("Bg",typeof(RectTransform),typeof(Image));
                    image = obj.GetComponent<Image>();
                    image.rectTransform.anchorMax=Vector2.one;
                    image.rectTransform.anchorMin=Vector2.zero;
                    obj.transform.SetParent(transform);
                    obj.transform.localPosition=Vector3.zero;
                    obj.transform.localScale=Vector3.one;
                    obj.transform.SetAsFirstSibling();

                }
                return image;
            }
        }

        private void Start()
        {
            InitHide();
        }
        #region 初始化
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="font"></param>
        /// <param name="textColor"></param>
        public void Initialize(Font font,LabelData data)
        {
            Data=data;
            Rect.localScale=Vector3.one;
            InitHide();
            //初始化Text组件
            Text.font=font;
            Text.horizontalOverflow=HorizontalWrapMode.Overflow;
            Text.alignment=TextAnchor.MiddleCenter;
            OnUpdate();
        }
        /// <summary>
        /// 初始化参数并隐藏
        /// </summary>
        public void InitHide()
        {
            isShow=true;
            allowShow=false;
            showing=true;
            OnHide();
        }
        /// <summary>
        /// 设置标签
        /// </summary>
        /// <param name="data"></param>
        public void SetLabel(LabelData data)
        {
            Data=data;
#if UNITY_EDITOR
            if (Application.isPlaying) return;
            //  LateUpdate();
#endif
        }
        #endregion


        public void OnExit()
        {
            allowShow=false;
        }

        public void OnEnter()
        {
            allowShow=true;
        }

        #region 标签数据设置
        /// <summary>
        /// 同步所有标签数据设置
        /// </summary>
        public void OnUpdate()
        {
            SetObjName(Data.labelName);

            // SetObjName(Data.labelName);
            SetFontSize(Data.fontSize);
            SetFontColor(Data.color);
            SetLabelBG(Data.labelBackground);
            SetShowType(Data.type);
            SetStyle(Data.fontStyle);
            SetFontShadow(Data.useShadow);
            SetFontOutLine(Data.useOutline);
        }

        public void SetFontColor(Color color)
        {
            Text.color=color;
        }
        public void SetFontSize(int size)
        {
            Text.fontSize=size;
        }


        public void SetLabelBG(Sprite sprite)
        {
            if (sprite==null)
            {
                if (Image!=null)
                    DestroyImmediate(Image.gameObject);
                return;
            }
            Data.labelBackground=sprite;
            Image.sprite=sprite;
        }


        public void SetFontOutLine(bool useOutline)
        {
            Outline outline = Text.GetComponent<Outline>();
            if (useOutline)
            {
                if (outline==null)
                    Text.gameObject.AddComponent<Outline>();
            }
            else
            {
                if (outline!=null)
                    Destroy(outline);
            }
        }

        public void SetObjName(string name)
        {
            gameObject.name=name;
        }

        public void SetContent(string content)
        {
            Text.text=content;
        }
        public void SetStyle(FontStyle style)
        {
            Text.fontStyle=style;
        }


        public void SetFontShadow(bool useShadow)
        {
            Shadow shadow = Text.GetComponent<Shadow>();
            if (useShadow)
            {
                if (shadow==null)
                    Text.gameObject.AddComponent<Shadow>();
            }
            else
            {
                if (shadow!=null)
                    DestroyImmediate(shadow);
            }
        }

        /// <summary>
        /// 设置显示类型
        /// </summary>
        /// <param name="type"></param>
        public void SetShowType(LabelType type)
        {
            if (curType!=type)
            {
                curType=type;
            }
        }
        #endregion

        public void LateUpdate()
        {
            if (Data!=null)
            {
                SetContent(Data.labelName);
                Vector3 targetPos = Data.appertaining.transform.position;
                //判断是否在相机显示范围内
                Vector3 temp = Camera.main.transform.worldToLocalMatrix.MultiplyPoint(targetPos);
                if (temp.z>0)
                {
                    Vector3 offsetPos = Data.labelOffset;
                    Vector3 worldPos = targetPos+offsetPos;
                    SetPos(worldPos);

                    Rect.sizeDelta=Data.labelSize;
                    float minDis = Data.peakZreaZ.x;
                    float maxDis = Data.peakZreaZ.y;

                    float minScale = Data.clearAreaZ.x;
                    float maxScale = Data.clearAreaZ.y;

                    SetScale(worldPos,minDis,maxDis,minScale,maxScale);
                    //  print("当前状态："+Data.type+"--显示标签："+isShow+"允许显示"+allowShow+"正在显示："+showing);
                    SetShowType(Data.type);
                }
                else
                {
                    isShow=false;
                }
                SetShow();
            }
        }


        /// <summary>
        /// 设置显示
        /// </summary>
        private void SetShow()
        {
            if (isShow)
            {
                if (curType==LabelType.总是显示)
                    OnShow();
                else if (curType==LabelType.选中显示)
                {
                    if (allowShow)
                        OnShow();
                    else
                        OnHide();
                }
                else
                    OnHide();
            }
            else
                OnHide();
        }

        /// <summary>
        /// 设置缩放
        /// </summary>
        /// <param name="worldPos"></param>
        /// <param name="minDis"></param>
        /// <param name="maxDis"></param>
        /// <param name="minScale"></param>
        /// <param name="maxScale"></param>
        private void SetScale(Vector3 worldPos,float minDis,float maxDis,float minScale,float maxScale)
        {
            float cameraDisRange = maxDis -minDis;       //与相机的距离，截取一段范围
            float scaleRange = maxScale-minScale;         //在cameraDisRange范围内，标签的缩放范围
            Vector3 cameraPos = Camera.main.transform.position;
            float dis = Vector3.Distance(worldPos,cameraPos);
            if (!dis.FloatContains(minDis,maxDis))
            {
                isShow=false;
            }
            else
            {
                isShow=true;
                float disVal = (dis-minDis)/cameraDisRange;
                float scaleVal = (disVal*scaleRange)+minScale;
                Rect.localScale=Vector3.one*scaleVal;
            }
        }


        private void OnShow()
        {

            if (showing) return;
            showing=true;
#if UNITY_EDITOR
            Color color = this.Text.color;
            color.a=1;
            this.Text.color=color;
#endif
            Text.DOFade(1,1.0f);
            if (Image!=null)
            {
                Image.DOKill();
                Image.DOFade(1,1.0f);
            }
        }
        private void OnHide()
        {
            if (!showing) return;
            showing=false;
#if UNITY_EDITOR
            Color color = this.Text.color;
            color.a=0;
            this.Text.color=color;
#endif
            Text.DOFade(0,1.0f);
            if (Image!=null)
            {
                Image.DOKill();
                Image.DOFade(0,1.0f);
            }
        }
        /// <summary>
        /// 设置坐标
        /// </summary>
        /// <param name="data"></param>
        private void SetPos(Vector3 worldPos)
        {
            //更新坐标
            Vector3 ScreenPoint = Camera.main.WorldToScreenPoint(worldPos);
            ScreenPoint=ScreenPoint-new Vector3(960,540,ScreenPoint.z);
            transform.localPosition=ScreenPoint;// Vector3.zero;
        }

        public void Destroy()
        {
            KGUI_LabelController.Instance.DestroyByLabel(this);
        }

    }
}

