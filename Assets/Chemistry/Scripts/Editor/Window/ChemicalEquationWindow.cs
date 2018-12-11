using System;
using UnityEngine;
using UnityEditor;
using MagiCloud.Json;
using System.Linq;
using Chemistry.Data;

namespace Chemistry.Editor.Window
{
    /// <summary>
    /// 化学方程式
    /// </summary>
    public class ChemicalEquationWindow
    {
        private DI_ReactionInfo reactionInfo;

        private bool reactionAdd = false;
        private string reactionPath = string.Empty;

        private ChemicalEditorWindows chemicalEditor;

        public string WindowName;

        public ChemicalEquationWindow(ChemicalEditorWindows chemicalEditor, string windowName)
        {
            this.WindowName = windowName;

            if (string.IsNullOrEmpty(reactionPath))
            {
                reactionPath = Application.streamingAssetsPath + DefineConst.PATH_JSON_REACTIONEQUATION;
            }

            this.chemicalEditor = chemicalEditor;

        }

        public void OnGUI()
        {
            GUILayout.BeginHorizontal();

            //列举化学仪器

            CreateChemicalEquation();

            GUILayout.EndHorizontal();
        }


        void CreateChemicalEquation()
        {
            if (reactionInfo == null)
            {
                reactionInfo = new DI_ReactionInfo();
                reactionAdd = true;
            }

            GUILayout.BeginVertical("box");

            LoadChemicalEquation();

            GUILayout.EndVertical();
        }

        /// <summary>
        /// 列举出化学方程式信息
        /// </summary>
        void LoadChemicalEquation()
        {
            GUILayout.BeginHorizontal();

            GUILayout.Box("选中", chemicalEditor.boxStyle, GUILayout.Width(50));
            GUILayout.Box("反应物", chemicalEditor.boxStyle, GUILayout.Width(400));
            GUILayout.Box("反应条件", chemicalEditor.boxStyle, GUILayout.Width(150));
            GUILayout.Box("生成物", chemicalEditor.boxStyle, GUILayout.Width(400));
            GUILayout.Box("说明", chemicalEditor.boxStyle, GUILayout.Width(400));
            GUILayout.Box("化学方程式", chemicalEditor.boxStyle, GUILayout.Width(400));

            GUILayout.EndHorizontal();

            foreach (var item in DataLoading.DicReactionLoadingInfo.ToList())
            {
                GUILayout.BeginHorizontal();

                if (GUILayout.Button("<-", GUILayout.Width(50)))
                {
                    reactionInfo = item.Value;
                    reactionAdd = false;
                }

                GUILayout.Box(item.Value.ReactantStr, GUILayout.Width(400));
                GUILayout.Box(item.Value.ConditionStr, GUILayout.Width(150));
                GUILayout.Box(item.Value.ProductStr, GUILayout.Width(400));

                GUILayout.Box(item.Value.describe, GUILayout.Width(400));
                GUILayout.Box(item.Value.equationName, GUILayout.Width(400));

                GUILayout.EndHorizontal();
            }
        }
    }
}
