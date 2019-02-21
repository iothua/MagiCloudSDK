using UnityEngine;
using UnityEngine.UI;
namespace MagiCloud.Common
{
    /// <summary>
    /// Image进度条条
    /// </summary>
    public class ImageBar :BarBase
    {
        public Image bgImage, frontImage;           //背景图和前景图组件

        public bool isFilled = false;
        public Image.FillMethod fillMethod;
        public ImageBar(bool isHorizontal,bool isReverse,Transform parent,bool isFilled = false,Image.FillMethod fillMethod = Image.FillMethod.Horizontal) : base(isHorizontal,isReverse,parent)
        {
            barType=KGUI.ButtonType.Image;
            this.isFilled=isFilled;
            this.fillMethod=fillMethod;
        }

        public RectTransform BgRect { get { return bgImage.rectTransform; } }
        public RectTransform FrontRect { get { return frontImage.rectTransform; } }



        public override void Init(Sprite bgSprite,Sprite frontSprite,Vector2 bgSize,Vector2 frontSize)
        {
            bgImage=bgRoot.GetComponent<Image>();
            if (bgImage==null)
                bgImage=bgRoot.gameObject.AddComponent<Image>();
            bgImage.sprite=bgSprite;
            bgImage.rectTransform.sizeDelta=bgSize;

            frontImage =frontRoot.GetComponent<Image>();
            if (frontImage==null)
                frontImage=frontRoot.gameObject.AddComponent<Image>();
            frontImage.sprite=frontSprite;
            frontImage.rectTransform.sizeDelta=isFilled ? bgSize : frontSize;
            if (isFilled)
            {
                frontImage.type=Image.Type.Filled;
                frontImage.fillMethod=fillMethod;
            }
        }
        public override float GetValue()
        {
            return base.GetValue();
        }
        public override void SetValue(float value)
        {
            if (isFilled)
            {
                frontImage.fillAmount=isReverse ? (1-value) : value;
            }
            else
            {
                if (isHorizontal)
                {
                    float w = BgRect.sizeDelta.x;
                    float fw = FrontRect.sizeDelta.x;
                    float start = (fw-w)*0.5f;
                    float end = (w-fw)*0.5f;
                    // float x = isReverse ? end-value*(w-fw) : start+value*(w-fw);
                    float x = isReverse ? (0.5f-value)*(w-fw) : (value-0.5f)*(w-fw);
                    Vector3 pos = FrontRect.localPosition;
                    pos.x=x;
                    FrontRect.localPosition=pos;
                }
                else
                {
                    float w = BgRect.sizeDelta.y;
                    float fw = FrontRect.sizeDelta.y;
                    float start = (fw-w)*0.5f;
                    float end = (w-fw)*0.5f;
                    // float x = isReverse ? end-value*(w-fw) : start+value*(w-fw);
                    float y = isReverse ? (0.5f-value)*(w-fw) : (value-0.5f)*(w-fw);
                    Vector3 pos = FrontRect.localPosition;
                    pos.y=y;
                    FrontRect.localPosition=pos;
                }
            }
            this.value=value;
        }
        public override bool Remove()
        {
            if (bgImage!=null) Object.DestroyImmediate(bgImage);
            if (frontImage!=null) Object.DestroyImmediate(frontImage);
            return base.Remove();

        }

    }
}