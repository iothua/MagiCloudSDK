using UnityEngine;

namespace Chemistry.Equipments
{
    /// <summary>
    /// 铁环
    /// </summary>
    public class IronRing :SlideEquipment
    {

        public override void OnInitializeEquipment_Editor(string name)
        {
            base.OnInitializeEquipment_Editor(name);
            BoxCollider box = Collider as BoxCollider;
            box.size=new Vector3(1f,0.2f,0.4f);
        }
    }

}
