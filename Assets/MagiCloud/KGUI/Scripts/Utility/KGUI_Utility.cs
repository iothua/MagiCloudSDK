using System;
using System.Collections.Generic;
using UnityEngine;

namespace MagiCloud.KGUI
{
    /// <summary>
    /// KGUI公用类
    /// </summary>
    public static class KGUI_Utility
    {

        /// <summary>
        /// 此时KGUI摄像机
        /// </summary>
        public static Camera kguiCamera;
        public static LayerMask layerMask;
        /// <summary>
        /// 将物体世界坐标转化为屏幕坐标
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public static Vector3 GetScreenPoint(Vector3 position)
        {
            //return KinectConfig.mainCamera.WorldToScreenPoint(position);

            if (kguiCamera == null)
                throw new Exception("KGUI摄像机对象为Null，如果是在编辑器上出现此问题，运行后，即可");

            return kguiCamera.WorldToScreenPoint(position);
        }

        /// <summary>
        /// 将屏幕坐标转化为世界坐标
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public static Vector3 GetWorldPoint(Vector3 position)
        {
            if (kguiCamera == null)
                throw new Exception("KGUI摄像机对象为Null，如果是在编辑器上出现此问题，运行后，即可");

            //return KinectConfig.mainCamera.ScreenToWorldPoint(position);
            return kguiCamera.ScreenToWorldPoint(position);
        }

        /// <summary>
        /// 判断坐标是否在指定物体内
        /// </summary>
        /// <param name="position">物体屏幕坐标</param>
        /// <param name="size">物体的大小</param>
        /// <param name="handPotion">手屏幕坐标</param>
        /// <returns></returns>
        public static bool ScreenPointContains(Vector2 position, Vector2 size, Vector2 handPotion)
        {
            //X轴比较
            float xMin = position.x - size.x / 2;
            float xMax = position.x + size.x / 2;

            float yMin = position.y - size.y / 2;
            float yMax = position.y + size.y / 2;

            return handPotion.x.FloatContains(xMin, xMax) && handPotion.y.FloatContains(yMin, yMax);
        }

        /// <summary>
        /// 比较float值是否在指定范围内
        /// </summary>
        /// <param name="value">指定值</param>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        /// <returns>在范围内，返回True，否则返回false</returns>
        public static bool FloatContains(this float value, float min, float max)
        {
            return value >= min && value <= max;

        }

        /// <summary>
        /// 是否存在区域内
        /// </summary>
        /// <param name="transform">指定物体</param>
        /// <param name="handIndex"></param>
        /// <returns></returns>
        public static bool IsAreaContains(Transform transform,int handIndex)
        {
            //if (!KinectTransfer.IsHandActive(handIndex)) return false;

            try
            {

                //获取到此时手的屏幕坐标屏幕坐标
                Vector3 screenHandPoint = MOperateManager.GetHandScreenPoint(handIndex);

                Vector3 screenPoint = GetScreenPoint(transform.position);

                //根据自身此时的屏幕坐标，去算区域

                RectTransform rectTransform = transform.GetComponent<RectTransform>();

                return ScreenPointContains(screenPoint, rectTransform.sizeDelta, screenHandPoint);
            }
            catch (Exception)
            {
                return false;
                //throw new Exception("手势可能没激活，如果是在编辑器上遇到此问题，不用理会");
            }
        }

        /// <summary>
        /// 是否存在区域内
        /// </summary>
        /// <param name="screenPoint">屏幕坐标</param>
        /// <param name="size">大小</param>
        /// <param name="handIndex">手势</param>
        /// <returns></returns>
        public static bool IsAreaContains(Vector2 screenPoint, Vector2 size, int handIndex)
        {
            //if (!KinectTransfer.IsHandActive(handIndex)) return false;

            //获取到此时手的屏幕坐标屏幕坐标
            Vector3 screenHandPoint = MOperateManager.GetHandScreenPoint(handIndex);

            //根据自身此时的屏幕坐标，去算区域

            return ScreenPointContains(screenPoint, size, screenHandPoint);
        }

        /// <summary>
        /// 返回是否在屏幕内
        /// </summary>
        /// <param name="thingAttach"></param>
        /// <returns></returns>
        public static bool AttachThingPosInCamera(Transform thingAttach, Vector2 xlimits, Vector2 ylimits)
        {
            Transform camTransform = Camera.main.transform;

            Vector2 viewPos = Camera.main.WorldToViewportPoint(thingAttach.position);

            Vector3 dir = (thingAttach.position - camTransform.position).normalized;

            float dot = Vector3.Dot(camTransform.forward, dir);     //判断物体是否在相机前面  

            Vector2 screenPos = Camera.main.WorldToScreenPoint(thingAttach.position);

            if (screenPos.x < xlimits.y &&
                screenPos.x > xlimits.x &&
                screenPos.y < ylimits.y &&
                screenPos.y > ylimits.x
               && dot > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
