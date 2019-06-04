using MagiCloud.Core.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MagiCloud.KGUI
{
    /// <summary>
    /// UIButton组
    /// </summary>
    public class KGUI_ButtonGroup :MonoBehaviour
    {

        public List<KGUI_Button> Interactions;

        /// <summary>
        /// 当前选中Button
        /// </summary>
        public KGUI_Button CurrentButton { get; set; }

        private void Awake()
        {
            OnInitialize();
        }

        void OnInitialize()
        {
            if (Interactions == null)
                Interactions = new List<KGUI_Button>();

            if (Interactions.Count == 0)
            {
                Interactions = FindObjectsOfType<KGUI_Button>().ToList().FindAll(obj => obj.IsButtonGroup
                && obj.buttonGroup != null && obj.buttonGroup.Equals(this));
            }
        }

        public void AddButton(KGUI_Button button)
        {
            OnInitialize();

            if (Interactions.Contains(button)) return;

            if (!button.IsButtonGroup) return;

            if (button.buttonGroup == null)
                button.buttonGroup = this;

            Interactions.Add(button);
        }

        public void RemoveButton(KGUI_Button button)
        {
            if (!Interactions.Contains(button)) return;

            Interactions.Remove(button);
        }

        public void RemoveButtonAll()
        {
            Interactions.Clear();
        }

        public void SetButton(int index = 0)
        {
            if (Interactions == null || index < 0 || Interactions.Count < index) return;

            Interactions[index].OnClick(0);
        }

        public void SetButton(KGUI_Button button)
        {
            if (Interactions == null) return;

            if (Interactions.Contains(button))
                button.OnClick(0);
        }
    }
}
