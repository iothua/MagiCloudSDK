using System;
using UnityEngine;
using UnityEditor;
using MagiCloud.Json;
using System.Linq;
using Chemistry.Data;
using System.Collections.Generic;

namespace Chemistry.Editor.Window
{
    /// <summary>
    /// 化学窗口
    /// </summary>
    public class ChemicalEditorWindows : EditorWindow
    {
        Vector2 scrollPos = Vector2.zero;

        public GUIStyle boxStyle;
        public GUIStyle titleStyle;
        public GUISkin magiCloud;

        private ChemicalEquationWindow chemicalEquationWindow; //化学方程式
        private DrugInfoWindow drugInfoWindow;
        private EquipmentGeneratorWindow generatorWindow;

        private Dictionary<string, Action> DicWindow;

        ChemicalEditorWindows()
        {
            this.titleContent = new GUIContent("化学数据窗口");
        }

        [MenuItem("MagiCloud/工具/化学数据窗口")]
        static void OnChemical()
        {
            EditorWindow.GetWindow(typeof(ChemicalEditorWindows));
        }

        private Action currentAction;

        private void OnEnable()
        {
            if (DicWindow == null)
                DicWindow = new Dictionary<string, Action>();

            if (!DicWindow.ContainsKey("读取文件数据"))
            {
                DicWindow.Add("读取文件数据", InitializeDataLoading);
            }

            generatorWindow = new EquipmentGeneratorWindow(this,"创建仪器");
            if (!DicWindow.ContainsKey(generatorWindow.WindowName))
                DicWindow.Add(generatorWindow.WindowName, generatorWindow.OnGUI);

            drugInfoWindow = new DrugInfoWindow(this, "药品数据");
            if (!DicWindow.ContainsKey(drugInfoWindow.WindowName))
                DicWindow.Add(drugInfoWindow.WindowName, drugInfoWindow.OnGUI);

            chemicalEquationWindow = new ChemicalEquationWindow(this, "化学方程式");

            if (!DicWindow.ContainsKey(chemicalEquationWindow.WindowName))
                DicWindow.Add(chemicalEquationWindow.WindowName, chemicalEquationWindow.OnGUI);

            currentAction = DicWindow["创建仪器"];
        }

        private void OnGUI()
        {
            if (magiCloud == null)
                magiCloud = Resources.Load<GUISkin>("MagiCloud");

            if (titleStyle == null)
            {
                titleStyle = new GUIStyle(magiCloud.label);
                titleStyle.fontSize = 13;
            }

            if (boxStyle == null)
            {
                boxStyle = new GUIStyle(GUI.skin.box);
                boxStyle.fontStyle = FontStyle.Bold;
                boxStyle.alignment = TextAnchor.MiddleCenter;
                boxStyle.normal.textColor = Color.white;
            }

            GUILayout.BeginHorizontal();

            MenuButton();

            GUILayout.Space(5);

            ChemicalWindows();

            GUILayout.EndHorizontal();
        }

        void MenuButton()
        {
            GUILayout.BeginVertical("box", GUILayout.Width(100));

            foreach (var item in DicWindow)
            {
                GUILayout.Space(10);

                if (currentAction == item.Value)
                {
                    GUILayout.Box(item.Key, GUILayout.Width(100));
                }
                else
                {
                    if (GUILayout.Button(item.Key, GUILayout.Width(100)))
                        currentAction = item.Value;
                }
            }

            GUILayout.EndVertical();
        }

        void ChemicalWindows()
        {
            //读静态字典信息
            scrollPos = GUILayout.BeginScrollView(scrollPos,"box", GUILayout.Width(position.width - 110), GUILayout.Height(position.height));

            if (currentAction != null)
                currentAction();

            GUILayout.EndScrollView();
        }

        public static void InitializeDataLoading()
        {
            ChemistryInitialize.InitializeDataLoading();
        }
    }
}
