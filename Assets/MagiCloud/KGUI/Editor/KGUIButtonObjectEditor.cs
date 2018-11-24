using System.Collections;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

namespace MagiCloud.KGUI
{
    [CustomEditor(typeof(KGUI_ButtonObject))]
    [CanEditMultipleObjects]
    public class KGUIButtonObjectEditor : Editor
    {
        public SerializedProperty onGroupReset;

        public SerializedProperty AudioClip;

        public KGUI_ButtonObject button;

        public SerializedProperty BindObject;
        public SerializedProperty Panel;

        KGUIButtonTypeEditor buttonType;
        KGUIButtonEventEditor buttonEvent;


        private void OnEnable()
        {

            button = serializedObject.targetObject as KGUI_ButtonObject;

            if (buttonType == null)
                buttonType = new KGUIButtonTypeEditor();
            if (buttonEvent == null)
                buttonEvent = new KGUIButtonEventEditor();

            onGroupReset = serializedObject.FindProperty("onGroupReset");

            AudioClip = serializedObject.FindProperty("audioClip");

            BindObject = serializedObject.FindProperty("bindObject");
            Panel = serializedObject.FindProperty("panel");
        }

        public override void OnInspectorGUI()
        {
            buttonType.OnInspectorButtonType(button);

            EditorGUILayout.BeginVertical(GUILayout.Width(500));

            GUILayout.Space(10);

            GUILayout.Space(10);

            button.IsEnable = EditorGUILayout.Toggle("是否启用(IsEnable)", button.IsEnable);

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

            button.zValue = EditorGUILayout.FloatField("相对摄像机Z轴值：", button.zValue);
            button.maxCount = EditorGUILayout.IntField("最大数：", button.maxCount);
            EditorGUILayout.PropertyField(BindObject, true, null);
            EditorGUILayout.PropertyField(Panel, true, null);


            EditorGUILayout.EndVertical();

            buttonEvent.OnInspectorButtonEvent();


            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();
        }
    }
}
