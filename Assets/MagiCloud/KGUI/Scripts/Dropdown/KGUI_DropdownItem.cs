using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace MagiCloud.KGUI
{
    /// <summary>
    /// 下拉框子项
    /// </summary>
    public class KGUI_DropdownItem : KGUI_Button
    {

        public new string Name;//信息

        public KGUI_Dropdown dropdown;

        /// <summary>
        /// 初始化
        /// </summary>
        public void OnInitialized(KGUI_Dropdown dropdown,string name)
        {
            buttonGroup = dropdown.buttonGroup;
            IsButtonGroup = true;

            dropdown.buttonGroup.AddButton(this);

            switch(buttonType)
            {
                case ButtonType.Image:
                case ButtonType.None:
                case ButtonType.SpriteRenderer:
                    var text = gameObject.GetComponentInChildren<KGUI_Text>();
                    if (text != null)
                        text.Text = name;
                    break;
                case ButtonType.Object:

                    var normalText = normalObject.GetComponentInChildren<KGUI_Text>();
                    var enterText = enterObject.GetComponentInChildren<KGUI_Text>();
                    var pressedText = pressedObject.GetComponentInChildren<KGUI_Text>();
                    var disableText = disableObject.GetComponentInChildren<KGUI_Text>();

                    if (normalText != null)
                        normalText.Text = name;

                    if (enterText != null)
                        enterText.Text = name;

                    if (pressedText != null)
                        pressedText.Text = name;

                    if (disableText != null)
                        disableText.Text = name;

                    break;
            }

            Name = name;

            this.dropdown = dropdown;
        }

        /// <summary>
        /// 设置子项的大小以及碰撞体的大小
        /// </summary>
        /// <param name="size"></param>
        public void SetItemData(Vector2 size)
        {
            //设置碰撞体大小
            var box = gameObject.GetComponent<BoxCollider>();
            box.size = new Vector3(size.x, size.y, box.size.z);
        }


        public override void OnClick(int handIndex)
        {
            if(IsButtonGroup)
            {
                if (buttonGroup == null || (buttonGroup != null && buttonGroup.CurrentButton == this))
                {
                    return;
                }
            }

            base.OnClick(handIndex);
            dropdown.OnSetDropdownText(this);
        }
    }
}

