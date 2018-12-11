using MagiCloud.Equipments;
using MagiCloud.Interactive;
namespace Chemistry.Equipments
{
    /// <summary>
    /// 传输仪器基类
    /// </summary>
    public class TransmissionBase :EquipmentBase, ITransmission
    {
        public ITransmission InPut { get; set; }            //输入点
        public ITransmission OutPut { get; set; }           //输出点

        public virtual bool IsInPut => InPut!=null;           //是否已经连接输入端

        public virtual bool IsOutPut => OutPut!=null;     //是否已经连接输出端

        public float Data
        {
            get;
            set;
        }

        public override void OnDistanceRelease(InteractionEquipment interaction)
        {
            base.OnDistanceRelease(interaction);
            ITransmission temp = interaction.Equipment as ITransmission;
            Connection(temp);
        }
        /// <summary>
        /// 传输数据
        /// </summary>
        /// <param name="values">数据</param>
        public virtual void Transport(params object[] values)
        {
            if (IsOutPut)
            {
                OutPut.Transport(values);
            }
        }



        public override void OnDistanceExit(InteractionEquipment interaction)
        {
            base.OnDistanceExit(interaction);
            ITransmission temp = interaction.Equipment as ITransmission;
            Disconnect(temp);
        }

        /// <summary>
        /// 连接
        /// </summary>
        /// <param name="connect"></param>
        public void Connection(ITransmission connect)
        {
            if (connect==null) return;
            if (connect==InPut||connect==OutPut) return;
            //
            if (!IsInPut)
            {
                print(name+"输入端连接到："+connect.GetType().Name);
                connect.OutPut=this;
                InPut =connect;
            }
            else
            {
                if (!IsOutPut)
                {
                    OutPut=connect;
                    print(name+"输出端连接到："+connect.GetType().Name);
                }
            }
        }

        public void Disconnect(ITransmission connect)
        {
            if (InPut==connect) InPut=null;
            if (OutPut==connect) OutPut=null;
        }
    }
}
