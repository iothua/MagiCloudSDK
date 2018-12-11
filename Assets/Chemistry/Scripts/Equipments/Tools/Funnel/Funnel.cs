using MagiCloud.Equipments;
using MagiCloud.Interactive;
using UnityEngine;

namespace Chemistry.Equipments
{
    /// <summary>
    /// 漏斗
    /// </summary>
    public class Funnel :TransmissionBase
    {
        private FilterPaper filterPaper;        //滤纸缓存

        protected override void Start()
        {
            base.Start();
            OnInitializeEquipment();

        }

        public override bool IsCanInteraction(InteractionEquipment interaction)
        {
            if (interaction.Equipment as ET_GlassBar)
            {
                return filterPaper!=null;
            }
            return base.IsCanInteraction(interaction);
        }

        public override void OnDistanceRelease(InteractionEquipment interaction)
        {
            base.OnDistanceRelease(interaction);
            if (interaction.Equipment as FilterPaper !=null)
                filterPaper=interaction.Equipment as FilterPaper;
        }

        public override void OnInitializeEquipment_Editor(string name)
        {
            base.OnInitializeEquipment_Editor(name);
            BoxCollider col = Collider as BoxCollider;
            col.center=new Vector3(0f,1.1f,0f);
            col.size=new Vector3(0.8f,0.4f,0.8f);
        }
        public override void Transport(params object[] values)
        {
            base.Transport(values);
        }
    }
}
