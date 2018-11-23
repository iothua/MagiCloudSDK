using System;
using UnityEngine;
using UnityEditor;

namespace MagiCloud.KGUI
{
    [CustomEditor(typeof(KGUI_BackpackItem))]
    [CanEditMultipleObjects]
    public class KGUIBackpackItemEditor : Editor
    {

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

        private KGUI_BackpackItem item;

        private void OnEnable()
        {
            item = serializedObject.targetObject as KGUI_BackpackItem;

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
        }

        public override void OnInspectorGUI()
        {

            EditorGUILayout.BeginVertical(GUILayout.Width(500));
            GUILayout.Space(10);


            item.buttonType = (ButtonType)EditorGUILayout.EnumPopup("Button类型：", item.buttonType);

            switch (item.buttonType)
            {
                case ButtonType.Image:

                    EditorGUI.BeginChangeCheck();

                    EditorGUILayout.PropertyField(image, true, null);
                    EditorGUILayout.PropertyField(normalSprite, true, null);
                    EditorGUILayout.PropertyField(enterSprite, true, null);

                    if (item.pressedSprite == null)
                    {
                        item.pressedSprite = item.enterSprite;
                    }

                    EditorGUILayout.PropertyField(pressedSprite, true, null);

                    if (item.disableSprite == null)
                    {
                        item.disableSprite = item.normalSprite;
                    }

                    EditorGUILayout.PropertyField(disableSprite, true, null);

                    break;
                case ButtonType.Object:
                    EditorGUI.BeginChangeCheck();

                    EditorGUILayout.PropertyField(normalObject, true, null);
                    EditorGUILayout.PropertyField(enterObject, true, null);

                    if (item.pressedObject == null)
                    {
                        item.pressedObject = item.enterObject;
                    }

                    EditorGUILayout.PropertyField(pressedObject, true, null);

                    if (item.disableObject == null)
                    {
                        item.disableObject = item.normalObject;
                    }

                    EditorGUILayout.PropertyField(disableObject, true, null);

                    break;
                case ButtonType.SpriteRenderer:
                    EditorGUI.BeginChangeCheck();

                    EditorGUILayout.PropertyField(spriteRenderer, true, null);

                    EditorGUILayout.PropertyField(normalSprite, true, null);
                    EditorGUILayout.PropertyField(enterSprite, true, null);

                    if (item.pressedSprite == null)
                    {
                        item.pressedSprite = item.enterSprite;
                    }

                    EditorGUILayout.PropertyField(pressedSprite, true, null);

                    if (item.disableSprite == null)
                    {
                        item.disableSprite = item.normalSprite;
                    }

                    EditorGUILayout.PropertyField(disableSprite, true, null);
                    break;
                default:
                    EditorGUI.BeginChangeCheck();
                    break;
            }

            GUILayout.Space(20);
            EditorGUILayout.PropertyField(onClick, true, null);
            EditorGUILayout.PropertyField(onEnter, true, null);
            EditorGUILayout.PropertyField(onExit, true, null);

            EditorGUILayout.PropertyField(onDown, true, null);
            EditorGUILayout.PropertyField(onUp, true, null);

            EditorGUILayout.EndVertical();
            serializedObject.ApplyModifiedProperties();
        }
    }
}
