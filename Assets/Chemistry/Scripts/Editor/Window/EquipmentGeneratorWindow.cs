using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Chemistry.Data;
using Chemistry.Help;

namespace Chemistry.Editor.Window
{
    public class EquipmentGeneratorWindow
    {
        private ChemicalEditorWindows chemicalEditor;

        public string WindowName;

        public EquipmentGeneratorWindow(ChemicalEditorWindows chemicalEditor,string windowName)
        {
            this.chemicalEditor = chemicalEditor;

            this.WindowName = windowName;
        }

        public void OnGUI()
        {
            EditorGUILayout.BeginHorizontal("box", GUILayout.Width(chemicalEditor.position.width - 10), GUILayout.Height(chemicalEditor.position.height - 10));

            OnInspectorSuccess();

            EditorGUILayout.EndHorizontal();
        }

        private void OnInspectorSuccess()
        {
            EditorGUILayout.BeginHorizontal("box", GUILayout.Width(chemicalEditor.position.width / 2));

            EditorGUILayout.BeginVertical();
            GUILayout.Label("已经配置完整的仪器（包括抓取操作、距离检测等）", chemicalEditor.titleStyle);
            List<string> temp = EquipmentInitializationHelper.GetAssetNames();

            if (temp != null)
            {
                for (int i = 0; i < temp.Count; i++)
                {
                    if (GUILayout.Button(temp[i], GUILayout.Width(120)))
                    {
                        EquipmentInitializationHelper.CreateSuccessEquipment(temp[i]);
                    }
                }
            }

            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();
        }


    }
}
