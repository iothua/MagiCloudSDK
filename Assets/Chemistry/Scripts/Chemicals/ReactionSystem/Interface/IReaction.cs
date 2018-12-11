

namespace Chemistry.Chemicals
{
    /// <summary>
    /// 反应系统
    /// </summary>
    public interface IReaction
    {
        /// <summary>
        /// 药品系统实例
        /// </summary>
        DrugSystem DrugSystemIns { get; }

        /// <summary>
        /// 反应系统实例
        /// </summary>
        ReactionControl ReactionControlIns { get; }
    }

}