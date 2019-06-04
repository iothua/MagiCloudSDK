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

        KGUIButtonTypeEditor ButtonType;
        KGUIButtonEventEditor ButtonEvent;
        KGUIButtonAudioEditor ButtonAudio;

        public SerializedProperty buttonGroup;

        private KGUI_ButtonCustom button;

        private void OnEnable()
        {
            button = serializedObject.targetObject as KGUI_ButtonCustom;

            if (ButtonType == null)
                ButtonType = new KGUIButtonTypeEditor();
            if (ButtonEvent == null)
                ButtonEvent = new KGUIButtonEventEditor();

            if (ButtonAudio == null)
                ButtonAudio = new KGUIButtonAudioEditor();

            ButtonType.OnInstantiation(serializedObject);
            ButtonEvent.OnInstantiation(serializedObject);
            ButtonAudio.OnInstantiation(serializedObject);
        }

        public override void OnInspectorGUI()
        {
            ButtonType.OnInspectorButtonType(button);

            GUILayout.BeginVertical(GUILayout.Width(500));

            GUILayout.Space(10);

            button.IsEnable = EditorGUILayout.Toggle("是否启用(IsEnable)", button.IsEnable);

            EditorGUILayout.EndVertical();

            ButtonAudio.OnInspectorButtonAudio(button);

            ButtonEvent.OnInspectorButtonEvent();

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
