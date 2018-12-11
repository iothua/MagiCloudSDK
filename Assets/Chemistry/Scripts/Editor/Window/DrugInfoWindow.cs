using UnityEngine;
using UnityEditor;
using System.Linq;
using MagiCloud.Json;
using Chemistry.Data;

namespace Chemistry.Editor.Window
{
    /// <summary>
    /// 药品信息
    /// </summary>
    public class DrugInfoWindow
    {
        private DI_DrugRetrieveInfo retrieveInfo;
        private bool retrieveAdd = false;
        private string retrievePath = string.Empty;

        private DI_DrugPureInfo pureInfo;
        private bool pureAdd = false;
        private string purePath = string.Empty;

        private DI_DrugMixtureInfo mixtureInfo;
        private bool mixtureAdd = false;
        private string mixturePath = string.Empty;

        private ChemicalEditorWindows chemicalEditor;

        public string WindowName;


        public DrugInfoWindow(ChemicalEditorWindows chemicalEditor, string windowName)
        {

            this.chemicalEditor = chemicalEditor;
            this.WindowName = windowName;

            if (string.IsNullOrEmpty(retrievePath))
            {
                retrievePath = Application.streamingAssetsPath + DefineConst.PATH_JSON_DRUG_RETRIEVE;
            }

            if (string.IsNullOrEmpty(purePath))
            {
                purePath = Application.streamingAssetsPath + DefineConst.PATH_JSON_DRUG_PURE;
            }

            if (string.IsNullOrEmpty(mixturePath))
            {
                mixturePath = Application.streamingAssetsPath + DefineConst.PATH_JSON_DRUG_MIXTURE;
            }

        }

        public void OnGUI()
        {
            GUILayout.BeginHorizontal();

            //药品索引信息
            CreateRetrieveInfo();
            //纯净物信息
            CreatePureInfo();
            //混合物信息
            CreateMixtureInfo();

            GUILayout.EndHorizontal();
        }


        /// <summary>
        /// 药品索引信息
        /// </summary>
        void CreateRetrieveInfo()
        {
            //管理信息
            if (retrieveInfo == null)
            {
                retrieveInfo = new DI_DrugRetrieveInfo();
                retrieveAdd = true;
            }

            GUILayout.BeginVertical("box", GUILayout.Width(400));

            GUILayout.Label("药品索引信息：", chemicalEditor.titleStyle);
            GUILayout.Space(5);

            //添加和编辑窗口
            retrieveInfo.name = EditorGUILayout.TextField("药品名称：", retrieveInfo.name);

            GUILayout.BeginHorizontal();

            retrieveInfo.drugType = EditorGUILayout.IntField("药品类型：", retrieveInfo.drugType);

            GUILayout.Space(5);
            GUILayout.Label("1:纯净物 2:溶液(有一个溶质的溶液)");

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            if (retrieveAdd)
            {
                if (GUILayout.Button("添加", GUILayout.Width(70)))
                {
                    if (!DataLoading.DicDrugRetrieveLoadingInfo.ContainsKey(retrieveInfo.name))
                    {
                        DataLoading.DicDrugRetrieveLoadingInfo.Add(retrieveInfo.name, retrieveInfo);

                        WriteJson(DataLoading.DicDrugRetrieveLoadingInfo.Values, retrievePath);
                    }

                    retrieveInfo = null;
                }
            }
            else
            {
                if (GUILayout.Button("编辑", GUILayout.Width(70)))
                {
                    WriteJson(DataLoading.DicDrugRetrieveLoadingInfo.Values, retrievePath);
                    retrieveInfo = null;
                }
            }

            GUILayout.EndHorizontal();

            if (DataLoading.DicDrugRetrieveLoadingInfo.Count > 0)
            {
                LoadDrugRetrieveLoadingInfo();
            }

            GUILayout.EndVertical();
        }

