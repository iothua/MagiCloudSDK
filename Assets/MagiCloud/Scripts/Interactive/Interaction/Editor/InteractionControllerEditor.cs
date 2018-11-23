using System.Collections;
using UnityEngine;
using UnityEditor;
using MagiCloud.Interactive.Actions;

namespace MagiCloud.Interactive
{
    [CustomEditor(typeof(InteractionController))]
    [CanEditMultipleObjects]
    public class InteractionControllerEditor : Editor
    {
        private InteractionController interactionController;
        private Interaction_AddParent addParent;
        private Interaction_Shadow shadow;

        private void OnEnable()
        {
            interactionController = serializedObject.targetObject as InteractionController;
        }

        public override void OnInspectorGUI()
        {
            GUILayout.Space(10);

            EditorGUILayout.BeginVertical("box", GUILayout.Width(500));

            //高亮面板
            EditorGUILayout.BeginVertical("box");

            interactionController.StartShadow = GUILayout.Toggle(interactionController.StartShadow, "  激活【距离虚影】--------------------------------------------------------------");
            InspectorShadow();

            EditorGUILayout.EndVertical();

            //高亮面板
            EditorGUILayout.BeginVertical("box");

            interactionController.StartAddParent = GUILayout.Toggle(interactionController.StartAddParent, "  激活【加入父物体】--------------------------------------------------------------");
            InspectorAddParent();

            EditorGUILayout.EndVertical();

            EditorGUILayout.EndVertical();

            //serializedObject.ApplyModifiedProperties();
        }

        void InspectorShadow()
        {
            if (interactionController.StartShadow)
            {
                shadow = interactionController.AddShadow();
                if (shadow == null) return;

                shadow.Interaction = EditorGUILayout.ObjectField(new GUIContent("距离对象："), shadow.Interaction, 
                    typeof(Distance.DistanceInteraction), true) as Distance.DistanceInteraction;

                shadow.IsSelf = EditorGUILayout.Toggle(new GUIContent("本身：", "当进行交互时，交互是否这些本身，True为执行本身，对方不执行。反之同理!"), shadow.IsSelf);

                shadow.localPosition = EditorGUILayout.Vector3Field("局部坐标：", shadow.localPosition);
                shadow.localRotation = EditorGUILayout.Vector3Field("局部旋转值：", shadow.localRotation);
                shadow.localScale = EditorGUILayout.Vector3Field("局部大小：", shadow.localScale);
            }
            else
            {
                interactionController.RemoveShadow();
                shadow = null;
            }
        }

        void InspectorAddParent()
        {
            if (interactionController.StartAddParent)
            {
                addParent = interactionController.AddParent();

                if (addParent == null) return;

                addParent.InteractionSelf = EditorGUILayout.ObjectField(new GUIContent("距离对象："), addParent.InteractionSelf,
                    typeof(Distance.DistanceInteraction), true) as Distance.DistanceInteraction;

                addParent.IsSelf = EditorGUILayout.Toggle(new GUIContent("本身：", "当进行交互时，交互是否这些本身，True为执行本身，对方不执行。反之同理!"), addParent.IsSelf);

                addParent.Parent = EditorGUILayout.ObjectField(new GUIContent("父对象", "需要加入子父物体的父对象"), addParent.Parent, typeof(Transform), true) as Transform;
                addParent.localPosition = EditorGUILayout.Vector3Field("局部坐标：", addParent.localPosition);
                addParent.localRotation = EditorGUILayout.Vector3Field("局部旋转值：", addParent.localRotation);
            }
            else
            {
                interactionController.RemoveParent();
                addParent = null;
            }
        }
    }
}

