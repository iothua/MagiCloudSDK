using Chemistry.Data;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;


namespace Chemistry.Editor.Window
{
    public class EquipmentDrugInfoWindow
    {
        /*
        private DI_EquipmentDrugInfo equipmentDrugInfo;
        private bool isEquipmentAdd = false; //仪器添加
        private string equipmentDrugPath = string.Empty;

        private DI_DrugInfo drugInfo;
        private bool isDrugInfo = false; //药品添加

        private ChemicalEditorWindows chemicalEditor;
        public string WindowName;

        public EquipmentDrugInfoWindow(ChemicalEditorWindows chemicalEditor, string windowName)
        {
            this.chemicalEditor = chemicalEditor;
            this.WindowName = windowName;

            if (string.IsNullOrEmpty(equipmentDrugPath))
            {
                equipmentDrugPath = Application.streamingAssetsPath + DefineConst.PATH_JSON_EQUIPMENT_DRUG;
            }
        }

        public void OnGUI()
        {
            GUILayout.BeginHorizontal();

            //绘制数据添加面板
            DrawInfoEditor();
            DrawShowInfos();

            GUILayout.EndHorizontal();
        }

        //绘制数据添加面板
        private void DrawInfoEditor()
        {
            GUILayout.BeginVertical("box", GUILayout.Width(500));

            if (equipmentDrugInfo == null)
            {
                equipmentDrugInfo = new DI_EquipmentDrugInfo();
                isEquipmentAdd = true;
            }

            GUILayout.Label("仪器药品信息", chemicalEditor.titleStyle);

            equipmentDrugInfo.equipmentName = EditorGUILayout.TextField("仪器名称：", equipmentDrugInfo.equipmentName);
            equipmentDrugInfo.sumVolume = EditorGUILayout.FloatField("容积：", equipmentDrugInfo.sumVolume);

            GUILayout.Space(10);
            GUILayout.BeginHorizontal();

            if (isEquipmentAdd)
            {
                if (GUILayout.Button("添加仪器", GUILayout.Width(100)))
                {
                    if (!DataLoading.DicEquipmentDrugLoadingInfo.ContainsKey(equipmentDrugInfo.equipmentName))
                    {
                        DataLoading.DicEquipmentDrugLoadingInfo.Add(equipmentDrugInfo.equipmentName, equipmentDrugInfo);
                        //写入文件
                        DataLoading.WriteJson(DataLoading.DicEquipmentDrugLoadingInfo.Values, equipmentDrugPath);
                    }

                    equipmentDrugInfo = null;
                }
            }
            else
            {
                if (GUILayout.Button("编辑仪器", GUILayout.Width(100)))
                {
                    DataLoading.WriteJson(DataLoading.DicEquipmentDrugLoadingInfo.Values, equipmentDrugPath);
                    equipmentDrugInfo = null;
                }
            }
            GUILayout.Space(10);

            if (GUILayout.Button("取消", GUILayout.Width(100)))
            {
                equipmentDrugInfo = null;
                drugInfo = null;
            }

            GUILayout.EndHorizontal();
            DrawDrugInfo();

            GUILayout.EndVertical();
        }

        void DrawDrugInfo()
        {
            if (equipmentDrugInfo == null) return;

            GUILayout.Label("药品信息", chemicalEditor.titleStyle);

            if (drugInfo == null)
            {
                drugInfo = new DI_DrugInfo();
                isDrugInfo = true;
            }

            drugInfo.drugName = EditorGUILayout.TextField("药品名称：", drugInfo.drugName);

            //根据药品的类型，容积的值需要定义
            drugInfo.drugVolume = EditorGUILayout.Slider("药品容积：", drugInfo.drugVolume, 0, equipmentDrugInfo.sumVolume);
            //包括模型特效
            drugInfo.drugPosition.Vector = EditorGUILayout.Vector3Field("模型初始坐标：", drugInfo.drugPosition.Vector);
            //模型的名称等
            drugInfo.drugModelName = EditorGUILayout.TextField("药品模型名称：", drugInfo.drugModelName);

            GUILayout.Space(10);

            DrawDrugInfoAdd();

            GUILayout.Space(10);

            DrawShowDrugInfo();
        }

        void DrawDrugInfoAdd()
        {
            GUILayout.BeginHorizontal();
            if (isDrugInfo)
            {
                if (GUILayout.Button("添加药品", GUILayout.Width(100)))
                {
                    if (!equipmentDrugInfo.drugInfos.Contains(drugInfo))
                    {
                        equipmentDrugInfo.drugInfos.Add(drugInfo);

                        DataLoading.WriteJson(DataLoading.DicEquipmentDrugLoadingInfo.Values, equipmentDrugPath);

                        drugInfo = null;
                    }
                    else
                        Debug.LogError("添加的药品已经存在");
                }
            }
            else
            {
                if (GUILayout.Button("编辑药品", GUILayout.Width(100)))
                {
                    DataLoading.WriteJson(DataLoading.DicEquipmentDrugLoadingInfo.Values, equipmentDrugPath);
                    drugInfo = null;
                }
            }
            GUILayout.Space(10);

            if (GUILayout.Button("取消", GUILayout.Width(100)))
            {
                drugInfo = new DI_DrugInfo();
                isDrugInfo = true;
            }

            GUILayout.EndHorizontal();
        }

        void DrawShowDrugInfo()
        {
            if (equipmentDrugInfo == null) return;
            if (equipmentDrugInfo.drugInfos.Count == 0) return;

            GUILayout.BeginHorizontal();

            GUILayout.Box("选中", GUILayout.Width(50));
            GUILayout.Box("药品名称", GUILayout.Width(70));
            GUILayout.Box("药品容积", GUILayout.Width(70));
            GUILayout.Box("模型初始坐标", GUILayout.Width(100));
            GUILayout.Box("模型名称", GUILayout.Width(70));
            GUILayout.Box("删除", GUILayout.Width(50));

            GUILayout.EndHorizontal();

            foreach (var item in equipmentDrugInfo.drugInfos.ToList())
            {
                GUILayout.BeginHorizontal();

                if (GUILayout.Button("<-", GUILayout.Width(50)))
                {
                    drugInfo = item;
                    isDrugInfo = false;
                }

                GUILayout.Box(item.drugName, GUILayout.Width(70));
                GUILayout.Box(item.drugVolume.ToString(), GUILayout.Width(70));
                GUILayout.Box(item.drugPosition.ToString(), GUILayout.Width(100));
                GUILayout.Box(item.drugModelName, GUILayout.Width(70));

                if (GUILayout.Button("X", GUILayout.Width(50)))
                {
                    equipmentDrugInfo.drugInfos.Remove(item);

                    DataLoading.WriteJson(DataLoading.DicEquipmentDrugLoadingInfo.Values, equipmentDrugPath);
                }

                GUILayout.EndHorizontal();
            }
        }

        /// <summary>
        /// 绘制数据显示面板
        /// </summary>
        private void DrawShowInfos()
        {
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();

            GUILayout.Box("选中", chemicalEditor.boxStyle, GUILayout.Width(50));
            GUILayout.Box("删除", chemicalEditor.boxStyle, GUILayout.Width(50));
            GUILayout.Box("编号", chemicalEditor.boxStyle, GUILayout.Width(50));
            GUILayout.Box("仪器名称", chemicalEditor.boxStyle, GUILayout.Width(100));
            GUILayout.Box("容器容积", chemicalEditor.boxStyle, GUILayout.Width(100));
            GUILayout.Box("药品名称", chemicalEditor.boxStyle, GUILayout.Width(100));
            GUILayout.Box("药品体积", chemicalEditor.boxStyle, GUILayout.Width(100));

            GUILayout.EndHorizontal();
            int id = 1;
            foreach (var item in DataLoading.DicEquipmentDrugLoadingInfo)
            {
                var infos = item.Value.drugInfos;

                GUILayout.BeginHorizontal();

                if (GUILayout.Button("<-", GUILayout.Width(50)))
                {
                    equipmentDrugInfo = item.Value;
                    isEquipmentAdd = false;
                    drugInfo = equipmentDrugInfo.drugInfos.Count > 0 ? equipmentDrugInfo.drugInfos[0] : null;
                    isDrugInfo = false;

                }

                if (GUILayout.Button("X", GUILayout.Width(50)))
                {
                    DataLoading.DicEquipmentDrugLoadingInfo.Remove(item.Key);
                    DataLoading.WriteJson(DataLoading.DicEquipmentDrugLoadingInfo.Values, equipmentDrugPath);

                    equipmentDrugInfo = null;
                    drugInfo = null;

                    return;
                }

                GUILayout.Box(id.ToString(), GUILayout.Width(50));
                GUILayout.Box(item.Key, GUILayout.Width(100));
                GUILayout.Box(item.Value.sumVolume.ToString(), GUILayout.Width(100));
                GUILayout.BeginVertical();

                for (int i = 0; i < infos.Count; i++)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Box(infos[i].drugName, GUILayout.Width(100));
                    GUILayout.Box(infos[i].drugVolume.ToString(), GUILayout.Width(100));
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndVertical();

                GUILayout.EndHorizontal();
                id++;
            }
            GUILayout.EndVertical();

        }
        */
    }
}
