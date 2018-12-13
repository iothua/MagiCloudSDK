using Chemistry.Data;
using Chemistry.Effects;
using MagiCloud.Interactive;
using UnityEngine;
using MagiCloud.Core.Events;

namespace Chemistry.Equipments
{
    /// <summary>
    /// 酒精灯
    /// </summary>
    public class EC_S_AlcoholLamp :EC_Save, ICombustible
    {

        public bool isInitFire = false;
        public Fire fire;
        public bool CanIgnite { get { return cover.IsCover&&!Fire.Burning; } }      //是否可以点燃


        public IFire Fire
        {
            get { return fire; }
        }


        [HideInInspector]
        public Effect_Fire effect_Fire;

        protected override void Start()
        {
            base.Start();
            OnInitializeEquipment();
            //KinectEventHandGrabObjectKey.AddListener(_Cap.gameObject,EventLevel.B,OnClick);

            cover.gameObject.AddGrabObject(OnClick);
        }

        private void OnClick(int obj)
        {
            CapLeave();
        }
        protected override void OnDestroy()
        {
            if (cover!=null)
                cover.gameObject.RemoveGrabObject(OnClick);

            base.OnDestroy();
        }
        public override void OnInitializeEquipment()
        {

            containerType=EContainerType.酒精灯;

            if (isInitFire)
            {
                OpenCap();
                if (cover!=null) cover.transform.position+=new Vector3(4f,-3f,0);


                Ignite();
            }
            else
            {
                CloseCap();
            }

            base.OnInitializeEquipment();
        }

        public override bool IsCanInteraction(InteractionEquipment interaction)
        {
            if( !base.IsCanInteraction(interaction))
            {
                return false;
            }

            if (interaction.Equipment.GetComponent<IFire>() != null)
                return !fire.Burning;

            if (interaction.Equipment.GetComponent<ICombustible>() != null)
                return true;

            return false;
        }


        public override void OnDistanceEnter(InteractionEquipment interaction)
        {
            base.OnDistanceEnter(interaction);
            //如果目标是可燃物，尝试点燃
            var combustible = interaction.Equipment.GetComponent<ICombustible>();
            if (combustible!=null)
                fire.PassFire(combustible);
        }

        public override void OnDistanceRelease(InteractionEquipment interaction)
        {

            base.OnDistanceRelease(interaction);
            if (cover==interaction.Equipment)
            {
                CapJion();

                Slake();
            }
        }

        public override void OnDistanceExit(InteractionEquipment interaction)
        {
            base.OnDistanceExit(interaction);

        }

        /// <summary>
        /// 点燃
        /// </summary>
        public void Ignite()
        {
            if (!CanIgnite) return;
            Fire.Ignite();
        }
        /// <summary>
        /// 熄灭
        /// </summary>
        public void Slake()
        {
            Fire.Slake();
        }

        private void CapLeave()
        {
            OpenCap();
        }

        private void CapJion()
        {
            CloseCap();
        }

        public override void OnInitializeEquipment_Editor(string equipmentName)
        {
            base.OnInitializeEquipment_Editor(equipmentName);

            fire=EffectNode.gameObject.AddComponent<Fire>();
            containerType =EContainerType.酒精灯;
            var boxCol = Collider as BoxCollider;
            boxCol.center=new Vector3(0f,0.3f,0f);
            boxCol.size=new Vector3(1f,0.6f,1f);
        }
    }
}