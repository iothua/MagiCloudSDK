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

        //public SerializedProperty onClick;  //鼠标点击

        //public SerializedProperty onEnter;  //鼠标移入
        //public SerializedProperty onExit;   //鼠标移出
        //public SerializedProperty onDown;   //鼠标按下
        //public SerializedProperty onUp;     //鼠标抬起

        //public SerializedProperty onDownStay; //按下持续

        //public SerializedProperty spriteRenderer;
        //public SerializedProperty image;

        //public SerializedProperty normalSprite, enterSprite, pressedSprite, disableSprite;
        //public SerializedProperty normalObject, enterObject, pressedObject, disableObject;

        //public SerializedProperty buttonType;

        KGUIButtonTypeEditor ButtonType;
        KGUIButtonEventEditor ButtonEvent;

        public SerializedProperty buttonGroup;

        public SerializedProperty AudioClip;

        private KGUI_ButtonCustom button;

        private void OnEnable()
        {
            button = serializedObject.targetObject as KGUI_ButtonCustom;

            //onClick = serializedObject.FindProperty("onClick");
            //onEnter = serializedObject.FindProperty("onEnter");
            //onExit = serializedObject.FindProperty("onExit");
            //onDown = serializedObject.FindProperty("onDown");
            //onUp = serializedObject.FindProperty("onUp");

            //onDownStay = serializedObject.FindProperty("onDownStay");

            //buttonType = serializedObject.FindProperty("buttonType");

            //spriteRenderer = serializedObject.FindProperty("spriteRenderer");
            //image = serializedObject.FindProperty("image");

            //normalSprite = serializedObject.FindProperty("normalSprite");
            //enterSprite = serializedObject.FindProperty("enterSprite");
            //pressedSprite = serializedObject.FindProperty("pressedSprite");
            //disableSprite = serializedObject.FindProperty("disableSprite");

            //normalObject = serializedObject.FindProperty("normalObject");
            //enterObject = serializedObject.FindProperty("enterObject");
            //pressedObject = serializedObject.FindProperty("pressedObject");
            //disableObject = serializedObject.FindProperty("disableObject");

            if (ButtonType == null)
                ButtonType = new KGUIButtonTypeEditor();
            if (ButtonEvent == null)
                ButtonEvent = new KGUIButtonEventEditor();

            ButtonType.OnInstantiation(serializedObject);
            ButtonEvent.OnInstantiation(serializedObject);

            AudioClip = serializedObject.FindProperty("audioClip");
        }

        public override void OnInspectorGUI()
        {
            ButtonType.OnInspectorButtonType(button);

            GUILayout.BeginVertical(GUILayout.Width(500));

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

            EditorGUILayout.EndVertical();

            ButtonEvent.OnInspectorButtonEvent();

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
