using UnityEditor;
using UnityEngine;
using MagiCloud.Json;
using System.Linq;
using Chemistry.Data;
using System.Collections.Generic;

namespace Chemistry.Editor.Window
{
    public class EquipmentWaterModelWindow
    {
        //选中的或正在编辑的水体模型信息
        private DI_ContainerWaterModelInfo waterModelInfo;
        //对应数据的JSON文件路径
        private string path = string.Empty;
        //是否为正在增加状态
        private bool isAdding = false;
        //序列化对象
        private SerializedObject obj;
        //选中的编号--0为未选中任何编号
        private int chooseId = 0;

        private ChemicalEditorWindows chemicalEditor;

        public string WindowName;

        public EquipmentWaterModelWindow(ChemicalEditorWindows chemicalEditor,string windowName)
        {
            this.WindowName = windowName;

            if (string.IsNullOrEmpty(path))
            {
                path = Application.streamingAssetsPath + DefineConst.PATH_JSON_EQUIPMENT_WATERMODEL;
            }

            if (!DataLoading.IsInitialized)
            {
                DataLoading.OnInitialize();
            }

            obj = new SerializedObject(chemicalEditor);

            this.chemicalEditor = chemicalEditor;

        }

        public void OnGUI()
        {
            GUILayout.BeginHorizontal();

            CreateWaterModelInfo();

            LoadWaterModelInfo(DataLoading.DicContainerWaterModelLoadingInfo.Values.ToList());

            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// 水体模型信息框
        /// </summary>
        void CreateWaterModelInfo()
        {
            if (waterModelInfo == null)
            {
                waterModelInfo = new DI_ContainerWaterModelInfo();
                isAdding = true;
            }

            GUILayout.BeginVertical("box", GUILayout.Width(400));

            GUILayout.Space(5);
            GUILayout.Label("仪器名称：         " + waterModelInfo.equipmentName, chemicalEditor.titleStyle);
            GUILayout.Space(5);

            waterModelInfo.equipmentName = EditorGUILayout.TextField("仪器名称：", waterModelInfo.equipmentName);
            GUILayout.Space(10);
            waterModelInfo.containerType = (EContainerType)EditorGUILayout.EnumPopup("仪器类型：", waterModelInfo.containerType);
            GUILayout.Space(10);
            waterModelInfo.modelName = EditorGUILayout.TextField("水体模型名称：", waterModelInfo.modelName);
            GUILayout.Space(10);
            waterModelInfo.pos.Vector = EditorGUILayout.Vector3Field("相对仪器的坐标", waterModelInfo.pos.Vector);
            GUILayout.Space(10);

            obj.Update();

            GUILayout.BeginHorizontal();
            if (isAdding)
            {
                if (GUILayout.Button(new GUIContent("生成水体模型数据"), GUILayout.Width(100)))
                {
                    //创建
                    if (chooseId == 0)
                    {
                        if (!DataLoading.DicContainerWaterModelLoadingInfo.ContainsKey(waterModelInfo.equipmentName))
                        {
                            DataLoading.DicContainerWaterModelLoadingInfo.Add(waterModelInfo.equipmentName, waterModelInfo);
                            DataLoading.WriteJson(DataLoading.DicContainerWaterModelLoadingInfo.Values, path);
                        }
                        else
                        {
                            Debug.LogError("当前仪器已存在此水体模型信息");
                        }

                    }
                    waterModelInfo = null;
                }
            }
            else
            {
                //编辑
                if (GUILayout.Button(new GUIContent("编辑仪器数据"), GUILayout.Width(100)))
                {
                    DataLoading.WriteJson(DataLoading.DicContainerWaterModelLoadingInfo.Values, path);
                }
                GUILayout.Space(10);
            }
            GUILayout.Space(5);

            if (GUILayout.Button(new GUIContent("取消", "清空当前所选信息"), GUILayout.Width(70)))
            {
                waterModelInfo = null;
                chooseId = 0;
            }

            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
        }

        /// <summary>
        /// 仪器水体模型列表
        /// </summary>
        void LoadWaterModelInfo(List<DI_ContainerWaterModelInfo> waterModelInfos)
        {
            GUILayout.BeginVertical("box");

            GUILayout.BeginHorizontal();

            GUILayout.Box("选中", chemicalEditor.boxStyle, GUILayout.Width(50));
            GUILayout.Box("删除", chemicalEditor.boxStyle, GUILayout.Width(50));
            GUILayout.Box("编号", chemicalEditor.boxStyle, GUILayout.Width(50));
            GUILayout.Box("仪器名称", chemicalEditor.boxStyle, GUILayout.Width(100));
            GUILayout.Box("仪器类型", chemicalEditor.boxStyle, GUILayout.Width(100));
            GUILayout.Box("水体模型预制体名称", chemicalEditor.boxStyle, GUILayout.Width(300));
            GUILayout.Box("相对仪器的坐标", chemicalEditor.boxStyle, GUILayout.Width(300));

            GUILayout.EndHorizontal();

            if (waterModelInfos == null)
            {
                Debug.Log("水体模型信息为空");
                return;
            }

            int id = 1;
            foreach (var info in waterModelInfos)
            {
                GUILayout.BeginHorizontal();

                if (GUILayout.Button("<-", GUILayout.Width(50)))
                {
                    waterModelInfo = info;
                    isAdding = false;
                }

                if (GUILayout.Button("X", GUILayout.Width(50)))
                {
                    if (chooseId == 0)
                    {
                        DataLoading.DicContainerWaterModelLoadingInfo.Remove(info.equipmentName);
                    }

                    DataLoading.WriteJson(DataLoading.DicContainerWaterModelLoadingInfo.Values, path);
                }

                GUILayout.Box(id.ToString(), GUILayout.Width(50));
                GUILayout.Box(info.equipmentName, GUILayout.Width(100));
                GUILayout.Box(info.containerType.ToString(), GUILayout.Width(100));
                GUILayout.Box(info.modelName, GUILayout.Width(300));
                GUILayout.Box(info.pos.ToString(), GUILayout.Width(300));

                GUILayout.EndHorizontal();

                id++;
            }


            GUILayout.EndVertical();
        }
    }
}
