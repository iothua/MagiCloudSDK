using UnityEngine;
using System.Collections;
using MagiCloud.Core;

namespace MagiCloud.RotateAndZoomTool
{

    public class DragMouseOrbit
    {
        public Transform target;

        /// <summary>
        /// 水平轴旋转速度
        /// </summary>
        public float xSpeed = RotateAndZoomManager.Speed_CameraRotateAroundCenter_HorizontalAxis;

        /// <summary>
        /// 垂直轴旋转速度
        /// </summary>
        public float ySpeed = RotateAndZoomManager.Speed_CameraRotateAroundCenter_VerticalAxis;

        /// <summary>
        /// 速度系数标准化
        /// </summary>
        private float speednormalize = 0.02f;

        /// <summary>
        /// 水平和垂直轴角度限制
        /// </summary>
        public float yMinLimit = 0f;
        public float yMaxLimit = 0f;
        public float xMinLimit = 0f;
        public float xMaxLimit = 0f;

        /// <summary>
        /// 平滑度
        /// </summary>
        public float smoothTime = 3f;
        /// <summary>
        /// 缓冲量
        /// </summary>
        float rotationYAxis = 0.0f;
        float rotationXAxis = 0.0f;
        public float velocityX = 0.0f;
        public float velocityY = 0.0f;

        // Use this for initialization
        public DragMouseOrbit(Transform targettrans,Transform cameratrans)
        {
            //目标物体
            target = targettrans;

            //相机物体
            Vector3 angles = cameratrans.eulerAngles;
            velocityX = AngleCC(angles.y);
            velocityY = AngleCC(angles.x);

            // Make the rigid body not change rotation
        }


        public void LateUpdateCameraRotate(Transform cameratrans,Vector3 pos,float distance)
        {
            if (target)
            {

                #region New 2019-4-1
                rotationXAxis+=pos.x/(Screen.width)*360f*xSpeed;
                velocityX+=rotationXAxis;
                rotationYAxis+=pos.y/(Screen.height)*360f*ySpeed;
                velocityY-=rotationYAxis;
                //限制范围
                velocityX=Mathf.Clamp(velocityX,xMinLimit,xMaxLimit);
                velocityY=Mathf.Clamp(velocityY,yMinLimit,yMaxLimit);

                //计算旋转值
                Quaternion q = Quaternion.Euler(velocityY,velocityX,0);

                Vector3 direction = q*target.forward;
                Vector3 targetPos = target.position-direction*distance;

                cameratrans.position=targetPos;
                cameratrans.rotation=q;
                rotationXAxis = Mathf.Lerp(rotationXAxis,0,Time.deltaTime * smoothTime);
                rotationYAxis = Mathf.Lerp(rotationYAxis,0,Time.deltaTime * smoothTime);
                #endregion

                #region Old
                //velocityX += RotateAndZoomManager.Speed_CameraRotateAroundCenter_HorizontalAxis * -pos.x * speednormalize;
                //velocityY += RotateAndZoomManager.Speed_CameraRotateAroundCenter_VerticalAxis * -pos.y * speednormalize;

                //rotationYAxis += velocityX;
                //rotationXAxis -= velocityY;

                ////限制垂直方向上角度
                //rotationXAxis = ClampAngle(rotationXAxis,yMinLimit,yMaxLimit);

                ////当有水平轴上的角度限制的时候
                //if (xMinLimit != 0 || xMaxLimit != 0) rotationYAxis = ClampAngle(rotationYAxis,xMinLimit,xMaxLimit);
                ////Quaternion fromRotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0);

                //Quaternion toRotation = Quaternion.Euler(rotationXAxis,rotationYAxis,0);

                //Vector3 negDistance = new Vector3(0.0f,0.0f,-distance);
                //Vector3 position = toRotation * negDistance + target.position;

                ////cameratrans.rotation= Quaternion.Slerp(cameratrans.rotation,toRotation,Time.deltaTime*smoothTime);

                //cameratrans.rotation = toRotation;
                ////cameratrans.position =Vector3.Slerp(cameratrans.position,position,Time.deltaTime*smoothTime);
                //cameratrans.position=position;
                //velocityX = Mathf.Lerp(velocityX,0,Time.deltaTime * smoothTime);
                //velocityY = Mathf.Lerp(velocityY,0,Time.deltaTime * smoothTime);
                #endregion

            }
        }

        public void ClearData()
        {
            //rotationYAxis = 0.0f;
            //rotationXAxis = 0.0f;
            velocityX = 0.0f;
            velocityY = 0.0f;
        }

        public static float ClampAngle(float angle,float min,float max)
        {
            if (angle < -360F)
                angle += 360F;
            if (angle > 360F)
                angle -= 360F;
            return Mathf.Clamp(angle,min,max);
        }

        float AngleCC(float s)
        {
            if (s > 180)
            {
                return s - 360;
            }
            return s;
        }
    }
}