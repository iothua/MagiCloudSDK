using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace MagiCloud.KGUI
{
    /// <summary>
    /// 滚动视图
    /// </summary>
    [CustomEditor(typeof(KGUI_ScrollView))]
    [CanEditMultipleObjects]
    public class KGUIScrollViewEditor :Editor
    {
        public SerializedProperty vertical;
        public SerializedProperty horizontal;
        public SerializedProperty panel;
        public SerializedProperty content;
        public SerializedProperty isFollowHand;
        public SerializedProperty initYNum;
        public SerializedProperty initXNum;
        public SerializedProperty posCurrection;
        public SerializedProperty isFixedMouseSpeed;
        public SerializedProperty fixedMouseSpeed;
        private KGUI_ScrollView scrollView;

        private void OnEnable()
        {
            scrollView = serializedObject.targetObject as KGUI_ScrollView;

            vertical = serializedObject.FindProperty("vertical");
            horizontal = serializedObject.FindProperty("horizontal");
            panel = serializedObject.FindProperty("panel");
            content = serializedObject.FindProperty("content");
            isFollowHand=serializedObject.FindProperty("isFollowHand");
            initYNum=serializedObject.FindProperty("initNum");
            initXNum=serializedObject.FindProperty("initXNum");
            posCurrection=serializedObject.FindProperty("posCurrection");
            isFixedMouseSpeed=serializedObject.FindProperty("isFixedMouseSpeed");
            fixedMouseSpeed=serializedObject.FindProperty("fixedMouseSpeed");
        }

        public override void OnInspectorGUI()
        {
            GUILayout.BeginVertical("box",GUILayout.Width(500));

            EditorGUILayout.PropertyField(vertical,true,null);
            EditorGUILayout.PropertyField(horizontal,true,null);
            EditorGUILayout.PropertyField(panel,true,null);
            EditorGUILayout.PropertyField(content,true,null);
            EditorGUILayout.PropertyField(isFollowHand,true,null);
            EditorGUILayout.PropertyField(initYNum,true,null);
            EditorGUILayout.PropertyField(initXNum,true,null);
            EditorGUILayout.PropertyField(posCurrection,new GUIContent("自动修正坐标"),true);
            if (EditorGUILayout.PropertyField(isFixedMouseSpeed,new GUIContent("是否使用固定鼠标速度"),true)) ;
            if (scrollView.isFixedMouseSpeed)
            {
                EditorGUILayout.PropertyField(fixedMouseSpeed,new GUIContent("固定鼠标速度"),true);
            }
            GUILayout.Space(20);

            if (GUILayout.Button("刷新",GUILayout.Width(100),GUILayout.Height(21)))
            {
                scrollView.SetRectData();
            }

            GUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();
        }
    }
}
