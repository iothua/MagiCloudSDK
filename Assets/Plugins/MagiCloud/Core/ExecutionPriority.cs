namespace MagiCloud.Core
{
    /// <summary>
    /// 执行优先级
    /// </summary>
    public enum ExecutionPriority
    {
        /// <summary>
        /// 应用于框架最高初始化
        /// </summary>
        Highest = 0,
        /// <summary>
        /// 应该与框架的数据与UI初始化
        /// </summary>
        High,
        /// <summary>
        /// 应用与其他使用框架的数据初始化
        /// </summary>
        Mid,
        /// <summary>
        /// 应用与其他
        /// </summary>
        Low,
        /// <summary>
        /// 最低级别
        /// </summary>
        Lowest
    }
}
