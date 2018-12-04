using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MagiCloud.RotateAndZoomTool
{

    public enum AxisLimits
    {
        /// <summary>
        /// X轴
        /// </summary>
        X,
        /// <summary>
        /// Y轴
        /// </summary>
        Y,
        /// <summary>
        /// X轴
        /// </summary>
        Z,

        /// <summary>
        /// 无限制
        /// </summary>
        None,
    }
    public class RotateCore
    {

        #region 旋转方式 1 插件旋转 方式  2  自写旋转

        /// <summary>
        /// 插件实现的自身物体旋转
        /// </summary>
        /// <param name="rotateGameobjOnlyY">只转y轴的自身物体</param>
        /// <param name="rotateGameobjFatherOnlyX">旋转X轴的物体</param>
        /// <param name="pos"></param>
        /// <param name="otherRotateHelp"></param>
        public void RotateWithPluginHelp(GameObject rotateGameobjOnlyY, GameObject rotateGameobjFatherOnlyX, Vector3 pos, OtherRotateHelp otherRotateHelp)
        {
            otherRotateHelp.RotateFun(rotateGameobjFatherOnlyX.transform, pos);
        }



        /// <summary>
        /// 自写旋转方式 让物体 上下按照相机的X轴转 左右按照自己的Y轴转
        /// </summary>
        /// <param name="rotateGameobj">旋转物体</param>
        /// <param name="mainCamera">主相机</param>
        /// <param name="rotateSpeed">旋转速率</param>
        /// <param name="pos">旋转向量</param>
        public void RotateFunInWorldSpace(Transform rotateGameobj, Camera mainCamera, float rotateSpeed, Vector3 pos)
        {
            //自己实现方式
            if (Mathf.Abs(pos.x) > Mathf.Abs(pos.y))
            {
                if (pos.x > 0)
                {
                    rotateGameobj.Rotate(mainCamera.transform.up * -1, pos.magnitude * rotateSpeed, Space.World);
                }
                else
                {
                    rotateGameobj.Rotate(mainCamera.transform.up, pos.magnitude * rotateSpeed, Space.World);
                }
            }
            else
            {
                if (pos.y > 0)
                {
                    rotateGameobj.Rotate(mainCamera.transform.right, pos.magnitude * rotateSpeed, Space.World);
                }
                else
                {
                    rotateGameobj.Rotate(mainCamera.transform.right * -1, pos.magnitude * rotateSpeed, Space.World);
                }
            }
        }

        /// <summary>
        /// 自写旋转方式 物体按自身轴旋转方式
        /// </summary>
        /// <param name="rotatego">旋转物体</param>
        /// <param name="pos">旋转向量</param>
        /// <param name="axisLimits">旋转限制轴</param>
        public void RotateFunInSelfSpace(GameObject rotatego, Vector3 pos, AxisLimits axisLimits)
        {
            float r = 0;
            if (Math.Abs(pos.x) > Math.Abs(pos.y))
            {
                r = -pos.x;
            }
            else
            {
                r = pos.y;
            }
            switch (axisLimits)
            {
                case AxisLimits.X:
                    rotatego.transform.Rotate(Vector3.right, r, Space.Self);
                    break;
                case AxisLimits.Y:
                    rotatego.transform.Rotate(Vector3.up, r, Space.Self);
                    break;
                case AxisLimits.Z:
                    rotatego.transform.Rotate(Vector3.forward, r, Space.Self);
                    break;
                case AxisLimits.None:
                    rotatego.transform.Rotate(pos, Space.Self);
                    break;
                default:
                    break;
            }
        }

        #endregion


        #region 物体自身角度限制
        /// <summary>
        /// 物体自身角度限制
        /// </summary>
        /// <param name="go"></param>
        /// <param name="axis"></param>
        /// <param name="vector2"></param>
        public void AngleLimits(Transform go, AxisLimits axis, Vector2 vector2)
        {
            if (go == null) return;
            switch (axis)
            {
                case AxisLimits.X:
                    if (ValueChange(go.localEulerAngles.x) > vector2.y)
                    {
                        go.rotation = Quaternion.Euler(vector2.y, go.localEulerAngles.y, go.localEulerAngles.z);
                        //go.localEulerAngles = new Vector3(vector2.y, go.localEulerAngles.y, go.localEulerAngles.z);
                    }
                    if (ValueChange(go.localEulerAngles.x) <= vector2.x)
                    {
                        go.rotation = Quaternion.Euler(vector2.x, go.localEulerAngles.y, go.localEulerAngles.z);
                        //go.localEulerAngles = new Vector3(vector2.x, go.localEulerAngles.y, go.localEulerAngles.z);
                    }
                    break;
                case AxisLimits.Y:
                    if (ValueChange(go.localEulerAngles.y) >= vector2.y)
                    {
                        go.rotation = Quaternion.Euler(go.localEulerAngles.x, vector2.y, go.localEulerAngles.z);
                        //go.localEulerAngles = new Vector3(go.localEulerAngles.x, vector2.y, go.localEulerAngles.z);
                    }
                    if (ValueChange(go.localEulerAngles.y) <= vector2.x)
                    {
                        go.rotation = Quaternion.Euler(go.localEulerAngles.x, vector2.x, go.localEulerAngles.z);
                        //go.localEulerAngles = new Vector3(go.localEulerAngles.x, vector2.x, go.localEulerAngles.z);
                    }

                    break;
                case AxisLimits.Z:
                    if (ValueChange(go.localEulerAngles.z) >= vector2.y)
                    {
                        go.rotation = Quaternion.Euler(go.localEulerAngles.x, go.localEulerAngles.y, vector2.y);
                        //go.localEulerAngles = new Vector3(go.localEulerAngles.x, go.localEulerAngles.y, vector2.y);
                    }
                    if (ValueChange(go.localEulerAngles.z) <= vector2.x)
                    {
                        go.rotation = Quaternion.Euler(go.localEulerAngles.x, go.localEulerAngles.y, vector2.x);
                        //go.localEulerAngles = new Vector3(go.localEulerAngles.x, go.localEulerAngles.y, vector2.x);
                    }
                    break;
                default:
                    break;
            }
        }
        #endregion


        #region 数学工具

        /// <summary>
        /// 换算角度 位置角度在-180-0-180
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public float ValueChange(float value)
        {
            float angle = value - 180;

            if (angle > 0)
                return angle - 180;

            return angle + 180;
        }
        #endregion
    }
}
