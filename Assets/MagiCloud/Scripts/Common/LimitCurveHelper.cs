using MagiCloud.Features;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace MagiCloud.Common
{
    /// <summary>
    /// 用于设置曲线限制移动的扩展
    /// </summary>
    public class LimitCurveHelper :MonoBehaviour
    {
        [Header("默认为自己,建议中心为父对象")]
        public Transform center;
        public Transform self;
        [Header("精度,注意精度*start的值应为整数")]
        public int precision = 30;
        [Header("长半径")]
        public float longR = 3;
        [Header("短半径")]
        public float shortR = 2;
        [Header("方向")]
        public Vector3 dir = Vector3.back;
        //[Header("中心")]
        //public Vector3 center = new Vector3(0,2,0);
        [Range(0f,0.25f)]
        public float start = 0f;
        public bool down = false;
        public float Range
        {
            get
            {
                return 0.5f-2*start;
            }

        }

        [Header("是否从左向右")]
        public bool leftToRight = false;

        [Header("当line为null时不显示线")]
        public LineRenderer line;
        private int num;

        [HideInInspector]
        public List<Vector3> keys;
        public FeaturesObjectController features;
        private MCLimitMove limit;
        Vector3[] track;
        private void Start()
        {
            if (self==null) self=transform;
            if (center==null) center=transform;
            keys =new List<Vector3>();
            if (features==null)
                features = center.GetComponent<FeaturesObjectController>();
            limit = features.LimitMove;


            UpdateLimitCurve();
        }
        public void Set(float start,float range)
        {
            this.start=start;
            UpdateLimitCurve();
        }

        public void UpdateLimitCurve()
        {
            //var temp = self.position;
            //temp.y=0;
            keys.Clear();
            self.localPosition=Vector3.zero;
            track = Track(self,precision,longR,shortR,Vector3.forward,center.position);
            int startIndex = Convert.ToInt32(precision*(down ? (start+0.5f) : start));     //起始点索引
            num =   Convert.ToInt32(Range*precision);              //截取点的数量
            int endIndex = startIndex+ num;                        //终点索引
            int over = 0;                                          //超出部分

            if (endIndex>track.Length-1)
            {
                over=endIndex-track.Length+1;
                endIndex =track.Length-1;
            }
            limit.xRange.x=(float)Math.Round(track[endIndex].x-center.position.x,5);
            limit.xRange.y=(float)Math.Round(track[startIndex].x-center.position.x,5);

            for (int i = limit.minYCurve.keys.Length-1; i >=0; i--)
            {
                limit.minYCurve.RemoveKey(i);
            }
            for (int i = limit.maxYCurve.keys.Length-1; i >=0; i--)
            {
                limit.maxYCurve.RemoveKey(i);
            }
            for (int i = startIndex; i <=endIndex; i++) //导入到曲线组件
                TrackToLimit(i);

            if (over!=0)                                //计算超出部分
                for (int i = 0; i <= over; i++)
                    TrackToLimit(i);

            if (line!=null)
            {
                line.positionCount=0;
                line.positionCount=keys.Count;
                line.SetPositions(keys.ToArray());
            }
            SeetSmooth(limit.maxYCurve);
            SeetSmooth(limit.minYCurve);

        }

        private void TrackToLimit(int i)
        {

            //椭圆上的点转成限制移动曲线x坐标
            Vector3 key = track[i]-center.position;
            key.z=0;
            //  key.z=center.position.z;
            var sum = limit.xRange.y-limit.xRange.x;
            var min = limit.xRange.x;
            float t = leftToRight ? (key.x-min)/sum : -1f*(min+(float)Math.Round(key.x,5))/sum;
            //float t = leftToRight ? (key.x/sum+0.5f) : (0.5f-(float)Math.Round(key.x,5)/sum);
            limit.AddKeyGroup(key.y,key.y,AxisLimits.Y,t);
            keys.Add(key);
        }

        /// <summary>
        /// 设置曲线平滑
        /// </summary>
        /// <param name="curve"></param>
        private void SeetSmooth(AnimationCurve curve)
        {
            curve.postWrapMode = WrapMode.Loop;
            if (curve.keys.Length<2) return;
            Keyframe startkey = curve.keys[0];
            startkey.outTangent = 0;
            curve.MoveKey(0,startkey);
            Keyframe lastkey = curve.keys[curve.length-1];
            lastkey.inTangent = 0;
            curve.MoveKey(curve.length-1,lastkey);
        }


        /// <summary>
        /// 得到轨道函数图像取样点
        /// </summary>
        /// <param name="num">取样数量</param>
        /// <param name="longR">半长轴</param>
        /// <param name="shortR">半短轴</param>
        /// <param name="A">初始角度</param>
        /// <returns>x为长半轴值,y为短半轴值</returns>
        public Vector3[] Track(Component target,int num,float longR,float shortR,Vector3 dir,Vector3 center = default(Vector3),float A = 0)
        {
            Vector3[] samples = new Vector3[num + 1];
            for (int i = 0; i <= num; i++)
            {
                Vector3 pos = center;
                float radian = (A / 180f) * Mathf.PI;
                pos.x = center.x + longR * Mathf.Cos(radian);
                pos.y = center.y + shortR * Mathf.Sin(radian);
                pos = QuantionToPos(target,pos,dir,center);
                samples[i] = pos;
                A += (360f / num);
            }

            return samples;
        }

        public Vector3 QuantionToPos(Component target,Vector3 pos,Vector3 dir,Vector3 center = default(Vector3))
        {
            Quaternion q = Quaternion.FromToRotation(Vector3.forward,dir);
            pos = q * pos + center;
            pos = target.transform.worldToLocalMatrix.MultiplyPoint(pos);
            return pos;
        }
    }
}