        void LoadDrugRetrieveLoadingInfo()
        {
            //列出所有的信息
            GUILayout.BeginHorizontal();
            GUILayout.Box("选中", GUILayout.Width(50));
            GUILayout.Box("名称", GUILayout.Width(100));
            GUILayout.Box("类型", GUILayout.Width(100));
            GUILayout.Box("删除", GUILayout.Width(50));
            GUILayout.EndHorizontal();

            foreach (var item in DataLoading.DicDrugRetrieveLoadingInfo.ToList())
            {
                GUILayout.BeginHorizontal();

                if (GUILayout.Button("<-", GUILayout.Width(50)))
                {
                    retrieveInfo = item.Value;
                    retrieveAdd = false;
                }

                GUILayout.Box(item.Value.name, GUILayout.Width(100));
                GUILayout.Box(item.Value.drugType == 1 ? "纯净物" : "溶剂", GUILayout.Width(100));

                if (GUILayout.Button("X", GUILayout.Width(50)))
                {
                    DataLoading.DicDrugRetrieveLoadingInfo.Remove(item.Key);
                    WriteJson(DataLoading.DicDrugRetrieveLoadingInfo.Values, retrievePath);
                }

                GUILayout.EndHorizontal();
            }
        }

        /// <summary>
        /// 写入索引信息
        /// </summary>
        void WriteJson(object o, string path)
        {
            //string path = Application.streamingAssetsPath+ "\\Equipments\\ReactionInfo.json";
            string jsonData = JsonHelper.ObjectToJsonString(o);

            JsonHelper.SaveJson(jsonData, path);
        }

        void LoadDrugPureInfo()
        {

            //列出所有的信息
            GUILayout.BeginHorizontal();
            GUILayout.Box("选中", GUILayout.Width(50));
            GUILayout.Box("名称", GUILayout.Width(50));
            GUILayout.Box("类型", GUILayout.Width(100));

            GUILayout.Box("单位", GUILayout.Width(50));
            GUILayout.Box("密度", GUILayout.Width(50));
            GUILayout.Box("溶解度", GUILayout.Width(50));
            GUILayout.Box("删除", GUILayout.Width(50));
            GUILayout.EndHorizontal();

            foreach (var item in DataLoading.DicDrugPureLoadingInfo.ToList())
            {
                GUILayout.BeginHorizontal();

                if (GUILayout.Button("<-", GUILayout.Width(50)))
                {
                    pureInfo = item.Value;
                    pureAdd = false;
                }

                GUILayout.Box(item.Value.name, GUILayout.Width(50));
                GUILayout.Box(item.Value.drugType.ToString(), GUILayout.Width(100));
                GUILayout.Box(item.Value.unit.ToString(), GUILayout.Width(50));
                GUILayout.Box(item.Value.density.ToString(), GUILayout.Width(50));
                GUILayout.Box(item.Value.solubility.ToString(), GUILayout.Width(50));

                if (GUILayout.Button("X", GUILayout.Width(50)))
                {
                    DataLoading.DicDrugPureLoadingInfo.Remove(item.Key);

                    WriteJson(DataLoading.DicDrugPureLoadingInfo.Values, purePath);
                }

                GUILayout.EndHorizontal();
            }

        }

        /// <summary>
        /// 纯净药品信息
        /// </summary>
        void CreatePureInfo()
        {
            if (pureInfo == null)
            {
                pureInfo = new DI_DrugPureInfo();
                pureAdd = true;
            }

            GUILayout.BeginVertical("box", GUILayout.Width(450));
            GUILayout.Label("药品纯净物信息：", chemicalEditor.titleStyle);
            GUILayout.Space(5);

            //添加和编辑窗口
            pureInfo.name = EditorGUILayout.TextField("药品名称：", pureInfo.name);
            pureInfo.density = EditorGUILayout.FloatField("药品密度：", pureInfo.density);

            pureInfo.drugType = (EDrugType)EditorGUILayout.EnumPopup("药品类型：", pureInfo.drugType);
            pureInfo.unit = (EMeasureUnit)EditorGUILayout.EnumPopup("药品单位：", pureInfo.unit);

            pureInfo.solubility = EditorGUILayout.Slider("溶解度：", pureInfo.solubility, 0, 1);

            if (pureAdd)
            {
                if (GUILayout.Button("添加", GUILayout.Width(70)))
                {
                    if (!DataLoading.DicDrugPureLoadingInfo.ContainsKey(pureInfo.name))
                    {
                        DataLoading.DicDrugPureLoadingInfo.Add(pureInfo.name, pureInfo);

                        WriteJson(DataLoading.DicDrugPureLoadingInfo.Values, purePath);
                    }

                    pureInfo = null;
                }
            }
            else
            {
                if (GUILayout.Button("编辑", GUILayout.Width(70)))
                {
                    WriteJson(DataLoading.DicDrugPureLoadingInfo.Values, purePath);
                    pureInfo = null;
                }
            }

            if (DataLoading.DicDrugRetrieveLoadingInfo.Count > 0)
            {
                LoadDrugPureInfo();
            }

            GUILayout.EndVertical();
        }



