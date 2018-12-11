using Chemistry.Data;
using Chemistry.Help;
using MagiCloud.Equipments;
using UnityEngine;

namespace Chemistry.Equipments
{
    /// <summary>
    /// 火柴盒
    /// </summary>
    public class Matchbox :EquipmentBase
    {

        public Matches matches;

        protected override void Start()
        {
            base.Start();
            OnInitializeEquipment();
        }

        public override void OnInitializeEquipment_Editor(string name)
        {
            base.OnInitializeEquipment_Editor(name);
            var col = Collider as BoxCollider;
            col.size=new Vector3(0.5f,0.2f,0.8f);
            col.center=new Vector3(0f,-0.1f,0f);

            //Transform matchTra = null;
            //if (DataLoading.DicEquipmentLoadingInfo.ContainsKey("火柴"))
            //{
            //    matchTra= EquipmentInitializationHelper.CreateSuccessEquipment("火柴").transform;
            //}
            //else
            //{
            //    matchTra  = EquipmentInitializationHelper.CreatEquipment("火柴",EContainerType.火柴,ECapType.无,null);
            //}
            //Matches matches = matchTra.GetComponent<Matches>();
            //matches.owner=transform;
            //matches.BackOwner();
        }

        public override void OnInitializeEquipment()
        {
            base.OnInitializeEquipment();

            if (matches == null)
            {
                matches = gameObject.GetComponentInChildren<Matches>();
                matches.owner = transform;
                matches.BackOwner();
            }
        }

        private void BackOwner(Transform matchTra)
        {
            matchTra.SetParent(transform);
            matchTra.localPosition=new Vector3(0.5f,0,0.3f);
        }

    }
}
