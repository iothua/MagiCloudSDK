using MagiCloud.Core.MInput;
using System;
using UnityEngine;

namespace MagiCloud.Operate
{
    public interface IOperate
    {
        Action<IOperateObject,int> OnGrab { get; set; }
        Action<IOperateObject,int,float> OnSetGrab { get; set; }
        MInputHand InputHand { get; set; }
        Func<bool> RayExternaLimit { get; set; }
        UIOperate UIOperate { get; set; }
        IHandController HandController { get; set; }
        void OnEnable();
        void OnDisable();
        void SetObjectRelease();
        void SetObjectGrab(GameObject target,float z);
        GameObject GetObjectGrab();
    }
}

