using MagiCloud.Equipments;
using UnityEngine;

namespace Chemistry.Equipments
{
    /// <summary>
    /// 酒精灯冒
    /// </summary>
    [ExecuteInEditMode]
    public class ET_AlcoholLampCover :EquipmentBase
    {
        protected override void Start()
        {
            base.Start();
            OnInitializeEquipment();
        }


        public override void OnInitializeEquipment_Editor(string name)
        {
            var boxCol = Collider as BoxCollider;
           
            boxCol.center=Vector3.zero;
            boxCol.size=new Vector3(0.4f,0.5f,0.4f);
            base.OnInitializeEquipment_Editor(name);
            transform.localPosition = new Vector3(0,0.895f,0);
        }
    }
}