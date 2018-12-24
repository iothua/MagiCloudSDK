using UnityEngine;
using MagiCloud.Equipments;

namespace Chemistry.Equipments
{
    /// <summary>
    /// 试管
    /// </summary>
    public partial class EC_Burette : EC_Container
    {

        protected override void Start()
        {
            base.Start();

            OnInitializeEquipment();
        }

    }
}
