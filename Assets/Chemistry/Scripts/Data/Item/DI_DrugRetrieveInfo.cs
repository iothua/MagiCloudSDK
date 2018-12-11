namespace Chemistry.Data
{
    /// <summary>
    /// 药品检索信息
    /// </summary>
    public class DI_DrugRetrieveInfo : DataItemBase
    {
        /// <summary>
        /// 药品数据 1:纯净物 2:溶液(有一个溶质的溶液)
        /// </summary>
        public int drugType = 1;
    }

}