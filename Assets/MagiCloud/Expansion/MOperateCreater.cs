using System;
using MagiCloud.Core.MInput;
using MagiCloud.Operate;

namespace MagiCloud
{
    public class MOperateCreater :OperateCreaterBase
    {
        public override IOperate Creat(MInputHand inputHand,IHandController handController,Func<bool> func = null)
        {
            return new MOperate(inputHand,func,handController);
        }
    }

}
