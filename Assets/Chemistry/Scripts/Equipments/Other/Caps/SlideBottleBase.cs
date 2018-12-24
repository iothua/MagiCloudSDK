using MagiCloud;
using MagiCloud.Equipments;
using MagiCloud.Interactive;
using UnityEngine;
namespace Chemistry.Equipments
{
    /// <summary>
    /// 带有滑片的瓶子基类
    /// </summary>
    public class SlideBottleBase :EquipmentBase, ISlideBottle
    {
        [Header("瓶口左端")]
        public Transform left;
        [Header("瓶口右端")]
        public Transform right;
        [Header("盖子localY值范围限制")]
        public Vector2 limitRange = Vector2.zero;
        [Header("默认为限制盖子Y值")]
        public AxisLimits axisLimits = AxisLimits.Y;

        public Vector2 Bound => new Vector2(left.position.x,right.position.x);

        public Vector2 LimitRange => limitRange;

        public float OpenRate { get { return SlideCover==null ? 1f : SlideCover.GetRange(this); } }


        public ISlideCover SlideCover { get; set; }

        public AxisLimits AxisLimits => axisLimits;



        public override bool IsCanInteraction(InteractionEquipment interaction)
        {
            return true;
        }
        protected override void Start()
        {
            base.Start();
            OnInitializeEquipment();
        }
    }
}
