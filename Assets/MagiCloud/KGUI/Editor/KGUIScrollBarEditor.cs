using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace MagiCloud.KGUI
{
    /// <summary>
    /// KGUI滚动条
    /// </summary>
    [CustomEditor(typeof(KGUI_ScrollBar))]
    [CanEditMultipleObjects]
    public class KGUIScrollBarEditor : Editor
    {
        KGUIButtonTypeEditor buttonType;


        public SerializedProperty handleRect;

        private KGUI_ScrollBar scrollBar;

        public SerializedProperty OnValueChanged;
        public SerializedProperty OnRelease;

        private void OnEnable()
        {
            scrollBar = serializedObject.targetObject as KGUI_ScrollBar;

            if (buttonType == null)
                buttonType = new KGUIButtonTypeEditor();

            buttonType.OnInstantiation(serializedObject);

            handleRect = serializedObject.FindProperty("handleRect");

            OnValueChanged = serializedObject.FindProperty("OnValueChanged");
            OnRelease = serializedObject.FindProperty("OnRelease");

        }

        public override void OnInspectorGUI()
        {
            buttonType.OnInspectorButtonType(scrollBar);

            GUILayout.BeginVertical(GUILayout.Width(500));

            GUILayout.Space(10);

            scrollBar.KguiAxis = (Axis)EditorGUILayout.EnumPopup(new GUIContent("移动轴：","Z轴暂不支持"), scrollBar.KguiAxis);

            switch (scrollBar.KguiAxis)
            {
                case Axis.X:

                    EditorGUI.BeginChangeCheck();
                    scrollBar.horizontal = (Horizontal)EditorGUILayout.EnumPopup("滚动方向：", scrollBar.horizontal);
                    break;
                case Axis.Y:

                    EditorGUI.BeginChangeCheck();
                    scrollBar.vertical = (Vertical)EditorGUILayout.EnumPopup("滚动方向：", scrollBar.vertical);
                    break;
                default:
                    break;
            }

            EditorGUILayout.PropertyField(handleRect, true, null);

            GUILayout.Space(20);

            if (GUILayout.Button("刷新", GUILayout.Width(100), GUILayout.Height(21)))
            {
                scrollBar.SetRectData();
            }

            ////值的赋予
            scrollBar.Value = EditorGUILayout.FloatField("Value：", scrollBar.Value);

            scrollBar.IsFullHandle = EditorGUILayout.Toggle("是否填充滚动条：", scrollBar.IsFullHandle);

            if (scrollBar.IsFullHandle)
            {
                scrollBar.Size = EditorGUILayout.FloatField("Size：", scrollBar.Size);
            }

            EditorGUILayout.PropertyField(OnValueChanged);
            EditorGUILayout.PropertyField(OnRelease);

            EditorGUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();
        }
    }
}
