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

        public string Name;//信息

        public KGUI_Dropdown dropdown;

        public Text text;

        /// <summary>
        /// 初始化
        /// </summary>
        public void OnInitialized(KGUI_Dropdown dropdown,string name)
        {
            buttonGroup = dropdown.buttonGroup;
            IsButtonGroup = true;

            if (text == null)
                text = gameObject.GetComponentInChildren<Text>();

            text.text = name;

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

            base.OnClick(handIndex);

            dropdown.OnSetDropdownText(this);
        }
    }
}

