using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using MagiCloud.KGUI;

namespace MagiCloud.UIFrame
{
    /// <summary>
    /// UI管理窗口
    /// </summary>
    public class ManagerEditorWindow : EditorWindow
    {
        private GUISkin magiCloud;

        private UI_Base[] Bases;

        private Vector2 scrollPos = Vector2.zero;

        private GUIStyle labelStyle;

        private UIManager manager; //UI管理端

        //private KGUI_Canvas canvas;
        //private KGUI_SpriteRenderer spriteRenderer;

        ManagerEditorWindow()
        {
            this.titleContent = new GUIContent("UI管理窗口");
        }

        [MenuItem("MagiCloud/UIFrame/UIManager管理")]
        static void CreateManager()
        {
            EditorWindow.GetWindow(typeof(ManagerEditorWindow));
        }

        private void OnEnable()
        {
            if (magiCloud == null)
                magiCloud = Resources.Load<GUISkin>("MagiCloud");

            Bases = FindObjectsOfType<UI_Base>();

            manager = FindObjectOfType<UIManager>();

            //canvas = manager.GetComponentInChildren<KGUI_Canvas>();
            //spriteRenderer = manager.GetComponentInChildren<KGUI_SpriteRenderer>();

        }

        private void OnDestroy()
        {
            Bases = null;
        }

        private void OnGUI()
        {
            scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.Width(position.width), GUILayout.Height(position.height));

            GUILayout.Space(10);

            GUILayout.BeginVertical("box", GUILayout.Width(600));

            if (GUILayout.Button("刷新", GUILayout.Width(150), GUILayout.Height(21)))
            {
                Bases = FindObjectsOfType<UI_Base>();
            }

            GUILayout.BeginHorizontal();

            GUILayout.Box("位于UIManager下", GUILayout.Width(120), GUILayout.Height(20));
            GUILayout.Box("物体名称", GUILayout.Width(100), GUILayout.Height(20));
            GUILayout.Box("TagID", GUILayout.Width(100), GUILayout.Height(20));
            GUILayout.Box("UI类型状态", GUILayout.Width(100), GUILayout.Height(20));
            GUILayout.Box("激活状态", GUILayout.Width(100), GUILayout.Height(20));
            GUILayout.Box("UI类", GUILayout.Width(300), GUILayout.Height(20));

            GUILayout.EndHorizontal();

            for (int i = 0; i < manager.UIs.Count; i++)
            {
                BaseGUI(manager.UIs[i]);
            }

            for (int i = 0; i < Bases.Length; i++)
            {
                if (!manager.IsContains(Bases[i]))
                {
                    BaseGUI(Bases[i]);
                }
            }

            GUILayout.EndVertical();

            GUILayout.EndScrollView();
        }


        private void BaseGUI(UI_Base ui)
        {
            GUILayout.BeginHorizontal();

            if (manager.IsContains(ui))
            {
                if (GUILayout.Button("从Manager移除", GUILayout.Width(120), GUILayout.Height(20)))
                {
                    manager.RemoveUI(ui);
                }
            }
            else
            {
                if (GUILayout.Button(new GUIContent("加入Manager", "点击之后，加入到Manager下"), GUILayout.Width(120), GUILayout.Height(20)))
                {
                    manager.AddUI(ui, Vector3.zero);
                }
            }

            if (GUILayout.Button(ui.name, GUILayout.Width(100), GUILayout.Height(20)))
            {
                Selection.activeGameObject = ui.gameObject;
            }

            ui.TagID = EditorGUILayout.TextField("", ui.TagID, GUILayout.Width(100), GUILayout.Height(20));
            ui.type = (UIType)EditorGUILayout.EnumPopup("", ui.type, GUILayout.Width(100), GUILayout.Height(20));

            if (ui.gameObject.activeSelf)
            {
                if (GUILayout.Button("隐藏", GUILayout.Width(100), GUILayout.Height(20)))
                {
                    ui.gameObject.SetActive(false);
                }
            }
            else
            {
                if (GUILayout.Button("显示", GUILayout.Width(100), GUILayout.Height(20)))
                {
                    ui.gameObject.SetActive(true);
                }
            }

            GUILayout.Box(ui.GetType().ToString(), GUILayout.Width(300), GUILayout.Height(20));

            switch (ui.type)
            {
                case UIType.Canvas:

                    if (ui.transform.parent == manager.Canvas.transform)
                        break;
                    ui.transform.SetParent(manager.Canvas.transform);
                    ui.transform.localPosition = Vector3.zero;

                    break;
                case UIType.SpriteRender:
                    if (ui.transform.parent == manager.SpriteRenderer.transform)
                        break;
                    ui.transform.SetParent(manager.SpriteRenderer.transform);
                    ui.transform.localPosition = Vector3.zero;
                    break;
                default:
                    break;

            }

            GUILayout.EndHorizontal();
        }

    }
}

