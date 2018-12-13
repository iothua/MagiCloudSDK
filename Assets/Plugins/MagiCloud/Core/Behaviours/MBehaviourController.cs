using System.Collections;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

namespace MagiCloud.Core
{
    /// <summary>
    /// 行为控制端
    /// </summary>
    [DefaultExecutionOrder(1000)]
    public class MBehaviourController : MonoBehaviour
    {
        private static List<MBehaviour> Behaviours = new List<MBehaviour>();

        private static MBehaviourController behaviourController;

        internal static int Count {
            get {
                return Behaviours.Count;
            }
        }

        internal static MBehaviourController Instance {
            get {
                if (behaviourController == null)
                    behaviourController = FindObjectOfType<MBehaviourController>();

                return behaviourController;
            }
        }

        //private void Start()
        //{

        //    //执行OnAwake
        //    //执行OnEnable
        //    //执行OnStart

        //    OnExcute();

        //    IsInitialize = true;
        //}

        //private void OnExcute()
        //{
        //    ExcuteAwake();

        //    ExcuteEnable();

        //    ExcuteStart();
        //}

        private void Update()
        {
            ExcuteUpdate();
        }

        private static void ExcuteAwake()
        {
            foreach (var item in Behaviours)
            {
                item.OnExcuteAwake();
            }
        }

        private static void ExcuteEnable()
        {
            foreach (var item in Behaviours)
            {
                item.OnExcuteEnable();
            }
        }

        private static void ExcuteStart()
        {
            foreach (var item in Behaviours)
            {
                item.OnExcuteStart();
            }
        }

        private static void ExcuteUpdate()
        {
            for (int i = 0; i < Behaviours.Count; i++)
            {
                if (Behaviours[i] != null)
                {
                    Behaviours[i].OnExcuteUpdate();
                }
            }
        }

        /// <summary>
        /// 添加行为
        /// </summary>
        /// <param name="behaviour"></param>
        internal static void AddBehaviour(MBehaviour behaviour)
        {
            if (IsContainsBehaviour(behaviour)) return;

            Behaviours.Add(behaviour);

            SortBehaviour();

            //if(IsInitialize)
            //{
            //    Instance.ExcuteDelay(() =>
            //    {
            //        behaviour.OnExcuteAwake();
            //        behaviour.OnExcuteEnable();
            //        behaviour.OnExcuteStart();
            //    });
            //}
        }

        /// <summary>
        /// 对行为集合进行排序
        /// </summary>
        internal static void SortBehaviour()
        {
            Behaviours = Behaviours.OrderBy(obj => obj.ExecutionPriority).ThenBy(obj => obj.ExecutionOrder).ToList();
        }

        /// <summary>
        /// 移除行为
        /// </summary>
        /// <param name="behaviour"></param>
        internal static void RemoveBehaviour(MBehaviour behaviour)
        {
            if (!IsContainsBehaviour(behaviour)) return;

            Behaviours.Remove(behaviour);
        }

        /// <summary>
        /// 是否存在此行为
        /// </summary>
        /// <param name="behaviour"></param>
        /// <returns></returns>
        internal static bool IsContainsBehaviour(MBehaviour behaviour)
        {
            return Behaviours.Contains(behaviour);
        }

        #region 延时处理

        IEnumerator Delay(System.Action action)
        {
            yield return 0;
            action();
        }

        /// <summary>
        /// 执行延时
        /// </summary>
        /// <param name="action"></param>
        internal void ExcuteDelay(System.Action action)
        {
            StartCoroutine(Delay(action));
        }

        #endregion


    }

}
