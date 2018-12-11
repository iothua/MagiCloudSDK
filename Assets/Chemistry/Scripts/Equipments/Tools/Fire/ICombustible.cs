namespace Chemistry.Equipments
{
    /// <summary>
    /// 可燃物
    /// </summary>
    public interface ICombustible
    {
        /// <summary>
        /// 是否可以点燃
        /// </summary>
        bool CanIgnite { get; }

        /// <summary>
        /// 火焰
        /// </summary>
        IFire Fire { get; }
    }
}