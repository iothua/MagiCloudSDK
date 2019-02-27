using UnityEngine;
using UnityEditor;
using System.Collections;

namespace HighlightingSystem
{
    [CustomEditor(typeof(HighlightingRenderer), true)]
    public class HighlightingRendererEditor : HighlightingBaseEditor
    {
        // 
        public override void OnInspectorGUI()
        {
            RendererGUI();
        }
    }
}