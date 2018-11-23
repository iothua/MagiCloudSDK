using UnityEngine;
using System.Collections.Generic;
using MagiCloud.Core.MInput;
using UnityEngine.UI;

namespace MagiCloud.Operate
{
    /// <summary>
    /// UI管理
    /// </summary>
    public static class MHandUIManager
    {

        private readonly static List<IHandUI> HandUIs = new List<IHandUI>();

        /// <summary>
        /// 手父物体
        /// </summary>
        public static Transform HandParent {
            get;set;
        }

        /// <summary>
        /// 创建HandUI
        /// </summary>
        /// <param name="sprite">默认值</param>
        /// <param name="size">大小</param>
        /// <returns></returns>
        public static MHandUI CreateHandUI(Transform parent,Sprite sprite, Vector2? size= null)
        {
            if (HandParent == null)
            {
                HandParent = GameObject.Instantiate<Transform>(Resources.Load<Transform>("Hands/HandCanvas"));
                if (parent != null)
                {
                    HandParent.SetParent(parent);
                    HandParent.localPosition = Vector3.zero;
                }
            }

            var handObject = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("Hands/HandIcon"), HandParent);
            var handUI = handObject.AddComponent<MHandUI>();

            handUI.OnInitialized(sprite, size);

            HandUIs.Add(handUI);

            return handUI;
        }

        /// <summary>
        /// 移除手
        /// </summary>
        /// <param name="handUI"></param>
        public static void RemoveUIHand(IHandUI handUI)
        {
            if (!HandUIs.Contains(handUI)) return;

            HandUIs.Remove(handUI);
        }
    }
}
