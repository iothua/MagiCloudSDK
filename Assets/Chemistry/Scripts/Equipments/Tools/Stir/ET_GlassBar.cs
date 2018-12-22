using MagiCloud.Interactive;
using System.Collections;
using UnityEngine;
using MagiCloud.Interactive.Distance;
using DG.Tweening;
using MagiCloud.Core.Events;
using MagiCloud;

namespace Chemistry.Equipments
{
    /// <summary>
    /// 玻璃棒
    /// </summary>
    public class ET_GlassBar :TransmissionBase
    {
        /// <summary>
        /// 漏斗
        /// </summary>
        private Funnel funnel;
        /// <summary>
        /// 可搅拌物体
        /// </summary>
        private IStir stir;
        public InteractionEquipment send;
        public InteractionEquipment stirInteraction;
        /// <summary>
        /// 旋转点
        /// </summary>
        public Transform rotatePoint;
        /// <summary>
        /// 旋转轴方向
        /// </summary>
        public Vector3 dir = new Vector3(0.2f,1.9f,0);
        /// <summary>
        /// 开始搅拌时的初始倾斜角度
        /// </summary>
        public Vector3 initTile = new Vector3(0,0,10);
        /// <summary>
        /// 搅拌速率
        /// </summary>
        public float rate = 7.2f;
        /// <summary>
        /// 搅拌时间
        /// </summary>
        public float striTime = 4;
        /// <summary>
        /// 是否停止搅拌
        /// </summary>
        bool isStop = false;
        protected override void Start()
        {
            base.Start();
            OnInitializeEquipment();
            EventHandGrabObjectKey.AddListener(gameObject,OnGrad);

        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            EventHandGrabObjectKey.RemoveListener(gameObject,OnGrad);
        }

        private void OnGrad(int obj)
        {
            if (stirInteraction!=null)
            {
                InteractionDistanceController.OnExit(send,stirInteraction);
                stirInteraction=null;
            }
            StopStir();
        }

        public override bool IsCanInteraction(InteractionEquipment interaction)
        {
            if (interaction.Equipment is IStir)
            {
                return stir == null;
            }
            return base.IsCanInteraction(interaction);
        }

        public override void OnDistanceRelease(InteractionEquipment interaction)
        {
            base.OnDistanceRelease(interaction);

            float hight = 0.0f;

            if (interaction.Equipment as EC_Beaker)
            {
                var tmpInteractionObj = interaction.Equipment as EC_Beaker;
                hight = tmpInteractionObj.FallHight;

                this.transform.DOLocalMoveY(transform.localPosition.y - hight, 0.5f).OnComplete(() =>
                {
                    //此处添加玻璃棒搅拌动画;
                    if (interaction.Equipment as IStir != null)
                    {
                        stir = interaction.Equipment as IStir;
                        stirInteraction = interaction;
                        StartStir();
                    }
                });

                return;
            }

            if (interaction.Equipment as Funnel)
            {
                funnel=interaction.Equipment as Funnel;
            }
            if (interaction.Equipment as  IStir!=null)
            {
                stir = interaction.Equipment as IStir;
                stirInteraction=interaction;
                StartStir();
            }
            Transport("水",1);
            
        }

        public override void OnDistanceExit(InteractionEquipment interaction)
        {
            base.OnDistanceExit(interaction);

            if (funnel==interaction.Equipment)
            {
                transform.SetParent(null);
                transform.localRotation=Quaternion.identity;
            }
            //IStir stir = interaction.Equipment as IStir;

            //StopStir(stir);
        }

        public void StartStir()
        {
            if (stir!=null&&stir.AllowStir)
            {
                isStop=false;
                stir.StartStir(this);
                transform.localRotation=Quaternion.Euler(initTile);
                StartCoroutine(IEStir());
            }
        }


        private IEnumerator IEStir()
        {
            float time = 0;
            while (time<striTime&&isStop==false)
            {
                DoAction();
                time+=Time.fixedDeltaTime;
                yield return Time.fixedDeltaTime;
            }
            //---
            StopStir();
            yield break;
        }


        public void DoAction()
        {
            if (rotatePoint==null)
            {
                rotatePoint=new GameObject("rotatePoint").transform;
                rotatePoint.ResetTransform(transform);
                rotatePoint.localPosition=Vector3.zero;//new Vector3(0,5f,0);

            }

            transform.RotateAround(rotatePoint.position,dir,rate);
        }
        public void StopStir()
        {
            if (stir!=null)
            {
                stir.StopStir();
                isStop=true;
                StopCoroutine(IEStir());
                // transform.ResetTransform();
                transform.SetParent(null);
                transform.rotation=Quaternion.identity;
                stir =null;
            }
        }
    }
}
