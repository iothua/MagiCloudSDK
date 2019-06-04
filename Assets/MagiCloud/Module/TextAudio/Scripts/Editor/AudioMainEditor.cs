using MagiCloud.TextAudio;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace MagiCloud.TextToAudio
{
    [CustomEditor(typeof(AudioMain),true)]
    public class AudioMainEditor :Editor
    {
        SerializedObject obj;
        SerializedProperty speeker;
        SerializedProperty speedType;
        private void OnEnable()
        {
            obj=new SerializedObject(target);
            speeker=obj.FindProperty("speeker");
            speedType=obj.FindProperty("speedType");
        }

        public override void OnInspectorGUI()
        {
            obj.Update();
            EditorGUI.BeginChangeCheck();
            var main = target as AudioMain;
            EditorGUILayout.PropertyField(speeker,new GUIContent("选择发音人"));
            EditorGUILayout.PropertyField(speedType,new GUIContent("语速"));
            if (EditorGUI.EndChangeCheck())
            {
                //  main.SetSpeeker(speedType.enumValueIndex);
                // main.SetSpeed(speedType.enumValueIndex);
                obj.ApplyModifiedProperties();
            }
        }
    }
}