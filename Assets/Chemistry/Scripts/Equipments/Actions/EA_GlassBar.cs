using DG.Tweening;
using System;
using UnityEngine;

namespace Chemistry.Equipments.Actions
{
    /// <summary>
    /// 玻璃棒动作
    /// </summary>
    public class EA_GlassBar :MonoBehaviour
    {
        public Transform rotatePoint;
        public Vector3 pos;
        public Vector3 dir;
        public void DoAction()
        {
            pos=rotatePoint.position;
            transform.localRotation.Set(0,0,5,1);
            transform.RotateAround(pos,dir,15);
        }
    }
}
