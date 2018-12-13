using UnityEngine;
using UnityEditor;
using MagiCloud.Interactive.Distance;

namespace Chemistry.Interactions
{
    [CustomEditor(typeof(InteractionPourWater))]
    [CanEditMultipleObjects]
    public class InteractionPourWaterEditor : DistanceInteractionEditor
    {
        private SerializedProperty PourPointSide;
        private InteractionPourWater interactionPourWater;

        protected override void OnEnable()
        {
            interactionPourWater = serializedObject.targetObject as InteractionPourWater;
            PourPointSide = serializedObject.FindProperty("pointSide");

            base.OnEnable();
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.BeginVertical("box", GUILayout.Width(500));

            interactionPourWater.pointSide = (PourPointSide)EditorGUILayout.EnumPopup("选择左边右边倒接水点：", interactionPourWater.pointSide);

            EditorGUILayout.EndVertical();

            base.OnInspectorGUI();
        }
    }
}
