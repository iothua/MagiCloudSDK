using System.Collections;
using UnityEngine;
using MagiCloud.Core;

namespace MagiCloud
{
    /// <summary>
    /// 框架配置
    /// </summary>
    public sealed class FrameConfig :MonoBehaviour
    {
        //public Camera mainCamera;

        //public HighlightType highlightType;
        [Space(5)]
        public Color highlightColor;
        public Color grabColor = Color.yellow;

        [Space(5)]
        public Color initLabelColor = new Color(0.0588f,0.4241f,0.945f,1f);
        public Font labelFont;
        public int initLabelFontSize = 14;
        public Sprite labelBg = null;

        private static FrameConfig _frameConfig;
        /// <summary>
        /// 框架配置对象
        /// </summary>
        public static FrameConfig Config
        {
            get
            {
                if (_frameConfig == null)
                    _frameConfig = FindObjectOfType<FrameConfig>();

                return _frameConfig;
            }
        }

    }
}

