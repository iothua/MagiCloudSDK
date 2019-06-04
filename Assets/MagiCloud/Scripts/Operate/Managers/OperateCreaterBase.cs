using UnityEngine;
using System;
using MagiCloud.Core.MInput;
using MagiCloud.Operate;

namespace MagiCloud
{
    public abstract class OperateCreaterBase :MonoBehaviour, IOperateCreater
    {
        public abstract IOperate Creat(MInputHand inputHand,IHandController handController,Func<bool> func = null);
    } 
}
