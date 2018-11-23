using System.Collections;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

namespace MagiCloud.Core
{
    /// <summary>
    /// 行为控制端
    /// </summary>
    public class MBehaviourController : MonoBehaviour
    {
        public static List<MBehaviour> Behaviours = new List<MBehaviour>();

        private static MBehaviourController behaviourController;

        public static MBehaviourController Instance {
            get {
                if (behaviourController == null)
                    behaviourController = FindObjectOfType<MBehaviourController>();

                return behaviourController;
            }
        }

        private void Start()
        {

            //执行OnAwake
            //执行OnEnable
            //执行OnStart

            ExcuteAwake();

            ExcuteEnable();

            ExcuteStart();
        }

        private void Update()
        {
            ExcuteUpdate();
        }

        public static void ExcuteAwake()
        {
            foreach (var item in Behaviours)
            {
                item.OnExcuteAwake();
            }
        }

        public static void ExcuteEnable()
        {
            foreach (var item in Behaviours)
            {
                item.OnExcuteEnable();
            }
        }

        public static void ExcuteStart()
        {
            foreach (var item in Behaviours)
            {
                item.OnExcuteStart();
            }
        }

        public static void ExcuteUpdate()
        {
            foreach (var item in Behaviours)
            {
                item.OnExcuteUpdate();
            }
        }

        /// <summary>
        /// 添加行为
        /// </summary>
        /// <param name="behaviour"></param>
        public static void AddBehaviour(MBehaviour behaviour)
        {
            if (IsContainsBehaviour(behaviour)) return;

            Behaviours.Add(behaviour);

            SortBehaviour();
        }

        /// <summary>
        /// 对行为集合进行排序
        /// </summary>
        public static void SortBehaviour()
        {
            Behaviours = Behaviours.OrderBy(obj => obj.ExecutionPriority).ThenBy(obj => obj.ExecutionOrder).ToList();
        }

        /// <summary>
        /// 移除行为
        /// </summary>
        /// <param name="behaviour"></param>
        public static void RemoveBehaviour(MBehaviour behaviour)
        {
            if (!IsContainsBehaviour(behaviour)) return;

            Behaviours.Remove(behaviour);
        }

        /// <summary>
        /// 是否存在此行为
        /// </summary>
        /// <param name="behaviour"></param>
        /// <returns></returns>
        public static bool IsContainsBehaviour(MBehaviour behaviour)
        {
            return Behaviours.Contains(behaviour);
        }

        #region 延时处理


        /// <summary>
        /// 执行延时
        /// </summary>
        /// <param name="action"></param>
        /// <param name="time"></param>
        public void ExcuteDelay(System.Action action, float time)
        {
            StartCoroutine(Delay(action, time));
        }

        IEnumerator Delay(System.Action action, float time)
        {
            yield return new WaitForSeconds(time);

            action();
        }

        IEnumerator Delay(System.Action action)
        {
            //yield return new WaitForFixedUpdate();
            yield return 0;
            action();
        }

        /// <summary>
        /// 执行延时
        /// </summary>
        /// <param name="action"></param>
        public void ExcuteDelay(System.Action action)
        {
            StartCoroutine(Delay(action));
        }

        #endregion


    }

}
