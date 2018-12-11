using MagiCloud.Interactive;
using UnityEngine;
using MagiCloud.Core.Events;

namespace Chemistry.Equipments
{
    /// <summary>
    /// 火柴
    /// </summary>
    public class Matches :Combustible
    {
        public Transform owner;    //拥有者 ：火柴盒

        protected override void Start()
        {
            base.Start();

            gameObject.AddGrabObject(OnClick);
            gameObject.AddReleaseObject(OnFreed);

            OnInitializeEquipment();
        }

        private void OnFreed(int obj)
        {
            BackOwner();
            Fire.Slake();
        }

        private void OnClick(int obj)
        {
            Fire.Ignite();
        }


        public void BackOwner()
        {
            if (owner!=null)
            {
                transform.SetParent(owner);
                transform.localPosition=new Vector3(0.5f,0,0.3f);
            }
        }


        protected override void OnDestroy()
        {
            base.OnDestroy();
            gameObject.RemoveGrabObjectAll();
            gameObject.RemoveReleaseObjectAll();
        }
        /// <summary>
        /// 进入距离
        /// </summary>
        /// <param name="interaction"></param>
        public override void OnDistanceEnter(InteractionEquipment interaction)
        {
            base.OnDistanceEnter(interaction);
            //如果目标是可燃物，尝试点燃
            var combustible = interaction.Equipment.GetComponent<ICombustible>();
            if (combustible!=null)
                Fire.PassFire(combustible);
        }


        public override void OnInitializeEquipment_Editor(string name)
        {
            base.OnInitializeEquipment_Editor(name);

            var boxCol = Collider as BoxCollider;
            boxCol.center=new Vector3(0,0,-0.4f);
            boxCol.size=new Vector3(0.2f,0.2f,1f);
        }
    }
}


