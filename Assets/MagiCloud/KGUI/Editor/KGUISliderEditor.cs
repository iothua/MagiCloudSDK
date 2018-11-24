using System;
using UnityEngine;
using UnityEditor;

namespace MagiCloud.KGUI
{
    [CustomEditor(typeof(KGUI_Slider))]
    [CanEditMultipleObjects]
    public class KGUISliderEditor : Editor
    {
        //private GUIStyle style;
        //public SerializedProperty onEnter;  //鼠标移入
        //public SerializedProperty onExit;   //鼠标移出
        //public SerializedProperty onDown;   //鼠标按下

        //public SerializedProperty spriteRenderer;
        //public SerializedProperty image;

        //public SerializedProperty normalSprite, enterSprite, pressedSprite;
        //public SerializedProperty normalObject, enterObject, pressedObject;

        private KGUIButtonTypeEditor buttonType;

        private KGUI_Slider slider;

        public SerializedProperty sliderObject;
        public SerializedProperty rectMove;

        public SerializedProperty value;

        public SerializedProperty OnValueChanged;

        private void OnEnable()
        {
            slider = serializedObject.targetObject as KGUI_Slider;

            //onEnter = serializedObject.FindProperty("onEnter");
            //onExit = serializedObject.FindProperty("onExit");
            //onDown = serializedObject.FindProperty("onDown");

            //spriteRenderer = serializedObject.FindProperty("spriteRenderer");
            //image = serializedObject.FindProperty("image");

            //normalSprite = serializedObject.FindProperty("normalSprite");
            //enterSprite = serializedObject.FindProperty("enterSprite");
            //pressedSprite = serializedObject.FindProperty("pressedSprite");

            //normalObject = serializedObject.FindProperty("normalObject");
            //enterObject = serializedObject.FindProperty("enterObject");
            //pressedObject = serializedObject.FindProperty("pressedObject");
            if (buttonType == null)
                buttonType.OnInstantiation(serializedObject);

            sliderObject = serializedObject.FindProperty("sliderObject");

            rectMove = serializedObject.FindProperty("rectMove");

            value = serializedObject.FindProperty("Value");

            OnValueChanged = serializedObject.FindProperty("OnValueChanged");
        }

        private int test;

        public override void OnInspectorGUI()
        {
            buttonType.OnInspectorButtonType(slider);

            GUILayout.Space(10);

            EditorGUILayout.LabelField("滚动属性", MUtilityStyle.LabelStyle);

            slider.sliderType = (SliderType)EditorGUILayout.EnumPopup("滚动类型：", slider.sliderType);

            EditorGUILayout.PropertyField(sliderObject, true, null);

            slider.moveSpeed = EditorGUILayout.FloatField("移动速度：", slider.moveSpeed);

            GUILayout.Space(10);

            slider.KguiAxis = (Axis)EditorGUILayout.EnumPopup("移动轴：", slider.KguiAxis);

            switch (slider.KguiAxis)
            {
                case Axis.X:
                    EditorGUI.BeginChangeCheck();
                    slider.horizontal = (Horizontal)EditorGUILayout.EnumPopup("滚动方向：", slider.horizontal);

                    slider.minValue = EditorGUILayout.FloatField("左边界值：", slider.minValue);
                    slider.maxValue = EditorGUILayout.FloatField("右边界值：", slider.maxValue);

                    break;
                case Axis.Y:

                    EditorGUI.BeginChangeCheck();
                    slider.vertical = (Vertical)EditorGUILayout.EnumPopup("滚动方向：", slider.vertical);

                    slider.minValue = EditorGUILayout.FloatField("顶边界值：", slider.minValue);
                    slider.maxValue = EditorGUILayout.FloatField("底边界值：", slider.maxValue);

                    break;
                default:

                    EditorGUILayout.LabelField("暂不支持");

                    break;
            }

            switch (slider.sliderType)
            {
                case SliderType.None:
                    //EditorGUI.BeginChangeCheck();
                    break;
                case SliderType.Bar:
                    EditorGUI.BeginChangeCheck();

                    EditorGUILayout.LabelField("移动的区域", MUtilityStyle.LabelStyle);
                    EditorGUILayout.PropertyField(rectMove, true, null);

                    if (GUILayout.Button(new GUIContent("刷新", "根据当前属性设置，刷新滚动条位置"), GUILayout.Width(100), GUILayout.Height(21)))
                    {
                        slider.SetRectData();
                    }

                    break;
                default:
                    //EditorGUI.BeginChangeCheck();
                    break;
            }


            EditorGUILayout.PropertyField(value);

            EditorGUILayout.PropertyField(OnValueChanged);

            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();
        }
    }
}
