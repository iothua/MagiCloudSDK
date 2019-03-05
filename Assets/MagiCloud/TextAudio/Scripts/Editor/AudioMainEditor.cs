using MagiCloud.TextAudio;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace MagiCloud.TextToAudio
{
    [CustomEditor(typeof(AudioMain),true)]
    public class AudioMainEditor :Editor
    {
        private void OnEnable()
        {
        }
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var main = target as AudioMain;

            main.Speeker= (SpeekerType)EditorGUILayout.EnumPopup("选择发音人",main.Speeker);
            main.Speed= (SpeedType)EditorGUILayout.EnumPopup("语速",main.Speed);
        }
    }
}