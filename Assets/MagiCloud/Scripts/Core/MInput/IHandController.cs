using System;
using System.Collections.Generic;

namespace MagiCloud.Core.MInput
{
    public interface IHandController
    {
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
    }
}
