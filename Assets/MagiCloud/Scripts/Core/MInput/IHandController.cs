using System;
using System.Collections.Generic;

namespace MagiCloud.Core.MInput
{
    public interface IHandController
    {
        /// <summary>
        /// 功能控制端是否激活
        /// </summary>
        bool IsEnable { get; set; }

        /// <summary>
        /// 输入端集合
        /// </summary>
        Dictionary<int, MInputHand> InputHands { get; set; }

        /// <summary>
        /// 获取输入端对象
        /// </summary>
        /// <param name="handIndex"></param>
        /// <returns></returns>
        MInputHand GetInputHand(int handIndex);

        bool IsPlaying { get; }

        void StartOnlyHand();
        void StartMultipleHand();


    }
}
