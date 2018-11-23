using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace MagiCloud.KGUI
{
    [CustomEditor(typeof(KGUI_Dropdown))]
    [CanEditMultipleObjects]
    public class KGUIDropdownEditor : Editor
    {
        private GUIStyle style;
        public SerializedProperty onEnter;  //鼠标移入
        public SerializedProperty onExit;   //鼠标移出
        public SerializedProperty onDown;   //鼠标按下

        public SerializedProperty spriteRenderer;
        public SerializedProperty image;

        public SerializedProperty normalSprite, enterSprite, pressedSprite;
        public SerializedProperty normalObject, enterObject, pressedObject;

        private KGUI_Dropdown dropdown;

        public SerializedProperty Names;

        public SerializedProperty ScrollView;

        public SerializedProperty Template;
        public SerializedProperty textName;

        public SerializedProperty gridLayout;
        public SerializedProperty dropdownItem;

        private void OnEnable()
        {
            dropdown = serializedObject.targetObject as KGUI_Dropdown;

            onEnter = serializedObject.FindProperty("onEnter");
            onExit = serializedObject.FindProperty("onExit");
            onDown = serializedObject.FindProperty("onDown");

            spriteRenderer = serializedObject.FindProperty("spriteRenderer");
            image = serializedObject.FindProperty("image");

            normalSprite = serializedObject.FindProperty("normalSprite");
            enterSprite = serializedObject.FindProperty("enterSprite");
            pressedSprite = serializedObject.FindProperty("pressedSprite");

            normalObject = serializedObject.FindProperty("normalObject");
            enterObject = serializedObject.FindProperty("enterObject");
            pressedObject = serializedObject.FindProperty("pressedObject");

            Names = serializedObject.FindProperty("Names");

            ScrollView = serializedObject.FindProperty("scrollView");

            Template = serializedObject.FindProperty("Template");

            textName = serializedObject.FindProperty("textName");
            gridLayout = serializedObject.FindProperty("gridLayout");
            dropdownItem = serializedObject.FindProperty("dropdownItem");

        }

        public override void OnInspectorGUI()
        {
            if (style == null)
            {
                style = new GUIStyle(GUI.skin.name);
                style.normal.textColor = GUI.skin.label.normal.textColor;
                style.fontStyle = FontStyle.Bold;
                style.alignment = TextAnchor.UpperLeft;
            }

            GUILayout.BeginVertical("box", GUILayout.Width(500));

            GUILayout.Space(10);
            EditorGUILayout.LabelField("交互属性", style);

            dropdown.buttonType = (ButtonType)EditorGUILayout.EnumPopup("交互类型：", dropdown.buttonType);

            switch (dropdown.buttonType)
            {
                case ButtonType.Image:

                    EditorGUI.BeginChangeCheck();

                    EditorGUILayout.PropertyField(image, true, null);
                    EditorGUILayout.PropertyField(normalSprite, true, null);
                    EditorGUILayout.PropertyField(enterSprite, true, null);
                    EditorGUILayout.PropertyField(pressedSprite, true, null);

                    break;
                case ButtonType.Object:
                    EditorGUI.BeginChangeCheck();

                    EditorGUILayout.PropertyField(normalObject, true, null);
                    EditorGUILayout.PropertyField(enterObject, true, null);
                    EditorGUILayout.PropertyField(pressedObject, true, null);

                    break;
                case ButtonType.SpriteRenderer:
                    EditorGUI.BeginChangeCheck();

                    EditorGUILayout.PropertyField(spriteRenderer, true, null);
                    EditorGUILayout.PropertyField(normalSprite, true, null);
                    EditorGUILayout.PropertyField(enterSprite, true, null);
                    EditorGUILayout.PropertyField(pressedSprite, true, null);
                    break;
                default:
                    EditorGUI.BeginChangeCheck();
                    break;
            }

            GUILayout.Space(10);

            EditorGUILayout.LabelField("下拉框属性", style);

            EditorGUILayout.PropertyField(Names, true, null);
            EditorGUILayout.PropertyField(ScrollView, true, null);
            EditorGUILayout.PropertyField(Template, true, null);
            EditorGUILayout.PropertyField(textName, true, null);

            EditorGUILayout.PropertyField(gridLayout, true, null);
            EditorGUILayout.PropertyField(dropdownItem, true, null);

            EditorGUILayout.EndVertical();

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
        }

    }
}
