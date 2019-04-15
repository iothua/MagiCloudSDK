using MagiCloud.KGUI;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace MagiCloud.UISystem
{
    /// <summary>
    /// 
    /// </summary>
    public class UIComponent :MonoBehaviour
    {
        public List<UIGroup> groups;
        public List<KGUI_Button> uis;
        private void Start()
        {
            for (int i = 0; i < groups.Count; i++)
            {
                UISystem.Instance.AddGroup(groups[i],i);
            }
            for (int j = 0; j < uis.Count; j++)
            {
                UISystem.Instance.AddUI(uis[j],groups[0].Name,true);
                int temp = j;
                uis[j].onClick.AddListener((x) => Click(temp));
            }
            UISystem.Instance.OnOpen(0);
        }

        public void Click(string str)
        {
            print(str);
        }

        private void Update()
        {

        }
        public void Click(int i)
        {
            UISystem.Instance.OnOpen(i>=uis.Count-1 ? 0 : i+1);
            //  UISystem.Instance.OnClose(i);
        }
    }
}
