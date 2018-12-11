using MagiCloud.Equipments;
using UnityEngine;

namespace Chemistry.Equipments
{
    /// <summary>
    /// 铁架台上的夹
    /// </summary>
    public class Folder :SlideEquipment
    {
       
        public override void OnInitializeEquipment_Editor(string name)
        {
            base.OnInitializeEquipment_Editor(name);
            BoxCollider box = Collider as BoxCollider;
            box.center=new Vector3(0.3f,0f,0f);
            box.size=new Vector3(1.4f,0.2f,0.4f);
        }
    }
}