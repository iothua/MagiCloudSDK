using UnityEngine;
using System.Collections;
using MagiCloud.Equipments;
using DG.Tweening;
using MagiCloud.Interactive;

namespace Chemistry.Equipments
{
    /// <summary>
    /// 温度计
    /// 开发者：阮榆皓
    /// </summary>
    public class ET_Thermometer : EquipmentBase
    {
        //    [Header("温度条搜寻路径")]
        //    public string BarPath = "Model/wen du di_01/sheng";
        //温度条
        public Transform TempBar;

        [Header("初始温度")]
        public float DefaultTemperature = 0.0f;

        [Header("实时温度")]
        public float CurTemperature;

        [Header("最高温度(默认为110摄氏度)")]
        public float MaxTemp = 110.0f;

        [Header("当前温度(默认为-20摄氏度)")]
        public float MinTemp = -20.0f;
        [Header("是否有温度计下降动画")]
        public bool HaveAnimation = true;
        //温度与温度条缩放的比率系数的倒数
        float rate = 1.0f;
        //float testNum = 0.5f;

        Tween t;

        protected override void Start()
        {
            base.Start();
            CurTemperature = DefaultTemperature;
            if (TempBar==null)
            {
                Debug.Log("温度条为空");
            }
            //if (!string.IsNullOrEmpty(BarPath))
            //{
            //    TempBar = this.transform.Find(BarPath);
            //}
            //else
            //{
            //    Debug.LogError("温度条路径为空");
            //}

            rate = DefaultTemperature - MinTemp;
            //Tween t = DOTween.To(() => testNum, x => testNum = x, 100, 5);
        }


        /// <summary>
        /// 测试
        /// </summary>
        protected override void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                TemperatureChangeWithTime(10.0f);
            }

            if (Input.GetKeyDown(KeyCode.W))
            {
                TemperatureChangeWithTime(0.0f);
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                TemperatureChangeWithTime(-10.0f);
            }
            //if (Input.GetKeyDown(KeyCode.R))
            //{
            //    TemperatureChangeWithTime(87.0f);
            //}
            //if (Input.GetKeyDown(KeyCode.T))
            //{
            //    TemperatureChangeWithTime(MaxTemp);
            //}
            //if (Input.GetKeyDown(KeyCode.Y))
            //{
            //    TemperatureChangeWithTime(MinTemp);
            //}
        }


        public override void OnDistanceRelease(InteractionEquipment interaction)
        {
            float hight = 0.0f;
            //温度计只和容器交互
            if (interaction.Equipment as EC_M_MeasuringCylinder)
            {
                var tmpInteractionObj = interaction.Equipment as EC_M_MeasuringCylinder;
                hight = tmpInteractionObj.FallHight;
            }
            else if (interaction.Equipment as EC_Beaker)
            {
                var tmpInteractionObj = interaction.Equipment as EC_Beaker;
                hight = tmpInteractionObj.FallHight;
            }

            if (HaveAnimation)
            {
                this.transform.DOLocalMoveY(transform.localPosition.y - hight, 0.5f).OnComplete(OnCompleteOperate);
            }

            base.OnDistanceRelease(interaction);
        }

        public override void OnDistanceExit(InteractionEquipment interaction)
        {
            //温度计只和容器交互
            if (interaction.Equipment as EC_Container)
            {
                transform.SetParent(null);
                transform.localRotation = Quaternion.identity;
            }

            base.OnDistanceExit(interaction);
        }

        /// <summary>
        /// 温度改变(随传入的时间，实时改变)
        /// </summary>
        /// <param name="endValue">终值</param>
        /// <param name="time">变化所需时间</param>
        public virtual void TemperatureChangeWithTime(float endValue, float time = 2.0f)
        {
            if (t != null && t.IsPlaying())
            {
                t.Kill();
            }

            float targetScaleZ = CaculateScaleWithTemperature(endValue);
            if (TempBar!=null)
            {
                t = TempBar.DOScaleZ(targetScaleZ, time).OnUpdate(() =>
                {
                    CurTemperature = (TempBar.transform.localScale.z - 1) * rate;
                });
            }
            
            
        }


        /// <summary>
        /// 温度改变（温度值为立刻变化，但液柱升降效果仍在）
        /// </summary>
        /// <param name="endValue">终值</param>
        /// <param name="isComplete">在变化效果后/前赋值</param>
        /// <param name="time">液柱变化时间</param>
        /// <returns>返回温度变化状态(-1 失败，0成功，1超量程，2低于量程)</returns>
        public virtual int TemperatureChangeInstant(float endValue,bool isComplete=false,float time=2.0f)
        {

            if (endValue > MaxTemp)
            {
                return 1;
            }
            else if (endValue < MinTemp)
            {
                return 2;
            }

            float targetScaleZ = CaculateScaleWithTemperature(endValue);

            if (TempBar != null)
            {
                var tmpTween = TempBar.DOScaleZ(targetScaleZ, time);

                if (!isComplete)
                {
                    CurTemperature = endValue;
                }
                else
                {
                    tmpTween.OnComplete(() => { CurTemperature = endValue; });
                }
                return 0;
            }

            return -1;
        }

        //public override void OnInitializeEquipment_Editor(string name)
        //{
        //    base.OnInitializeEquipment_Editor(name);
        //    //var col = Collider as BoxCollider;
        //    //col.center = new Vector3(0f, 0.25f, 0f);
        //    //col.size = new Vector3(0.4f, 0.3f, 0.4f);
        //}

        /// <summary>
        /// 计算缩放的数学公式
        /// </summary>
        /// <param name="curT"></param>
        /// <returns></returns>
        float CaculateScaleWithTemperature(float curT)
        {
            return 1.0f + curT / rate;
        }

        //放入温度计播放完毕后的回调
        public virtual void OnCompleteOperate()
        {

        }
    }
}