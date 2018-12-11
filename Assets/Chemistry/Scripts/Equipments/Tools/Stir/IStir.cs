namespace Chemistry.Equipments
{
    /// <summary>
    /// 搅拌
    /// </summary>
    public interface IStir
    {
        bool AllowStir { get; }

        /// <summary>
        /// 开始搅拌
        /// </summary>
        void StartStir(ET_GlassBar glassBar);


        /// <summary>
        /// 停止搅拌
        /// </summary>
        void StopStir();
    }
}
