using Chemistry.Chemicals;
using MagiCloud.Interactive;

namespace Chemistry.Equipments
{
    /// <summary>
    /// 存储容器
    /// 功能  ：I. 1.容积 2.液体 3.初始化数据 4.药品系统
    ///         II.盖子
    /// </summary>
    public class EC_Save :EC_Container, IDrugSystem
    {
#pragma warning disable CS0436 // 类型与导入类型冲突
        public CapOperateType capOperate = CapOperateType.唯一型;
#pragma warning restore CS0436 // 类型与导入类型冲突

        public bool isOpen = true;
        public EO_Cap _Cap;

        public override bool IsCanInteraction(InteractionEquipment interaction)
        {

            //如果是瓶盖的话，则进行瓶盖处理
            if(interaction.Equipment is EO_Cap)
            {
                switch(capOperate)
                {
#pragma warning disable CS0436 // 类型与导入类型冲突
                    case CapOperateType.唯一型:
#pragma warning restore CS0436 // 类型与导入类型冲突

                        //如果是相等的，则可进行交互
                        if (_Cap == interaction.Equipment)
                            return isOpen;

                        return false;

#pragma warning disable CS0436 // 类型与导入类型冲突
                    case CapOperateType.交互型:
#pragma warning restore CS0436 // 类型与导入类型冲突

                        var cap = interaction.Equipment as EO_Cap;

                        if (!cap.capOperate.Equals(capOperate)) return false;

                        return isOpen;

                    default:
                        break;
                }

            }

            return true;
        }

        /// <summary>
        /// 打开盖子
        /// </summary>
        public void OpenCap()
        {
            _Cap.IsCap = true;
            isOpen=true;
        }

        /// <summary>
        /// 关闭盖子
        /// </summary>
        public void CloseCap()
        {
            _Cap.IsCap = false;
            isOpen=false;
        }
    }
}
