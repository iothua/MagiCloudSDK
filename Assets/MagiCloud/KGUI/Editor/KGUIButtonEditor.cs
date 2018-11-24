using System.Collections;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

namespace MagiCloud.KGUI
{
    [CustomEditor(typeof(KGUI_Button))]
    [CanEditMultipleObjects]
    public class KGUIButtonEditor : Editor
    {

        public SerializedProperty onGroupReset;

        public SerializedProperty buttonGroup;

        public SerializedProperty AudioClip;

        private KGUI_Button button;

        KGUIButtonTypeEditor ButtonType;
        KGUIButtonEventEditor ButtonEvent;

        private void OnEnable()
        {
            button = serializedObject.targetObject as KGUI_Button;

            if (ButtonType == null)
                ButtonType = new KGUIButtonTypeEditor();

            if (ButtonEvent == null)
                ButtonEvent = new KGUIButtonEventEditor();

            ButtonType.OnInstantiation(serializedObject);
            ButtonEvent.OnInstantiation(serializedObject);

            onGroupReset = serializedObject.FindProperty("onGroupReset");

            buttonGroup = serializedObject.FindProperty("buttonGroup");

            AudioClip = serializedObject.FindProperty("audioClip");
        }

        public override void OnInspectorGUI()
        {

            ButtonType.OnInspectorButtonType(button);

            EditorGUILayout.BeginVertical(GUILayout.Width(500));

            GUILayout.Space(10);

            button.IsEnable = EditorGUILayout.Toggle("是否启用(IsEnable)", button.IsEnable);

            button.IsButtonGroup = EditorGUILayout.Toggle("是否归属Button组：", button.IsButtonGroup);

            if (button.IsButtonGroup)
            {
                EditorGUILayout.PropertyField(buttonGroup, true, null);
                button.IsShowButton = EditorGUILayout.Toggle("IsShowButton[是否默认按下]：", button.IsShowButton);
            }

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
            
            if (button.IsButtonGroup)
            {
                EditorGUILayout.PropertyField(onGroupReset, true, null);
            }

            ButtonEvent.OnInspectorButtonEvent();

            EditorGUILayout.EndVertical();

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}

