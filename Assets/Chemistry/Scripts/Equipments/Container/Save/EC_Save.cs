using Chemistry.Chemicals;

namespace Chemistry.Equipments
{
    /// <summary>
    /// 存储容器
    /// 功能  ：I. 1.容积 2.液体 3.初始化数据 4.药品系统
    ///         II.盖子
    /// </summary>
    public class EC_Save :EC_Container, IDrugSystem
    {
        public bool isOpen = true;
        public EO_Cap _Cap;

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
