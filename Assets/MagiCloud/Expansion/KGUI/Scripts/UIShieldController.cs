using System.Collections.Generic;
using UnityEngine;

namespace MagiCloud.KGUI
{
    /// <summary>
    /// UI屏蔽控制
    /// </summary>
    public static class UIShieldController
    {
        private static List<KGUI_Base> downCache;
        private static List<KGUI_Base> assignCache;
        /// <summary>
        /// 向下屏蔽（场景中的UI的Order大于指定值的全部屏蔽）
        /// </summary>
        /// <param name="order"></param>
        public static void ShieldDownward(int order = 0)
        {
            KGUI_Base[] guis = Object.FindObjectsOfType<KGUI_Base>();
            for (int i = 0; i < guis.Length; i++)
            {
                if (guis[i].Order>order)
                {
                    guis[i].Active=false;
                    if (downCache==null) downCache=new List<KGUI_Base>();
                    downCache.Add(guis[i]);
                }
            }
        }

        /// <summary>
        /// 取消向下屏蔽
        /// </summary>
        /// <param name="order"></param>
        public static void UnShieldDownward()
        {
            if (downCache==null) return;
            for (int i = 0; i < downCache.Count; i++)
            {
                downCache[i].Active=true;
            }
            downCache.Clear();
        }

        /// <summary>
        /// 屏蔽指定UI
        /// </summary>
        /// <param name="guis"></param>
        public static void ShileldAssign(params KGUI_Base[] guis)
        {
            for (int i = 0; i < guis.Length; i++)
            {
                guis[i].Active=false;
                if (assignCache==null)
                    assignCache=new List<KGUI_Base>();
                assignCache.Add(guis[i]);
            }
        }
        /// <summary>
        /// 取消屏蔽指定UI
        /// </summary>
        /// <param name="gui"></param>
        public static void UnShileldAssign(KGUI_Base gui)
        {
            if (!assignCache.Contains(gui)) return;
            gui.Active=true;
            assignCache.Remove(gui);
        }
        /// <summary>
        /// 取消所有指定屏蔽UI
        /// </summary>
        public static void UnAllShileldAssign()
        {
            if (assignCache==null) return;
            for (int i = 0; i < assignCache.Count; i++)
                assignCache[i].Active=true;
            assignCache.Clear();
        }
    }
}
