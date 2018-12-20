using System;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;

namespace MagiCloud.Features
{
    [Serializable]
    public class EventCustomizeUpdate :UnityEvent<GameObject,Vector3,int> { }

    /// <summary>
    /// 自定义处理
    /// </summary>
    public class MCustomize :MCOperaObject
    {
        /// <summary>
        /// 自定义移动
        /// </summary>
        public EventCustomizeUpdate OnCustomizeUpdate;

        private Coroutine coroutine;

        private void Awake()
        {
            if (OnCustomizeUpdate == null)
                OnCustomizeUpdate = new EventCustomizeUpdate();
        }

        IEnumerator OnUpdate(int handIndex)
        {
            while (true)
            {
                yield return new WaitForFixedUpdate();

                Vector3 screenHand = MOperateManager.GetHandScreenPoint(handIndex);

                Vector3 screenPosition = MUtility.MainWorldToScreenPoint(GrabObject.transform.position);
                Vector3 position = MUtility.MainScreenToWorldPoint(new Vector3(screenHand.x,screenHand.y,screenPosition.z));

                if (OnCustomizeUpdate != null)
                {
                    OnCustomizeUpdate.Invoke(GrabObject,position,handIndex);
                }

            }
        }

        public void OnOpen(int handIndex)
        {
            if (coroutine==null)
                coroutine = StartCoroutine(OnUpdate(handIndex));
        }

        public void OnClose()
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
                coroutine=null;
            }
        }
    }
}
