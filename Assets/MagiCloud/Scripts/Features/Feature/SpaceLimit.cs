using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MagiCloud.Core.Events;

namespace MagiCloud.Features
{
    public class SpaceLimit : MonoBehaviour
    {
        public GameObject limitObj;
        public bool topLimit = true;
        public bool bottomLimit = true;
        public bool leftLimit = true;
        public bool rightLimit = true;
        public float topOffset = 0.5f;
        public float bottomOffset = 0.5f;
        public float leftOffset = 0.5f;
        public float rightOffset = 0.5f;

        private Vector3 meshMin;
        private Vector3 meshMax;
        Coroutine coroutine;
        void Awake()
        {
            //if (limitObj == null) limitObj = gameObject.transform.parent.gameObject;
            EventHandGrabObject.AddListener(OnGrab, Core.ExecutionPriority.High);
            EventHandReleaseObject.AddListener(OnIdle, Core.ExecutionPriority.High);
        }


        public void OnDestroy()
        {
            EventHandGrabObject.RemoveListener(OnGrab);
            EventHandReleaseObject.RemoveListener(OnIdle);
        }
        /// <summary>
        /// 全部关闭空间限制
        /// </summary>
        public void CloseLimit()
        {
            topLimit = false;
            bottomLimit = false;
            leftLimit = false;
            rightLimit = false;
        }
        /// <summary>
        /// 全部打开空间限制
        /// </summary>
        public void OpenLimit()
        {
            topLimit = true;
            bottomLimit = true;
            leftLimit = true;
            rightLimit = true;
        }
        private void OnGrab(GameObject grabObj, int index)
        {


        }

        private void OnIdle(GameObject grabObj, int index)
        {
            if (grabObj == limitObj)
                coroutine = StartCoroutine(OutLimitReset());
        }
        void Update()
        {

        }

        /// <summary>
        /// 用于手动开启一次空间限制
        /// </summary>
        public void StartOnceSpaceLimit()
        {
            coroutine = StartCoroutine(OutLimitReset());
        }
        /// <summary>
        /// 越界重置
        /// </summary>
        /// <returns></returns>
        IEnumerator OutLimitReset()
        {
            yield return new WaitForEndOfFrame();
            Vector3 limitObjPos = limitObj.transform.position;
            meshMin = limitObj.BoundsMin();
            meshMax = limitObj.BoundsMax();

            Vector3 limitObjPosToScreen = Camera.main.WorldToScreenPoint(limitObjPos);
            Vector3 screenMinPointToWorld = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, limitObjPosToScreen.z));
            Vector3 screenMaxPointToWorld = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, limitObjPosToScreen.z));
            if (topLimit)
            {
                if (meshMax.y >= screenMaxPointToWorld.y)   //上边越界
                {
                    float temp = meshMax.y - screenMaxPointToWorld.y;
                    //limitObjPos.y -= temp + topOffset;

                    limitObjPos += (temp + topOffset) * -MUtility.MainCamera.transform.up;
                }
            }
            if (bottomLimit)
            {
                if (meshMin.y <= screenMinPointToWorld.y)   //下边越界
                {
                    float temp = meshMin.y - screenMinPointToWorld.y;
                    //limitObjPos.y -= temp - bottomOffset;

                    limitObjPos += (-temp + bottomOffset) * MUtility.MainCamera.transform.up;
                }
            }
            if (leftLimit)
            {
                if (meshMin.x <= screenMinPointToWorld.x)   //左边越界
                {
                    float temp = meshMin.x - screenMinPointToWorld.x;
                    //limitObjPos.x -= temp - leftOffset;

                    limitObjPos += (-temp + leftOffset) * MUtility.MainCamera.transform.right;
                }
            }
            if (rightLimit)
            {
                if (meshMax.x >= screenMaxPointToWorld.x)   //右边越界
                {
                    float temp = meshMax.x - screenMaxPointToWorld.x;
                    //limitObjPos.x -= temp + rightOffset;

                    limitObjPos += (temp + rightOffset) * -MUtility.MainCamera.transform.right;
                }
            }

            limitObj.transform.position = limitObjPos;
            StopCoroutine(coroutine);
        }
    }
}
