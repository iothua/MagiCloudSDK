using System;
using System.Collections.Generic;
using Chemistry.Data;

namespace Chemistry.Chemicals
{
    /// <summary>
    /// 反应过程中的信息
    /// </summary>
    [Serializable]
    public class ReactionInfo
    {
        private DI_ReactionInfo di_reaction;

        public float startTime; //开始反应的时间
        public float stopTime; //暂停反应的时间
        public float lastStartTime; //上一次反应的时间
        public float lastStopTime; //上一次反应的时间

        public float sumProductAmount; //总反应产物的量
        public float sumDeltaProduct; //每帧反应产物的量

        public bool isStart;

        /*药品不允许添加新药品，或者把药品删除，只是记录一次反应的状况*/
        /// <summary>
        /// 反应物
        /// </summary>
        private List<ResponseDrugInfo> _lstReactionDrugInfos = new List<ResponseDrugInfo>();

        /// <summary>
        /// 反应条件
        /// </summary>
        private List<ConditionBase> _lstConditionInfos = new List<ConditionBase>();

        /// <summary>
        /// 反应产物
        /// </summary>
        private List<ProductDrugInfo> _lstProductDrugInfos = new List<ProductDrugInfo>();

        public DI_ReactionInfo DI_Reaction
        {
            get { return di_reaction; }
        }

        /// <summary>
        /// 反应物
        /// </summary>
        public List<ResponseDrugInfo> LstReactionDrugInfos
        {
            get { return _lstReactionDrugInfos; }
        }

        /// <summary>
        /// 反应条件
        /// </summary>
        public List<ConditionBase> LstConditionInfos
        {
            get { return _lstConditionInfos; }
        }

        /// <summary>
        /// 反应产物
        /// </summary>
        public List<ProductDrugInfo> LstProductDrugInfos
        {
            get { return _lstProductDrugInfos; }
        }

        /// <summary>
        /// 实例化数据
        /// </summary>
        /// <param name="reactionInfo">反应编号</param>
        /// <param name="drugSystem">仪器的药品系统，用来筛选数据</param>
        public ReactionInfo(DI_ReactionInfo reactionInfo, DrugSystem drugSystem)
        {
            di_reaction = reactionInfo;

            //反应物赋值
            for (int i = 0; i < reactionInfo.reactants.Count; i++)
            {
                //优化
                DrugData drugObject;

                if (drugSystem.FindDrugForName(reactionInfo.reactants[i].Name, out drugObject))
                {
                    //在仪器里的药品中找到这个药品
                    _lstReactionDrugInfos.Add(new ResponseDrugInfo((Drug)drugObject.DrugObject, reactionInfo.reactants[i].Coefficient));
                }
            }

            //反应条件赋值
            for (int i = 0; i < reactionInfo.conditions.Count; i++)
            {
                _lstConditionInfos.Add(new ConditionBase(reactionInfo.conditions[i]));
            }

            //反应产物赋值
            for (int i = 0; i < reactionInfo.products.Count; i++)
            {
                DrugData drugData;
                drugSystem.AddDrug(reactionInfo.products[i].Name, 0f, out drugData);

                Drug drug = drugData.DrugObject as Drug;

                if (drug == null) return;
                _lstProductDrugInfos.Add(new ProductDrugInfo(reactionInfo.products[i].Coefficient, drug));
            }
        }

        /// <summary>
        /// 重置信息（归零）
        /// </summary>
        public void ResetInfo()
        {
            startTime = 0;
            stopTime = 0;
            lastStartTime = 0;
            lastStopTime = 0;
            sumProductAmount = 0;
            sumDeltaProduct = 0;
        }



        /// <summary>
        /// 结束反应
        /// </summary>
        public void EndProduct()
        {

        }

        /// <summary>
        /// 反应产物是否存在
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool FindProduct(string name)
        {
            for (int i = 0; i < LstProductDrugInfos.Count; i++)
            {
                if (LstProductDrugInfos[i].drugInfo.Name.Equals(name))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 获得产物
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Drug GeiProduct(string name)
        {
            for (int i = 0; i < LstProductDrugInfos.Count; i++)
            {
                if (LstProductDrugInfos[i].drugInfo.Name.Equals(name))
                    return LstProductDrugInfos[i].drugInfo;
            }
            return null;
        }
    }
}
