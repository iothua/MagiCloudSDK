using MagiCloud.Interactive.Distance;
using System;
using UnityEngine;
using MagiCloud.KGUI;

namespace MagiCloud.Interactive
{
    /// <summary>
    /// 距离公用
    /// </summary>
    public static class Utilitys
    {
        /// <summary>
        /// 距离判断
        /// </summary>
        /// <param name="v1">距离检测1</param>
        /// <param name="v2">距离检测2</param>
        /// <param name="distanceType">类型</param>
        /// <returns></returns>
        public static float Distance(Vector3 v1, Vector3 v2, DistanceType distanceType,out float distanceValue)
        {
            
            switch (distanceType)
            {
                case DistanceType.D3D:
                    return distanceValue= Vector3.Distance(v1, v2);
                case DistanceType.D2D:

                    //return Vector2.Distance(v1, v2);

                    Vector2 v21 = Camera.main.WorldToScreenPoint(v1);
                    Vector2 v22 = Camera.main.WorldToScreenPoint(v2);
                    float zoom = 750 / Mathf.Sqrt(Mathf.Pow(1920, 2) + Mathf.Pow(1080, 2));
                    zoom = Mathf.Sqrt(Mathf.Pow(Screen.height, 2) + Mathf.Pow(Screen.width, 2)) * zoom;

                    return distanceValue = Vector3.Distance(v21, v22) / zoom;

                case DistanceType.DScreen:
                    Vector3 screen1 = MUtility.MainCamera.WorldToScreenPoint(v1);
                    Vector3 screen2 = MUtility.MainCamera.WorldToScreenPoint(v2);

                    return distanceValue = Vector2.Distance(screen1, screen2);

                default:
                    return distanceValue = -1;
            }
        }

        /// <summary>
        /// 判断是否在立方体内
        /// </summary>
        /// <param name="position"></param>
        /// <param name="size"></param>
        /// <param name="targetPosition"></param>
        /// <param name="distanceType"></param>
        /// <returns></returns>
        public static bool CubeDistance(Vector3 position, Vector3 size,Vector3 targetPosition, DistanceType distanceType,out float distanceValue)
        {
            Distance(position, targetPosition, distanceType, out distanceValue);

            switch (distanceType)
            {
                case DistanceType.D3D:

                    float d3d_xMin = position.x - size.x / 2;
                    float d3d_xMax = position.x + size.x / 2;

                    float d3d_yMin = position.y - size.y / 2;
                    float d3d_yMax = position.y + size.y / 2;

                    float d3d_zMin = position.z - size.z / 2f;
                    float d3d_zMax = position.z + size.z / 2f;

                    return targetPosition.x.FloatContains(d3d_xMin, d3d_xMax) && targetPosition.y.FloatContains(d3d_yMin, d3d_yMax)
                        && targetPosition.z.FloatContains(d3d_zMin, d3d_zMax);

                case DistanceType.D2D:
                    float xMin = position.x - size.x / 2;
                    float xMax = position.x + size.x / 2;

                    float yMin = position.y - size.y / 2;
                    float yMax = position.y + size.y / 2;

                    return targetPosition.x.FloatContains(xMin, xMax) && targetPosition.y.FloatContains(yMin, yMax);

                case DistanceType.DScreen:

                    //计算两个坐标的屏幕坐标
                    Vector3 screen1 = MUtility.MainCamera.WorldToScreenPoint(position);
                    Vector3 screen2 = MUtility.MainCamera.WorldToScreenPoint(targetPosition);

                    //在计算范围
                    float sizeScreenX = Mathf.Abs(MUtility.MainCamera.WorldToScreenPoint(new Vector3(position.x + size.x / 2, position.y, position.z)).x
                        - MUtility.MainCamera.WorldToScreenPoint(new Vector3(position.x - size.x / 2, position.y, position.z)).x);

                    float sizeScreenY = Mathf.Abs(MUtility.MainCamera.WorldToScreenPoint(new Vector3(position.x, position.y, position.z)).x
                        - MUtility.MainCamera.WorldToScreenPoint(new Vector3(position.x - size.x / 2, position.y, position.z)).x);

                    return MUtility.ScreenPointContains(screen1, new Vector2(sizeScreenX, sizeScreenY), screen2);

                default:
                    return false;
            }
        }
    }
}
