using MagiCloud.Equipments;
using MagiCloud.Features;
using UnityEngine;

namespace Chemistry.Equipments
{
    /// <summary>
    /// 滑动的仪器
    /// </summary>
    public class SlideEquipment :EquipmentBase, ISlideEquipment
    {
        public MCLimitMove LimitMove { get { return FeaturesObject.LimitMove; } }

        protected override void Start()
        {
            base.Start();
            OnInitializeEquipment();
        }

        public override void OnInitializeEquipment_Editor(string name)
        {
            base.OnInitializeEquipment_Editor(name);
            FeaturesObject.ActiveLimitMove=true;
        }

    }
}
