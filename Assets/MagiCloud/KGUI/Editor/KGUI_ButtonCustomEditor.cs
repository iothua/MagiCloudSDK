using System;
using UnityEditor;
using UnityEngine;

namespace MagiCloud.KGUI
{
    /// <summary>
    /// 按钮的自定义触发
    /// </summary>
    [CustomEditor(typeof(KGUI_ButtonCustom))]
    [CanEditMultipleObjects]
    public class KGUI_ButtonCustomEditor : Editor
    {
        private GUIStyle style;

        public SerializedProperty onClick;  //鼠标点击

        public SerializedProperty onEnter;  //鼠标移入
        public SerializedProperty onExit;   //鼠标移出
        public SerializedProperty onDown;   //鼠标按下
        public SerializedProperty onUp;     //鼠标抬起

        public SerializedProperty onDownStay; //按下持续

        public SerializedProperty spriteRenderer;
        public SerializedProperty image;

        public SerializedProperty normalSprite, enterSprite, pressedSprite, disableSprite;
        public SerializedProperty normalObject, enterObject, pressedObject, disableObject;

        public SerializedProperty buttonType;

        public SerializedProperty buttonGroup;

        public SerializedProperty AudioClip;

        private KGUI_ButtonCustom button;

        private void OnEnable()
        {
            button = serializedObject.targetObject as KGUI_ButtonCustom;

            onClick = serializedObject.FindProperty("onClick");
            onEnter = serializedObject.FindProperty("onEnter");
            onExit = serializedObject.FindProperty("onExit");
            onDown = serializedObject.FindProperty("onDown");
            onUp = serializedObject.FindProperty("onUp");

            onDownStay = serializedObject.FindProperty("onDownStay");

            buttonType = serializedObject.FindProperty("buttonType");

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

            AudioClip = serializedObject.FindProperty("audioClip");
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

            GUILayout.BeginVertical(GUILayout.Width(500));

            GUILayout.Space(10);

            GUILayout.Label("说明：ButtonCustom一切的触发，执行OnEnter与正常Button一致，但是想执行OnExit需要手动去调用OnCustomExit()，其他事件与正常Button一致");

            EditorGUILayout.LabelField("常用属性", style);

            button.buttonType = (ButtonType)EditorGUILayout.EnumPopup("Button类型：", button.buttonType);

            switch (button.buttonType)
            {
                case ButtonType.Image:

                    EditorGUI.BeginChangeCheck();

                    EditorGUILayout.PropertyField(image, true, null);
                    EditorGUILayout.PropertyField(normalSprite, true, null);
                    EditorGUILayout.PropertyField(enterSprite, true, null);

                    if (button.pressedSprite == null)
                    {
                        button.pressedSprite = button.enterSprite;
                    }

                    if (button.disableSprite == null)
                    {
                        button.disableSprite = button.normalSprite;
                    }

                    break;
                case ButtonType.Object:
                    EditorGUI.BeginChangeCheck();

                    EditorGUILayout.PropertyField(normalObject, true, null);
                    EditorGUILayout.PropertyField(enterObject, true, null);

                    if (button.pressedObject == null)
                    {
                        button.pressedObject = button.enterObject;
                    }

                    if (button.disableObject == null)
                    {
                        button.disableObject = button.normalObject;
                    }

                    break;
                case ButtonType.SpriteRenderer:
                    EditorGUI.BeginChangeCheck();

                    EditorGUILayout.PropertyField(spriteRenderer, true, null);

                    EditorGUILayout.PropertyField(normalSprite, true, null);
                    EditorGUILayout.PropertyField(enterSprite, true, null);

                    if (button.pressedSprite == null)
                    {
                        button.pressedSprite = button.enterSprite;
                    }

                    if (button.disableSprite == null)
                    {
                        button.disableSprite = button.normalSprite;
                    }

                    break;
                default:
                    EditorGUI.BeginChangeCheck();
                    break;
            }

            GUILayout.Space(10);

            button.IsEnable = EditorGUILayout.Toggle("是否启用(IsEnable)", button.IsEnable);
            button.IsStartAudio = EditorGUILayout.Toggle("启动音频：", button.IsStartAudio);

            if (button.IsStartAudio)
            {
                EditorGUILayout.PropertyField(AudioClip, true, null);
                button.AddAudio();
            }
            else
            {
                button.DestroyAudio();
            }

            GUILayout.Space(20);
            EditorGUILayout.LabelField("Button事件", style);
            EditorGUILayout.PropertyField(onClick, true, null);
            EditorGUILayout.PropertyField(onEnter, true, null);
            EditorGUILayout.PropertyField(onExit, true, null);

            EditorGUILayout.PropertyField(onDown, true, null);
            EditorGUILayout.PropertyField(onUp, true, null);

            EditorGUILayout.PropertyField(onDownStay, true, null);

            EditorGUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();
        }
    }
}
