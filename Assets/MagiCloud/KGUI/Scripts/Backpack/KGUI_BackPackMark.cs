using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MagiCloud.KGUI
{
    /// <summary>
    /// 背包标记
    /// </summary>
    [RequireComponent(typeof(KGUI_ObjectFrontUI))]
    public class KGUI_BackPackMark : MonoBehaviour
    {
        /// <summary>
        /// 是否能删除
        /// </summary>
        public Func<bool> CanPutInBag;

        /// <summary>
        /// 从背包生成物体时，事件通知
        /// </summary>
        public Action<string, int> CreateForBag;

        /// <summary>
        /// 从背包删除物体时，触发此事件
        /// </summary>
        public Action DestroyForBag;

        /// <summary>
        /// 目标名称
        /// </summary>
        public string TagName { get; set; }

        /// <summary>
        /// 目标物体
        /// </summary>
        public GameObject Target { get { return gameObject; } }

        /// <summary>
        /// 所属背包子项
        /// </summary>
        public KGUI_BackpackItem OwnItem { get; private set; }

        /// <summary>
        /// 功能控制端
        /// </summary>
        public Features.FeaturesObjectController[] FeaturesObjects;

        public KGUI_ObjectFrontUI ObjectFrontUI;

        /// <summary>
        /// 是否放入背包
        /// </summary>
        /// <returns></returns>
        public bool IsCanPutInBag()
        {
            if (CanPutInBag != null)
                return CanPutInBag();
            else
                return true;
        }

        /// <summary>
        /// 从背包创建时执行
        /// </summary>
        /// <param name="name"></param>
        /// <param name="handIdex"></param>
        public void OnCreateForBag(KGUI_BackpackItem item, string name, int handIdex)
        {
            TagName = name;
            OwnItem = item;

            FeaturesObjects = gameObject.GetComponentsInChildren<Features.FeaturesObjectController>();

            ObjectFrontUI = gameObject.GetComponent<KGUI_ObjectFrontUI>();

            if (CreateForBag != null)
                CreateForBag(TagName, handIdex);
        }

        /// <summary>
        /// 判断两个值是否相等
        /// </summary>
        /// <returns></returns>
        public bool IsFeaturesObjectEquals()
        {
            var featuresObjects = gameObject.GetComponentsInChildren<Features.FeaturesObjectController>();

            return Enumerable.SequenceEqual(FeaturesObjects, featuresObjects);
        }

        /// <summary>
        /// 从背包中删除时执行
        /// </summary>
        public void OnDestroyForBag()
        {
            if (DestroyForBag != null)
                DestroyForBag();
        }

        private void OnDestroy()
        {
            OwnItem.DestroyEquipment(gameObject);
        }


    }
}
