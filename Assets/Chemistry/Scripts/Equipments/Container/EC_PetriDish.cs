using UnityEngine;
using MagiCloud.Equipments;

namespace Chemistry.Equipments
{
    /// <summary>
    /// 培养皿
    /// </summary>
    public class EC_PetriDish : EC_Container
    {
        protected override void Start()
        {
            base.Start();

            OnInitializeEquipment();
        }
    }
}
