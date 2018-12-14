using UnityEngine;
namespace MagiCloud.KGUI
{
    /// <summary>
    /// 记录者基类
    /// </summary>
    public abstract class RecordBase :MonoBehaviour, IRecord
    {
        //是否允许记录
        public virtual bool CanReocrd => (true);
        /// <summary>
        /// 数据记录完成后是否覆盖
        /// </summary>
        public virtual bool IsCover => false;
        public abstract string[] GetData();

    }
}