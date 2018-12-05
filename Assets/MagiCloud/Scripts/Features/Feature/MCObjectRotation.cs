using System;
using UnityEngine;

namespace MagiCloud.Features
{
    /// <summary>
    /// 物体旋转
    /// </summary>
    public class MCObjectRotation : MCOperaObject
    {
        [Header("选“能抓取”、“限制移动”是无效的")]
        public ObjectOperaType operaType;

        public Space space = Space.Self;
        public AxisLimits axisLimits = AxisLimits.None;
        public float minAngle = -360;
        public float maxAngle = 360;

        /// <summary>
        /// 设置操作类型
        /// </summary>
        /// <param name="type"></param>
        public void SetOperaType(ObjectOperaType type)
        {
            operaType = type;
        }

        /// <summary>
        /// 打开
        /// </summary>
        public void OnOpen()
        {
            switch (operaType)
            {
                case ObjectOperaType.无:
                    break;
                case ObjectOperaType.物体自身旋转:
                    //RotateManager.StartGoRotate(GrabObject, space, axisLimits, minAngle, maxAngle);
                    break;
                case ObjectOperaType.摄像机围绕物体旋转:
                    //RotateManager.StartCameraAroundCenter(GrabObject.transform);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 关闭
        /// </summary>
        public void OnClose()
        {
            switch (operaType)
            {
                case ObjectOperaType.无:
                    break;
                case ObjectOperaType.物体自身旋转:
                    //RotateManager.StopGoRotate(GrabObject);
                    break;
                case ObjectOperaType.摄像机围绕物体旋转:
                    //RotateManager.StopCameraAroundCenter();
                    break;
                default:
                    break;
            }
        }

    }
}
