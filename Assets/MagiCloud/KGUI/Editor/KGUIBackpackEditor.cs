using System;
using UnityEditor;
using UnityEngine;

namespace MagiCloud.KGUI
{
    /// <summary>
    /// 背包编辑器
    /// </summary>
    [CustomEditor(typeof(KGUI_Backpack))]
    [CanEditMultipleObjects]
    public class KGUIBackpackEditor : Editor
    {
        public SerializedProperty content;
        public SerializedProperty backpackItem;

        public SerializedProperty backpackIcons;

        public SerializedProperty areaPanel;

        public SerializedProperty backpackTrigger;

        private KGUI_Backpack backpack;

        private void OnEnable()
        {
            backpack = serializedObject.targetObject as KGUI_Backpack;

            content = serializedObject.FindProperty("content");
            backpackItem = serializedObject.FindProperty("backpackItem");
            areaPanel = serializedObject.FindProperty("areaPanel");
            backpackTrigger = serializedObject.FindProperty("backpackTrigger");

            backpackIcons = serializedObject.FindProperty("backpackIcons");
        }

        public override void OnInspectorGUI()
        {
            GUILayout.Space(10);

            GUILayout.BeginVertical("box", GUILayout.Width(500));

            backpack.backpackJsonFileNmae = EditorGUILayout.TextField(new GUIContent("背包配置文件名称：", "在路径StreamingAssets/Backpack/JsonData下根据名称，将该文件导入成背包数据"),
                backpack.backpackJsonFileNmae);

            GUILayout.Space(5);
            EditorGUILayout.PropertyField(backpackIcons, new GUIContent("背包仪器图标图集(backpackIcons)："), true);

            GUILayout.Space(5);
            backpack.universalPath = EditorGUILayout.TextField(new GUIContent("仪器通用物体路径：", "位于Resources下"), backpack.universalPath);

            GUILayout.Space(5);
            EditorGUILayout.PropertyField(content, new GUIContent("生成背包子项的父对象(Content)："), true);

            GUILayout.Space(5);
            EditorGUILayout.PropertyField(backpackItem, new GUIContent("背包子项预制物体(backpackItem)：", "系统会自动查看，可不用手动配置，但是可根据自身情况手动配置"), true);

            GUILayout.Space(5);
            EditorGUILayout.PropertyField(areaPanel, new GUIContent("背包移动区域(areaPanel)：", "背包移动额区域，用于判断生成的物体是否离开或加入到区域内"), true);

            GUILayout.Space(5);
            EditorGUILayout.PropertyField(backpackTrigger, new GUIContent("背包触发按钮(backpackTrigger)：", "用于触发背包区域"), true);

            GUILayout.Space(5);
            backpack.openPosition = EditorGUILayout.Vector2Field(new GUIContent("打开坐标(openPosition)", "背包打开时，背包区域的坐标值"),
                backpack.openPosition);

            GUILayout.Space(5);
            backpack.closePosition = EditorGUILayout.Vector2Field(new GUIContent("关闭坐标(closePosition)", "背包关闭时，背包区域的坐标值"),
                backpack.closePosition);

            GUILayout.Space(5);
            backpack.IsOpen = EditorGUILayout.Toggle("背包状态(IsOpen)：", backpack.IsOpen);

            GUILayout.Space(5);
            backpack.AutoInitialize = EditorGUILayout.Toggle("是否自动初始化(AutoInitialize)：", backpack.AutoInitialize);

            GUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();
        }
    }
}
