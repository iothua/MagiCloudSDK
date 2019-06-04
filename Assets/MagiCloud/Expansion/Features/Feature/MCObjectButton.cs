using MagiCloud.Core;
using System;
using UnityEngine.Events;

namespace MagiCloud.Features
{
    /// <summary>
    /// 物体按钮事件
    /// </summary>
    public class MCObjectButton :MCOperaObject
    {
        /// <summary>
        /// 按下
        /// </summary>
        public UnityEvent onDown;
        /// <summary>
        /// 释放
        /// </summary>
        public UnityEvent onFreed;
        /// <summary>
        /// 长按
        /// </summary>
        public UnityEvent onPress;
        private MBehaviour behaviour;
        private bool isDown = false;
        private void Awake()
        {
            if (onDown==null) onDown=new UnityEvent();
            if (onFreed==null) onFreed=new UnityEvent();
            if (onPress==null) onPress=new UnityEvent();
            behaviour=new MBehaviour();
            behaviour.OnUpdate_MBehaviour(OnUpdate);
      
        }
        private void OnDestroy()
        {
          //  behaviour.OnExcuteDestroy();
        }
        private void OnUpdate()
        {
            if (isDown)
                onPress?.Invoke();
        }

        /// <summary>
        /// 释放
        /// </summary>
        public void OnFreed(int handIndex = 0)
        {
            isDown=false;
            onFreed?.Invoke();
        }
        /// <summary>
        /// 按下
        /// </summary>
        public void OnDown(int handIndex = 0)
        {
            isDown=true;
            onDown?.Invoke();
        }
    }

}
