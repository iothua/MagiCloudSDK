namespace Chemistry.Data
{
    /// <summary>
    /// 药品（混合物）
    /// </summary>
    public class DI_DrugMixtureInfo : DataItemBase
    {
        /// <summary>
        /// 含量（溶质质量/溶液质量）
        /// </summary>
        public float percent;

        /// <summary>
        /// 溶质名字
        /// </summary>
        public string soluteName;

        /// <summary>
        /// 溶剂名字
        /// </summary>
        public string solventName;

    }

}