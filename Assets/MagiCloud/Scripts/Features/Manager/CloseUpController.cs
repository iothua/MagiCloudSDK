using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using System;

namespace MagiCloud.Features
{
    /// <summary>
    /// 特写数据
    /// </summary>
    [Serializable]
    public class CloseUpData
    {
        public Transform target;        //特写目标
        public Vector3 normal;          //特写视角(单位向量)
        public float distance;          //特写距离
        public float time;              //特写动画时长
        public UnityEvent startEvent;   //特写前的事件
        public UnityEvent endEvent;     //特写后的事件
    }

    /// <summary>
    /// 特写控制管理
    /// </summary>
    public static class CloseUpController
    {
        private static bool playing = false;
        /// <summary>
        /// 当前是否正在执行特写
        /// </summary>
        public static bool Playing { get { return playing; } }

        /// <summary>
        /// 执行特写
        /// </summary>
        /// <param name="target">特写目标</param>
        /// <param name="normal">特写方向,为null时默认为（0，1，-1）</param>
        /// <param name="camera">用于特写的相机,为空时默认为主相机</param>
        /// <param name="distance">特写距离</param>
        /// <param name="time">动作时长</param>
        /// <param name="allowKill">是否允许结束之前的Tween</param>
        /// <param name="inChild">相机成为子物体</param>
        /// <param name="startAction">特写前执行</param>
        /// <param name="endAction">特写后执行</param>
        public static void CloseUp(this Transform target,Vector3? normal = null,
            float distance = 1f,float time = 1f,bool allowKill = false,Camera camera = null,bool inChild = true,UnityEvent startAction = null,UnityEvent endAction = null,Ease ease = Ease.Linear)
        {
            if (target==null) return;
            if (!allowKill)
                if (Playing) return;
            if (startAction!=null) startAction.Invoke();
            playing=true;
            Transform cameraTrans = (camera!=null ? camera.transform : Camera.main.transform);
            cameraTrans.SetParent(target);
            Vector3 dir = (normal!=null ? normal.Value.normalized : new Vector3(0,1,-1).normalized);
            Vector3 pos = dir*distance;
            cameraTrans.DOKill();
            cameraTrans.DOLocalMove(pos,time).SetEase(ease);
            cameraTrans.DOLocalRotate(Quaternion.LookRotation(-dir).eulerAngles,time).SetEase(ease).OnComplete(() => OnComplete(cameraTrans,inChild,endAction));
        }

        /// <summary>
        /// 执行特写
        /// </summary>
        /// <param name="data">特写数据</param>
        /// <param name="allowKill">是否允许结束之前的Tween</param>
        /// <param name="camera">用于特写的相机,为空时默认为主相机</param>
        /// <param name="inChild">相机成为子物体</param>
        public static void CloseUp(CloseUpData data,bool allowKill = false,Camera camera = null,bool inChild = true,Ease ease = Ease.Linear)
        {
            CloseUp(data.target,data.normal,data.distance,data.time,allowKill,camera,inChild,data.startEvent,data.endEvent,ease);
        }

        /// <summary>
        /// Kill掉特写
        /// </summary>
        /// <param name="KillCompleteEvent"></param>
        /// <param name="camera"></param>
        public static void KillCloseUp(bool KillComplete = false,Camera camera = null)
        {
            if (!playing) return;
            if (camera==null)
                camera=Camera.main;
            camera.transform.DOKill(KillComplete);
            camera.transform.SetParent(null);
            playing=false;
            // camera.transform.CloseUp(Vector3.zero,0,0,true,camera,false,null,KillCompleteEvent);
        }


        private static void OnComplete(Transform cameraTrans,bool inChild,UnityEvent endAction)
        {
            if (!inChild)
                cameraTrans.SetParent(null);
            playing=false;
            if (endAction!=null)
                endAction.Invoke();
        }

    }
}
