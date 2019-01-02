using UnityEngine;

namespace MagiCloud.KGUI
{
    public class KGUI_BoxCollider : MBoxCollider
    {
        private KGUI_ButtonBase buttonBase;

        protected override void Awake()
        {
            buttonBase = gameObject.GetComponent<KGUI_ButtonBase>();
            base.Awake();
        }

        protected override void SetOffsetValue()
        {
            base.SetOffsetValue();

            switch (buttonBase.buttonType)
            {
                case ButtonType.None:
                case ButtonType.Object:
                    if (gameObject.GetComponent<RectTransform>() != null)
                    {
                        offsetValue = new Vector3(30, 30, 0);
                    }
                    else
                    {
                        offsetValue = new Vector3(0.5f, 0.5f, 0.5f);
                    }
                    break;
                case ButtonType.Image:
                    offsetValue = new Vector3(30, 30, 0);
                    break;
                case ButtonType.SpriteRenderer:
                    offsetValue = new Vector3(0.5f, 0.5f, 0.5f);
                    break;
            }
        }
    }
}
