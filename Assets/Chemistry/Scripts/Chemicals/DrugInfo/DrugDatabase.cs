using Chemistry.Data;
using System;

namespace Chemistry.Chemicals
{
    /// <summary>
    /// 药品数据库
    /// </summary>
    public class DrugDatabase
    {
        public static DrugData AddDrug(string name, float volume, EMeasureUnit unit = EMeasureUnit.ml)
        {
            //直接从表中获取数据
            DI_DrugRetrieveInfo retrieveInfo;
            if (DataLoading.DicDrugRetrieveLoadingInfo.TryGetValue(name, out retrieveInfo))
            {
                if (retrieveInfo.drugType == 1)
                {
                    //添加纯净物
                    return new DrugData(AddDrugPure(name, volume, unit));
                }
                else if (retrieveInfo.drugType == 2)
                {
                    //添加混合物
                    return new DrugData(AddDrugMixture(name, volume));
                }
                else
                {
                    return new DrugData() ;
                }
            }
            else
            {
                return new DrugData();
            }
        }

        /// <summary>
        /// 添加纯净物
        /// </summary>
        /// <param name="name"></param>
        /// <param name="volume"></param>
        private static Drug AddDrugPure(string name, float volume, EMeasureUnit unit = EMeasureUnit.ml)
        {
            return new Drug(name, volume, unit); 
        }

        /// <summary>
        /// 添加混合物
        /// </summary>
        /// <param name="name"></param>
        /// <param name="vol"></param>
        private static DrugMixture AddDrugMixture(string name, float vol)
        {
            return new DrugMixture(name, vol);
        }
    }
}
