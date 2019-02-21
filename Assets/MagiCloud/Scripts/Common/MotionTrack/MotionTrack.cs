using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MagiCloud.Common
{
    /// <summary>
    /// 运动轨迹
    /// </summary>
    public class MotionTrack :MonoBehaviour
    {
        [Header("开启记录")]
        public bool activeRecord = false;
        [Header("显示记录线")]
        public bool activeShowLine = false;
        [Header("记录时间间隔"), Range(0.0001f,10f)]
        public float interval = 0.02f;  //记录间隔 
        [Header("显示时长"), Range(0f,100000f)]
        public float lifeTime = 1;  //存在时间
        [Header("显示延迟时间"), Range(0f,100f)]
        public float delay = 0.2f;  //延迟时间
        [Space]
        [Header("设置线"), Range(0f,100f)]
        public float width = 0.01f;
        public Color color = Color.white;

        [Header("以下参数不配置会做默认处理")]
        public Material material;
        public Transform target;
        public LineRenderer line;

        private List<TimePoint<Vector3>> recordCache;    //记录缓存
        private float time = 0;     //计时

        private WaitForSeconds IeDelay { get { return new WaitForSeconds(delay); } } //显示计时

        private void Start()
        {
            if (target==null)
                target=transform;
            recordCache =new List<TimePoint<Vector3>>();
            line=target.GetComponent<LineRenderer>();

            if (line==null)
                line=target.gameObject.AddComponent<LineRenderer>();
            if (material==null)
                material=new Material(Shader.Find("Particles/Additive"));
            SetLine(color,width,material);
        }

        void Update()
        {

            if (activeRecord&&recordCache!=null)
            {
                time+=Time.deltaTime;
                RecordPoint();
            }
            if (activeShowLine)
                UpdatePaths();
        }

        public void SetLine(Color color,float width,Material material,int order = 1)
        {
            line.startColor=color;
            line.endColor=color;
            line.sortingOrder=1;
            line.useWorldSpace=true;
            line.startWidth=width;
            line.endWidth=width;
            line.material=material;
        }


        public void PlayRecord()
        {
            activeRecord=true;
        }
        public void PlayShowLine()
        {
            activeShowLine=true;
        }
        public void StopRecord()
        {
            activeRecord=false;
        }
        public void StopShowLine()
        {
            activeShowLine =false;
        }
        public void ClearTrack()
        {
            recordCache.Clear();
        }
        public void ClearLine()
        {
            line.positionCount=0;
        }

        private void RecordPoint()
        {

            if (time>=interval)
            {
                time=0;
                if (recordCache.Count>0&&recordCache[recordCache.Count-1].data ==target.position) return;
                var result = new TimePoint<Vector3>(Time.realtimeSinceStartup+delay,lifeTime,target.position);
                StartCoroutine(IEAdd(result));

            }
        }

        private IEnumerator IEAdd(TimePoint<Vector3> result)
        {
            yield return IeDelay;
            recordCache.Add(result);
            yield break;
        }

        void UpdatePaths()
        {
            for (int i = 0; i < recordCache.Count; i++)
            {
                var cur = recordCache[i];
                float t = Time.realtimeSinceStartup-cur.startTime;
                if (t>cur.lifeTime)
                {
                    recordCache.Remove(cur);
                    continue;
                }
            }

            line.positionCount=recordCache.Count;
            for (int i = 0; i < recordCache.Count; i++)
                line.SetPosition(i,recordCache[i].data);
        }
    }
}
