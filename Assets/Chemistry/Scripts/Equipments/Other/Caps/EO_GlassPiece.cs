using MagiCloud;
using MagiCloud.Features;
using MagiCloud.KGUI;
using System;
using UnityEngine;
using MagiCloud.Core.Events;
using MagiCloud.Interactive;
using MagiCloud.Core;
using MagiCloud.Common;

namespace Chemistry.Equipments
{
    /// <summary>
    /// 玻璃片
    /// 1、支持滑动移出
    /// 2、支持滑动移入
    /// 3、靠近时，进行交互
    /// </summary>
    public class EO_GlassPiece :EO_Cover, ISlideCover
    {
        [Header("是否需要释放")]
        public bool needRelease = false;
        public Transform left;
        public Transform right;

        public ProgressBar bar;
        public ProgressBar timeBar;
        public TimeController timeController;


        private InteractionEquipment receive;
        private ISlideBottle _bound;
        public Vector2 Bound => new Vector2(left.position.x,right.position.x);
        public MCLimitMove LimitMove => FeaturesObject.LimitMove;
        MBehaviour behaviour;
        private int dir = 1;
        public float Range
        {
            get
            {
                if (_bound==null) return 1;
                return GetRange(_bound);
            }
        }

        protected override void Start()
        {
            base.Start();
            behaviour=new MBehaviour();
            behaviour.OnUpdate_MBehaviour(OnUpdate);
            OnInitializeEquipment();
            if (timeController!=null)
            {
                timeController.playingEvent.AddListener(OnTime);
                timeController.stopEvent.AddListener(OnStop);
            }
        }

        private void OnStop()
        {
            if (_bound!=null)
            {
                Active(_bound,_bound.LimitRange,_bound.AxisLimits);

                transform.SetParent((_bound as Component).transform);
            }

            timeBar?.gameObject.SetActive(false);
        }

        private void OnTime(float t)
        {
            if (timeBar!=null)
                timeBar.value=t;
        }

        private void OnUpdate()
        {
            if (_bound!=null)
            {
                if (bar!=null)
                    bar.value=(1+Range*dir)*0.5f;
                if (left.position.x>_bound.Bound.y||right.position.x<_bound.Bound.x)
                    Close();
            }
        }

        public override bool IsCanInteraction(InteractionEquipment interaction)
        {
            return _bound==null;
        }

        public override void OnDistanceEnter(InteractionEquipment interaction)
        {
            base.OnDistanceEnter(interaction);
            if (!needRelease)
            {
                if (Interaction(interaction))
                {
                    timeBar?.gameObject.SetActive(true);
                    if (timeController!=null) timeController.Play();
                }
            }
        }
        public override void OnDistanceExit(InteractionEquipment interaction)
        {
            if (!needRelease)
            {
                if (Interaction(interaction))
                {
                    if (timeController!=null) timeController.Pause();
                    timeBar?.gameObject.SetActive(false);
                }
            }
            base.OnDistanceExit(interaction);
        }

        public override void OnDistanceRelease(InteractionEquipment interaction)
        {
            base.OnDistanceRelease(interaction);
            if (needRelease)
            {
                if (Interaction(interaction))
                    Active(_bound,_bound.LimitRange,_bound.AxisLimits);
            }
        }

        private bool Interaction(InteractionEquipment interaction)
        {
            if (interaction.Equipment is ISlideBottle)
            {
                var temp = interaction.Equipment as ISlideBottle;
                if (_bound!=null&&_bound!=temp) return false;
                _bound=temp;
                receive =interaction;
                receive.ActiveParent=false;
                receive.ActiveShadow=false;
                return true;
            }
            return false;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (behaviour!=null)
                behaviour.OnExcuteDestroy();
            if (timeController!=null)
            {
                timeController.playingEvent.RemoveListener(OnTime);
                timeController.stopEvent.RemoveListener(OnStop);
            }
        }





        public float GetRange(ISlideBottle slideBottle)
        {
            float left = Bound.x- slideBottle.Bound.x;
            float right = slideBottle.Bound.y-Bound.y;

            if (left<=0&&right<=0) //完全覆盖
                return 0;
            else if (left<=0&&right>0)    //向左滑动，右边开口
            {
                dir=-1;
                return Mathf.Clamp01(right/(slideBottle.Bound.y-slideBottle.Bound.x));
            }
            else if (left>0&&right<=0)    //向右滑动，左边开口
            {
                dir=1;
                return Mathf.Clamp01(left/(slideBottle.Bound.y-slideBottle.Bound.x));
            }
            else
                throw new Exception("滑片的边界值不应大于瓶口的边界值");
        }


        public void Active(ISlideBottle bound,Vector2 value,AxisLimits axis = AxisLimits.Y,bool isLocal = true)
        {

            LimitMove.isLocal=isLocal;
            LimitMove.activeX=true;
            switch (axis)
            {
                case AxisLimits.X:
                    LimitMove.SetMin(AxisLimits.X,value.x);
                    LimitMove.SetMax(AxisLimits.X,value.y);
                    break;
                case AxisLimits.Y:
                    LimitMove.activeY=true;
                    LimitMove.SetMin(AxisLimits.Y,value.x);
                    LimitMove.SetMax(AxisLimits.Y,value.y);
                    break;
                case AxisLimits.Z:
                    LimitMove.activeZ=true;
                    LimitMove.SetMin(AxisLimits.Z,value.x);
                    LimitMove.SetMax(AxisLimits.Z,value.y);
                    break;
                default:
                    break;
            }

            _bound.SlideCover=this;
            bar?.gameObject.SetActive(true);

            //   timeController.
        }

        public void Close(AxisLimits axis = AxisLimits.Y)
        {
            if (_bound==null) return;
            switch (axis)
            {
                case AxisLimits.X:
                    LimitMove.activeX=false;
                    break;
                case AxisLimits.Y:
                    LimitMove.activeY=false;
                    break;
                case AxisLimits.Z:
                    LimitMove.activeZ=false;
                    break;
                default:
                    break;
            }
            _bound.SlideCover=null;
            _bound = null;
            if (receive!=null)
            {
                receive.ActiveParent=true;
                receive.ActiveShadow=true;
            }
            bar?.gameObject.SetActive(false);

        }

    }
}
