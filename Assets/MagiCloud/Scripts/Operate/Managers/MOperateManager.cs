using UnityEngine;
using System;
using MagiCloud.Core.MInput;
using System.Collections.Generic;
using MagiCloud.Core;
using MagiCloud.Operate;
using Utility;

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
        public const int layerRay = 12;
        public static IOperateCreater operateCreater;
        /// <summary>
        /// 物体层
        /// </summary>
        public const int layerObject = 8;
        /// <summary>
        /// UI层
        /// </summary>
        public const int layerUI = 5;

        private readonly static Dictionary<OperateKey,IOperate> Operates = new Dictionary<OperateKey,IOperate>();

        private static int activeHandControllerOrder = -1; //手势激活优先级

        ///// <summary>
        ///// 添加手势端
        ///// </summary>
        ///// <param name="inputHand"></param>
        ///// <param name="func"></param>
        public static IOperate AddOperateHand(MInputHand inputHand,IHandController handController,Func<bool> func = null)
        {
            OperateKey key = new OperateKey(inputHand.HandIndex,inputHand.Platform);
            if (Operates.ContainsKey(key))
            {
                Operates[key].RayExternaLimit=func;
                return Operates[key];
            }
            IOperate operate = operateCreater.Creat(inputHand,handController,func);
            Operates.Add(key,operate);
            return operate;
        }


        /// <summary>
        /// 移除手势端
        /// </summary>
        /// <param name="inputHand"></param>
        public static void RemoveOperateHand(MInputHand inputHand)
        {
            OperateKey key = new OperateKey(inputHand.HandIndex,inputHand.Platform);

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
        public static IOperate GetOperateHand(int handIndex)
        {
            return GetOperateHand(handIndex,MUtility.CurrentPlatform);
        }

        /// <summary>
        /// 获取到UI操作对象
        /// </summary>
        /// <param name="handIndex"></param>
        /// <returns></returns>
        public static UIOperate GetUIOperate(int handIndex)
        {
            return GetUIOperate(handIndex,MUtility.CurrentPlatform);
        }

        /// <summary>
        /// 获取到UI操作对象
        /// </summary>
        /// <param name="handIndex"></param>
        /// <param name="platform"></param>
        /// <returns></returns>
        public static UIOperate GetUIOperate(int handIndex,OperatePlatform platform)
        {
            IOperate operate = GetOperateHand(handIndex,platform);

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
        public static IOperate GetOperateHand(int handIndex,OperatePlatform platform)
        {
            IOperate operate;

            Operates.TryGetValue(new OperateKey(handIndex,platform),out operate);

            return operate;
        }

        public static IOperate GetOperateHand(OperateKey key)
        {
            IOperate operate;
            Operates.TryGetValue(key,out operate);
            return operate;
        }

        /// <summary>
        /// 激活操作手
        /// </summary>
        /// <param name="handIndex"></param>
        public static void OnEnableOperateHand(int handIndex)
        {
            OnEnableOperateHand(handIndex,MUtility.CurrentPlatform);
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
        /// 激活功能状态(应该添加一个优先级，以保证其他的选项在设置时，因为优先级问题，导致重复设置)
        /// </summary>
        /// <param name="result"></param>
        public static void ActiveHandController(bool result,int activeOrder = 1)
        {
            if (activeOrder < activeHandControllerOrder) return;

            activeHandControllerOrder = activeOrder; //设置优先级

            if (result)
            {
                activeHandControllerOrder = -1;
            }

            foreach (var item in Operates)
            {
                if (item.Key.platform == MUtility.CurrentPlatform)
                    item.Value.HandController.IsEnable = result;
            }
        }

        /// <summary>
        /// 激活手
        /// </summary>
        /// <param name="handIndex"></param>
        /// <param name="platform"></param>
        public static void OnEnableOperateHand(int handIndex,OperatePlatform platform)
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
            OnDisableOperateHand(handIndex,MUtility.CurrentPlatform);
        }

        /// <summary>
        /// 禁止操作手
        /// </summary>
        /// <param name="handIndex"></param>
        /// <param name="platform"></param>
        public static void OnDisableOperateHand(int handIndex,OperatePlatform platform)
        {
            var operate = GetOperateHand(handIndex,platform);

            if (operate != null)
                operate.OnDisable();
        }


        /// <summary>
        /// 设置被抓取
        /// </summary>
        /// <param name="target">需要被设置抓取的物体对象</param>
        /// <param name="zValue"></param>
        public static void SetObjectGrab(GameObject target,int handIndex,float zValue)
        {
            var operate = GetOperateHand(handIndex);
            if (operate == null) return;

            operate.SetObjectGrab(target,zValue);
        }

        public static void SetObjectGrab(GameObject target,int handIndex = 0)
        {
            var operate = GetOperateHand(handIndex);
            if (operate == null) return;

            Vector3 tempPos = MUtility.MainWorldToScreenPoint(target.transform.position - new Vector3(0,0,MUtility.MainCamera.transform.position.z));

            tempPos = MUtility.MainScreenToWorldPoint(tempPos);

            operate.SetObjectGrab(target,tempPos.z);
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

        /// <summary>
        /// 清空全部数据
        /// </summary>
        public static void SetObjectReleaseAll()
        {
            SetObjectRelease(0);
            SetObjectRelease(1);
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
