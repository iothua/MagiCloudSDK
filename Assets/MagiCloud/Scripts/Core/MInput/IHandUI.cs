using System;
using UnityEngine;

namespace MagiCloud.Core.MInput
{
    public interface IHandUI
    {
        /// <summary>
        /// 激活
        /// </summary>
        bool IsEnable { get; set; }

        /// <summary>
        /// 设置手形状
        /// </summary>
        /// <param name="sprite"></param>
        /// <param name="size"></param>
        void SetHandIcon(Sprite sprite, Vector2? size = null);

        /// <summary>
        /// 设置默认值
        /// </summary>
        void SetNormalIcon();
        /// <summary>
        /// 根据状态，设置手图标
        /// </summary>
        /// <param name="status">Status.</param>
        void SetHandIcon(MInputHandStatus status);
        /// <summary>
        /// 移动手（UI）
        /// </summary>
        /// <param name="screenPoint"></param>
        void MoveHand(Vector3 screenPoint);
    }
}
