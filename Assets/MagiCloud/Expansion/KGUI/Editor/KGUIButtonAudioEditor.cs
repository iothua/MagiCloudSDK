using UnityEditor;
using UnityEngine;

namespace MagiCloud.KGUI
{
    public class KGUIButtonAudioEditor
    {
        public SerializedProperty AudioClip;

        public void OnInstantiation(SerializedObject serializedObject)
        {
            AudioClip = serializedObject.FindProperty("audioClip");
        }

        public void OnInspectorButtonAudio(KGUI_ButtonBase button)
        {

            EditorGUILayout.BeginVertical(GUILayout.Width(500));

            button.IsStartAudio = EditorGUILayout.Toggle("启动音频：", button.IsStartAudio);

            if (button.IsStartAudio)
            {
                EditorGUILayout.PropertyField(AudioClip, true, null);
                button.audioStatus = (AudioStatus)EditorGUILayout.EnumPopup("声音触发类型：", button.audioStatus);
                button.AddAudio();
            }
            else
            {
                button.DestroyAudio();
            }

            EditorGUILayout.EndVertical();
        }
    }
}
