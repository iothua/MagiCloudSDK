using MagiCloud.Core.MInput;
using System;
using UnityEngine;

namespace MagiCloud.Operate
{
    public class OperateBase :IOperate
    {
        public OperateBase(MInputHand inputHand,Func<bool> func,IHandController handController) { }
        public virtual Action<IOperateObject,int> OnGrab { get; set; }
        public virtual Action<IOperateObject,int,float> OnSetGrab { get; set; }
        public virtual MInputHand InputHand { get; set; }
        public virtual Func<bool> RayExternaLimit { get; set; }
        public virtual UIOperate UIOperate { get; set; }
        public virtual IHandController HandController { get; set; }

        public virtual GameObject GetObjectGrab() { return null; }
        public virtual void OnDisable() { }
        public virtual void OnEnable() { }
        public virtual void SetObjectGrab(GameObject target,float z) { }
        public virtual void SetObjectRelease() { }
    }
}

