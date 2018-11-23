#if UNITY_EDITOR
namespace Sirenix.OdinInspector.Demos
{
    using UnityEngine;
    using Sirenix.OdinInspector;

    public class EnumPagingExamples : MonoBehaviour
    {
        [EnumPaging]
        public SomeEnum SomeEnumField;
        
        public enum SomeEnum
        {
            A, B, C
        }

#if UNITY_EDITOR

        [EnumPaging, OnValueChanged("SetCurrentTool")]
        [InfoBox("Example of using EnumPaging together with OnValueChanged.")]
        public UnityEditor.Tool SceneTool;

        private void SetCurrentTool()
        {
            UnityEditor.Tools.current = this.SceneTool;
        }

#endif
    }
}
#endif
