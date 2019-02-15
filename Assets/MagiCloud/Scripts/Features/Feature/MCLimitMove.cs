using System;
using UnityEngine;
using MagiCloud.Core.Events;

namespace MagiCloud.Features
{

    /// <summary>
    /// 限制移动
    /// </summary>
    public class MCLimitMove :MonoBehaviour
    {
        [SerializeField, Header("当射线照射该物体时，赋予谁被抓取，不赋值默认为本身")]
        public GameObject grabObject;

        private bool YActiving = false;     //Y轴限制是否已经启用
        private bool ZActiving = false;     //Z轴限制是否已经启用

        public ProcessType type;

        public bool isLocal = false;        //缩放启用本地坐标
        public bool activeX = false;        //是否启用X轴限制
        public bool activeY = false;        //是否启用Y轴限制
        public bool activeZ = false;        //是否启用Z轴限制

        public Vector2 xRange = new Vector2(float.MinValue,float.MaxValue);     //x的限制范围
        public Vector2 yRange;                                                  //y的限制范围
        public Vector2 zRange;                                                  //z的限制范围

        public AnimationCurve minYCurve = new AnimationCurve();//time: x的坐标与xRange比值 ; value:   Y的最小值的曲线
        public AnimationCurve maxYCurve = new AnimationCurve();
        public AnimationCurve minZCurve = new AnimationCurve();
        public AnimationCurve maxZCurve = new AnimationCurve();

        public float Min
        {
            get { return xRange.x; }
        }
        public float Max { get { return xRange.y; } }



        private void Start()
        {
            EventHandReleaseObject.AddListener(Release, Core.ExecutionPriority.High);
        }


        public void Init()
        {
            if (minYCurve==null)
                minYCurve=new AnimationCurve();
            if (maxYCurve==null)
                maxYCurve=new AnimationCurve();
            if (minZCurve==null)
                minZCurve=new AnimationCurve();
            if (maxZCurve==null)
                maxZCurve =new AnimationCurve();

            if (activeY)
            {
                if (!YActiving)
                {
                    YActiving=true;
                    AddKeyGroup(yRange.x,yRange.y,AxisLimits.Y,0);
                    AddKeyGroup(yRange.x,yRange.y,AxisLimits.Y,1);
                }
            }
            if (activeZ)
            {
                if (!ZActiving)
                {

                    AddKeyGroup(zRange.x,zRange.y,AxisLimits.Z,0);
                    AddKeyGroup(zRange.x,zRange.y,AxisLimits.Z,1);
                    ZActiving =true;
                }
            }

        }
        private void OnDestroy()
        {
            EventHandReleaseObject.RemoveListener(Release);
        }

        private void Release(GameObject arg1,int arg2)
        {
            
            if (grabObject==arg1&&type==ProcessType.Release)
            {
                OnUpdate();
            }
        }

