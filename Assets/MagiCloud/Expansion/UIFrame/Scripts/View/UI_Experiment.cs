using System;
using UnityEngine;

namespace MagiCloud.UIFrame
{
    /// <summary>
    /// Ui实验类，所有的实验类的父对象必须为它
    /// </summary>
    public class UI_Experiment : UI_MainInterface
    {
        public override void OnInitialize()
        {
            base.OnInitialize();
            Operate = UIOperate.Experiment;
        }
    }
}
