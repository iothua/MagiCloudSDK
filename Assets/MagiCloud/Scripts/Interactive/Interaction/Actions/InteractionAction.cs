using System;
using UnityEngine;
using MagiCloud.Interactive.Distance;

namespace MagiCloud.Interactive.Actions
{
    public enum ActionHandler
    {
        Stay,
        Enter,
        Release
    }

    [Serializable]
    public class InteractionAction : IInteraction_Limit
    {
        public bool IsOpen = false;

        [Header("是在哪种状态执行OnOpen()方法")]
        public ActionHandler actionHandler;

        /// <summary>
        /// 本身
        /// </summary>
        [Header("抓取本身时，是加入到自己身上(True)，还是在对方身上(False)")]
        public bool IsSelf = false;

        /// <summary>
        /// 是否限制
        /// </summary>
        public bool IsLimit {
            get; set;
        }

        /// <summary>
        /// 打开
        /// </summary>
        public virtual void OnOpen(DistanceInteraction InteractionSelf, DistanceInteraction interaction)
        {

        }

        /// <summary>
        /// 关闭
        /// </summary>
        public virtual void OnClose(DistanceInteraction InteractionSelf, DistanceInteraction interaction)
        {

        }
    }
}
