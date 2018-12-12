using System;
using System.Collections.Generic;
using Chemistry.Chemicals;
using Newtonsoft.Json;
using UnityEngine;
using MagiCloud.Json;
using UnityEngine.Events;
//using Excel;
//using System.Data;
//using System.IO;
//using System.Linq;

namespace Chemistry.Data
{
    /// <summary>
    /// 所有数据的中枢
    /// </summary>
    public static class DataLoading
    {

        public static bool IsInitialized = false;
        
        ///// <summary>
        ///// 仪器信息
        ///// </summary>
        //public static Dictionary<string,DI_EquipmentInfo> DicEquipmentLoadingInfo = new Dictionary<string,DI_EquipmentInfo>();

        ///// <summary>
        ///// 仪器药品信息
        ///// </summary>
        //public static Dictionary<string,DI_EquipmentDrugInfo> DicEquipmentDrugLoadingInfo = new Dictionary<string,DI_EquipmentDrugInfo>();

        /// <summary>
        /// 药品检索信息
        /// </summary>
        public static Dictionary<string,DI_DrugRetrieveInfo> DicDrugRetrieveLoadingInfo = new Dictionary<string,DI_DrugRetrieveInfo>();

        /// <summary>
        /// 药品（纯净物）信息
        /// </summary>
        public static Dictionary<string,DI_DrugPureInfo> DicDrugPureLoadingInfo = new Dictionary<string,DI_DrugPureInfo>();

        /// <summary>
        /// 药品（混合物）信息
        /// </summary>
        public static Dictionary<string,DI_DrugMixtureInfo> DicDrugMixtureLoadingInfo = new Dictionary<string,DI_DrugMixtureInfo>();

        /// <summary>
        /// 反应信息
        /// </summary>
        public static Dictionary<string,DI_ReactionInfo> DicReactionLoadingInfo = new Dictionary<string,DI_ReactionInfo>();

        ///// <summary>
        ///// 容器水体模型信息
        ///// </summary>
        //public static Dictionary<string,DI_ContainerWaterModelInfo> DicContainerWaterModelLoadingInfo=new Dictionary<string, DI_ContainerWaterModelInfo>();

        public static void OnInitialize()
        {
            LoadDataDrugRetrieve();
            LoadDataDrugPure();
            LoadDataDrugMixture();

            LoadChemicalEquation();
            //LoadExcelChemicalEquation();
            //LoadEquipmentInfo();
            //LoadEquipmentDrugInfo();

            //LoadContainerWaterModel();

            ReactionManager.OnInitialize(DicReactionLoadingInfo);//获取到反应信息

            IsInitialized =true;
        }

        //#region 添加

        ///// <summary>
        ///// 添加仪器数据
        ///// </summary>
        ///// <param name="info"></param>
        ///// <param name="isModify"></param>
        //public static void AddEquipmentInfo(DI_EquipmentInfo info, bool isModify = false)
        //{
        //    DicEquipmentLoadingInfo.Add(info.equipmentName, info, () =>
        //      WriteJson(DicEquipmentLoadingInfo.Values, Application.streamingAssetsPath + DefineConst.PATH_JSON_EQUIPMENT), isModify);
        //}

        ///// <summary>
        ///// 添加仪器药品数据
        ///// </summary>
        ///// <param name="info"></param>
        ///// <param name="isModify"></param>
        //public static void AddEquipmentDrugInfo(DI_EquipmentDrugInfo info, bool isModify = false)
        //{
        //    DicEquipmentDrugLoadingInfo.Add(info.equipmentName, info, () =>
        //        WriteJson(DicEquipmentDrugLoadingInfo.Values, Application.streamingAssetsPath + DefineConst.PATH_JSON_EQUIPMENT_DRUG), isModify
        //    );
        //}

        //#endregion

        #region LoadEquipment

        ///// <summary>
        ///// 加载仪器
        ///// </summary>
        ///// <param name="path"></param>
        //private static void LoadEquipmentInfo()
        //{
        //    DicEquipmentLoadingInfo.Clear();
        //    var jsonData = JsonHelper.ReadJsonString(Application.streamingAssetsPath+DefineConst.PATH_JSON_EQUIPMENT);
        //    if (jsonData==null) return;
        //    List<DI_EquipmentInfo> list = JsonHelper.JsonToList<DI_EquipmentInfo>(jsonData);

        //    foreach (var item in list)
        //    {
        //        DicEquipmentLoadingInfo.Add(item.equipmentName,item);
        //    }
        //}

        ///// <summary>
        ///// 加载仪器药品信息
        ///// </summary>
        ///// <param name="path"></param>
        //private static void LoadEquipmentDrugInfo()
        //{
        //    DicEquipmentDrugLoadingInfo.Clear();
        //    var jsonData = JsonHelper.ReadJsonString(Application.streamingAssetsPath+DefineConst.PATH_JSON_EQUIPMENT_DRUG);
        //    if (jsonData==null) return;
        //    List<DI_EquipmentDrugInfo> list = JsonHelper.JsonToList<DI_EquipmentDrugInfo>(jsonData);

