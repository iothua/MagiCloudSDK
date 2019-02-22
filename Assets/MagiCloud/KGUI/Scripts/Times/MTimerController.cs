using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MagiCloud.Common
{
    /// <summary>
    /// 框架时间控制端
    /// </summary>
    public class MTimerController : MonoBehaviour
    {
        public static List<MTimer> Timers = new List<MTimer>();

        private void Update()
        {
            if (Timers.Count == 0) return;

            for (int i = 0; i < Timers.Count; i++)
            {
                Timers[i].OnUpdate(Time.deltaTime);
            }
        }

        private void OnDestroy()
        {
            Timers.Clear();
        }
    }
}
