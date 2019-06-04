using System;
using UnityEditor;
using UnityEngine;

namespace MagiCloud.KGUI
{
    [CanEditMultipleObjects]
    public class KGUIButtonEventEditor
    {
        public SerializedProperty onClick;  //鼠标点击

        public SerializedProperty onEnter;  //鼠标移入
        public SerializedProperty onExit;   //鼠标移出
        public SerializedProperty onDown;   //鼠标按下
        public SerializedProperty onUp;     //鼠标抬起

        public SerializedProperty onDownStay; //按下持续
        public SerializedProperty onUpRange;

        public void OnInstantiation(SerializedObject serializedObject)
        {
            onClick = serializedObject.FindProperty("onClick");
            onEnter = serializedObject.FindProperty("onEnter");
            onExit = serializedObject.FindProperty("onExit");
            onDown = serializedObject.FindProperty("onDown");
            onUp = serializedObject.FindProperty("onUp");
            onDownStay = serializedObject.FindProperty("onDownStay");
            onUpRange = serializedObject.FindProperty("onUpRange");
        }

        public void OnInspectorButtonEvent()
        {
            EditorGUILayout.BeginVertical();
            GUILayout.Space(10);

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
