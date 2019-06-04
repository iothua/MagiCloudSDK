using System;
using MagiCloud.Core.MInput;
using MagiCloud.Operate;

namespace MagiCloud
{
    public interface IOperateCreater
    {
        IOperate Creat(MInputHand inputHand,IHandController handController,Func<bool> func = null);
    }
}
