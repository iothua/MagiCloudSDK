using System.Collections.Generic;
using Chemistry.Chemicals;
using UnityEngine;
using Chemistry.Data;

namespace Chemistry.Help
{
    /// <summary>
    /// 帮助查看数据
    /// </summary>
    public class Help_DisplayReactionInfo : MonoBehaviour
    {
        public ReactionControl reactionControl;

        private DI_ReactionInfo di_reaction;

        public float startTime; //开始反应的时间
        public float stopTime; //暂停反应的时间
        public float lastStartTime; //上一次反应的时间
        public float lastStopTime; //上一次反应的时间
        public float sumProductAmount; //总反应产物的量
        public float sumDeltaProduct; //每帧反应产物的量

        /// <summary>
        /// 反应物
        /// </summary>
        public List<ResponseDrugInfo> _lstReactionDrugInfos;

        /// <summary>
        /// 反应条件
        /// </summary>
        public List<ConditionBase> _lstConditionInfos;

        /// <summary>
        /// 反应产物
        /// </summary>
        public List<ProductDrugInfo> _lstProductDrugInfos;

        void Start()
        {
            IReaction ip = GetComponent<IReaction>();
            if (ip == null) return;


            reactionControl = ip.ReactionControlIns;
        }

        void Update()
        {
            if (reactionControl == null) return;
            if (reactionControl.ReactionInfo == null) return;


            di_reaction = reactionControl.ReactionInfo.DI_Reaction;

            startTime = reactionControl.ReactionInfo.startTime;
            stopTime = reactionControl.ReactionInfo.stopTime;
            lastStartTime = reactionControl.ReactionInfo.lastStartTime;
            lastStopTime = reactionControl.ReactionInfo.lastStopTime;
            sumProductAmount = reactionControl.ReactionInfo.sumProductAmount;
            sumDeltaProduct = reactionControl.ReactionInfo.sumDeltaProduct;

            _lstReactionDrugInfos = reactionControl.ReactionInfo.LstReactionDrugInfos;
            _lstConditionInfos = reactionControl.ReactionInfo.LstConditionInfos;
            _lstProductDrugInfos = reactionControl.ReactionInfo.LstProductDrugInfos;
        }
    }

}
