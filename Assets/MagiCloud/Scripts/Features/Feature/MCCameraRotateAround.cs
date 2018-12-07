using MagiCloud.Core;
using System;
using UnityEngine;

namespace MagiCloud.Features
{
    /// <summary>
    /// 相机围绕物体旋转
    /// </summary>
    public class MCCameraRotateAround :MCOperaObject
    {
        [Range(0f,360f)]
        public float speed = 1;
        public Vector2 upAndDown = new Vector2(-180,180);
        public Vector2 leftAndRight = new Vector2(-180,180);
        private bool isActive = false;
        private float x = 0;
        private float y = 0;
        private Vector3 recordPos;
        private MBehaviour behaviour;
        private int handIndex;


        private void Awake()
        {
            behaviour=new MBehaviour();
            behaviour.OnUpdate_MBehaviour(OnUpdate);

        }
        private void OnDestroy()
        {
            //   behaviour.OnExcuteDestroy();
        }
        public void OnUpdate()
        {
            if (!isActive) return;
            float dis = (GrabObject.transform.position-Camera.main.transform.position).magnitude;   //相机到物体的距离
            Vector3 screenHand = MOperateManager.GetHandScreenPoint(handIndex);//当前手的屏幕坐标
            Vector3 vector = (screenHand-recordPos)*speed*Time.deltaTime;  //手移动的向量
            //移动距离转旋转值
            x+= (screenHand.x-recordPos.x)/1920*360;
            y-= (screenHand.y-recordPos.y)/1080*360;
            //限制范围
            x=Mathf.Clamp(x,leftAndRight.x,leftAndRight.y);
            y=Mathf.Clamp(y,upAndDown.x,upAndDown.y);

            Quaternion q = Quaternion.Euler(y,x,0);

            Vector3 direction = q*GrabObject.transform.forward;
            Camera.main.transform.position=GrabObject.transform.position-direction*dis;
            Camera.main.transform.rotation=q;
            recordPos=screenHand;
        }




        public void OnClose()
        {
            isActive=false;
        }

        public void OnOpen(int handIndex)
        {
            this.handIndex=handIndex;
            Vector3 screenHand = MOperateManager.GetHandScreenPoint(handIndex);
           
            Vector3 vector = GrabObject.transform.position-Camera.main.transform.position;
            // Camera.main.transform.position=GrabObject.transform.forward*vector.magnitude;//相机朝向物体
            Camera.main.transform.forward=vector.normalized;
            recordPos =screenHand; //记录手的屏幕坐标
            isActive =true;
        }
    }
}
