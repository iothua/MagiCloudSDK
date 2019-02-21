using UnityEditor;
using UnityEngine;

namespace MagiCloud
{
    public class ColliderDataEditor
    {
        public void OnGUI(ColliderData colliderData)
        {
            GUILayout.BeginVertical();

            colliderData.Center.Vector = EditorGUILayout.Vector3Field("Center：", colliderData.Center.Vector);
            colliderData.Size.Vector = EditorGUILayout.Vector3Field("Size：", colliderData.Size.Vector);

            GUILayout.EndVertical();
        }
    }

    public class TransformDataEditor
    {
        public void OnGUI(TransformData transformData)
        {
            GUILayout.BeginVertical();

            transformData.localPosition.Vector = EditorGUILayout.Vector3Field("Position：", transformData.localPosition.Vector);
            transformData.localRotation.Vector = EditorGUILayout.Vector3Field("Rotation：", transformData.localRotation.Vector);
            transformData.localScale.Vector = EditorGUILayout.Vector3Field("Scale：", transformData.localScale.Vector);

            GUILayout.EndVertical();
        }
    }
}
