using System.Collections.Generic;
using Chemistry.Data;
using System.Linq;

namespace Chemistry.Chemicals
{
    /// <summary>
    /// 反应信息检索
    /// </summary>
    public static class ReactionManager
    {
        /// <summary>
        /// 以反应条件为key
        /// </summary>
        private static Dictionary<int, List<DI_ReactionInfo>> _dicReaction;

        /// <summary>
        /// 初始化数据
        /// </summary>
        /// <param name="dic"></param>
        public static void OnInitialize(Dictionary<string, DI_ReactionInfo> dic)
        {
            _dicReaction = new Dictionary<int, List<DI_ReactionInfo>>();
            foreach (var item in dic)
            {
                AddData(item.Value);
            }
        }

        /// <summary>
        /// 添加数据（反应条件数量为key，信息为value）
        /// </summary>
        /// <param name="di"></param>
        private static void AddData(DI_ReactionInfo di)
        {
            List<DI_ReactionInfo> listdata;
            if (_dicReaction.TryGetValue(di.conditions.Count, out listdata))
            {
                if (!listdata.Contains(di))
                    listdata.Add(di);
            }
            else
            {
                _dicReaction.Add(di.conditions.Count,
                    new List<DI_ReactionInfo>() { di });
            }
        }

        /// <summary>
        /// 获取反应id
        /// </summary>
        /// <param name="drugSystem">容器中的药品系统（用于检索）</param>
        /// <param name="conditionInfo">反应条件</param>
        /// <returns></returns>
        public static DI_ReactionInfo GetReaction(DrugSystem drugSystem, List<ConditionBase> lstConditions)
        {
            //在所有类型的库中找到对应的反应
            List<DI_ReactionInfo> data;
            if (_dicReaction.TryGetValue(lstConditions.Count, out data))
            {
                foreach (DI_ReactionInfo item in data)
                {
                    bool isCondition = true;
                    bool isReactant = true;
                    //判断 反应条件是否满足 满足继续，不满足下一个
                    for (int i = 0; i < lstConditions.Count; i++)
                    {
                        if (!item.conditions.Any(obj => obj.Equals(lstConditions[i].Name)))
                        {
                            isCondition = false;
                            break;
                        }
                    }
                    if (lstConditions.Count != 0 && isCondition == false)
                        continue;

                    //判断反应物是否满足 
                    for (int i = 0; i < item.reactants.Count; i++)
                    {
                        if (!drugSystem.IsHaveDrugForName(item.reactants[i].Name))
                        {
                            isReactant = false;
                            break;
                        }
                    }

                    if (isReactant == false)
                        continue;

                    if (isReactant)
                    {
                        UnityEngine.Debug.Log("存在反应：");

                        return item;
                    }
                }
            }

            return null;
        }

    }

}