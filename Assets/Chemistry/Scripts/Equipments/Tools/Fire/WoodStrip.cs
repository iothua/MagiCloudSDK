namespace Chemistry.Equipments
{
    /// <summary>
    /// 木条
    /// </summary>
    public class WoodStrip :Combustible
    {
        protected override void Start()
        {
            base.Start();
            OnInitializeEquipment();
        }


    }
}