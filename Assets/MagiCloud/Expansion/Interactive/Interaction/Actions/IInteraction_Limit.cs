namespace MagiCloud.Interactive.Actions
{
    /// <summary>
    /// 距离限制
    /// </summary>
    public interface IInteraction_Limit
    {
        /// <summary>
        /// 限制（为Ture时，所有的动作都将取消掉）
        /// </summary>
        bool IsLimit { get; set; }
    }
}
