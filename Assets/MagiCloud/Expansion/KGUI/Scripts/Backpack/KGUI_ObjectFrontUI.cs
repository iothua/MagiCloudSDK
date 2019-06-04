using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

namespace MagiCloud.KGUI
{
    /// <summary>
    /// KGUI使物体显示在前面
    /// </summary>
    public class KGUI_ObjectFrontUI : MonoBehaviour
    {
        /*
        1、一种是，背包，物体要一直动,但是摄像机可能不会动
        2、一种是物体静态，一直不动，摄像机也不动
        3、一种是摄像机动，物体不动。
        */

        public Dictionary<GameObject, int> DicInfos = new Dictionary<GameObject, int>();

        private Transform parent;

        private bool IsSet = false;

        /// <summary>
        /// 设置
        /// </summary>
        public void OnSet()
        {
            FrameConfig.Config.StartCoroutine(Set());
        }

        /// <summary>
        /// 将该物体设置【物体在UI前摄像机的】中的子物体
        /// </summary>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <param name="scale"></param>
        public void OnSet(Vector3 position, Quaternion rotation, Vector3 scale)
        {
            FrameConfig.Config.StartCoroutine(Set(position, rotation, scale));
        }

        /// <summary>
        /// 恢复默认
        /// </summary>
        public void OnReset()
        {
            FrameConfig.Config.StartCoroutine(Reset());
        }

        public void OnReset(Transform parent, Vector3 position, Quaternion rotation, Vector3 scale)
        {
            FrameConfig.Config.StartCoroutine(Reset(parent, position, rotation, scale));
        }

        IEnumerator Set()
        {
            yield return new WaitForFixedUpdate();

            SetLayer();
        }

        IEnumerator Set(Vector3 position, Quaternion rotation, Vector3 scale)
        {
            yield return new WaitForFixedUpdate();

            parent = transform.parent;

            transform.SetParent(GameObject.FindGameObjectWithTag("ObjectFrontUI").transform);
            transform.localPosition = position;
            transform.localRotation = rotation;
            transform.localScale = scale;

            SetLayer();
        }

        void SetLayer()
        {
            if (IsSet) return;

            Transform[] Transforms = GetComponentsInChildren<Transform>();

            foreach (var transform in Transforms)
            {
                var item = transform.gameObject;

                //Debug.Log(item + "设置：" + item.layer);

                if (DicInfos.ContainsKey(item))
                {
                    DicInfos[item] = item.layer;
                }
                else
                {
                    DicInfos.Add(item, item.layer);
                }

                item.layer = 10;
            }
            IsSet = true;

        }

        IEnumerator Reset()
        {
            yield return new WaitForFixedUpdate();
            
            ResetLayer();
        }

        IEnumerator Reset(Transform parent, Vector3 position, Quaternion rotation, Vector3 scale)
        {
            yield return new WaitForFixedUpdate();

            transform.parent = parent;
            transform.localPosition = position;
            transform.localRotation = rotation;
            transform.localScale = scale;

            ResetLayer();

        }

        void ResetLayer()
        {
            if (!IsSet) return;

            Transform[] Transforms = GetComponentsInChildren<Transform>();

            foreach (var transform in Transforms)
            {
                var item = transform.gameObject;
                if (DicInfos.ContainsKey(item))
                {
                    item.layer = DicInfos[item];
                    DicInfos.Remove(item);
                }
            }

            IsSet = false;
        }
    }
}
