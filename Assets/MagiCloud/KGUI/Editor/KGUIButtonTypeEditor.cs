using UnityEngine;
using UnityEditor;

namespace MagiCloud.KGUI
{

    /// <summary>
    /// 继承ButtonBase的交互类型布局
    /// </summary>
    public class KGUIButtonTypeEditor :Editor
    {

        public SerializedProperty spriteRenderer;
        public SerializedProperty image;

        public SerializedProperty normalSprite, enterSprite, pressedSprite, disableSprite;
        public SerializedProperty normalObject, enterObject, pressedObject, disableObject;

        public SerializedProperty buttonType;

        public SerializedProperty onClick;  //鼠标点击

        public SerializedProperty onEnter;  //鼠标移入
        public SerializedProperty onExit;   //鼠标移出
        public SerializedProperty onDown;   //鼠标按下
        public SerializedProperty onUp;     //鼠标抬起

        public SerializedProperty onDownStay; //按下持续
        public SerializedProperty onUpRange;

        public void OnInstantiation()
        {
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

            onClick = serializedObject.FindProperty("onClick");
            onEnter = serializedObject.FindProperty("onEnter");
            onExit = serializedObject.FindProperty("onExit");
            onDown = serializedObject.FindProperty("onDown");
            onUp = serializedObject.FindProperty("onUp");
            onDownStay = serializedObject.FindProperty("onDownStay");
            onUpRange = serializedObject.FindProperty("onUpRange");

        }

        /// <summary>
        /// button类型布局
        /// </summary>
        public void OnInspectorButtonType(KGUI_ButtonBase button)
        {
            EditorGUILayout.BeginVertical(GUILayout.Width(500));

            GUILayout.Space(10);
            EditorGUILayout.LabelField("常用属性", MUtilityStyle.LabelStyle);

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

                    EditorGUILayout.PropertyField(pressedSprite, true, null);

                    if (button.disableSprite == null)
                    {
                        button.disableSprite = button.normalSprite;
                    }

                    EditorGUILayout.PropertyField(disableSprite, true, null);

                    break;
                case ButtonType.Object:
                    EditorGUI.BeginChangeCheck();

                    EditorGUILayout.PropertyField(normalObject, true, null);
                    EditorGUILayout.PropertyField(enterObject, true, null);

                    if (button.pressedObject == null)
                    {
                        button.pressedObject = button.enterObject;
                    }

                    EditorGUILayout.PropertyField(pressedObject, true, null);

                    if (button.disableObject == null)
                    {
                        button.disableObject = button.normalObject;
                    }

                    EditorGUILayout.PropertyField(disableObject, true, null);

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

                    EditorGUILayout.PropertyField(pressedSprite, true, null);

                    if (button.disableSprite == null)
                    {
                        button.disableSprite = button.normalSprite;
                    }

                    EditorGUILayout.PropertyField(disableSprite, true, null);
                    break;
                default:
                    EditorGUI.BeginChangeCheck();
                    break;
            }


            EditorGUILayout.EndVertical();


        }

        /// <summary>
        /// 初始化事件
        /// </summary>
        public void OnInspectorButtonEvent()
        {
            //初始化事件相关的属性
            EditorGUILayout.BeginVertical();

            EditorGUILayout.LabelField("Button事件", MUtilityStyle.LabelStyle);

            EditorGUILayout.PropertyField(onClick, true, null);

            EditorGUILayout.PropertyField(onEnter, true, null);
            EditorGUILayout.PropertyField(onExit, true, null);

            EditorGUILayout.PropertyField(onDown, true, null);
            EditorGUILayout.PropertyField(onUp, true, null);

            EditorGUILayout.PropertyField(onDownStay, true, null);
            EditorGUILayout.PropertyField(onUpRange, true, null);

            EditorGUILayout.EndVertical();
        }
    }
}
