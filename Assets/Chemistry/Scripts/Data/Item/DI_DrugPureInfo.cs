namespace Chemistry.Data
{
    /// <summary>
    /// 药品（纯净物）
    /// </summary>
    public class DI_DrugPureInfo : DataItemBase
    {
        /// <summary>
        /// 密度
        /// </summary>
        public float density;

        /// <summary>
        /// 药品类型
        /// </summary>
        public EDrugType drugType;

        /// <summary>
        /// 单位
        /// </summary>
        public EMeasureUnit unit;

        /// <summary>
        /// 溶解度
        /// </summary>
        public float solubility;
    }

}