using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace MagiCloud.KGUI
{
    [CustomEditor(typeof(KGUI_DropdownItem))]
    [CanEditMultipleObjects]
    public class KGUIDropdownItemEditor : Editor
    {
        KGUIButtonTypeEditor buttonType;

        private KGUI_DropdownItem dropdownItem;

        private void OnEnable()
        {
            dropdownItem = serializedObject.targetObject as KGUI_DropdownItem;

            if(buttonType==null)
                buttonType=new KGUIButtonTypeEditor();

            buttonType.OnInstantiation(serializedObject);
            
        }

        public override void OnInspectorGUI()
        {

            buttonType.OnInspectorButtonType(dropdownItem);
        }
    }
}
