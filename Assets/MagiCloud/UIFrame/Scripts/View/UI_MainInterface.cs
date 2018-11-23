using System;
using UnityEngine;

namespace MagiCloud.UIFrame
{
    /// <summary>
    /// 主界面
    /// </summary>
    public class UI_MainInterface : UI_Base
    {
        public override void OnInitialize()
        {
            base.OnInitialize();
            Operate = UIOperate.MainInterface;
        }

        public override void OnOpen()
        {
            base.OnOpen();
            //先处理旧的
            UIManager.Instance.SetUI(this);
        }
    }
}
