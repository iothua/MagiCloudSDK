using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MagiCloud.RotateAndZoomTool
{

    public class OtherRotateHelp
    {

        /// <summary>
        /// 旋转利用插件代码完成
        /// </summary>

        /// <summary>
        /// 旋转灵敏度
        /// </summary>
        public float Sensitivity = 0.5f;

        private float Dampening = 10.0f;

        private Vector2 minmaxX = Vector2.zero;
        private Vector2 minmaxY = Vector2.zero;


        float Pitch = 0, Yaw = 0, currentPitch = 0, currentYaw = 0;

        public OtherRotateHelp(Vector2 limitsX, Vector2 limitsY, float sensitivity)
        {
            this.MinmaxX = limitsX;
            this.MinmaxY = limitsY;
            this.Sensitivity = sensitivity;
        }

        public Vector2 MinmaxX
        {
            get
            {
                return minmaxX;
            }

            set
            {
                minmaxX = value;
            }
        }

        public Vector2 MinmaxY
        {
            get
            {
                return minmaxY;
            }

            set
            {
                minmaxY = value;
            }
        }

        public void RotateFun(Transform rotatething, Vector3 pos)
        {

            Yaw = Mathf.Clamp(Yaw, MinmaxX.x, MinmaxX.y);
            Pitch = Mathf.Clamp(Pitch, MinmaxY.x, MinmaxY.y);

            Yaw += pos.x * Sensitivity;
            Pitch -= pos.y * Sensitivity;

            currentYaw = SgtHelper.Dampen(currentYaw, Yaw, Dampening, Time.deltaTime * 0.2f, 0.1f);
            currentPitch = SgtHelper.Dampen(currentPitch, Pitch, Dampening, Time.deltaTime * 0.2f, 0.1f);

            var rotation = Quaternion.Euler(currentPitch, currentYaw, 0);

            SgtHelper.SetRotation(rotatething, rotation);
        }

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
