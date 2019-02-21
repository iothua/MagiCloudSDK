using UnityEngine;

namespace MagiCloud.Common
{
    public class SpriteImageBar :BarBase
    {
        public SpriteRenderer bgSpriteRenderer, frontSpriteRenderer;   //背景图和前景图组件

        public SpriteImageBar(bool isHorizontal,bool isReverse,Transform parent) : base(isHorizontal,isReverse,parent)
        {
            barType=KGUI.ButtonType.SpriteRenderer;
        }

        public override void Init(Sprite bgSprite,Sprite frontSprite,Vector2 bgSize,Vector2 frontSize)
        {
            bgSpriteRenderer=bgRoot.GetComponent<SpriteRenderer>();
            if (bgSpriteRenderer==null)
                bgSpriteRenderer=bgRoot.gameObject.AddComponent<SpriteRenderer>();
            bgSpriteRenderer.sprite=bgSprite;
            bgSpriteRenderer.drawMode=SpriteDrawMode.Sliced;
            bgSpriteRenderer.size=bgSize*0.01f;

            frontSpriteRenderer =frontRoot.GetComponent<SpriteRenderer>();
            if (frontSpriteRenderer==null)
                frontSpriteRenderer=frontRoot.gameObject.AddComponent<SpriteRenderer>();

            frontSpriteRenderer.sprite=frontSprite;
            frontSpriteRenderer.sortingOrder=1;
            frontSpriteRenderer.drawMode=SpriteDrawMode.Sliced;
            frontSpriteRenderer.size=frontSize*0.01f;
        }
        public override float GetValue()
        {
            return base.GetValue();
        }
        public override void SetValue(float value)
        {
            if (isHorizontal)
            {
                float w = bgSpriteRenderer.size.x;
                float fw = frontSpriteRenderer.size.x;
                float start = (fw-w)*0.5f;
                float end = (w-fw)*0.5f;
                // float x = isReverse ? end-value*(w-fw) : start+value*(w-fw);
                float x = isReverse ? (0.5f-value)*(w-fw) : (value-0.5f)*(w-fw);
                Vector3 pos = frontSpriteRenderer.transform.localPosition;
                pos.x=x;
                frontSpriteRenderer.transform.localPosition=pos;
            }
            else
            {
                float w = bgSpriteRenderer.size.y;
                float fw = frontSpriteRenderer.size.y;
                float start = (fw-w)*0.5f;
                float end = (w-fw)*0.5f;
                // float x = isReverse ? end-value*(w-fw) : start+value*(w-fw);
                float y = isReverse ? (0.5f-value)*(w-fw) : (value-0.5f)*(w-fw);
                Vector3 pos = frontSpriteRenderer.transform.localPosition;
                pos.y=y;
                frontSpriteRenderer.transform.localPosition=pos;
            }
            this.value=value;
        }

        public override bool Remove()
        {

            if (bgSpriteRenderer!=null) Object.DestroyImmediate(bgSpriteRenderer);
            if (frontSpriteRenderer!=null) Object.DestroyImmediate(frontSpriteRenderer);
            return base.Remove();
        }
    }
}