using MagiCloud.Core;
using System;
using UnityEngine;

namespace MagiCloud.Features
{
    /// <summary>
    /// 物体旋转
    /// </summary>
    public class MCObjectRotation :MCOperaObject
    {

        public Space space = Space.Self;
        public AxisLimits axisLimits = AxisLimits.Y;
        public float minAngle = -360;
        public float maxAngle = 360;
        [Range(0f,1000f)]
        public float speed = 1;

        protected Vector3 recordPos;
        protected Vector3 recordEuler;
        protected int handIndex = 0;
        private bool isActive = false;

        private MBehaviour behaviour;
        private void Awake()
        {
            behaviour=new MBehaviour();
            behaviour.OnUpdate_MBehaviour(OnUpdate);
        }

        private void OnDestroy()
        {
        //    behaviour.OnExcuteDestroy();
        }

        public virtual void OnUpdate()
        {
            if (!isActive) return;
            Vector3 screenHand = MOperateManager.GetHandScreenPoint(handIndex);
            Vector3 screenPosition = MUtility.MainWorldToScreenPoint(recordPos);
            Vector3 handPos = MUtility.MainScreenToWorldPoint(new Vector3(screenHand.x,screenHand.y,screenPosition.z));//手的坐标
            float dis = handPos.x-recordPos.x;  //手移动的距离

            float ratio = dis*speed;
            float angle = 0;
            Vector3 euler = recordEuler;
            angle=GetAngle(ratio,angle);
            euler=GetEuler(angle,euler);
            RotateSelf(euler);

        }

        private void RotateSelf(Vector3 euler)
        {
            switch (space)
            {
                case Space.World:
                    GrabObject.transform.rotation=Quaternion.Euler(euler);
                    break;
                case Space.Self:
                    GrabObject.transform.localRotation=Quaternion.Euler(euler);
                    break;
                default:
                    break;
            }
        }

        private Vector3 GetEuler(float angle,Vector3 euler)
        {
            switch (axisLimits)
            {
                case AxisLimits.None:
                    break;
                case AxisLimits.X:
                    euler.x=angle;
                    break;
                case AxisLimits.Y:
                    euler.y=angle;
                    break;
                case AxisLimits.Z:
                    euler.z=angle;
                    break;
                default:
                    break;
            }

            return euler;
        }

        private float GetAngle(float ratio,float angle)
        {
            switch (axisLimits)
            {
                case AxisLimits.None:
                    break;
                case AxisLimits.X:
                    angle=recordEuler.x-ratio;
                    break;
                case AxisLimits.Y:
                    angle=recordEuler.y-ratio;
                    break;
                case AxisLimits.Z:
                    angle=recordEuler.z -ratio;

                    break;
                default:
                    break;
            }
            if (angle>=maxAngle) angle=maxAngle;
            if (angle<=minAngle) angle=minAngle;
            return angle;
        }



        /// <summary>
        /// 打开
        /// </summary>
        public virtual void OnOpen(int handIndex = 0)
        {
            this.handIndex=handIndex;
            Vector3 screenHand = MOperateManager.GetHandScreenPoint(handIndex);

            Vector3 screenPosition = MUtility.MainWorldToScreenPoint(GrabObject.transform.position);
            recordPos = MUtility.MainScreenToWorldPoint(new Vector3(screenHand.x,screenHand.y,screenPosition.z));//手的坐标


            recordEuler=space== Space.World ? GrabObject.transform.eulerAngles : GrabObject.transform.localEulerAngles;
            isActive =true;
        }

        /// <summary>
        /// 关闭
        /// </summary>
        public virtual void OnClose()
        {
            isActive=false;
        }


    }
}
