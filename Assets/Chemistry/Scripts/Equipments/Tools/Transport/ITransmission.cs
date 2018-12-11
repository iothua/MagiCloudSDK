namespace Chemistry.Equipments
{
    /// <summary>
    /// 传输接口
    /// </summary>
    public interface ITransmission
    {
        ITransmission InPut { get; set; }
        ITransmission OutPut { get; set; }

        float Data { get; set; }


        bool IsInPut { get; }


        /// <summary>
        ///  传输
        /// </summary>
        /// <param name="values">数据</param>
        void Transport(params object[] values);
        /// <summary>
        /// 连接
        /// </summary>
        /// <param name="connect"></param>
        void Connection(ITransmission connect);

        void Disconnect(ITransmission connect);
    }

}
