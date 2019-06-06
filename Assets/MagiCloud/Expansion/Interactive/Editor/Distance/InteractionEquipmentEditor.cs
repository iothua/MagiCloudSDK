using UnityEngine;
using UnityEditor;
using MagiCloud.Interactive.Distance;

namespace MagiCloud.Interactive
{
    [CustomEditor(typeof(InteractionEquipment))]
    [CanEditMultipleObjects]
    public class InteractionEquipmentEditor : DistanceInteractionEditor
    {
        public SerializedProperty Equipment;

        protected override void OnEnable()
        {
            base.OnEnable();

            Equipment = serializedObject.FindProperty("Equipment");
        }

        public override void OnInspectorGUI()
        {
            GUILayout.Space(10);

            EditorGUILayout.BeginVertical("box", GUILayout.Width(500));

            EditorGUILayout.PropertyField(Equipment, new GUIContent("仪器对象(Equipment)：", "该距离交互的仪器对象"), true, null);

            EditorGUILayout.EndVertical();

            base.OnInspectorGUI();
        }
    }
}
