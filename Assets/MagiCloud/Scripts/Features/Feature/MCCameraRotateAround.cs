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

        public void OnUpdate()
        {
            if (!isActive) return;
            float dis = (GrabObject.transform.position-Camera.main.transform.position).magnitude;   //相机到物体的距离
            Vector3 screenHand = MOperateManager.GetHandScreenPoint(handIndex);//当前手的屏幕坐标
            Vector3 vector = (screenHand-recordPos)*speed*Time.deltaTime;  //手移动的向量
            x+= (screenHand.x-recordPos.x);
            y-= (screenHand.y-recordPos.y);
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
            x=0;
            y=0;
            Vector3 vector = GrabObject.transform.position-Camera.main.transform.position;
            Camera.main.transform.position=GrabObject.transform.forward*vector.magnitude;//相机朝向物体
            recordPos =screenHand; //记录手的屏幕坐标
            isActive =true;
        }
    }
}
