namespace Chemistry.Equipments
{
    /// <summary>
    /// 火焰接口
    /// </summary>
    public interface IFire
    {
        /// <summary>
        /// 是否正在燃烧
        /// </summary>
        bool Burning { get; }
        /// <summary>
        /// 点燃
        /// </summary>
        void Ignite();
        /// <summary>
        /// 熄灭
        /// </summary>
        void Slake();
        /// <summary>
        /// 传火
        /// </summary>
        /// <param name="combustible"></param>
        void PassFire(ICombustible combustible);

    }

}