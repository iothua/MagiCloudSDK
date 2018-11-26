using System;
using UnityEngine;
using UnityEditor;

namespace MagiCloud.KGUI
{
    [CustomEditor(typeof(KGUI_Toggle))]
    [CanEditMultipleObjects]
    public class KGUIToggleEditor : Editor
    {
        public SerializedProperty spriteRenderer;
        public SerializedProperty image;

        public SerializedProperty onNormalSprite, offNormalSprite;
        public SerializedProperty onEnterSprite, offEnterSprite;

        public SerializedProperty onNormalObject, offNormalObject;
        public SerializedProperty onEnterObject, offEnterObject;

        public SerializedProperty OnValueChanged;  //鼠标点击

        public KGUIButtonAudioEditor buttonAudio;

        public KGUI_Toggle toggle;

        private void OnEnable()
        {
            toggle = serializedObject.targetObject as KGUI_Toggle;

            if (buttonAudio == null)
                buttonAudio = new KGUIButtonAudioEditor();

            buttonAudio.OnInstantiation(serializedObject);

            OnValueChanged = serializedObject.FindProperty("OnValueChanged");

            spriteRenderer = serializedObject.FindProperty("spriteRenderer");
            image = serializedObject.FindProperty("image");

            onNormalSprite = serializedObject.FindProperty("onNormalSprite");
            offNormalSprite = serializedObject.FindProperty("offNormalSprite");

            onEnterSprite = serializedObject.FindProperty("onEnterSprite");
            offEnterSprite = serializedObject.FindProperty("offEnterSprite");

            onNormalObject = serializedObject.FindProperty("onNormalObject");
            offNormalObject = serializedObject.FindProperty("offNormalObject");

            onEnterObject = serializedObject.FindProperty("onEnterObject");
            offEnterObject = serializedObject.FindProperty("offEnterObject");

        }

        public override void OnInspectorGUI()
        {
            GUILayout.Space(10);

            EditorGUILayout.BeginVertical(GUILayout.Width(500));

            toggle.buttonType = (ButtonType)EditorGUILayout.EnumPopup("交互类型：", toggle.buttonType);

            switch (toggle.buttonType)
            {
                case ButtonType.Image:

                    EditorGUI.BeginChangeCheck();

                    EditorGUILayout.PropertyField(image, new GUIContent("Image对象(Image)："), true, null);
                    EditorGUILayout.PropertyField(onNormalSprite, new GUIContent("默认开纹理(onNormalSprite)：", "Toggle默认开时的状态"), true, null);
                    EditorGUILayout.PropertyField(offNormalSprite, new GUIContent("默认关纹理(offNormalSprite)：", "Toggle默认关的状态"), true, null);

                    EditorGUILayout.PropertyField(onEnterSprite, new GUIContent("开时，移入纹理(onEnterSprite)：", "如果此时状态为开，当移入时，开的一种交互状态"), true, null);
                    EditorGUILayout.PropertyField(offEnterSprite, new GUIContent("关时，移入纹理(offEnterSprite)：", "如果此时状态为关，当移入时，关的一种交互状态"), true, null);

                    break;
                case ButtonType.Object:

                    EditorGUI.BeginChangeCheck();

                    EditorGUILayout.PropertyField(onNormalObject, new GUIContent("默认开物体对象(onNormalObject)：", "Toggle默认开时的状态"), true, null);
                    EditorGUILayout.PropertyField(offNormalObject, new GUIContent("默认关物体对象(offNormalObject)：", "Toggle默认关时的状态"), true, null);

                    EditorGUILayout.PropertyField(onEnterObject, new GUIContent("开时，移入物体对象(onEnterObject)：", "如果此时状态为开，当移入时，开的一种交互状态"), true, null);
                    EditorGUILayout.PropertyField(offEnterObject, new GUIContent("关时，移入物体对象(offEnterObject)：", "如果此时状态为关，当移入时，关的一种交互状态"), true, null);

                    break;
                case ButtonType.SpriteRenderer:

                    EditorGUI.BeginChangeCheck();

                    EditorGUILayout.PropertyField(spriteRenderer, new GUIContent("SpriteRenderer对象(SpriteRenderer)："), true, null);

                    EditorGUILayout.PropertyField(onNormalSprite, new GUIContent("默认开纹理(onNormalSprite)：", "Toggle默认开的状态"), true, null);
                    EditorGUILayout.PropertyField(offNormalSprite, new GUIContent("默认关纹理(offNormalSprite)：", "Toggle默认关的状态"), true, null);

                    EditorGUILayout.PropertyField(onEnterSprite, new GUIContent("开时，移入纹理(onEnterSprite)：", "如果此时状态为开，当移入时，开的一种交互状态"), true, null);
                    EditorGUILayout.PropertyField(offEnterSprite, new GUIContent("关时，移入纹理(offEnterSprite)：", "如果此时状态为关，当移入时，关的一种交互状态"), true, null);

                    break;
                default:
                    EditorGUI.BeginChangeCheck();
                    break;
            }

            buttonAudio.OnInspectorButtonAudio(toggle);

            GUILayout.Space(10);
            toggle.IsValue = EditorGUILayout.Toggle("IsValue：", toggle.IsValue);

            EditorGUILayout.PropertyField(OnValueChanged, true, null);

            GUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();
        }
    }
}
