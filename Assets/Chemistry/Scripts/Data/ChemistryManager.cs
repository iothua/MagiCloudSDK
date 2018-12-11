namespace Chemistry.Data
{
    /// <summary>
    /// 操作界面的总体控制
    /// </summary>
    public static class ChemistryManager
    {
        /// <summary>
        /// 是否显示辅助操作
        /// </summary>
        private static bool isShowGizmos;

        public static bool IsShowGizmos
        {
            get { return isShowGizmos; }
        }

        public static void SetGizmos(bool bl)
        {
            if (bl != IsShowGizmos)
                isShowGizmos = bl;
        }
    }

}