        //    foreach (var item in list)
        //    {
        //        DicEquipmentDrugLoadingInfo.Add(item.equipmentName,item);
        //    }
        //}

        /// <summary>
        /// 加载药品类型相关信息
        /// </summary>
        private static void LoadDataDrugRetrieve()
        {
            DicDrugRetrieveLoadingInfo.Clear();

            var jsonData = JsonHelper.ReadJsonString(Application.streamingAssetsPath + DefineConst.PATH_JSON_DRUG_RETRIEVE);

            List<DI_DrugRetrieveInfo> list = JsonHelper.JsonToList<DI_DrugRetrieveInfo>(jsonData);
            foreach (var item in list)
            {
                DicDrugRetrieveLoadingInfo.Add(item.name,item);
            }
        }

        /// <summary>
        /// 加载纯净物相关信息
        /// </summary>
        private static void LoadDataDrugPure()
        {
            DicDrugPureLoadingInfo.Clear();

            var jsonData = JsonHelper.ReadJsonString(Application.streamingAssetsPath + DefineConst.PATH_JSON_DRUG_PURE);

            List<DI_DrugPureInfo> list = JsonHelper.JsonToList<DI_DrugPureInfo>(jsonData);

            foreach (var item in list)
            {
                DicDrugPureLoadingInfo.Add(item.name,item);
            }
        }

        /// <summary>
        /// 加载混合物相关信息
        /// </summary>
        private static void LoadDataDrugMixture()
        {
            DicDrugMixtureLoadingInfo.Clear();

            var jsonData = JsonHelper.ReadJsonString(Application.streamingAssetsPath + DefineConst.PATH_JSON_DRUG_MIXTURE);

            List<DI_DrugMixtureInfo> list = JsonHelper.JsonToList<DI_DrugMixtureInfo>(jsonData);

            foreach (var item in list)
            {
                DicDrugMixtureLoadingInfo.Add(item.name,item);
            }
        }

        /// <summary>
        /// 从Json中加载化学方程式
        /// </summary>
        private static void LoadChemicalEquation()
        {
            DicReactionLoadingInfo.Clear();

            var jsonData = JsonHelper.ReadJsonString(Application.streamingAssetsPath + DefineConst.PATH_JSON_REACTIONEQUATION);

            List<DI_ReactionInfo> reactionInfos = JsonHelper.JsonToList<DI_ReactionInfo>(jsonData);

            foreach (var item in reactionInfos)
            {
                item.id = item.ReactantStr + "+" + item.ConditionStr + "+" + item.ProductStr;

                if (DicReactionLoadingInfo.ContainsKey(item.id))
                {
                    DicReactionLoadingInfo[item.id] = item;//替换旧的
                }
                else
                {
                    DicReactionLoadingInfo.Add(item.id,item);
                }
            }
        }

        ///// <summary>
        ///// 从Json中加载水体模型相关信息
        ///// </summary>
        //private static void LoadContainerWaterModel()
        //{
        //    DicContainerWaterModelLoadingInfo.Clear();

        //    var jsonData = JsonHelper.ReadJsonString(Application.streamingAssetsPath + DefineConst.PATH_JSON_EQUIPMENT_WATERMODEL);

        //    if (jsonData == null) 
        //    {
        //        return;
        //    }

        //    List<DI_ContainerWaterModelInfo> waterModelInfos = JsonHelper.JsonToList<DI_ContainerWaterModelInfo>(jsonData);

        //    if (waterModelInfos==null)
        //    {
        //        //Debug.Log("水体模型信息列表为空");
        //        return;
        //    }

        //    foreach (var info in waterModelInfos)
        //    {
        //        DicContainerWaterModelLoadingInfo.Add(info.equipmentName, info);
        //    }
        //}
        #endregion

        #region 共用方法
        /// <summary>
        /// 写入索引信息
        /// </summary>
        public static void WriteJson(object o, string path)
        {
            //string path = Application.streamingAssetsPath+ "\\Equipments\\ReactionInfo.json";
            string jsonData = JsonHelper.ObjectToJsonString(o);

            JsonHelper.SaveJson(jsonData, path);
        }

        /// <summary>
        /// 字典中添加元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="t"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="OnAddEvent"></param>
        /// <param name="isModify">是否覆盖</param>
        public static void Add<T, TKey, TValue>(this T t, TKey key, TValue value, UnityAction OnAddEvent = null, bool isModify = false) where T : Dictionary<TKey, TValue>
        {
            if (t == null)
                t = new Dictionary<TKey, TValue>() as T;
            if (!t.ContainsKey(key))
            {
                t.Add(key, value);
                OnAddEvent?.Invoke();
            }
            else
            {
                if (isModify)
                {
                    t[key] = value;
                    OnAddEvent?.Invoke();
                }
                else
                    Debug.LogWarningFormat("集合{0}中已经存在{1}", t.ToString(), key.ToString());
            }
        }


