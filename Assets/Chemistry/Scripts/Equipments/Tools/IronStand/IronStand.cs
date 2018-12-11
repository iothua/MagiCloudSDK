using Chemistry.Help;
using MagiCloud.Equipments;
using MagiCloud.Interactive;
using System.Collections.Generic;
using UnityEngine;

namespace Chemistry.Equipments
{
    /// <summary>
    /// 铁架台
    /// </summary>
    public class IronStand :EquipmentBase
    {
        public List<ISlideEquipment> slideEquipments;   //0在最下面，count-1在最上面
        private Transform slideEquipmentNode;

        public Transform SlideEquipmentNode
        {
            get
            {
                if (slideEquipmentNode==null)
                    slideEquipmentNode=EquipmentUtility.CreateNode(transform,"Slide");
                return slideEquipmentNode;
            }
        }

        public int Count { get { return slideEquipments.Count; } }

        public float max;
        public float min;

        private bool isInteracted = false;          //是否已经交互,一次只能放置一个物体在铁架台上

        protected override void Start()
        {
            base.Start();

            OnInitializeEquipment();
        }

        protected override void Update()
        {
            base.Update();
            SetLimt();
        }

        public override bool IsCanInteraction(InteractionEquipment interaction)
        {
            return !isInteracted;
        }

        public override void OnDistanceRelease(InteractionEquipment interaction)
        {
            base.OnDistanceRelease(interaction);
            isInteracted=true;
        }
        public override void OnDistanceExit(InteractionEquipment interaction)
        {
            base.OnDistanceExit(interaction);
            isInteracted=false;
        }


        public void SetLimt()
        {
            for (int i = 1; i < Count; i++)
            {
                ISlideEquipment last = slideEquipments[i-1];
                ISlideEquipment current = slideEquipments[i];
                float up = (current as EquipmentBase).transform.localPosition.y-0.2f;
                float down = (last as EquipmentBase).transform.localPosition.y+0.2f;
                last.LimitMove.SetMax(MagiCloud.AxisLimits.Y,up);
                current.LimitMove.SetMin(MagiCloud.AxisLimits.Y,down);
            }
        }

        public override void OnInitializeEquipment()
        {
            base.OnInitializeEquipment();
            if (slideEquipments==null)
                slideEquipments=new List<ISlideEquipment>();
            slideEquipments.AddRange(SlideEquipmentNode.GetComponentsInChildren<ISlideEquipment>());
            slideEquipments[0].LimitMove.SetMin(MagiCloud.AxisLimits.Y,min);
            slideEquipments[Count-1].LimitMove.SetMax(MagiCloud.AxisLimits.Y,max);
        }


        public override void OnInitializeEquipment_Editor(string name)
        {
            base.OnInitializeEquipment_Editor(name);
            Collider.enabled=false;
            max=3f;
            min=0.8f;
        }
    }
}
