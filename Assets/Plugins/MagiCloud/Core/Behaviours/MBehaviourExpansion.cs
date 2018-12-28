using System;

namespace MagiCloud.Core
{
    public static class MBehaviourExpansion
    {
        #region 注册

        internal static void OnAwake_MBehaviour(this MBehaviour behaviour, Action action)
        {
            behaviour.onAwake.AddListener(action);
        }

        internal static void OnEnable_MBehaviour(this MBehaviour behaviour, Action action)
        {
            behaviour.onEnable.AddListener(action);
        }

        internal static void OnStart_MBehaviour(this MBehaviour behaviour, Action action)
        {
            behaviour.onStart.AddListener(action);
        }

        internal static void OnDisable_MBehaviour(this MBehaviour behaviour, Action action)
        {
            behaviour.onDisable.AddListener(action);
        }

        internal static void OnDestroy_MBehaviour(this MBehaviour behaviour, Action action)
        {
            behaviour.onDestroy.AddListener(action);
        }

        public static void OnUpdate_MBehaviour(this MBehaviour behaviour, Action action)
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
            //behaviour.SetEnable();
            behaviour.IsEnable = true;
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

        private static void OnExcuteDisableDestroy(MBehaviour behaviour)
        {
            if (behaviour == null) return;

            if (!behaviour.IsEnable) return;
            behaviour.onDisable.SendListener();
        }

        public static void OnExcuteDestroy(this MBehaviour behaviour)
        {
            OnExcuteDisableDestroy(behaviour);

            if (behaviour == null) return;

            behaviour.onDestroy.SendListener();

            behaviour.onAwake.RemoveListenerAll();
            behaviour.onEnable.RemoveListenerAll();
            behaviour.onDisable.RemoveListenerAll();
            behaviour.onStart.RemoveListenerAll();
            behaviour.onUpdate.RemoveListenerAll();

            MBehaviourController.RemoveBehaviour(behaviour);
        }

        internal static void OnExcuteUpdate(this MBehaviour behaviour)
        {
            if (!behaviour.IsEnable) return;
            behaviour.onUpdate.SendListener();
        }

        #endregion

    }
}
