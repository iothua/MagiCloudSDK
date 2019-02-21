using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MagiCloud.Operate
{
    /// <summary>
    /// 动作事件约束
    /// </summary>
    public static class ActionConstraint
    {
        /// <summary>
        /// 抓取动作
        /// </summary>
        public const string Grab_Action = "Grab";

        /// <summary>
        /// 摄像机缩放动作
        /// </summary>
        public const string Camera_Zoom_Action = "CameraZoom";
        /// <summary>
        /// 摄像机旋转动作
        /// </summary>
        public const string Camera_Rotate_Action = "CameraRotate";
        public const string Left_Camera_Rotate_Action = "LeftCameraRotate";
        public const string Right_Camera_Rotate_Action = "RightCameraRotate";
        /// <summary>
        /// 动作约束
        /// </summary>
        private readonly static Dictionary<string,bool> Actions = new Dictionary<string,bool>();

        /// <summary>
        /// 添加动作约束
        /// </summary>
        /// <param name="actionName"></param>
        public static bool AddBind(string actionName)
        {
            if (IsBind(actionName)) return false;

            Actions.Add(actionName,true);

            return true;
        }

        /// <summary>
        /// 返回绑定数目
        /// </summary>
        public static int BindCount
        {
            get
            {
                return Actions.Count;
            }
        }

        /// <summary>
        /// 移除动作约束
        /// </summary>
        /// <param name="actionName"></param>
        public static void RemoveBind(string actionName)
        {
            if (!IsBind(actionName)) return;
            Actions.Remove(actionName);
        }


        /// <summary>
        /// 存在动作约束
        /// </summary>
        /// <param name="actionName"></param>
        public static bool IsBind(string actionName)
        {

            if (actionName.Equals(Camera_Rotate_Action))
            {
                return Actions.ContainsKey(Left_Camera_Rotate_Action)||Actions.ContainsKey(Right_Camera_Rotate_Action)||Actions.ContainsKey(Camera_Rotate_Action);
            }
            return Actions.ContainsKey(actionName);
        }

        /// <summary>
        /// 存在该动作之外的
        /// </summary>
        /// <param name="actionName"></param>
        /// <returns></returns>
        public static bool IsBindOther(string actionName)
        {
            if (Actions.Count == 0) return false;

            return Actions.Any(obj => !obj.Key.Equals(actionName));
        }
    }
}
