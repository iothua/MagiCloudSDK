using MagiCloud.Equipments;

namespace Chemistry.Equipments
{
    /// <summary>
    /// 可燃物
    /// </summary>
    public class Combustible :EquipmentBase, ICombustible
    {
        public Fire fire;
        public bool CanIgnite { get { return !fire.Burning; } }

        public IFire Fire => fire;
        public override void OnInitializeEquipment_Editor(string name)
        {
            base.OnInitializeEquipment_Editor(name);
            if (fire==null)
                fire=EffectNode.gameObject.AddComponent<Fire>();
        }
    }
}