        void LoadMixtureInfo()
        {
            //列出所有的信息
            GUILayout.BeginHorizontal();

            GUILayout.Box("选中", GUILayout.Width(50));

            GUILayout.Box("名称", GUILayout.Width(100));
            GUILayout.Box("含量", GUILayout.Width(50));

            GUILayout.Box("溶质名称", GUILayout.Width(100));
            GUILayout.Box("溶剂名称", GUILayout.Width(100));

            GUILayout.Box("删除", GUILayout.Width(50));

            GUILayout.EndHorizontal();

            foreach (var item in DataLoading.DicDrugMixtureLoadingInfo.ToList())
            {
                GUILayout.BeginHorizontal();

                if (GUILayout.Button("<-", GUILayout.Width(50)))
                {
                    mixtureInfo = item.Value;
                    mixtureAdd = false;
                }

                GUILayout.Box(item.Value.name, GUILayout.Width(100));
                GUILayout.Box(item.Value.percent.ToString(), GUILayout.Width(50));
                GUILayout.Box(item.Value.soluteName.ToString(), GUILayout.Width(100));
                GUILayout.Box(item.Value.solventName.ToString(), GUILayout.Width(100));

                if (GUILayout.Button("X", GUILayout.Width(50)))
                {
                    DataLoading.DicDrugMixtureLoadingInfo.Remove(item.Key);
                    WriteJson(DataLoading.DicDrugMixtureLoadingInfo.Values, mixturePath);
                }

                GUILayout.EndHorizontal();
            }
        }

        /// <summary>
        /// 混合物信息
        /// </summary>
        void CreateMixtureInfo()
        {
            if (mixtureInfo == null)
            {
                mixtureInfo = new DI_DrugMixtureInfo();
                mixtureAdd = true;
            }

            GUILayout.BeginVertical("box", GUILayout.Width(450));
            GUILayout.Label("药品混合物信息：", chemicalEditor.titleStyle);
            GUILayout.Space(5);

            //添加和编辑窗口
            mixtureInfo.name = EditorGUILayout.TextField("药品名称：", mixtureInfo.name);

            mixtureInfo.percent = EditorGUILayout.Slider("含量(溶质质量/溶液质量)：", mixtureInfo.percent, 0, 1);
            mixtureInfo.soluteName = EditorGUILayout.TextField("溶质名字：", mixtureInfo.soluteName);
            mixtureInfo.solventName = EditorGUILayout.TextField("溶剂名字：", mixtureInfo.solventName);

            if (mixtureAdd)
            {
                if (GUILayout.Button("添加", GUILayout.Width(70)))
                {
                    if (!DataLoading.DicDrugMixtureLoadingInfo.ContainsKey(mixtureInfo.name))
                    {
                        DataLoading.DicDrugMixtureLoadingInfo.Add(mixtureInfo.name, mixtureInfo);
                        WriteJson(DataLoading.DicDrugMixtureLoadingInfo.Values, mixturePath);
                    }

                    mixtureInfo = null;
                }
            }
            else
            {
                if (GUILayout.Button("编辑", GUILayout.Width(70)))
                {
                    WriteJson(DataLoading.DicDrugMixtureLoadingInfo.Values, mixturePath);

                    mixtureInfo = null;
                }
            }

            if (DataLoading.DicDrugMixtureLoadingInfo.Count > 0)
            {
                LoadMixtureInfo();
            }

            GUILayout.EndVertical();
        }
    }
}
