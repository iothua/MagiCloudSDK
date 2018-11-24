using System;
using UnityEngine;
using UnityEditor;

namespace MagiCloud.KGUI
{
    [CustomEditor(typeof(KGUI_BackpackItem))]
    [CanEditMultipleObjects]
    public class KGUIBackpackItemEditor : Editor
    {

        KGUIButtonTypeEditor buttonType;
        KGUIButtonEventEditor buttonEvent;

        private KGUI_BackpackItem item;

        private void OnEnable()
        {
            item = serializedObject.targetObject as KGUI_BackpackItem;

            if(buttonType==null)
            {
                buttonType = new KGUIButtonTypeEditor();
            }
            if(buttonEvent==null)
            {
                buttonEvent = new KGUIButtonEventEditor();
            }


            buttonType.OnInstantiation(serializedObject);
            buttonEvent.OnInstantiation(serializedObject);
        }

        public override void OnInspectorGUI()
        {

            buttonType.OnInspectorButtonType(item);

            buttonEvent.OnInspectorButtonEvent();

            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();
        }
    }
}
