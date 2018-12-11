using UnityEditor;
using UnityEngine;
using MagiCloud.Json;
using System.Linq;
using Chemistry.Data;
using System.Collections.Generic;

namespace Chemistry.Editor.Window
{
    public class EquipmentWindow
    {
        private DI_EquipmentInfo equipmentInfo;

        private bool equipmentAdd = false;
        private string path = string.Empty;

        private ChemicalEditorWindows chemicalEditor;
        public string WindowName;

        private string equipmentName = string.Empty;

        public EquipmentWindow(ChemicalEditorWindows chemicalEditor,string windowName)
        {
            this.chemicalEditor = chemicalEditor;
            this.WindowName = windowName;

            if (string.IsNullOrEmpty(path))
            {
                path = Application.streamingAssetsPath + DefineConst.PATH_JSON_EQUIPMENT;
            }
            if (!DataLoading.IsInitialized)
            {
                DataLoading.OnInitialize();
            }

            //obj = new SerializedObject(chemicalEditor);
            //editorChildList = obj.FindProperty("childList");
        }

        private void AddMenuItemForValue(GenericMenu menu, string value)
        {
            menu.AddItem(new GUIContent(value), equipmentName.Equals(value),
                OnValueSelected, value);
        }

        private void OnValueSelected(object value)
        {
            equipmentName = value.ToString();
        }

