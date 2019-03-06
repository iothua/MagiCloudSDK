using UnityEngine;
using System;
using MagiCloud.Core.MInput;
using System.Collections.Generic;
using MagiCloud.Core;

namespace MagiCloud
{
  
    /// <summary>
    /// 功能管理
    /// </summary>
    public static class MOperateManager
    {
        /// <summary>
        /// 射线层
        /// </summary>
        public const int layerRay = 9;
        /// <summary>
        /// 物体层
        /// </summary>
        public const int layerObject = 8;
        /// <summary>
        /// UI层
        /// </summary>
        public const int layerUI = 5;

        private readonly static Dictionary<OperateKey, MOperate> Operates = new Dictionary<OperateKey, MOperate>();


        /// <summary>
        /// 添加手势端
        /// </summary>
        /// <param name="inputHand"></param>
        /// <param name="func"></param>
        public static MOperate AddOperateHand(MInputHand inputHand,  IHandController handController,Func<bool> func = null)
        {
            MOperate operate = GetOperateHand(inputHand.HandIndex, inputHand.Platform);

            if (operate != null)
            {
                operate.RayExternaLimit = func;
                return operate;
            }

            operate = new MOperate(inputHand, func, handController);

            Operates.Add(new OperateKey(inputHand.HandIndex, inputHand.Platform), operate);

            return operate;
        }

        /// <summary>
        /// 移除手势端
        /// </summary>
        /// <param name="inputHand"></param>
        public static void RemoveOperateHand(MInputHand inputHand)
        {
            OperateKey key = new OperateKey(inputHand.HandIndex, inputHand.Platform);

            if (Operates.ContainsKey(key))
            {
                Operates.Remove(key);
            }
        }

        /// <summary>
        /// 获取指定操作手对象
        /// </summary>
        /// <param name="handIndex"></param>
        /// <returns></returns>
        public static MOperate GetOperateHand(int handIndex)
        {
            return GetOperateHand(handIndex, MUtility.CurrentPlatform);
        }

        /// <summary>
        /// 获取到UI操作对象
        /// </summary>
        /// <param name="handIndex"></param>
        /// <returns></returns>
        public static UIOperate GetUIOperate(int handIndex)
        {
            return GetUIOperate(handIndex, MUtility.CurrentPlatform);
        }

        /// <summary>
        /// 获取到UI操作对象
        /// </summary>
        /// <param name="handIndex"></param>
        /// <param name="platform"></param>
        /// <returns></returns>
        public static UIOperate GetUIOperate(int handIndex, OperatePlatform platform)
        {
            MOperate operate = GetOperateHand(handIndex, platform);

            if (operate == null)
                return null;

            return operate.UIOperate;
        }

        /// <summary>
        /// 获取平台操作手对象
        /// </summary>
        /// <param name="handIndex"></param>
        /// <param name="platform"></param>
        /// <returns></returns>
        public static MOperate GetOperateHand(int handIndex, OperatePlatform platform)
        {
            MOperate operate;

            Operates.TryGetValue(new OperateKey(handIndex, platform), out operate);

            return operate;
        }

        /// <summary>
        /// 激活操作手
        /// </summary>
        /// <param name="handIndex"></param>
        public static void OnEnableOperateHand(int handIndex)
        {
            OnEnableOperateHand(handIndex, MUtility.CurrentPlatform);
        }

        /// <summary>
        /// 启动多个手
        /// </summary>
        public static void StartMultipleHand()
        {
            foreach (var item in Operates)
            {
                item.Value.HandController.StartMultipleHand();
            }
        }

        /// <summary>
        /// 启动一只手
        /// </summary>
        public static void StartOnlyHand()
        {
            foreach (var item in Operates)
            {
                item.Value.HandController.StartOnlyHand();
            }
        }

        /// <summary>
        /// 激活功能状态
        /// </summary>
        /// <param name="result"></param>
        public static void ActiveHandController(bool result)
        {
            foreach (var item in Operates)
            {
                item.Value.HandController.IsEnable = result;
            }
        }

        /// <summary>
        /// 激活手
        /// </summary>
        /// <param name="handIndex"></param>
        /// <param name="platform"></param>
        public static void OnEnableOperateHand(int handIndex, OperatePlatform platform)
        {
            var operate = GetOperateHand(handIndex,platform);

            if (operate != null)
                operate.OnEnable();
        }

        /// <summary>
        /// 禁止操作手
        /// </summary>
        /// <param name="handIndex"></param>
        public static void OnDisableOperateHand(int handIndex)
        {
            OnDisableOperateHand(handIndex, MUtility.CurrentPlatform);
        }

        /// <summary>
        /// 禁止操作手
        /// </summary>
        /// <param name="handIndex"></param>
        /// <param name="platform"></param>
        public static void OnDisableOperateHand(int handIndex, OperatePlatform platform)
        {
            var operate = GetOperateHand(handIndex, platform);

            if (operate != null)
                operate.OnDisable();
        }


        /// <summary>
        /// 设置被抓取
        /// </summary>
        /// <param name="target">需要被设置抓取的物体对象</param>
        /// <param name="zValue"></param>
        public static void SetObjectGrab(GameObject target,  int handIndex, float zValue)
        {
            var operate = GetOperateHand(handIndex);
            if (operate == null) return;

            operate.SetObjectGrab(target, zValue);
        }

        public static void SetObjectGrab(GameObject target, int handIndex = 0)
        {
            var operate = GetOperateHand(handIndex);
            if (operate == null) return;

            Vector3 tempPos = MUtility.MainWorldToScreenPoint(target.transform.position - new Vector3(0, 0, MUtility.MainCamera.transform.position.z));

            tempPos = MUtility.MainScreenToWorldPoint(tempPos);

            operate.SetObjectGrab(target, tempPos.z);
        }

        /// <summary>
        /// 获取到屏幕坐标
        /// </summary>
        /// <param name="handIndex"></param>
        /// <returns></returns>
        public static Vector3 GetHandScreenPoint(int handIndex)
        {
            var operate = GetOperateHand(handIndex);
            if (operate == null) return Vector3.zero;

            return operate.InputHand.ScreenPoint; 
        }

        /// <summary>
        /// 设置释放当前物体
        /// </summary>
        /// <param name="target"></param>
        public static void SetObjectRelease(int handIndex)
        {
            var operate = GetOperateHand(handIndex);
            if (operate == null) return;

            operate.SetObjectRelease();
        }

        public static MInputHandStatus GetHandStatus(int handIndex)
        {
            var operate = GetOperateHand(handIndex);

            if (operate == null)
                return MInputHandStatus.Idle;

            return operate.InputHand.HandStatus;
        }

        /// <summary>
        /// 获取到被抓取的物体
        /// </summary>
        /// <param name="handIndex"></param>
        /// <returns></returns>
        public static GameObject GetObjectGrab(int handIndex)
        {
            var operate = GetOperateHand(handIndex);

            if (operate == null)
                return null;

            return operate.GetObjectGrab();
        }
    }
}
