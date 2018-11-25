using System;

namespace MagiCloud.Core
{
    public static class MBehaviourExpansion
    {
        #region 注册

        public static void OnAwake(this MBehaviour behaviour, Action action)
        {
            behaviour.onAwake.AddListener(action);
        }

        public static void OnEnable(this MBehaviour behaviour, Action action)
        {
            behaviour.onEnable.AddListener(action);
        }

        public static void OnStart(this MBehaviour behaviour, Action action)
        {
            behaviour.onStart.AddListener(action);
        }

        public static void OnDisable(this MBehaviour behaviour, Action action)
        {
            behaviour.onDisable.AddListener(action);
        }

        public static void OnDestroy(this MBehaviour behaviour, Action action)
        {
            behaviour.onDestroy.AddListener(action);
            MBehaviourController.RemoveBehaviour(behaviour);
        }

        public static void OnUpdate(this MBehaviour behaviour, Action action)
        {
            behaviour.onUpdate.AddListener(action);
        }

        #endregion

        #region 执行

        internal static void OnExcuteAwake(this MBehaviour behaviour)
        {
            if (behaviour.IsAwake) return;

            //如果没有激活，则跳过
            if (!behaviour.IsActive) return;

            behaviour.onAwake.SendListener();

            behaviour.IsAwake = true;
        }

        internal static void OnExcuteEnable(this MBehaviour behaviour)
        {
            if (!behaviour.IsActive) return;

            behaviour.onEnable.SendListener();
            behaviour.SetEnable();
            //behaviour.IsEnable = true;
        }

        internal static void OnExcuteStart(this MBehaviour behaviour)
        {
            if (behaviour.IsStart) return;

            if (!behaviour.IsEnable) return;

            behaviour.onStart.SendListener();

            behaviour.IsStart = true;
        }

        internal static void OnExcuteDisable(this MBehaviour behaviour)
        {
           if (!behaviour.IsEnable) return;

           behaviour.onDisable.SendListener();
           behaviour.IsEnable = false;
        }

        public static void OnExcuteDestroy(this MBehaviour behaviour)
        {
            OnExcuteDisable(behaviour);
            behaviour.onDestroy.SendListener();

            behaviour.onAwake.RemoveListenerAll();
            behaviour.onEnable.RemoveListenerAll();
            behaviour.onDisable.RemoveListenerAll();
            behaviour.onStart.RemoveListenerAll();
            behaviour.onUpdate.RemoveListenerAll();
        }

        internal static void OnExcuteUpdate(this MBehaviour behaviour)
        {
            if (!behaviour.IsEnable) return;
            behaviour.onUpdate.SendListener();
        }

        #endregion

    }
}