        public void OnGUI()
        {
            GUILayout.BeginHorizontal();

            CreateEquipmentInfo();

            LoadEquipmentInfo(DataLoading.DicEquipmentLoadingInfo.Values.ToList());

            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// 仪器信息框
        /// </summary>
        void CreateEquipmentInfo()
        {
            if (equipmentInfo == null)
            {
                equipmentInfo = new DI_EquipmentInfo();
                equipmentAdd = true;
            }

            GUILayout.BeginVertical("box", GUILayout.Width(400));

            GUILayout.Space(5);
            GUILayout.Label("仪器名称：" + equipmentInfo.equipmentName, chemicalEditor.titleStyle);
            GUILayout.Space(5);

            equipmentInfo.equipmentName = EditorGUILayout.TextField("仪器名称：", equipmentInfo.equipmentName);

            equipmentInfo.resourcesName = EditorGUILayout.TextField(new GUIContent("仪器资源名称：", "在Resources下模型资源名称"), equipmentInfo.resourcesName);

            GUILayout.Space(10);

            equipmentInfo.colliderData.Center.Vector = EditorGUILayout.Vector3Field("碰撞体中心点：", equipmentInfo.colliderData.Center.Vector);
            equipmentInfo.colliderData.Size.Vector = EditorGUILayout.Vector3Field("碰撞体大小：", equipmentInfo.colliderData.Size.Vector);

            GUILayout.Space(10);

            equipmentInfo.transformData.localPosition.Vector = EditorGUILayout.Vector3Field("局部坐标：", equipmentInfo.transformData.localPosition.Vector);
            equipmentInfo.transformData.localRotation.Vector = EditorGUILayout.Vector3Field("局部旋转：", equipmentInfo.transformData.localRotation.Vector);
            equipmentInfo.transformData.localScale.Vector = EditorGUILayout.Vector3Field("局部大小：", equipmentInfo.transformData.localScale.Vector);

            GUILayout.Space(10);

            equipmentInfo.Namespaces = EditorGUILayout.TextField("仪器脚本命名空间：", equipmentInfo.Namespaces);

            equipmentInfo.scriptName = EditorGUILayout.TextField(new GUIContent("仪器脚本名称："), equipmentInfo.scriptName);

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();

            if (equipmentAdd)
            {
                if (GUILayout.Button(new GUIContent("生成仪器数据"), GUILayout.Width(100)))
                {
                    if (!DataLoading.DicEquipmentLoadingInfo.ContainsKey(equipmentInfo.equipmentName))
                    {
                        DataLoading.DicEquipmentLoadingInfo.Add(equipmentInfo.equipmentName, equipmentInfo);

                        DataLoading.WriteJson(DataLoading.DicEquipmentLoadingInfo.Values, path);
                    }
                    else
                    {
                        Debug.LogError(equipmentInfo.equipmentName + "仪器已经存在字典中");
                    }

                    equipmentInfo = null;
                }
            }
            else
            {
                //编辑
                if (GUILayout.Button(new GUIContent("编辑仪器数据"), GUILayout.Width(100)))
                {
                    DataLoading.WriteJson(DataLoading.DicEquipmentLoadingInfo.Values, path);
                }

                GUILayout.Space(10);

            }

            GUILayout.Space(5);

            if (GUILayout.Button(new GUIContent("取消", "清空当前所选中的仪器信息，数据全部复位"), GUILayout.Width(70)))
            {
                equipmentInfo = null;
            }

            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            DrawChildEquipment();

            GUILayout.EndVertical();
        }

        void DrawChildEquipment()
        {
            if (equipmentInfo == null) return;

            GUILayout.BeginHorizontal("box");

            GUILayout.Label("仪器：");

            if (EditorGUILayout.DropdownButton(new GUIContent(equipmentName), FocusType.Passive))
            {
                GenericMenu menu = new GenericMenu();

                foreach (var item in DataLoading.DicEquipmentLoadingInfo)
                {
                    if (string.IsNullOrEmpty(item.Key)) continue;

                    AddMenuItemForValue(menu, item.Key);
                }

                menu.ShowAsContext();
            }

            if (GUILayout.Button("添加", GUILayout.Width(70)))
            {
                if (!equipmentInfo.childEquipments.Contains(equipmentName))
                {
                    equipmentInfo.childEquipments.Add(equipmentName);

                    DataLoading.WriteJson(DataLoading.DicEquipmentLoadingInfo.Values, path);
                }
            }

            GUILayout.EndHorizontal();

            int number = equipmentInfo.childEquipments.Count;
            if (number == 0) return;

            GUILayout.BeginHorizontal();

            GUILayout.Box("序号", GUILayout.Width(50));
            GUILayout.Box("子仪器", GUILayout.Width(100));
            GUILayout.Box("移除", GUILayout.Width(50));

            GUILayout.EndHorizontal();

            for (int i = 0; i < number; i++)
            {
                GUILayout.BeginHorizontal();

                GUILayout.Box((i+1).ToString(), GUILayout.Width(50));
                GUILayout.Box(equipmentInfo.childEquipments[i], GUILayout.Width(100));

                if (GUILayout.Button("移除", GUILayout.Width(50)))
                {
                    equipmentInfo.childEquipments.RemoveAt(i);

                    DataLoading.WriteJson(DataLoading.DicEquipmentLoadingInfo.Values, path);
                    break;
                }

                GUILayout.EndHorizontal();
            }
        }

        /// <summary>
        /// 仪器信息列表
        /// </summary>
        void LoadEquipmentInfo(List<DI_EquipmentInfo> equipmentInfos)
        {
            GUILayout.BeginVertical("box");

            GUILayout.BeginHorizontal();

            GUILayout.Box("选中", chemicalEditor.boxStyle, GUILayout.Width(50));
            GUILayout.Box("删除", chemicalEditor.boxStyle, GUILayout.Width(50));
            GUILayout.Box("编号", chemicalEditor.boxStyle, GUILayout.Width(50));
            GUILayout.Box("仪器名称", chemicalEditor.boxStyle, GUILayout.Width(100));
            GUILayout.Box("资源名称", chemicalEditor.boxStyle, GUILayout.Width(150));
            GUILayout.Box("仪器脚本名称", chemicalEditor.boxStyle, GUILayout.Width(250));
            GUILayout.Box("局部坐标", chemicalEditor.boxStyle, GUILayout.Width(100));
            GUILayout.Box("局部旋转值", chemicalEditor.boxStyle, GUILayout.Width(100));
            GUILayout.Box("局部大小", chemicalEditor.boxStyle, GUILayout.Width(100));
            GUILayout.Box("子仪器:", chemicalEditor.boxStyle, GUILayout.Width(100));


            GUILayout.EndHorizontal();
            int id = 1;
            foreach (var item in equipmentInfos)
            {
                GUILayout.BeginHorizontal();

                if (GUILayout.Button("<-", GUILayout.Width(50)))
                {
                    equipmentInfo = item;
                    equipmentAdd = false;
                }
                if (GUILayout.Button("X", GUILayout.Width(50)))
                {
                    DataLoading.DicEquipmentLoadingInfo.Remove(item.equipmentName);

                    DataLoading.WriteJson(DataLoading.DicEquipmentLoadingInfo.Values, path);
                }
                GUILayout.Box(id.ToString(), GUILayout.Width(50));
                GUILayout.Box(item.equipmentName, GUILayout.Width(100));
                GUILayout.Box(item.resourcesName, GUILayout.Width(150));
                GUILayout.Box(item.scriptName, GUILayout.Width(250));

                GUILayout.Box(item.transformData.localPosition.ToString(), GUILayout.Width(100));
                GUILayout.Box(item.transformData.localRotation.ToString(), GUILayout.Width(100));
                GUILayout.Box(item.transformData.localScale.ToString(), GUILayout.Width(100));


                GUILayout.BeginVertical();
                if (item.childEquipments != null && item.childEquipments.Count > 0)
                {
                    for (int i = 0; i < item.childEquipments.Count; i++)
                    {
                        GUILayout.Box(item.childEquipments[i], GUILayout.Width(100));
                    }
                }
                GUILayout.EndVertical();


                GUILayout.EndHorizontal();

                id++;
            }

            GUILayout.EndVertical();
        }
    }
}
