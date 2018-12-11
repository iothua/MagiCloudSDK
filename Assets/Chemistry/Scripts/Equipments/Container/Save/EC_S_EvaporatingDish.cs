using Chemistry.Data;
using MagiCloud.Equipments;
using UnityEngine;

namespace Chemistry.Equipments
{
    /// <summary>
    /// 蒸发皿
    /// </summary>
    public class EC_S_EvaporatingDish :EC_Container, IStir
    {
        public bool AllowStir => true;


        protected override void Start()
        {
            base.Start();
            OnInitializeEquipment();
        }

        public override void OnInitializeEquipment_Editor(string name)
        {
            base.OnInitializeEquipment_Editor(name);
            BoxCollider col = Collider as BoxCollider;
            col.center=new Vector3(0f,0.2f,0f);
            col.size=new Vector3(0.8f,0.4f,0.8f);
        }


        public void StopStir()
        {
        }
       
        public void DoAction()
        {

        }
        public void StartStir(ET_GlassBar glassBar)
        {
         
        }
    }
}