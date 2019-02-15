using MagiCloud.Interactive.Distance;
using UnityEngine;
using MagiCloud.Core.Events;
using System.Collections;
using Sirenix.OdinInspector;

namespace MagiCloud.Interactive
{
    /// <summary>
    /// 交互控制端
    /// 1、不能读文件了
    /// 2、所有的距离检测点，在Awake初始化的时候，添加到集合点中
    /// </summary>
    [ExecuteInEditMode]
    public class InteractiveController :MonoBehaviour
    {
        private bool isEnable = false;

        private Coroutine coroutineUpdate;//每帧遍历

        public InteractiveSearch Search { get; private set; }

        public static InteractiveController Instance;

        private void Awake()
        {
            Instance = this;

            Search = new InteractiveSearch();
            IsEnable = true;
        }

        /// <summary>
        /// 是否激活
        /// </summary>
        public bool IsEnable
        {
            get
            {
                return isEnable;
            }
            set
            {

                if (isEnable == value) return;

                isEnable = value;

                if (isEnable)
                    OnInteractiveEnable();
                else
                    OnInteractiveDisable();
            }
        }

        /// <summary>
        /// 激活
        /// </summary>
        public void OnInteractiveEnable()
        {
            EventHandGrabObject.AddListener(OnGrabObject,Core.ExecutionPriority.High);
            EventHandReleaseObject.AddListener(OnIdleObject,Core.ExecutionPriority.High);
            if (coroutineUpdate==null)
                coroutineUpdate = StartCoroutine(OnUpdate());
        }

        /// <summary>
        /// 隐藏
        /// </summary>
        public void OnInteractiveDisable()
        {
            EventHandGrabObject.RemoveListener(OnGrabObject);
            EventHandReleaseObject.RemoveListener(OnIdleObject);

            Search.dataManagers.Clear();
            if (coroutineUpdate!=null)
            {
                StopCoroutine(coroutineUpdate);
                coroutineUpdate = null;
            }

        }

        void OnGrabObject(GameObject target,int handIndex)
        {
            Search.OnStartInteraction(target, true, handIndex);
        }

        void OnIdleObject(GameObject target,int handIndex)
        {
            Search.OnStopInteraction(target);
        }

        // /// <summary>
        // /// Update is called every frame, if the MonoBehaviour is enabled.
        // /// </summary>
        // void Update()
        // {
        //     if(!IsEnable)return;
            
        //     if(Time.frameCount % 5 == 0)
        //         Search.OnUpdate(); //先执行一次

        // }
        IEnumerator OnUpdate()
        {
            while (true)
            {
                yield return new WaitForSeconds(0.1f);
                Search.OnUpdate(); //先执行一次
            }
        }
    }
}