        /// <summary>
        /// 字典中移除元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="t"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="OnRemoveEvent"></param>
        public static void Remove<T, TKey, TValue>(this T t, TKey key, TValue value, UnityAction OnRemoveEvent = null) where T : Dictionary<TKey, TValue>
        {
            if (t.ContainsKey(key))
            {
                t.Remove(key);
                OnRemoveEvent?.Invoke();
            }
        }
        #endregion

        /*
         
            

        /// <summary>
        /// 将Excel表文件转化为List类型的
        /// </summary>
        private static void LoadExcelChemicalEquation()
        {

            List<DI_ReactionInfo> reactionInfos = new List<DI_ReactionInfo>();

            string path = Application.streamingAssetsPath + "/化学方程式.xlsx";

            DataSet result = ReadExcel(path);

            int columns = result.Tables[0].Columns.Count;
            int rows = result.Tables[0].Rows.Count;

            for (int i = 1; i < rows; i++)
            {
                DI_ReactionInfo equationInfo = new DI_ReactionInfo();

                for (int j = 0; j < columns; j++)
                {
                    string nvalue = result.Tables[0].Rows[i][j].ToString();

                    switch (j)
                    {
                        case 1:
                            equationInfo.equationName = nvalue;
                            break;
                        case 2:
                            equationInfo.describe = nvalue;
                            break;
                        case 3:
                            //equationInfo.id = nvalue;
                            equationInfo.reactants = AnalysisReactant(nvalue);

                            break;
                        case 4:
                            equationInfo.conditions = nvalue.Split('+').ToList();

                            break;
                        case 5:
                            equationInfo.products = AnalysisReactant(nvalue);

                            break;
                    }
                }

                reactionInfos.Add(equationInfo);

                equationInfo.id = equationInfo.ReactantStr + "+" + equationInfo.ConditionStr + "+" + equationInfo.ProductStr;

                if (DicReactionLoadingInfo.ContainsKey(equationInfo.id))
                {
                    DicReactionLoadingInfo[equationInfo.id] = equationInfo;//替换旧的
                }
                else
                {
                    DicReactionLoadingInfo.Add(equationInfo.id, equationInfo);
                }
            }

            string jsonData = JsonHelper.ObjectToJsonString(reactionInfos);
            JsonHelper.SaveJson(jsonData, Application.streamingAssetsPath + DefineConst.PATH_JSON_REACTIONEQUATION);

        }

        
        /// <summary>
        /// 读取Excel表文件
        /// </summary>
        /// <param name="pathExcel"></param>
        /// <returns></returns>
        public static DataSet ReadExcel(string pathExcel)
        {
            FileStream stream = File.Open(pathExcel, FileMode.Open, FileAccess.Read);
            IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);

            DataSet result = excelReader.AsDataSet();

            return result;
        }

         */

        static List<DI_ReactionInfo.DrugItem> AnalysisReactant(string value)
        {
            string[] values;
            List<DI_ReactionInfo.DrugItem> items = new List<DI_ReactionInfo.DrugItem>();

            if (value.Contains("+"))
            {
                values = value.Split('+');
            }
            else
            {
                values = new string[1];
                values[0] = value;
            }

            foreach (var item in values)
            {
                string[] datas = item.Split(',');

                EDrugType drugType = EDrugType.Empty;

                switch (datas[3])
                {
                    case "固体":
                        drugType = EDrugType.Solid;
                        break;
                    case "液体":
                        drugType = EDrugType.Liquid;
                        break;
                    case "气体":
                        drugType = EDrugType.Gas;
                        break;
                    case "固体粉末":
                        drugType = EDrugType.Solid_Powder;
                        break;
                    case "溶液":
                        drugType = EDrugType.Solution;
                        break;
                }

                DI_ReactionInfo.DrugItem drugItem = new DI_ReactionInfo.DrugItem(datas[0],datas[1],float.Parse(datas[2]),drugType);
                items.Add(drugItem);
            }

            return items;
        }


        /// <summary>
        /// 加载信息
        /// </summary>
        /// <typeparam name="T">自定义信息类型</typeparam>
        /// <param name="path">路径</param>
        /// <returns></returns>
        private static List<T> LoadData<T>(string path) where T : DataItemBase
        {
            TextAsset ta = Resources.Load<TextAsset>(path);
            if (ta == null)
            {
                Debug.LogError("请检查路径“Resources\\" + path + "”是否存在...");
                return new List<T>();
            }
            return JsonConvert.DeserializeObject<List<T>>(ta.text);
        }


    }


}