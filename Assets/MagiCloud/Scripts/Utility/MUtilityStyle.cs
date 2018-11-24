using UnityEngine;
using System.Collections;

namespace MagiCloud
{
    /// <summary>
    /// 框架相关字体
    /// </summary>
    public static class MUtilityStyle
    {
        private static GUIStyle labelStyle;

        /// <summary>
        /// 加粗加白label字体
        /// </summary>
        /// <value>The label style.</value>
        public static GUIStyle LabelStyle
        {
            get
            {
                if (labelStyle == null)
                {
                    labelStyle = new GUIStyle(GUI.skin.name);
                    labelStyle.normal.textColor = GUI.skin.label.normal.textColor;
                    labelStyle.fontStyle = FontStyle.Bold;
                    labelStyle.alignment = TextAnchor.UpperLeft;
                }
                return LabelStyle;
            }
        }
    }
}

