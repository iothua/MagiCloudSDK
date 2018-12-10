using System;
using UnityEngine;
using Sirenix.OdinInspector;

namespace MagiCloud
{
    [Serializable]
    [LabelText("Transform数据")]
    public class TransformData
    {
        [LabelText("局部坐标(LocalPosition)")]
        public MVector3 localPosition;

        [LabelText("局部旋转(LocalRotation)")]
        public MVector3 localRotation;

        [LabelText("局部大小(LocalScale)")]
        public MVector3 localScale = new MVector3(Vector3.one);

        public TransformData()
        { }

        public TransformData(Transform transform,bool isLocal = true)
        { 
            if(isLocal)
            {
                localPosition.Vector = transform.localPosition;
                localRotation.Vector = transform.localRotation.eulerAngles;
                localScale.Vector = transform.localScale;
            }
            else
            {
                localPosition.Vector = transform.position;
                localRotation.Vector = transform.rotation.eulerAngles;
                localScale.Vector = transform.localScale;
            }
        }
    }
}