        /// <summary>
        /// 添加一组key
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="type"></param>
        /// <param name="t">t的值在0-1之间才有效</param>
        public void AddKeyGroup(float min,float max,AxisLimits type,float t = 0)
        {
            switch (type)
            {
                case AxisLimits.X:
                    AddKey(LimitKeyType.MinX,min,t);
                    AddKey(LimitKeyType.MaxX,min,t);
                    break;
                case AxisLimits.Y:
                    AddKey(LimitKeyType.MinY,min,t);
                    AddKey(LimitKeyType.MaxY,max,t);
                    break;
                case AxisLimits.Z:
                    AddKey(LimitKeyType.MinZ,min,t);
                    AddKey(LimitKeyType.MaxZ,max,t);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 设置最小值
        /// </summary>
        /// <param name="type">轴</param>
        /// <param name="value">最小值</param>
        public void SetMin(AxisLimits type,float value)
        {
            switch (type)
            {
                case AxisLimits.X:
                    AddKey(LimitKeyType.MinX,value,0);
                    AddKey(LimitKeyType.MinX,value,1);
                    break;
                case AxisLimits.Y:
                    AddKey(LimitKeyType.MinY,value,0);
                    AddKey(LimitKeyType.MinY,value,1);
                    break;
                case AxisLimits.Z:
                    AddKey(LimitKeyType.MinZ,value,0);
                    AddKey(LimitKeyType.MinZ,value,1);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 设置最大值
        /// </summary>
        /// <param name="type">轴</param>
        /// <param name="value">最大值</param>
        public void SetMax(AxisLimits type,float value)
        {
            switch (type)
            {
                case AxisLimits.X:
                    AddKey(LimitKeyType.MaxX,value,0);
                    AddKey(LimitKeyType.MaxX,value,1);
                    break;
                case AxisLimits.Y:
                    AddKey(LimitKeyType.MaxY,value,0);
                    AddKey(LimitKeyType.MaxY,value,1);
                    break;
                case AxisLimits.Z:
                    AddKey(LimitKeyType.MaxZ,value,0);
                    AddKey(LimitKeyType.MaxZ,value,1);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 添加key到曲线
        /// </summary>
        /// <param name="curve"></param>
        /// <param name="t"></param>
        /// <param name="value"></param>
        private void KeyInCurve(AnimationCurve curve,float t,float value)
        {
            for (int i = 0; i < curve.length; i++)
            {
                Keyframe key = curve.keys[i];
                if (key.time==t)
                {
                    curve.RemoveKey(i);
                }
            }
            curve.AddKey(t,value);
        }


        /// <summary>
        /// 添加一个key
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <param name="t">t的值在0-1之间才有效</param>
        public void AddKey(LimitKeyType type,float value,float t = 0)
        {
            if (t<0||t>1) return;
            switch (type)
            {
                case LimitKeyType.MinX:
                    xRange.x=value;
                    break;
                case LimitKeyType.MaxX:
                    xRange.y =value;
                    break;
                case LimitKeyType.MinY:
                    KeyInCurve(minYCurve,t,value);
                    break;
                case LimitKeyType.MaxY:
                    KeyInCurve(maxYCurve,t,value);
                    break;
                case LimitKeyType.MinZ:
                    KeyInCurve(minZCurve,t,value);
                    break;
                case LimitKeyType.MaxZ:
                    KeyInCurve(maxZCurve,t,value);
                    break;
                default:
                    break;
            }
        }


        private void LateUpdate()
        {
            switch (type)
            {
                case ProcessType.LateUpdate:
                    OnUpdate();
                    break;
                case ProcessType.Grab:
                    GameObject right = MOperateManager.GetObjectGrab(0);
                    if (right!=null&&right==grabObject)
                        OnUpdate();
                    GameObject left = MOperateManager.GetObjectGrab(1);
                    if (left!=null&&left==grabObject)
                        OnUpdate();
                    break;
                default:
                    break;
            }

        }

        public void OnUpdate()
        {
            if (grabObject!=null)
            {
                Vector3 pos = isLocal ? grabObject.transform.localPosition : grabObject.transform.position;
                pos.x=Limit(pos.x,Min,Max);
                float time = 0;
                if (Max-Min>0.001f)
                    time= (pos.x-Min)/(Max-Min);
                if (activeY)
                {
                    float minY = minYCurve.Evaluate(time);
                    float maxY = maxYCurve.Evaluate(time);
                    pos.y=Limit(pos.y,minY,maxY);
                }
                if (activeZ)
                {
                    float minZ = minZCurve.Evaluate(time);
                    float maxZ = maxZCurve.Evaluate(time);
                    pos.z=Limit(pos.z,minZ,maxZ);
                }
                if (isLocal)
                {
                    grabObject.transform.localPosition=pos;
                }
                else
                {
                    grabObject.transform.position=pos;
                }
            }

        }

        /// <summary>
        /// 限制值范围
        /// </summary>
        /// <param name="value"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public float Limit(float value,float min,float max)
        {
            if (min>max)
            {
                float temp = max;
                max=min;
                min=temp;
            }
            if (value<=min)
                value=min;
            if (value>=max)
                value=max;
            return value;
        }
    }

    public enum LimitKeyType
    {
        MinX,
        MaxX,
        MinY,
        MaxY,
        MinZ,
        MaxZ
    }
}
