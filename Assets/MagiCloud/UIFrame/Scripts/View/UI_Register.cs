using UnityEngine;
using System.Collections;

namespace MagiCloud.UIFrame
{
    public class UI_Register : UI_Base
    {
        public override void OnInitialize()
        {
            base.OnInitialize();
            Operate = UIOperate.Register;

        }
    }
}

