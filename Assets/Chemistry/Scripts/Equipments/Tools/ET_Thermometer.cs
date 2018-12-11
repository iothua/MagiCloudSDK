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
        //温度条
        public Transform TempBar;

        [Header("初始温度")]
        public float DefaultTemperature = 25.0f;

        [Header("当前温度")]
        public float CurTemperature;

        [Header("最高温度(默认为50摄氏度)")]
        public float MaxTemp = 50.0f;

        [Header("当前温度(默认为-50摄氏度)")]
        public float MinTemp = -50.0f;
        

        protected override void Start()
        {
            base.Start();
            CurTemperature = DefaultTemperature;
            TempBar = this.transform.Find("Model/Object001");
        }


        /// <summary>
        /// 测试
        /// </summary>
        protected override void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                Debug.Log("qqq");
                TemperatureChange(0.0f);
            }

            if (Input.GetKeyDown(KeyCode.W))
            {
                TemperatureChange(20.0f);
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                TemperatureChange(10.0f);
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                TemperatureChange(-15.0f);
            }
            if (Input.GetKeyDown(KeyCode.T))
            {
                TemperatureChange(MaxTemp);
            }
            if (Input.GetKeyDown(KeyCode.Y))
            {
                TemperatureChange(MinTemp);
            }

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
            else if(interaction.Equipment as EC_Beaker)
            {
                var tmpInteractionObj = interaction.Equipment as EC_Beaker;
                hight = tmpInteractionObj.FallHight;
            }
            
            this.transform.DOLocalMoveY(hight, 0.5f);

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
        /// 温度改变
        /// </summary>
        /// <param name="endValue">终值</param>
        /// <param name="time">变化所需时间</param>
        public void TemperatureChange(float endValue, float time = 1.0f)
        {
            TempBar.DOScaleZ(((endValue - MinTemp) / (DefaultTemperature - MinTemp)), time).OnComplete(() =>
            {
                CurTemperature = endValue;
            });
        }


        public override void OnInitializeEquipment_Editor(string name)
        {
            base.OnInitializeEquipment_Editor(name);
            var col = Collider as BoxCollider;
            col.center = new Vector3(0f, 0.25f, 0f);
            col.size = new Vector3(0.4f, 0.3f, 0.4f);
        }


    }
}