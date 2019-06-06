using System;
using UnityEngine;
using Loxodon.Framework.Messaging;
using Loxodon.Framework.Observables;

namespace MagiCloud.KGUI
{
    public static class KGUI_PanelManager
    {
        private readonly static  ObservableList<KGUI_Panel> KguiPanels = new ObservableList< KGUI_Panel>();

        public static void AddPanel(KGUI_Panel panel)
        {
            if (KguiPanels.Contains(panel))
            {
                throw new Exception("在已经存在此Panel:" + panel);
            }

            KguiPanels.Add(panel);
        }

        public static void RemovePanel(KGUI_Panel panel)
        {
            if (KguiPanels.Contains(panel))
                KguiPanels.Remove(panel);
        }

        public static void EnablePanel(Transform parentPanel)
        {
            var panels = parentPanel.GetComponentsInChildren<KGUI_Panel>();
            foreach (KGUI_Panel panel in panels)
            {
                panel.IsEnable = true;
            }
        }

        public static void DisablePanel(Transform parentPanel)
        {
            var panels = parentPanel.GetComponentsInChildren<KGUI_Panel>();
            foreach (KGUI_Panel panel in panels)
            {
                panel.IsEnable = false;
                panel.OnExit();
            }
        }

        public static void EnablePanelAll()
        {
            foreach (var panel in KguiPanels)
            {
                panel.IsEnable = true;
            }
        }

        public static void DisablePanelAll()
        {
            foreach (var panel in KguiPanels)
            {
                panel.IsEnable = false;
                panel.OnExit();
            }
        }
    }
}