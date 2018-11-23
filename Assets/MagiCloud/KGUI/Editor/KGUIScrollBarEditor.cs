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
        public SerializedProperty onEnter;  //鼠标移入
        public SerializedProperty onExit;   //鼠标移出
        public SerializedProperty onDown;   //鼠标按下

        public SerializedProperty spriteRenderer;
        public SerializedProperty image;

        public SerializedProperty normalSprite, enterSprite, pressedSprite, disableSprite;
        public SerializedProperty normalObject, enterObject, pressedObject, disableObject;

        public SerializedProperty handleRect;

        //public SerializedProperty Value;

        private KGUI_ScrollBar scrollBar;

        public SerializedProperty OnValueChanged;
        public SerializedProperty OnRelease;

        private void OnEnable()
        {
            scrollBar = serializedObject.targetObject as KGUI_ScrollBar;

            onEnter = serializedObject.FindProperty("onEnter");
            onExit = serializedObject.FindProperty("onExit");
            onDown = serializedObject.FindProperty("onDown");

            spriteRenderer = serializedObject.FindProperty("spriteRenderer");
            image = serializedObject.FindProperty("image");

            normalSprite = serializedObject.FindProperty("normalSprite");
            enterSprite = serializedObject.FindProperty("enterSprite");
            pressedSprite = serializedObject.FindProperty("pressedSprite");
            disableSprite = serializedObject.FindProperty("disableSprite");

            normalObject = serializedObject.FindProperty("normalObject");
            enterObject = serializedObject.FindProperty("enterObject");
            pressedObject = serializedObject.FindProperty("pressedObject");
            disableObject = serializedObject.FindProperty("disableObject");

            handleRect = serializedObject.FindProperty("handleRect");

            OnValueChanged = serializedObject.FindProperty("OnValueChanged");
            OnRelease = serializedObject.FindProperty("OnRelease");

        }

        public override void OnInspectorGUI()
        {
            GUILayout.BeginVertical(GUILayout.Width(500));

            GUILayout.Space(10);

            EditorGUILayout.LabelField("交互属性：");

            scrollBar.buttonType = (ButtonType)EditorGUILayout.EnumPopup("滚动交互类型：", scrollBar.buttonType);

            switch (scrollBar.buttonType)
            {
                case ButtonType.Image:

                    EditorGUI.BeginChangeCheck();

                    EditorGUILayout.PropertyField(image, true, null);
                    EditorGUILayout.PropertyField(normalSprite, true, null);
                    EditorGUILayout.PropertyField(enterSprite, true, null);
                    EditorGUILayout.PropertyField(pressedSprite, true, null);
                    EditorGUILayout.PropertyField(disableSprite, true, null);

                    break;
                case ButtonType.Object:
                    EditorGUI.BeginChangeCheck();

                    EditorGUILayout.PropertyField(normalObject, true, null);
                    EditorGUILayout.PropertyField(enterObject, true, null);
                    EditorGUILayout.PropertyField(pressedObject, true, null);
                    EditorGUILayout.PropertyField(disableObject, true, null);

                    break;
                case ButtonType.SpriteRenderer:
                    EditorGUI.BeginChangeCheck();

                    EditorGUILayout.PropertyField(spriteRenderer, true, null);
                    EditorGUILayout.PropertyField(normalSprite, true, null);
                    EditorGUILayout.PropertyField(enterSprite, true, null);
                    EditorGUILayout.PropertyField(pressedSprite, true, null);
                    EditorGUILayout.PropertyField(disableSprite, true, null);

                    break;
                default:
                    break;

            }

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
