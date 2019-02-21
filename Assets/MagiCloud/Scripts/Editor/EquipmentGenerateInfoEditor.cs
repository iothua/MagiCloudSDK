using UnityEditor;
using UnityEngine;

namespace MagiCloud.Equipments
{
    [CustomEditor(typeof(EquipmentGenerateInfo))]
    public class EquipmentGenerateInfoEditor :Editor
    {
        public override void OnInspectorGUI()
        {
            EquipmentGenerateInfo equipment = target as EquipmentGenerateInfo;
            base.OnInspectorGUI();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("创建"))
            {
                equipment.OnCreate();
            }
            if (GUILayout.Button("获取物体数据"))
            {
                equipment.GetObjectData();
            }
            if (GUILayout.Button("设置物体数据"))
            {
                equipment.SetObjectData();
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}
